using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class EmailAttachmentInsert

    {

        [Key]
        public int EmailFileID
        {
            get; set;
        }
        public int EmailID { get; set; }
        public int? CaseID { get; set; }

        [StringLength(500)]
        public string EmailFileName { get; set; }

        [StringLength(500)]
        public string EmailOriginalFileName
        {
            get; set;
        }
        [Required]
        public string EmailFilePath { get; set; }

        public bool Isactive { get; set; }
        public DateTime? Createdon { get; set; }

        public string Country { get; set; }
        public string LastActedBy { get; set; }

        public DateTime? LastActedOn { get; set; }
        public bool IsInline { get; set; }

    }
}
