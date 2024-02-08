namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AppConfiguration")]
    public partial class AppConfiguration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppConfigurationId { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(200)]
        public string ConfigurationKey { get; set; }

        public string ConfigurationValue { get; set; }

        [StringLength(100)]
        public string Notes { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(100)]
        public string LastActedBy { get; set; }

        public DateTime? LastActedOn { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Country { get; set; }
    }
}
