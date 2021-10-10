using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class ViewLoginApiModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
