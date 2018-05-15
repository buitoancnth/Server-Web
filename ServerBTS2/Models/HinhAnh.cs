using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class HinhAnh
    {
        [Key]
        public int IDHinhAnh { get; set; }
        [Required(ErrorMessage = "IDTheLoaiAnh is required")]
        public int IDTheLoaiAnh { get; set; }
        [Required(ErrorMessage = "IDChung is required")]
        public int IDChung { get; set; }
        [Required(ErrorMessage = "TenTheLoai is required")]
        public String TenAnh { get; set; }
    }
}