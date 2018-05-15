using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class BaoCao
    {
        [Key]
        public int IDBaoCao { get; set; }
        public string IDQuanLy { get; set; }
        public DateTime ThoiGian { get; set; }
        [Required(ErrorMessage = "VanDe is required")]
        public string VanDe { get; set; }
    }
}