using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class ViewLoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string? ReturnUrl {  get; set; }
    }
}
