using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ServerBTS2.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Ten is required")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "DiaChi is required")]
        public string DiaChi { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode =true)]
        public DateTime NgaySinh { get; set; }
        [Required(ErrorMessage ="GioiTinh is required")]
        [RegularExpression(@"^Nam$|^Nu$", ErrorMessage = "GioiTinh must be Nam or Nu")]
        public string GioiTinh { get; set; }
        [Phone(ErrorMessage = "Phone is not valid")]
        public string Phone { get; set; }
        [RegularExpression(@"^.*\.(jpg|jpeg|gif|JPG|png|PNG)$", ErrorMessage = "Image is not valid")]
        public string Image { get; set; }
        [Required(ErrorMessage = "ChucVu is required")]
        [RegularExpression(@"^Admin$|^QuanLy$", ErrorMessage = "ChucVu must be Admin or QuanLy")]
        public string ChucVu { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangeRole
    {
        [EmailAddress(ErrorMessage = "UserName is not valid")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ChucVu is required")]
        [RegularExpression(@"^Admin$|^QuanLy$", ErrorMessage = "ChucVu must be Admin or QuanLy")]
        public string Role { get; set; }


    }
}
