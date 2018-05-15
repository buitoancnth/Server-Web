using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class MatDien
    {
        [Key]
        public int IDMatDien { get; set; }
        [Required(ErrorMessage = "IDNhaTram is required")]
        public int IDTram { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime NgayMatDien { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan GioMatDien { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ThoiGianMayNo { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ThoiGianNgung { get; set; }
        public String TongThoiGianChay { get; set; }
        [Range(0, 1000, ErrorMessage = "QuangDuongDiChuyen should be bigger than 0.")]
        public double QuangDuongDiChuyen { get; set; }
        public double? TienPhat { get; set; }
    }
}