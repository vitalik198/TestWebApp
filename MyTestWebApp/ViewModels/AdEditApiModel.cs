using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.ViewModels
{
    public class AdEditApiModel
    {
        [Required]
        [FromForm(Name = "number")]
        public int Number { get; set; }

        [Required]
        [FromForm(Name = "text")]
        public string Text { get; set; }

        [FromForm(Name = "image")]
        public IFormFile? Image { get; set; }
    }
}
