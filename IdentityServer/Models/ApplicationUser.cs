using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>
    {
        public string NickName { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public int Sex { get; set; } = 0;

        public DateTime Birth { get; set; } = DateTime.Now;

        public string Address { get; set; }

        public int Status { get; set; } = 0;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string GetFullName()
        {
            return $"{LastName}{FirstName}";
        }
    }
}
