using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.ViewModels
{
    public class AdCreateApiModel
    {
        [Required]
        [FromForm(Name ="number")]
        public int Number { get; set; }

        [Required]
        [FromForm(Name = "text")]
        public string Text { get; set; }

        [Required]
        [FromForm(Name = "image")]
        public IFormFile Image { get; set; }
    }
}
