namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            SubCategories = new HashSet<SubCategory>();
        }

        public int CategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; }

        [StringLength(200)]
        public string CategoryShortCode { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(200)]
        public string CategoryKeywords { get; set; }

        [StringLength(108)]
        public string SLA { get; set; }

        [StringLength(50)]
        public string TouchTAT { get; set; }

        [StringLength(50)]
        public string PostSLATouchTAT { get; set; }

        [StringLength(50)]
        public string Complexity { get; set; }

        [StringLength(50)]
        public string ProcessingTime { get; set; }

        public DateTime? LastActed0n { get; set; }

        [StringLength(50)]
        public string LastactedBy { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        public string CustomAttributeMetadata { get; set; }

        [StringLength(50)]
        public string MLThreshold { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
