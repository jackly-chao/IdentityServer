using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Quickstart.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "The 密码 and 确认密码 do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "手机号")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "昵称")]
        public string NickName { get; set; }

        [Display(Name = "姓")]
        public string LastName { get; set; }

        [Display(Name = "名")]
        public string FirstName { get; set; }

        [Display(Name = "性别")]
        public int Sex { get; set; } = 0;

        [Display(Name = "生日")]
        public DateTime Birth { get; set; } = DateTime.Now;

        [Display(Name = "地址")]
        public string Address { get; set; }

        public string GetFullName()
        {
            return $"{LastName}{FirstName}";
        }
    }
}
