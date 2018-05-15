using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServerBTS2.Models
{
    public class UserBTS
    {
        [Key]
        public string IDUser { get; set; }
        [Required(ErrorMessage = "Ten is required")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "DiaChi is required")]
        public string DiaChi { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime NgaySinh { get; set; }
        [Required(ErrorMessage = "GioiTinh is required")]
        [RegularExpression(@"^Nam$|^Nu$", ErrorMessage = "GioiTinh must be Nam or Nu")]
        public string GioiTinh { get; set; }
        [RegularExpression(@"^.*\.(jpg|jpeg|gif|JPG|png|PNG)$", ErrorMessage = "Image is not valid")]
        public string Image { get; set; }
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Phone is not valid")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "ChucVu is required")]
        [RegularExpression(@"^Admin$|^QuanLy$", ErrorMessage = "ChucVu must be Admin or QuanLy")]
        public string ChucVu { get; set; }
    }
}