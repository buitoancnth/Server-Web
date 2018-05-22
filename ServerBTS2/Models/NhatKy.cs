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
        [Required(ErrorMessage = "IDTram is required")]
        public int IDTram { get; set; }
        [Required(ErrorMessage = "Loai is required")]
        [RegularExpression(@"^SuCo$|^NhatKy$", ErrorMessage = "Loai must be SuCo or NhatKy")]
        public string Loai { get; set; }
        [Required(ErrorMessage = "TieuDe is required")]
        public String TieuDe { get; set; }
        public DateTime ThoiGian { get; set; }
        [Required(ErrorMessage = "NoiDung is required")]
        public string NoiDung { get; set; }
        public Boolean DaGiaiQuyet { get; set; }
    }
}