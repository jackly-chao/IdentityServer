using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Quickstart.Account
{
    public class GetResetPasswordCodeViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
    }
}
