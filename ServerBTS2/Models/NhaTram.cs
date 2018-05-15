using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class NhaTram
    {
        [Key]
        public int IDNhaTram { get; set; }
        [Required(ErrorMessage = "IDNhaMang is required")]
        public int IDNhaMang { get; set; }
        [Required(ErrorMessage = "IDTram is required")]
        public int IDTram { get; set; }
        [Range(0, 15, ErrorMessage = "CauCap should be in 0 to 15 range.")]
        public int CauCap { get; set; }
        [Range(0, 15, ErrorMessage = "HeThongDien should be in 0 to 15 range.")]
        public int HeThongDien { get; set; }
        [Range(0, 15, ErrorMessage = "HangRao should be in 0 to 15 range.")]
        public int HangRao { get; set; }
        [Range(0, 15, ErrorMessage = "DieuHoa should be in 0 to 15 range.")]
        public int DieuHoa { get; set; }
        [Range(0, 15, ErrorMessage = "OnAp should be in 0 to 15 range.")]
        public int OnAp { get; set; }
        [Range(0, 15, ErrorMessage = "CanhBao should be in 0 to 15 range.")]
        public int CanhBao { get; set; }
        [Range(0, 15, ErrorMessage = "BinhCuuHoa should be in 0 to 15 range.")]
        public int BinhCuuHoa { get; set; }
        [Range(0, 15, ErrorMessage = "MayPhatDien should be in 0 to 15 range.")]
        public int MayPhatDien { get; set; }
        [Required(ErrorMessage = "ChungMayPhat is required")]
        public bool ChungMayPhat { get; set; }
    }
}