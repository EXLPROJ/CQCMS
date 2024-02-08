namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HolidayList
    {
        [Key]
        public int HolidayId { get; set; }

        public DateTime Date { get; set; }

        [StringLength(255)]
        public string Occasion { get; set; }

        [StringLength(255)]
        public string Day { get; set; }

        [StringLength(255)]
        public string Geography { get; set; }

        public int Year { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}
