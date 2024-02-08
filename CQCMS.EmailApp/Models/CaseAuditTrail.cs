namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CaseAuditTrail")]
    public partial class CaseAuditTrail
    {
        [Key]
        public int AuditTrailID { get; set; }

        public int? CaseId { get; set; }

        [Required]
        [StringLength(10)]
        public string Action { get; set; }

        [StringLength(50)]
        public string ActedBy { get; set; }

        public DateTime ActedOn { get; set; }

        public string Comment { get; set; }
    }
}
