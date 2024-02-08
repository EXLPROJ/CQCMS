using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Entities.Models
{
    [Table ("HolidayList")]
    public partial class Holiday
    {
        [Key]
        public int HolidayId { get; set; }

        public DateTime Date { get; set; }
        
        public string Occasion { get; set; }
        
        public string Day { get; set; }

        public string Geography { get; set; }

        public int Year { get; set; }
    }
}
