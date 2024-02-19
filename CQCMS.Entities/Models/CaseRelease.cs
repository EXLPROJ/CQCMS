using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class CaseRelease
    {

        public int CaseId { get; set; }
        [StringLength(100)]
        public string CurrentlyAssignedTo { get; set; }
        [StringLength(100)]
        public string PreviouslyAssignedTo { get; set; }

        public bool  IsAssigned { get; set; }

        public DateTime? AssignedTime { get; set; }

        public int caseAssignAttempts { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(100)]
        public string LastActedby { get; set; }

        public string CaseEscalatedManager { get; set; }

        public string country { get; set; }
    }
}
