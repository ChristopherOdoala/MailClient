using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailTest
{
    public interface IEmailConfiguration
    {
        string EscalationHost { get; }
        string EscalationPassword { get; }
        int EscalationPort { get; }
        int POP3EscalationPort { get; }
        string EscalationUsername { get; }


        string SmtpServer { get; }
        string POPServer { get; }
        int SmtpPort { get; }
        int POP3Port { get; }
        string Username { get; }
        string Password { get; }
    }
}
