using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CQCMS.Entities.Models
{

    [Table("Email")]
    public partial class Email
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214: DoNotCallOverridableMethodsInConstructors")]
        public Email()
        {
            Emailattachments = new HashSet<EmailAttachment>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailID { get; set; }
        public int? CaseID { get; set; }
        public int? EmailtypeID { get; set; }
        public int MailboxID { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime? SentOn { get; set; }
        public DateTime? LastActedon { get; set; }
        [StringLength(50)]
        public string LastActedBy { get; set; }
        public DateTime Createdon { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; }
        [Required]
        public string Emailsubject { get; set; }
        [Required]
        public string EmailFrom { get; set; }
        public string Emailto { get; set; }
        public string Emailcc { get; set; }
        public string EmailBcc { get; set; }
        public string TextBody { get; set; }
        public string EmailBody { get; set; }
        [StringLength(200)]
        public string EmailFolder { get; set; }
        [StringLength(200)]
        public string EmailSubFolder { get; set; }

        public int? Emailstatus { get; set; }
        [StringLength(25)]

        public string EmailDirection { get; set; }

        public bool? EchoStatus { get; set; }

        public DateTime? EchoLockedon { get; set; }

        [StringLength(100)]

        public string EchoLockedBy { get; set; }

        [StringLength(20)]

        public string EchoAttempts { get; set; }

        public bool? IsEchoLocked { get; set; }

        [StringLength(20)]

        public string Priority { get; set; }

        public virtual CaseDetail CaseDetail { get; set; }

        public virtual CaseStatusLookup CaseStatusLookup { get; set; }
        public virtual EmailType Emailtype { get; set; }

        public virtual Mailbox Mailbox { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227: CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailAttachment> Emailattachments { get; set; }
        public bool AwaitingReview { get; set; }

        public DateTime? ReviewedOn { get; set; }

        public DateTime? ReviewedBy { get; set; }

        public bool ReviewerEdited { get; set; }

        public bool? IsEmailComplaintIntegrated { get; set; }

        public string EmailTrimmedSubject { get; set; }

        public string Country { get; set; }

        public string EmailHash { get; set; }

        public int? EchoAttemptsNum { get; set; }
    }
}
