using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;

namespace BSIActivityManagement.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "ورود نام کاربری الزامیست")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ورود کلمه عبور الزامیست")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "به خاطر سپرده شود؟")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [EmailAddress]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "ورود کلمه عبور الزامیست")]
        [StringLength(100, ErrorMessage = "کلمه عبور نمیتواند کمتر از {2} حرف باشد", MinimumLength = 5)]
        [DataType(DataType.Password,ErrorMessage = "کلمه عبور وارد شده نامعتبر است لطفا تغییر دهید ")]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "کلمه عبور و تکرار آن با هم همخوانی ندارند")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "ورود نام الزامیست")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "ورود نام خانوادگی الزامیست")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "ورود نام کاربری الزامیست")]
        [Display(Name ="نام کاربری")]
        public string UserName { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public static class IdentityExtensions
    {
        public static string GetOrganizationId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("OrganizationId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
