using System;
using System.Collections.Generic;
using System.Text;

namespace EmailForumUpdates.RepoModels
{
    public class SmtpSettings
    {
        // not actually an Smtp properties, but working quick and dirty
        public string ForumTitle { get; set; }
        // this info not currently stored in the PopForums DB!
        public bool IsHttpSsl { get { return true; } }
        public string DomainName { get { return "anzics.nz";  } }
        // not bothering with Port for now
        //
        public string SmtpPassword { get; set; } 
        public int SmtpPort { get; set; } 
        public string SmtpServer { get; set; } 
        public string SmtpUser { get; set; } 
        public bool UseSslSmtp { get; set; }
        public bool UseEsmtp { get; set; }
        public bool IsMailerEnabled { get; set; } 
        public string MailerAddress { get; set; } 
        public int MailerQuantity{ get; set; } 
        public int MailSendingInverval{ get; set; }
        public string MailSignature{ get; set; }
    }
}

