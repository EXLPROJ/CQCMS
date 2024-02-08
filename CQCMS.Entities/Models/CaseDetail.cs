using CQCMS.Entities.Models;
using CQCMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class CaseDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]
        public CaseDetail()
        {
            Emails = new HashSet<Email>();
            EmailAttachments = new HashSet<EmailAttachment>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CaseID { get; set; }
        public int MailboxID { get; set; }

        [StringLength(100)]
        public string EchoCaseNumber { get; set; }

        [StringLength(100)]
        public string CurrentlyAssignedTo { get; set; }

        [StringLength(100)]
        public string PreviouslyAssignedTo { get; set; }
        public bool IsAssigned { get; set; }

        public DateTime AssignedTime { get; set; }
        public int CaseStatusID { get; set; }
        public DateTime Createdon { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }
        public string Comments { get; set; }
        public string OversightComments { get; set; }
        public string AdditionalClientInfo { get; set; }
        public bool IsCaseClosed { get; set; }
        public DateTime Closedon { get; set; }

        [StringLength(100)]
        public string ClosedBy { get; set; }

        public bool IsComplaint { get; set; }
        public bool IsPhoneCall { get; set; }
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        [StringLength(50)]

        public string CIN { get; set; }
        [StringLength(500)]

        public string ClientName { get; set; }
        [StringLength(500)]

        public string NOQ_1 { get; set; }
        public string NOQ_2 { get; set; }
        public string NOQ_3 { get; set; }
        public decimal? NOQ1_Score { get; set; }
        public decimal? NOQ2_Score { get; set; }
        public decimal? NOQ3_Score { get; set; }
        public string Country { get; set; }

        public bool HasCloneCases { get; set; }
        public int? CloneFromCaseID { get; set; }
        public string MultiAccountNumber { get; set; }
        [StringLength(500)]

        public string AccountNumber { get; set; }
        [StringLength(100)]

        public string BusinessSegment { get; set; }
        
        [StringLength(100)]

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

        public DateTime? CaseReOpenedOn { get; set; }

        public DateTime? Complainton { get; set; }

        public int? ComplaintProductorService { get; set; }
        public int? ComplaintRootCause { get; set; }
        public int? ComplaintAreaofOccurence { get; set; }

        [StringLength(500)]
        public string ComplaintOutcome { get; set; }




        public string ImpactToClient { get; set; }
        [StringLength(500)]

        public string ComplaintErrorCode { get; set; }
        
        [StringLength(500)]

        public string ComplaintContactChannel { get; set; }

        public bool? IsFeeReversal { get; set; }
        
        // (RegalarExpression (@"*-?\d+\ .\d(0,25")]
        public decimal? FeeReversalamount { get; set; }
        public string FeeReversalReason { get; set; }
        public int? CaseAssignAttempts { get; set; }
        public bool? DoesPartialSubjectMatch { get; set; }
        public bool? IsCaseComplaintIntegrated { get; set; }
        
        public string MatchedPartialCases { get; set; }

        public string UniqueldentifierGUID { get; set; }

        [StringLength(100)]

        public string CaseIdIdentifer { get; set; }
    

        public int? LastViewedEmailID { get; set; }
        
        public int? NewEmailCount { get; set; }
        public bool? IsParentCase { get; set; }
        public int? ParentCaseID { get; set; }

        public string EmailFailureToken { get; set; }
        
        public string InternationalAssignedTo { get; set; }
        
        public bool KeepWithMe { get; set; }
        
        public DateTime? LastUpdateSentOn { get; set; }
        
        public bool IsCaseEscalated { get; set; }
        
        public string CaseEscalatedManager { get; set; }
        
        public int? EscalationRootCauseID { get; set; }
        
        public int? EscalationOriginatorID { get; set; }
        
        public string EscalationOriginatorName { get; set; }
        
        public virtual CaseStatusLookup CaseStatusLookup { get; set; }
        public virtual Category Category { get; set; }
        public virtual Mailbox Mailbox { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Email> Emails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailAttachment> EmailAttachments { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SignerInformation> SignerInformation { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CaseLiveSignSigner> CaseLiveSignSigner { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SignerGroup> SignerGroups { get; set; }

        public int NoOfQueries { get; set; }
        public int? CustomDataId { get; set; }
        public string ParsedJSONData{ get; set; }
        [NotMapped]
        public string TagName{ get; set; }

    }






}














