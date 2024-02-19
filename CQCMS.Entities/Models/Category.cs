using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    [Table("Category")]
    public partial class Category
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]

        public Category()
        {
            CaseDetails = new HashSet<CaseDetail>();
            SubCategories = new HashSet<SubCategory>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; }

        [StringLength(200)]
        public string CategoryShortCode { get; set; }

        public bool Isactive { get; set; }

        [StringLength(200)]
        public string CategoryKeywords { get; set; }

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

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        [StringLength(50)]
        public string MLThreshold { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft .Usage", "CA2227:CollectionPropertiesShouldBeReadonly")]

        public virtual ICollection<CaseDetail> CaseDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft .Usage", "CA2227:CollectionPropertiesshouldBeReadonly")]
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
