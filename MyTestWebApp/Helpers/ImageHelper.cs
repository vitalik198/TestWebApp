using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyTestWebApp.Helpers
{
    public class ImageHelper
    {
        private const int maxSize = 5242880;

        public static async Task<string?> SaveImage(IFormFile image, IWebHostEnvironment webHost, Controller controller)
        {
            //begin validation
            if (image == null || image.Length == 0)
            {
                controller.ModelState.AddModelError("Image", "Отсутствует изображение");
                return null;
            }

            System.Drawing.Image img;
            try
            {
                if (image != null)
                    img = System.Drawing.Image.FromStream(image.OpenReadStream());
            }
            catch (Exception e)
            {
                controller.ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
            }

            if (image.Length > maxSize)
                controller.ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");

            //end validation

            if (!controller.ModelState.IsValid)
                return null;

            string path = String.Format("/Files/Images/{0}{1}", Guid.NewGuid(), image.FileName);

            using (var fileStream = new FileStream(webHost.WebRootPath + path, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return path;
        }
        public static async Task<string?> SaveImage(IFormFile image, IWebHostEnvironment webHost, ControllerBase controller)
        {
            //begin validation
            if (image == null || image.Length == 0)
            {
                controller.ModelState.AddModelError("Image", "Отсутствует изображение");
                return null;
            }

            System.Drawing.Image img;
            try
            {
                if (image != null)
                    img = System.Drawing.Image.FromStream(image.OpenReadStream());
            }
            catch (Exception e)
            {
                controller.ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
            }

            if (image.Length > maxSize)
                controller.ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");

            //end validation

            if (!controller.ModelState.IsValid)
                return null;

            string path = String.Format("/Files/Images/{0}{1}", Guid.NewGuid(), image.FileName);

            using (var fileStream = new FileStream(webHost.WebRootPath + path, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return path;
        }

        public static bool DeleteImage(string path, IWebHostEnvironment webHost)
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            string fullPart = webHost.WebRootPath + path;

            File.Delete(fullPart);

            return File.Exists(fullPart) ? false : true;
        }
    }
}
