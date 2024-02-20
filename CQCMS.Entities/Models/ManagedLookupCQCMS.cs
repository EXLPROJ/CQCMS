using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class ManagedLookupCQCMS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LookupID { get; set; }
        public string LookupName { get; set; }
        public string ActedBy { get; set; }
        public string Country { get; set; }
        public DateTime ActedOn { get; set; }
        public virtual List<LookupValues> LookupChilds { get; set; }

    }
}
