using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQCMS.Entities.Models;

namespace CQCMS.Entities.Models
{



    public class ManagedLookup

    {

        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int LookupID { get; set; }
        public string LookupName { get; set; }
        public bool IsUserDefined { get; set; }
        public string ActedBy { get; set; }
        public string Country { get; set; }
        public DateTime Actedon { get; set; }
        public int? Dependenton { get; set; }
        public virtual List<LookupValues> LookupChilds { get; set; }
        public virtual ManagedLookup LookupDependent { get; set; }
        public string MappedTo { get; set; }
        public bool ShowOnExternal { get; set; }
        public bool IsRequired { get; set; }

    }
}