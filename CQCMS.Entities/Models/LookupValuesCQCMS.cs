using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class LookupValuesCQCMS
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LookupValueID { get; set; }
        public int LookupID { get; set; }
        public virtual ManagedLookupCQCMS managelookups { get; set; }
        public string LookupValue { get; set; }
        public int Sequence { get; set; }
        public string ActedBy { get; set; }
        public DateTime ActedOn { get; set; }
    }
}
