using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class Mail
    {
        public Mail()
        {
            SentOn = DateTime.Now;
            ReceivedOn = DateTime.Now;
        }

        public string MailID { get; set; }
        public string Body { get; set; }

        public string Subject
        {
            get; set;
        }
        public DateTime ReceivedOn { get; set; }
        public DateTime SentOn
        {
            get; set;
        }
        public string From
        {
            get; set;
        }
        public string ToRecipients
        {
            get; set;
        }
        public string CCRecipients
        {
            get; set;
        }
        public string TextBody
        {
            get; set;
        }
        public string Folder { get; set; }
        public string Priority { get; set; }
        public List<Attachment> Attachments { get; set; }

    }

    public class SavedAttachment
    {
        public string OriginalAttachmentName { get; set; }
        public string SavedFilePath { get; set; }
        public bool IsInline { get; set; }
    }
}