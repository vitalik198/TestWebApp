using System;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.ViewModels
{
    public class AdDetailsModel
    {
        [Display(Name = "Номер")]
        public int Number { get; set; }

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
