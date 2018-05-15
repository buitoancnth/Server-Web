using System;
using System.ComponentModel.DataAnnotations;

namespace ServerBTS2.Models
{
    public class TheLoaiAnh
    {
        [Key]
        public int IDTheLoaiAnh { get; set; }
        [Required(ErrorMessage = "TenTheLoai is required")]
        public String TenTheLoai { get; set; }
    }
}