using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.DTOs
{
    public class ReleaseCaseDTO
    { 
        public int? CaseId { get; set; }

        public string ReleaseToUser{ get; set; }

        public string ReleaseFromUser { get; set; }

        public string ReleaseType{ get; set; }

        public string Country { get; set; }

        public string CurrentUserId { get; set; }

        public string MultipleCaseId { get; set; }

        public bool IsreleaseAll { get; set; }
    }
}
