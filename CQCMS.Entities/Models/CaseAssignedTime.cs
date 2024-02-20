using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class CaseAssignedTime
    {
        public int CaseID { get; set; }


        [StringLength(100)]
        public string PreviouslyAssignedTo { get; set; }

        [StringLength(100)]
        public string CurrentlyAssignedTo { get; set; }
        public DateTime? AssignedTime { get; set; }

        public DateTime? LastActedOn { get; set; }

        public string LastActedBy { get; set; }
        [StringLength(100)]
        public bool IsAssigned { get; set; }

        public string country { get; set; }

    }
}
