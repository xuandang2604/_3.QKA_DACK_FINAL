using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace _3.QKA_DACK.Models.Another
{
    public class ApplicationUser : IdentityUser
    {

        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
    }
}
