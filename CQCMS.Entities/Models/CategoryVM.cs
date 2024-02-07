using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CQCMS.Entities.Models
{

    public partial class CategoryVM
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]

        public CategoryVM()

        {
            SubCategories = new HashSet<SubCategoryVM>();
        }


        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; }
        [StringLength(200)]

        public string CategoryShortCode { get; set; }

        public bool IsActive { get; set; }
        [StringLength(200)]

        public string CategoryKeywords { get; set; }
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
        public DateTime? LastActedon { get; set; }
        [StringLength(50)]

        public string LastActedBy { get; set; }
        [StringLength(int.MaxValue)]

        public string CustomAttributeMeatadata { get; set; }

        public virtual ICollection<SubCategoryVM> SubCategories {get; set;}
        public string Country {get; set;}
    }
    
}
