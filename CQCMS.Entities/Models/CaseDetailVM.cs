using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class CaseDetailVM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]
        public CaseDetailVM()
        {
            Emails = new HashSet<EmailVM>();
            EmailAttachments = new HashSet<EmailAttachmentVM>();
        }

        [Key]
        public int CaseID { get; set; }
        public int MailboxID { get; set; }

        [StringLength(100)]
        public string CurrentlyAssignedTo { get; set; }

        [StringLength(100)]
        public string PreviouslyAssignedTo { get; set; }
        public bool IsAssigned { get; set; }

        public DateTime? AssignedTime { get; set; }
        public int CaseStatusID { get; set; }
        public DateTime Createdon { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }
        public string Comments { get; set; }
        public string OversightComments { get; set; }
        public string AdditionalClientInfo { get; set; }
        public bool IsCaseClosed { get; set; }
        public DateTime? Closedon { get; set; }

        [StringLength(100)]
        public string ClosedBy { get; set; }

        public bool? IsComplaint { get; set; }
        public bool? IsPhoneCall { get; set; }
        public int? CategoryID { get; set; }
        public int? SubCategoryID { get; set; }
        [StringLength(50)]

        public string CIN { get; set; }
        [StringLength(500)]

        public string ClientName { get; set; }
        [StringLength(500)]

        public string AccountNumber { get; set; }
        [StringLength(100)]

        public string BusinessSegment { get; set; }
        //{StxingLength (190) ]

        public string BusinessLineCode { get; set; }
       
        [StringLength(200)]

        public string PendingStatus { get; set; }

        public string CaseAdditionalDetail { get; set; }

        public int? LastEmailID { get; set; }

        public int? FirstEmailID { get; set; }

        public bool? IsFlagged { get; set; }

        [StringLength(10)]

        public string Priority { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0;yyyy-MM-dd}")]
        public DateTime? FollowUpDate { get; set; }

        public DateTime? SLADueDate { get; set; }

        public DateTime? TouchDueDate { get; set; }

        public DateTime ?CaseReOpenedOn { get; set; }

        public DateTime? ComplaintOn { get; set; }

        public bool? IsFeeReversal { get; set; }

        // {RegularExpression (@"*-;\d+\ .\d{0,25")]

        public decimal? FeeReversalamount { get; set; }

        public string FeeReversalReason { get; set; }

        public int? CaseAssignAttempts { get; set; }

        public bool? DoesPartialSubjectMatch { get; set; }

        public bool? IsCaseComplaintintegrated { get; set; }

        public string MatchedPartialCases { get; set; }

        public string UniqueIdentifierGUID { get; set; }
        public string CheckSumCaseID { get; }
        [StringLength(200)]

        public string CaseIdIdentifier { get; set; }
        public int? LastViewedEmailID { get; set; }
        public int? NewEmailcount { get; set; }

        public int? EacalationRootCauseID { get; set; }

        public int? EacalationOriginatorID { get; set; }

        public string EscalationOziginatorName { get; set; }

        public bool IsCaseEscalated { get; set; }

        public string CaseEscalatedManager { get; set; }

        public virtual CaseStatusLookup CaseStatusLookup { get; set; }

        public virtual CategoryVM Category { get; set; }

        public virtual MailboxVM Mailbox { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailVM> Emails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailAttachmentVM> EmailAttachments { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SignerInformation> SignerInformation { get; set; }
        public virtual SubCategoryVM SubCategory { get; set; }

        public string Country { get; set; }

        public string SelectedCaseCountry { get; set; }

        public bool? IsParentCase { get; set; }

        public int ? ParentCaseID { get; set; }

        public bool HasCloneCases { get; set; }

        public int? CloneFronCaseID { get; set; }

        public string EmailFailureToken { get; set; }

        public bool KeepWithMe { get; set; }

        public string InternationalAssignedTo { get; set; }

        public int NoOfQueries { get; set; }

        public string MultiAccountNumber { get; set; }

        public ComplaintsUpdateModel complaintsUpdateModel { get; set; }
        [StringLength(200)]

        public string EmployeeName { get; set; }

        public string NOQ_1 { get; set; }

        public bool? IsCaseAcknowledged { get; set; }

    }






}














