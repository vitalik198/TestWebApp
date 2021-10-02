using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class ViewLoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        public string? ReturnUrl {  get; set; }
    }
}
