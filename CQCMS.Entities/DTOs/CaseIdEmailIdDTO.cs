
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.DTOs
{
    public class CaseIdEmailIdDTO
    {
        public int EmailId { get; set; }
        public int? CaseId { get; set; }
        [StringLength(102)]
        public string CurrentlyAssignedTo { get; set; }
        [StringLength(109)]
        public string EmployeeName { get; set; }
        public int CaseStatusID { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string CaseIdIdentifier { get; set; }
        public string ParentCaseAssignedTo { get; set; }
        public int ParentCaseLastEmailId { get; set; }
    }
}

  