namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserToManagerMapping")]
    public partial class UserToManagerMapping
    {
        [Key]
        [StringLength(25)]
        public string EmployeeID { get; set; }

        [StringLength(255)]
        public string EmployeeName { get; set; }

        [StringLength(255)]
        public string EmployeeBusinessEmailAddress { get; set; }

        [StringLength(25)]
        public string EntityManagerEmployeeID { get; set; }

        [StringLength(255)]
        public string EntityManagerEmployeeName { get; set; }

        [StringLength(25)]
        public string FunctionalManagerEmployeeID { get; set; }

        [StringLength(255)]
        public string FunctionalManagerEmployeeName { get; set; }

        [StringLength(250)]
        public string Country { get; set; }
    }
}
