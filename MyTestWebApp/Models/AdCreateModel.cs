using System;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class AdCreateModel
    {
        [Required]
        [Display(Name = "Номер")]
        public int Number { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Информация")]
        public string Text { get; set; }

        public string Image { get; set; }
    }
}
