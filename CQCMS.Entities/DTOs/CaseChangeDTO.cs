
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.DTOs
{

    public class CaseChangeDTO
    {
        public bool IsCaseTurnedToComplaint { get; set; }

        public bool IsCaseClassifiedforFirstTime { get; set; }

        public bool IsNewCommentAddedToComplaints { get; set; }

        public bool Reclassified { get; set; }
        public bool IsNoOfQueriesChanged { get; set; }

        public bool IsNewCase { get; set; }

        public int CaseID { get; set; }

        public string Country { get; set; }

        public bool CaseReopened { get; set; }

        public bool EnforceMakerChecker { get; set; }
        public bool IsClientChanged { get; set; }
        public bool IsSubcategoryChanged { get; set; }
        public bool IsHashTagChanged { get; set; }
        public bool IsUserOOO { get; set; }

        public bool IsUserAway { get; set; }

        public bool HasNewEmail { get; set; }

        public bool IsReviewNeeded { get; set;}

        public bool IsNewEmailOnRolloverCase { get; set; }
        public bool IsNewEmailOnClosedCase { get; set; }

        [StringLength(100)]
        public string CurrentUserId { get; set; }
        public int SubCategoryID { get; set; }
        public int MailboxID { get; set; }
    }

}