using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class ViewLoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
