namespace CQCMS.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailAttachment")]
    public partial class EmailAttachment
    {
        [Key]
        public int EmailFileID { get; set; }

        public int EmailID { get; set; }

        public int? CaseID { get; set; }

        [StringLength(1000)]
        public string EmailFileName { get; set; }

        [StringLength(500)]
        public string EmailOriginalFileName { get; set; }

        [StringLength(500)]
        public string EmailFilePath { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(250)]
        public string Country { get; set; }

        [StringLength(250)]
        public string LastActedBy { get; set; }

        public DateTime? LastActedOn { get; set; }

        public bool IsInline { get; set; }
    }
}
