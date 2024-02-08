namespace CQCMS.EmailApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailHTMLBody")]
    public partial class EmailHTMLBody
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailID { get; set; }

        public string EmailBody { get; set; }

        public bool IsCompressed { get; set; }

        public byte[] CompressedBody { get; set; }
    }
}
