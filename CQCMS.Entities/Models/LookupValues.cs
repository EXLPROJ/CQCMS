using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public class LookupValues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LookupValueId { get; set; }
        public int LookupId { get; set; }
        public virtual ManagedLookup managedlookups { get; set; }
        public string LookupValue {  get; set; }
        public int Sequence { get; set; }
        public string ActedBy { get; set; }
        public string Description { get; set; }
        public DateTime ActedOn { get; set; }
        public int? RelatedID { get; set; }
    }
}
