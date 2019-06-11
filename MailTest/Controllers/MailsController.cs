using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Security;
using MailTest.Models;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace MailTest.Controllers
{
    [RoutePrefix("api/mails")]
    public class MailsController : ApiController
    {
        private readonly IEmailConfiguration appSettings;
        List<EmailAddress> mail = new List<EmailAddress> { };
        public MailsController(IEmailConfiguration appSettings)
        {
            this.appSettings = appSettings;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage SendMail(EmailMessage emailMessage)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(appSettings.EscalationUsername);
            mail.AddRange(emailMessage.ToAddresses);
            message.To.Add(mail.First().Address);
            message.Subject = emailMessage.Subject;
            message.IsBodyHtml = true;
            message.Body = emailMessage.Content;
            message.IsBodyHtml = true;


            using (var emailClient = new SmtpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                emailClient.Port = appSettings.EscalationPort;
                emailClient.Host = appSettings.EscalationHost;
                
                emailClient.Credentials = new NetworkCredential(appSettings.EscalationUsername, appSettings.EscalationPassword);
                emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                emailClient.Send(message);

                return Request.CreateResponse(HttpStatusCode.Created);
            }
        }

        [HttpGet]
        [Route("")]
        public HttpResponseMessage ReceiveEmail(int MaxCount = 10)
        {
            using (var client = new Pop3Client())
            {
                client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                client.Connect(appSettings.POPServer, appSettings.POP3Port);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(appSettings.Username, appSettings.Password);

                List<EmailMessage> emails = new List<EmailMessage>();
                for (int i = 0; i < client.Count && i < MaxCount; i++)
                {
                    var message = client.GetMessage(i);
                    var emailMessage = new EmailMessage
                    {
                        Content = !String.IsNullOrEmpty(message.TextBody) ? message.TextBody : message.TextBody,
                        Subject = message.Subject,
                        Date = message.Date
                    };
                    var hchl = message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name });
                    emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));

                    emails.Add(emailMessage);
                }
                
                client.Disconnect(true);

                return Request.CreateResponse(HttpStatusCode.OK, emails);
            }
        }

        [HttpPost]
        [Route("New")]
        public EmailMessage NewSendMail(EmailMessage emailMessage)
        {
            //Using Mail Kit
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            message.Subject = emailMessage.Subject;

            message.Body = new TextPart(TextFormat.Text)
            {
                Text = emailMessage.Content
            };

            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                emailClient.Connect(appSettings.SmtpServer, appSettings.SmtpPort, SecureSocketOptions.StartTls);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(appSettings.Username, appSettings.Password);

                emailClient.Send(message);

                emailClient.Disconnect(true);

                return emailMessage;
            }
        }

    }
}
