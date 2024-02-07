using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{

    public partial class UserDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]
        public UserDetail()
        {
            MailboxAccesses = new HashSet<MailboxAccess>();
            Signatures = new HashSet<Signature>();
            UserClientProcessMaps = new HashSet<UserClientProcessMap>();
        }

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
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsReviewRequired { get; set; }
        public int TotalAssignedCases { get; set; }
        public int? WeightedCapacity { get; set; }
        public bool IsAway { get; set; }
        public DateTime? AwayFrom { get; set; }

        public DateTime? AwayTill { get; set; }

        public bool IsOOO { get; set; }
        public DateTime? OOOFrom { get; set; }
        public DateTime? OOOTill { get; set; }
        public string ShortCode { get; set; }
        public DateTime? LastActedon { get; set; }




        [StringLength(50)]

        public string LastActedBy { get; set; }

        public string Country { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailboxAccess> MailboxAccesses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Signature> Signatures { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserClientProcessMap> UserClientProcessMaps { get; set; }
    }
}
