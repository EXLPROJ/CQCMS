using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class BackupUserDetailVM
    {
        [Key]
        public int BackupUserMapID { get; set; }

        public int UserID { get; set; }

        public int BackupUserID { get; set; }
        public DateTime? Createdon { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public string Country { get; set; }
    }
}