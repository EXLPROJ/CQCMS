using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class SubCategoryDisplayVM
    {
        public int SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryShortCode { get; set; }
        public string Complexity { get; set; }
        public string SLA { get; set; }
        public string TouchTAT { get; set; }
        public string PostSLATouchTAT { get; set; }
        public string ProcessingTime { get; set; }
        public string CustomAttributeMetadata { get; set; }
        public string Country { get; set; }
        public string MLThreshold { get; set; }
    }
}
