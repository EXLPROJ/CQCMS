namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserDetail
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(25)]
        public string EmployeeID { get; set; }

        [Required]
        [StringLength(100)]
        public string EmployeeName { get; set; }

        [Required]
        [StringLength(100)]
        public string EmployeeEmailID { get; set; }

        public bool IsActive { get; set; }

        public bool IsReviewRequired { get; set; }

        public int TotalAssignedCases { get; set; }

        public decimal? WeightedCapacity { get; set; }

        public bool IsAway { get; set; }

        public DateTime? AwayFrom { get; set; }

        public DateTime? AwayTill { get; set; }

        public bool IsOOO { get; set; }

        public DateTime? OOOFrom { get; set; }

        public DateTime? OOOTill { get; set; }

        [StringLength(50)]
        public string ShortCode { get; set; }

        public DateTime? LastActedOn { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        [StringLength(500)]
        public string Country { get; set; }

        public string CountryMultiple { get; set; }

        public string CountryCodeMultiple { get; set; }

        public TimeSpan? ShiftInTime { get; set; }

        public TimeSpan? ShiftOutTime { get; set; }

        public int? TimeZoneID { get; set; }

        [StringLength(100)]
        public string WeekendStartDay { get; set; }

        public DateTime? LastActiveSession { get; set; }

        public bool? IsAwayByShiftTime { get; set; }
    }
}
