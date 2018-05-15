using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class NhatKy
    {
        [Key]
        public int IDNhatKy { get; set; }
        public string IDQuanLy { get; set; }
        public int IDTram { get; set; }
        public DateTime ThoiGian { get; set; }
        [Required(ErrorMessage = "NoiDung is required")]
        public string NoiDung { get; set; }
    }
}