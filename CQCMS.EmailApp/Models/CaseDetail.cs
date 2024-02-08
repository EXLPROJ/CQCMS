namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CaseDetail
    {
        [Key]
        [Column(Order = 0)]
        public int CaseID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MailboxID { get; set; }

        [StringLength(100)]
        public string CurrentlyAssignedTo { get; set; }

        [StringLength(100)]
        public string PreviouslyAssignedTo { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool IsAssigned { get; set; }

        public DateTime? AssignedTime { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CaseStatusID { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        public string Comments { get; set; }

        public string AdditionalClientInfo { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool IsCaseClosed { get; set; }

        public DateTime? ClosedOn { get; set; }

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

        public DateTime? FollowUpDate { get; set; }

        public DateTime? SLADueDate { get; set; }

        public DateTime? TouchDueDate { get; set; }

        public DateTime? CaseReOpenedOn { get; set; }

        public DateTime? ComplaintOn { get; set; }

        public int? ComplaintProductOrService { get; set; }

        public int? ComplaintRootCause { get; set; }

        public int? ComplaintAreaofOccurence { get; set; }

        [StringLength(500)]
        public string ComplaintOutcome { get; set; }

        [StringLength(500)]
        public string ImpactToClient { get; set; }

        [StringLength(500)]
        public string ComplaintErrorCode { get; set; }

        [StringLength(500)]
        public string ComplaintContactChannel { get; set; }

        public bool? IsFeeReversal { get; set; }

        public decimal? FeeReversalAmount { get; set; }

        public string FeeReversalReason { get; set; }

        public int? CaseAssignAttempts { get; set; }

        public bool? DoesPartialSubjectMatch { get; set; }

        public bool? IsCaseComplaintIntegrated { get; set; }

        public string MatchedPartialCases { get; set; }

        public string UniqueIdentifierGUID { get; set; }

        [StringLength(250)]
        public string NOQ_1 { get; set; }

        [StringLength(250)]
        public string NOQ_2 { get; set; }

        [StringLength(250)]
        public string NOQ_3 { get; set; }

        public decimal? NOQ1_Score { get; set; }

        public decimal? NOQ2_Score { get; set; }

        public decimal? NOQ3_Score { get; set; }

        [StringLength(100)]
        public string CaseIdIdentifier { get; set; }

        [StringLength(580)]
        public string Country { get; set; }

        public int? LastViewedEmailID { get; set; }

        public int? NewEmailCount { get; set; }

        [StringLength(200)]
        public string EmailFailureToken { get; set; }

        public bool? IsParentCase { get; set; }

        public int? ParentCaseID { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool KeepWithMe { get; set; }

        public bool? HasCloneCases { get; set; }

        public int? CloneFromCaseID { get; set; }

        public DateTime? LastUpdateSentOn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? CheckSumCaseID { get; set; }

        public int? CaseEscalatedSourceId { get; set; }

        public bool? IsCaseEscalated { get; set; }

        public int? NoOfQueries { get; set; }

        [StringLength(100)]
        public string CaseEscalatedManager { get; set; }

        public int? EscalationRootCauseID { get; set; }

        public int? EscalationOriginatorID { get; set; }

        public string EscalationOriginatorName { get; set; }

        public int? CustomDataId { get; set; }

        public string MultiAccountNumber { get; set; }

        public string ParsedJSONData { get; set; }

        public DateTime? ClassificationTimeStamp { get; set; }

        public bool? IsCFTComplaint { get; set; }

        public int? MLReclassificationCounter { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool IsMLClassified { get; set; }

        public int? MLClassifiedCategoryID { get; set; }

        public int? MLClassifiedSubCategoryID { get; set; }

        [StringLength(100)]
        public string ReClassificationTriggeredBy { get; set; }

        [StringLength(100)]
        public string ForceReclassificationBy { get; set; }

        public bool? IsCaseAcknowledged { get; set; }

        public DateTime? CaseAckDateTime { get; set; }

        public int? AckMailId { get; set; }

        [StringLength(10)]
        public string AckSentBy { get; set; }
    }
}
