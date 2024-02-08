namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubCategory")]
    public partial class SubCategory
    {
        public int SubCategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string SubCategoryName { get; set; }

        public int CategoryID { get; set; }

        [StringLength(200)]
        public string SubCategoryShortCode { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(20)]
        public string SubCategoryKeywords { get; set; }

        [StringLength(50)]
        public string Encode { get; set; }

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

        [StringLength(50)]
        public string LastActedBy { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        public string CustomAttributeMetadata { get; set; }

        [StringLength(50)]
        public string MLThreshold { get; set; }

        public bool? IsCustomAttribute { get; set; }

        public bool? IsCustomAttributeCloseCase { get; set; }

        public virtual Category Category { get; set; }
    }
}
