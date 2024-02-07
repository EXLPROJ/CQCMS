using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.DTOs
{
    public partial class MailboxEmailSearchDTO
    {
        public string SourceFolder { get; set; }
        public int MailboxId { get; set; }
        public string MoveToFolder { get; set; }
        public string Country { get; set; }
        public int? TotalEmailToRead { get; set; }
        public DateTime? ReceivedFrom { get; set; }
        public DateTime? ReceivedTo { get; set; }
    }
}
