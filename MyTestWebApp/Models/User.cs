using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class User : IdentityUser
    {
        [Required]
        public bool Admin { get; set; }
    }
}