using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class EmailAttachmentVM
    {
        [Key]
        public int EmailFileID { get; set; }
        public int EmailID { get; set; }

        public int? CaseID { get; set; }

        [StringLength(500)]
        public string EmailFileName { get; set; }

        [StringLength(500)]
        public string EmailOriginalFileName { get; set; }

        [Required]
        public string EmailFilePath { get; set; }

        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }

        public CaseDetailVM CaseDetail { get; set; }
        public EmailVM Email { get; set; }
        public string Country { get; set; }
        public bool IsInline { get; set; }
    }
}














