namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailType")]
    public partial class EmailType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailTypelD { get; set; }

        [Column("EmailType")]
        [Required]
        [StringLength(100)]
        public string EmailType1 { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        [StringLength(100)]
        public string Country { get; set; }
    }
}
