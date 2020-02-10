using System.ComponentModel.DataAnnotations;

namespace EMR.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جاری")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تایید رمز عبور جدید")]
        [Compare("NewPassword", ErrorMessage = "پسورد جدید و تاییدیه پسورد جدید برابر نیستند")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "بخاطر آوردن")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تایید رمز عبور")]
        [Compare("Password", ErrorMessage = "پسورد جدید و تاییدیه پسورد جدید برابر نیستند")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "نوع کاربری")]
        public string RoleId { get; set; }
    }
}
