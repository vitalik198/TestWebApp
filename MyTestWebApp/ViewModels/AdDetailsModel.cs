using System;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.ViewModels
{
    public class AdDetailsModel
    {
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Display(Name = "Info")]
        public string Text { get; set; }

        [Display(Name = "Photo")]
        public string Image { get; set; }

        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Display(Name = "Date of creation")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Closing date")]
        public DateTime DropTime { get; set; }
    }
}
