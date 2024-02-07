using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.DTOs
{
    public class CaseAndEmailUpdateDTO
    {
        public CaseDetailVM UpdateCase { get; set; }
        public ComplaintsUpdateModel UpdatedClassifierModel { get; set; }
        public string CurrentUserId { get; set; }
       // public HashTagDTO HashTagDTO { get; set; }
        public string CustomData { get; set; }
        public bool AssignmentNeeded { get; set; }
        public CaseChangeDTO CaseChangeDTO { get; set; }
        public EmailVM UpdateEmail { get; set; }
        public bool IsSavedCalledAfterMICategorization { get; set; }
        //public List<GPIDetailsDTO> GPIDatalist { get; set; }

    }
}


  