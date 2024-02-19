using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    [Table("SubCategory")]
    public class SubCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]

        public SubCategory()
        {
            CaseDetails = new HashSet<CaseDetail>();            
        }       
        public int SubCategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string SubCategoryName { get; set; }

        [StringLength(200)]
        public string SubCategoryShortCode { get; set; }

        public bool Isactive { get; set; }

        [StringLength(200)]
        public string SubCategoryKeywords { get; set; }
        
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
        
        public DateTime? LastActedOn { get; set; }

        public string LastActedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft .Usage", "CA2227:CollectionPropertiesShouldBeReadonly")]
        public virtual ICollection<CaseDetail> CaseDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft .Usage", "CA2227:CollectionPropertiesshouldBeReadonly")]
        public virtual Category Category { get; set; }
        public string Country {  get; set; }
    }
}
