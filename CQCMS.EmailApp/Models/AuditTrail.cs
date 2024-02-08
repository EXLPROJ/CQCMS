namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuditTrail")]
    public partial class AuditTrail
    {
        [Required]
        [StringLength(10)]
        public string Action { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string RowID { get; set; }

        public DateTime ChangeDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Module { get; set; }

        [StringLength(10)]
        public string Field { get; set; }

        public string Oldvalue { get; set; }

        public string NewValue { get; set; }

        public int AuditTrailID { get; set; }
    }
}
