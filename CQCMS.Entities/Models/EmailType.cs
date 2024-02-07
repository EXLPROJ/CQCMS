using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    [Table("EmailType")]
    public partial class EmailType

    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214;DoNotCallOverridableMethodsInConstructors")]
        public EmailType()
        {

            Emails = new HashSet<Email>();

        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailTypeID { get; set; }

        [Column("EmailType")]
        [Required]
        [StringLength(100)]
        public string EmailType1 { get; set; }

        public bool IsActive { get; set; }
        public DateTime? LastActedon { get; set; }

        [StringLength(50)]
        public string LastActedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft Usage", "CA2227;CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Email> Emails { get; set; }
    }
}
