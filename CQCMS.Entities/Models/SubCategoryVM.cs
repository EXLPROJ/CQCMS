using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CQCMS.Entities.Models
{

    public partial class SubCategoryVM
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]
        public SubCategoryVM()
        {
            //CaseDetails = new HashSet<CaseDetail>();

        }

        [Key]
        public int SubCategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string SubCategoryName { get; set; }
        public int CategoryID { get; set; }
        [StringLength(200)]

        public string SubCategoryShortCode { get; set; }
        public bool IsActive { get; set; }
        [StringLength(200)]

        public string SubCategoryKeywords { get; set; }
        [StringLength(500)]

        public string EchoQuickLookup { get; set; }
        [StringLength(50)]

        public string QNCode { get; set; }
        [StringLength(100)]

        public string SLA { get; set; }
        [StringLength(50)]

        public string TouchTAT { get; set; }
        [StringLength(50)]

        public string PostSLATouchTAT { get; set; }
        [StringLength(50)]

        public string Complexity { get; set; }
        [StringLength(50)]

        public string ProcessingTime { get; set; }
        public bool? IsExternalClient { get; set; }
        [StringLength(200)]

        public string ExternalEmailID { get; set; }
        public DateTime? LastActedOn { get; set; }
        
        public string LastActedBy { get; set; }
        public virtual CategoryVM Category { get; set; }
        public string Country { get; set; }

        [StringLength(int.MaxValue)]

        public string CustomAttributeMetadata { get; set; }
    }
}