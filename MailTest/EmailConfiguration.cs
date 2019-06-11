using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MailTest
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string EscalationHost { get; private set; }
        public string EscalationPassword { get; private set; }
        public int EscalationPort { get; private set; }
        public int POP3EscalationPort { get; private set; }
        public string EscalationUsername { get; private set; }

        public string SmtpServer { get; private set; }
        public string POPServer { get; private set; }
        public int SmtpPort { get; private set; }
        public int POP3Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }



        public EmailConfiguration()
        {
            EscalationHost = ConfigurationManager.AppSettings["EscalationHost"];
            EscalationPassword = ConfigurationManager.AppSettings["EscalationPassword"];
            EscalationPort = int.Parse(ConfigurationManager.AppSettings["EscalationPort"]);
            POP3EscalationPort = int.Parse(ConfigurationManager.AppSettings["POP3EscalationPort"]);
            EscalationUsername = ConfigurationManager.AppSettings["EscalationUsername"];

            SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
            POPServer = ConfigurationManager.AppSettings["POPServer"];
            SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            POP3Port = int.Parse(ConfigurationManager.AppSettings["POP3Port"]);
            Username = ConfigurationManager.AppSettings["Username"];
            Password = ConfigurationManager.AppSettings["Password"];
        }
    }
}