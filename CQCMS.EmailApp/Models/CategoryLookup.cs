namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CategoryLookup")]
    public partial class CategoryLookup
    {
        [Column("ProductOrService ")]
        [StringLength(255)]
        public string ProductOrService_ { get; set; }

        [StringLength(255)]
        public string RootCause { get; set; }

        [StringLength(255)]
        public string AreaOfOccurence { get; set; }

        [StringLength(255)]
        public string LOB { get; set; }

        [StringLength(255)]
        public string Segment { get; set; }

        [Key]
        public int LookupId { get; set; }

        [StringLength(500)]
        public string Incident { get; set; }

        [StringLength(250)]
        public string Country { get; set; }

        public bool? IsActive { get; set; }
    }
}
