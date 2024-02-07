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
        public int EmailFilerp { get; set; }
        public int EmailmD { get; set; }

        public int? CaseID { get; set; }

        [StringLength(500)]
        public string EmailFileName { get; set; }

        [StringLength(500)]
        public string EmailoriginalFileName { get; set; }

        [Required]
        public string EmailFilePath { get; set; }

        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }

        virtual CaseDetailVM CaseDetail { get; set; }
        virtual EmailVM Email { get; set; }
        string Country { get; set; }
        bool IsInline { get; set; }
    }
}














