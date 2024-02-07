using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class ComplaintsUpdateModel
    {
        public int CaseNumber { get; set; }
        public string AccountNumber { get; set; }
        public string ReferenceID { get; set; }
        public string CaseActedBy { get; set; }
        public string CaseClosedBy
        {
            get; set;
        }
        public string Status { get; set; }
        public DateTime? CaseActedon { get; set; }
        public DateTime? ResolveDate { get; set; }
        public string AbortCaseReason { get; set; }
        public string Classifier1 { get; set; }
        public string Classifier2 { get; set; }
        public string Classifier3 { get; set; }
        public string Classifier4 { get; set; }
        public string Classifier5 { get; set; }
        public string Classifier6 { get; set; }
        public string Classifier7 { get; set; }

        public string BusinessLineCode { get; set; }
        public string Segment { get; set; }

        public string Impact { get; set; }

        public string ErrorCode { get; set; }
        public string ContactChannel { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }

        public bool? isComplaintUpdatedonEcho { get; set; }
    }
}
