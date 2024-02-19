using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class EmailVM

    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214: DoNotCallOverridableMethodsInConstructors")]
    

        public EmailVM()
        {
            EmailAttachments = new HashSet<EmailAttachmentVM>();
        }

        public int EmailID { get; set; }

        public int? CaseID { get; set; }

        public int? EmailTypeID { get; set; }
        public int MailboxID { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime? SentOn { get; set; }
        [NotMapped]
        public DateTime? LastActedOn { get; set; }
        
        public string LastActedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        

        public string CreatedBy { get; set; }
        [Required]

        [StringLength(200)]
        public string EmailSubject { get; set; }

        [Required]

        public string EmailFrom { get; set; }

        public string EmailTo { get; set; }

        public string EmailCC { get; set; }

        public string EmailBCC { get; set; }

        public string TextBody { get; set; }

        public string EmailBody { get; set; }
        [StringLength(200)]

        public string EmailFolder { get; set; }
        [StringLength(200)]

        public string EmailSubFolder { get; set; }
        public int? EmailStatus { get; set; }
        [StringLength(25)]

        public string EmailDirection { get; set; }
        public bool? EchoStatus { get; set; }

        public DateTime? EchoLockedOn { get; set; }
        [StringLength(100)]

        public string EcholockedBy { get; set; }
        [StringLength(20)]

        public string EchoAttempts { get; set; }

        public bool? IsEcholocked { get; set; }
        [StringLength(20)]

        public string Priority { get; set; }

        public virtual CaseDetailVM CaseDetail { get; set; }
        public virtual CaseStatusLookup CaseStatusLookup { get; set; }
        public virtual EmailType EmailType { get; set; }
        public virtual MailboxVM Mailbox { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227: CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailAttachmentVM> EmailAttachments { get; set; }
        public bool AwaitingReview { get; set; }

        public DateTime? ReviewedOn { get; set; }

        public DateTime? ReviewedBy { get; set; }

        public bool ReviewerEdited { get; set; }

        public bool? IsmailComplaintIntegrated { get; set; }
        public string EmailTrimmedSubject { get; set; }

        public string Country { get; set; }

        public string EmailHash { get; set; }

        public string BccReceipients { get; set; }

        public string AttachmentTempFolder { get; set; }

        public bool SaveAsDraft { get; set; }

        public bool SendAndClose { get; set; }

        public string Direction { get; set; }

        public bool IsCaseEscalated { get; set; }

        public string AutoReplyInfo { get; set; }

        public int orginalEmailid { get; set; }

        public string ListExistingFilesToRemoveForward { get; set; }
        public string CurrentUserId { get; set; }

        public string UserName { get; set; }

        public string AttachmentPath { get; set; }

        public string EmailClassificationLookup { get; set; }
        public List<SavedAttachment> savedAttachmentDetails { get; set; }
    }
}