using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Context;
using MyTestWebApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyTestWebApp.Controllers
{
    public class AdsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public AdsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? sort)
        {
            var result = await _context.Ads.ToListAsync();
            if (search != null && search.Length > 0)
                result = result.Where(x => x.Text.ToLower().Contains(search.ToLower())).ToList();

            if (sort != null)
                switch (sort)
                {
                    case "number":
                        result = result.OrderBy(x => x.Number).ToList();
                        break;
                    case "text":
                        result = result.OrderBy(x => x.Text).ToList();
                        break;
                    case "rating":
                        result = result.OrderBy(x => x.Rating).ToList();
                        break;
                    case "user":
                        result = result.OrderBy(x => x.UserName).ToList();
                        break;
                    default:
                        break;
                }

            return View(result);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdId,Number,Text")] Ad ad, IFormFile Image)
        {
            //validation for image
            try
            {
                var img = System.Drawing.Image.FromStream(Image.OpenReadStream());
            }
            catch
            {
                ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
            }

            if (Image == null)
            {
                ModelState.AddModelError("Image", "Отсутствует изображение");
            }
            else if (Image != null || Image.Length > 0)
            {
                ModelState.Remove("Image");

                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ad.Image = fileBytes;
                }
            }

            if (Image.Length > 5242880)
                ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");

            if (ModelState.IsValid)
            {
                ad.UserName = User.Identity.Name;
                ad.CreateTime = DateTime.Now;
                ad.DropTime = DateTime.Now.AddDays(90);
                ad.AdId = Guid.NewGuid();
                _context.Add(ad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ad);
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }

            return View(ad);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind(include: "AdId,Number,Text")] Ad ad, IFormFile Image)
        {
            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }

            var old = _context.Ads.AsNoTracking<Ad>().Where(x => x.AdId == ad.AdId).ToList()[0];

            //validation for image
            try
            {
                var img = System.Drawing.Image.FromStream(Image.OpenReadStream());
            }
            catch
            {
                ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
            }

            if (Image.Length > 5242880)
                ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");

            //old image return
            ModelState.Remove("Image");
            if (Image == null)
            {
                ad.Image = old.Image;
            }
            else if (Image != null || Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ad.Image = fileBytes;
                }
            }

            //old data return
            ad.DropTime = old.DropTime;
            ad.CreateTime = old.CreateTime;
            ad.Rating = old.Rating;
            ad.UserName = old.UserName;

            if (id != ad.AdId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdExists(ad.AdId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ad);
        }

        [Authorize]
        // GET: Ads/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (ad == null)
            {
                return NotFound();
            }

            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }

            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);

            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(Guid id)
        {
            return _context.Ads.Any(e => e.AdId == id);
        }
    }
}
