﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailTest.Models
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
            FromAddresses = new List<EmailAddress>();
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}