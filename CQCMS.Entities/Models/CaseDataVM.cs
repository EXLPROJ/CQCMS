
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;

//using System.Ling;

using System.Text;

using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class CaseDataVM
    {

        public int CaseID { get; set; }

        [StringLength(102)]
        public string EchoCasellumber { get; set; }
        public int MailboxID { get; set; }

        public string Comments { get; set; }
        public string OversightComments { get; set; }
        //public string AdditionalClientInfo { get; set; }

        public bool IsCaseClosed { get; set; }

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
        //[StringLength(100) ]

        //public string BusinessSegment { get; set; }
        [StringLength(102)]

        public string BusinessLineCode { get; set; }

        [StringLength(200)]
        public string PendingStatus { get; set; }

        public string CaseAdditionalDetail { get; set; }
        public bool? IsFlagged { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{@:yyyy-MM-dd}")]
        public DateTime? FollowUpDate { get; set; }

        [StringLength(502)]

        public string ComplaintOutcome { get; set; }
        public string ImpactToClient { get; set; }
        [StringLength(500)]

        public string ComplaintErrorCode { get; set; }
        [StringLength(500)]

        public string ComplaintContactChannel { get; set; }

        public bool? IsFeeReversal { get; set; }

        //[RegularExpression(@"*-?\d+\.\d{0,2}$")]
        public decimal? FeeReversalAmount { get; set; }
        public string FeeReversalReason { get; set; }

        [StringLength(102)]
        public string CaseIdIdentifier { get; set; }

        public bool? DoesPartialSubjectMatch { get; set; }
        public string MatchedPartialCases { get; set; }

        public int? LastEmailID { get; set; }
        public int? LastViewedEmailID { get; set; }
        public int? NewEmailCount { get; set; }
       

public bool? IsParentCase { get; set; }
        public int? ParentCaseID { get; set; }

        public int? CloneFromCaseID { get; set; }
        public string EmailFailureToken { get; set; }

        public bool KeepWlithMe { get; set; }

        public int NoOfQueries { get; set; }

        public int? EscalationRootCauseID { get; set; }

        public int? EscalationOriginatorID { get; set; }

        public string EscalationOriginatorName { get; set; }

        public bool IsCaseEscalated { get; set; }

        public string CaseEscalatedManager { get; set; }

        public ComplaintsUpdateModel complaintsUpdateModel { get; set; }

        public string Country { get; set; }
        public string MultiAccountNumber { get; set; }
        public string NOQ1 { get; set; }

        public string NOQ2 { get; set; }
        public string NOQ_3 { get; set; }
        public decimal? NOQI_Score { get; set; }
        public decimal? NOQ2_Score { get; set; }
        public decimal? NOQ3_Score { get; set; }
        public int? CaseStatusID { get; set; }
        public string TransformedMultiAccount

        {

            get
            {
                return String.Join(",", (MultiAccountNumber != null ? (MultiAccountNumber.Split(new char[] { ',' }).Select(c => "[" + c + ":" + c + "]")) : Enumerable.Empty<string>()));
            }
        }



public bool? IsCFTComplaint { get; set; }// Added as CFT complaint flag

        public Dictionary<string, string> CFTenabledCountries { get; set; }// Added To fetch data from config for CFT enabled countries

        public bool? IsMLClassified { get; set; }

        public int? MLClassifiedCategoryID { get; set; }
        public int? MLClassifiedSubCategoryID { get; set; }
        public bool? IsCaseAcknowledged { get; set; }
        public DateTime? CaseAckDateTime { get; set; }
        public int? AckMailId { get; set; }

        [StringLength(109)]
        public string AckSentBy { get; set; }
    }
}
