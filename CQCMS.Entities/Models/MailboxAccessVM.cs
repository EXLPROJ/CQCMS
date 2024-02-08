using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{

    public partial class MailboxAccessVM
    {
        [Key]
        public int MailboxAccessID { get; set; }

        public int UserID { get; set; }

        public int? AllowedMailboxID { get; set; }
        public int? RestrictedMailboxID { get; set; }
        public int? PriorityMailboxID { get; set; }
        public DateTime? Createdon { get; set; }

        [StringLength(50)]

        public string CreatedBy { get; set; }
        public virtual Mailbox Mailbox { get; set; }

        public virtual Mailbox Mailbox1 { get; set; }
        public virtual Mailbox Mailbox2 { get; set; }
        public virtual UserDetail UserDetail { get; set; }
        public string Country {  get; set; }
    }
}
