namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailBase")]
    public partial class EmailBase
    {
        [Key]
        public int EmailID { get; set; }

        public int? CaseID { get; set; }

        public int? EmailTypeID { get; set; }

        public int MailboxID { get; set; }

        public DateTime? ReceivedOn { get; set; }

        public DateTime? SentOn { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public string EmailSubject { get; set; }

        [Required]
        public string EmailFrom { get; set; }

        [Required]
        public string EmailTo { get; set; }

        public string EmailCC { get; set; }

        public string EmailBCC { get; set; }

        public string EmailFolder { get; set; }

        public string EmailSubFolder { get; set; }

        public int? EmailStatus { get; set; }

        [StringLength(25)]
        public string EmailDirection { get; set; }

        [StringLength(10)]
        public string Priority { get; set; }

        public bool AwaitingReview { get; set; }

        public DateTime? ReviewedOn { get; set; }

        public DateTime? ReviewedBy { get; set; }

        public bool ReviewerEdited { get; set; }

        public bool? IsEmailComplaintIntegrated { get; set; }

        public string EmailTrimmedSubject { get; set; }

        [StringLength(500)]
        public string Country { get; set; }

        [StringLength(100)]
        public string EmailHash { get; set; }
    }
}
