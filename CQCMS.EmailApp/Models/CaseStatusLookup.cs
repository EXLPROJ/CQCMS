namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CaseStatusLookup")]
    public partial class CaseStatusLookup
    {
        [Key]
        public int CaseStatusID { get; set; }

        [Required]
        [StringLength(200)]
        public string CaseStatus { get; set; }

        [Required]
        [StringLength(25)]
        public string StatusType { get; set; }

        public bool IsActive { get; set; }

        public int? SortOrder { get; set; }

        public DateTime? LastActed0n { get; set; }

        [StringLength(50)]
        public string LastactedBy { get; set; }

        [StringLength(100)]
        public string Country { get; set; }
    }
}
