using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    public partial class Country
    {
        [Key]
        public int CountryID { get; set; }

        public string Countryname { get; set; }
    }
}