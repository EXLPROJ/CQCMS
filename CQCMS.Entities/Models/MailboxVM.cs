using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class MailboxVM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]

        public MailboxVM()
        {
        }
        [Key]
        public int MailboxID { get; set; }
        [Required(ErrorMessage = "Mailbox Email ID is required")]
        [StringLength(200)]
        public string MailboxName { get; set; }

        [Required(ErrorMessage = "MailBox Type is required")]
        [StringLength(100)]
        public string MailboxType { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]

        public bool IsReviewRequired { get; set; }
        [Required(ErrorMessage = "Mailbox Folder is required")]

        [StringLength(200)]
        public string MailFolder { get; set; }
        [Required(ErrorMessage = "Mailbox MoveTo Folder is required")]
        [StringLength(200)]
        public string MoveToFolder { get; set; }
        [Required(ErrorMessage = "Mailbox Exception Folder is required")]
        [StringLength(200)]
        public string ExceptionFolder { get; set; }
        [RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string CPRApplicationName { get; set; }
        [RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string CPRApplicationName2 { get; set; }

        public string Country { get; set; }

        public virtual string[] Countrys { get; set; }

        public DateTime? LastActedon { get; set; }

        [StringLength(50)]

        public string LastActedBy { get; set; }

        public DateTime? BotLockedon { get; set; }

        [StringLength(50)]

        public string BotLockedBy { get; set; }

        public DateTime? BotReleasedon { get; set; }

        public Int64? BotLockedTimeTicks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseDetail> CaseDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Email> Emails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailboxAccess> MailboxAccesses { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Signature> Signatures { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailboxAccess> MailboxAccesses1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailboxAccess> MailboxAccesses2 { get; set; }
        //added by dines

        public string EWSEndpoint { get; set; }

    }
}
