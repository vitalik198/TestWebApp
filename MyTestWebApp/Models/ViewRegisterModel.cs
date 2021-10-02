using System;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class ViewRegisterModel
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password {  get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password Confirm")]
        [Compare("Password", ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]  
        [Display(Name = "Name")]
        public string UserName {  get; set; }

        [Required]
        [Display(Name ="IsAdmin")]
        public bool IsAdmin {  get; set; }
    }
}
