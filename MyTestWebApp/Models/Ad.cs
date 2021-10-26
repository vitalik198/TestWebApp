using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    [Bind(include: "AdId,Number,Text")]
    public class Ad
    {
        [Key]
        public Guid AdId { get; set; }

        [Required]
        [Display(Name = "Номер")]
        public int Number { get; set; }

        [Display(Name = "Пользователь")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Информация")]
        public string Text { get; set; }

        [Display(Name = "Фото")]
        public string Image { get; set; }

        [Display(Name = "Рейтинг")]
        public int Rating { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Дата закрытия")]
        public DateTime DropTime { get; set; }
    }
}
