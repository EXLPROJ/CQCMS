using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class CaseDetailUI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214: DoNlotCal10verridableMethodsInConstructors")]
        [Key]
        public int CaseID { get; set; }

        private int _MailboxID;
        public int MailboxID
        {
            get { return _MailboxID; }
            set
            {
                if (_MailboxID != value)
                {
                    _MailboxID = value;
                }
            }
        }

        private string _EchoCaseNumber;
        public string EchoCaseNumber

        {
            get { return _EchoCaseNumber; }
            set
            {
                if (_EchoCaseNumber != value)
                {
                    _EchoCaseNumber = value;

                }
            }
        }
        [StringLength(100)]

        public string CurrentlyAssignedTo { get; set; }

        [StringLength(102)]
        public string PreviouslyAssignedTo { get; set; }

        public bool IsAssigned { get; set; }
        public DateTime? AssignedTime { get; set; }
        public int CaseStatusID { get; set; }
        public DateTime CreatedOn { get; set; }

        [StringLength(102)]
        public string CreatedBy { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        public string Comments { get; set; }
        public string AdditionalClientInfo { get; set; }
        public bool IsCaseClosed { get; set; }

        public DateTime? ClosedOn { get; set; }

        [StringLength(100)]
        public string ClosedBy { get; set; }

        public bool? IsComplaint { get; set; }
        public bool? IsPhoneCall { get; set; }

        public int? CategoryID { get; set; }
        public int? SubCategoryID { get; set; }
        private string _CIN;

        [StringLength(50)]
        public string CIN
        {
            get { return _CIN; }
            set
            {
                if (CIN != value)
                {
                    _CIN = value;
                }
            }
        }

        [StringLength(500)]
        public string ClientName { get; set; }

        [StringLength(500)]
        public string AccountNumber { get; set; }
        [StringLength(100)]
        public string BusinessSegment { get; set; }
        [StringLength(100)]
        public string BusinessLineCode { get; set; }

        public string PendingStatus { get; set; }
        public string CaseAdditionalDetail { get; set; }
        public int? LastEmailID { get; set; }

        public int? FirstEmailID { get; set; }

        public bool? IsFlagged { get; set; }

        [StringLength(10)]
        public string Priority { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FollowUpDate { get; set; }

        public DateTime? SLADueDate { get; set; }

        public DateTime? TouchDueDate { get; set; }

        public DateTime? CaseReOpenedOn { get; set; }
        public DateTime? ComplaintOn { get; set; }

        public int? ComplaintProductorService { get; set; }
        public int? ComplaintRootCause { get; set; }
        public int? ComplaintAreaofOccurence { get; set; }
        [StringLength(500)]

        public string ComplaintOutcome { get; set; }
        [StringLength(522) ]

        public string ImpactToClient { get; set; }
        [StringLength(500)]

        public string ComplaintErrorCode { get; set; }
        [StringLength(500)]

        public string ComplaintContactChannel { get; set; }

        public bool? IsFeeReversal { get; set; }

        //[RegularExpression(@"*-?\d+\.\d{0,2}$")]
        public decimal? FeeReversalAmount { get; set; }
        public string FeeReversalReason { get; set; }

        public int? CaseAssignAttempts { get; set; }

        public bool? DoesPartialSubjectMatch { get; set; }
        public bool? IsCaseComplaintIntegrated { get; set; }
        public string MatchedPartialCases { get; set; }
        public string Country { get; set; }

        public string EmailFrom { get; set; }

        public bool IsCaseEscalated { get; set; }

        public bool KeepWithMe { get; set; }

        public int NoOfQueries { get; set; }

        public int? EscalationRootCauseID { get; set; }
        public int? EscalationOriginatorID { get; set; }
        public string EscalationOriginatorName { get; set; }
        public bool? IsCFTComplaint { get; set; }

        public bool IsMLClassified { get; set; }

        public int? MLClassifiedCategoryID { get; set; }
        public int? MLClassifiedSubCategoryID { get; set; }
        public string ReClassificationTriggeredBy { get; set; }
        public string ForceReclassificationBy { get; set; }
        public int? MLReclassificationCounter { get; set; }
        public bool? IsCaseAcknowledged { get; set; }

        public DateTime? CaseAckDateTime { get; set; }

        public int? AckMailld { get; set; }

        [StringLength(100)]
        public string AckSentBy { get; set; }
    }
}
