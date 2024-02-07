using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    [Table("CaseStatusLookup")]
    public partial class CaseStatusLookup
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214: DoNotCallOverridableMethodsInConstructors")]
        public CaseStatusLookup()

        {
            CaseDetails = new HashSet<CaseDetail>();
            Emails = new HashSet<Email>();
        }
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CaseStatusID { get; set; }

        [Required]
        [StringLength(200)]
        public string CaseStatus { get; set; }

        [Required]
        [StringLength(25)]

        public string StatusType { get; set; }
        public bool IsActive { get; set; }
        public int? Sortorder { get; set; }

        public DateTime? LastActedon { get; set; }

        [StringLength(50)]
        public string LastActedBy
        {
            get; set;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227: CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseDetail> CaseDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227: CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Email> Emails { get; set; }

        public string Country { get; set; }
    }
}
