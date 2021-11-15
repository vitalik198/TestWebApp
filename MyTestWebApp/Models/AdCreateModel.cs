using System;
using System.ComponentModel.DataAnnotations;

namespace MyTestWebApp.Models
{
    public class AdCreateModel
    {
        [Key]
        public Guid AdId { get; set; }

        [Required]
        [Display(Name = "Номер")]
        public int Number { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Информация")]
        public string Text { get; set; }

        /// <summary>
        ///  must be base64 bytes array
        /// </summary>
        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Фото")]
        public byte[] Image { get; set; }
    }
}
