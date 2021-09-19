using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyTestWebApp.Models
{
    public class Ad
    {
        [Key]
        public Guid AdId { get; set; }

        [Required]
        public int Number { get; set; }

        public User User { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public byte[] Image { get; set; }

        public int Rating { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime DropTime { get; set; }
    }
}
