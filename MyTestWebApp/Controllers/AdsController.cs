using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Context;
using MyTestWebApp.Helpers;
using MyTestWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyTestWebApp.Helpers.AttributeNoDirectAccess;

namespace MyTestWebApp.Controllers
{
    public class AdsController : Controller
    {
        private readonly int pageSize = 3;

        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public AdsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{page?}")]
        public async Task<IActionResult> Index(string? search, string? sort, int page = 1)
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

            var count = result.Count;
            var items = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexPaginationViewModel viewModel = new IndexPaginationViewModel
            {
                PageViewModel = pageViewModel,
                ads = items
            };

            Environment.SetEnvironmentVariable("page", page.ToString());
            return View(viewModel);
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

        [NoDirectAccess]
        [Authorize]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdCreateModel ad, IFormFile image)
        {
            //validation for image
            System.Drawing.Image img;
            try
            {
                if (image != null)
                    img = System.Drawing.Image.FromStream(image.OpenReadStream());
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.HelpLink);
                ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
            }

            if (image == null)
            {
                ModelState.AddModelError("Image", "Отсутствует изображение");
            }
            else if (image != null || image.Length > 0)
            {
                ModelState.Remove("Image");

                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ad.Image = fileBytes;
                }

                if (image.Length > 5242880)
                    ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");
            }

            Ad adResult = new Ad();
            if (ModelState.IsValid)
            {
                adResult.UserName = User.Identity.Name;
                adResult.CreateTime = DateTime.Now;
                adResult.DropTime = DateTime.Now.AddDays(90);
                adResult.AdId = ad.AdId;
                adResult.Number = ad.Number;
                adResult.Image = ad.Image;
                adResult.Text = ad.Text;
                _context.Add(adResult);
                await _context.SaveChangesAsync();
                var ads = await _context.Ads.ToListAsync();
                int page = Convert.ToInt32(Environment.GetEnvironmentVariable("page"));
                IndexPaginationViewModel model = new IndexPaginationViewModel()
                {
                    ads = ads,
                    PageViewModel = new PageViewModel(ads.Count, page, pageSize)
                };

                return Json(new { isValid = true, url = @Url.Action("Index", "Ads", new { page = Environment.GetEnvironmentVariable("page") }) });
            }

            return Json(new { isValid = false, html = RazorConverter.RenderRazorViewToString(this, "Create", ad) });
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
        public async Task<IActionResult> Edit(Guid id, [Bind(include: "AdId,Number,Text")] Ad ad, IFormFile image)
        {
            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }

            var old = _context.Ads.AsNoTracking<Ad>().Where(x => x.AdId == ad.AdId).ToList()[0];

            //validation for image
            System.Drawing.Image img;
            bool imageIsVlalid;
            try
            {
                if (image != null)
                    img = System.Drawing.Image.FromStream(image.OpenReadStream());
                imageIsVlalid = true;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
                imageIsVlalid = false;
            }

            if (image == null)
            {
                ModelState.AddModelError("Image", "Отсутствует изображение");
            }
            else if ((image != null || image.Length > 0) && imageIsVlalid)
            {
                ModelState.Remove("Image");

                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ad.Image = fileBytes;
                }

                if (image.Length > 5242880)
                    ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");
            }

            //old image return
            if (image == null)
            {
                ad.Image = old.Image;
                ModelState.Remove("Image");
            }
            else if (image != null || image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
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

                var ads = await _context.Ads.ToListAsync();
                int page = Convert.ToInt32(Environment.GetEnvironmentVariable("page"));
                IndexPaginationViewModel model = new IndexPaginationViewModel()
                {
                    ads = ads,
                    PageViewModel = new PageViewModel(ads.Count, page, pageSize)
                };

                return Json(new { isValid = true,url= @Url.Action("Index","Ads",new{page=Environment.GetEnvironmentVariable("page")}) });

            }
            return Json(new { isValid = false, html = RazorConverter.RenderRazorViewToString(this, "Edit", ad) });
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
            var ads = await _context.Ads.ToListAsync();
            int page = Convert.ToInt32(Environment.GetEnvironmentVariable("page"));
            IndexPaginationViewModel model = new IndexPaginationViewModel()
            {
                ads = ads,
                PageViewModel = new PageViewModel(ads.Count, page, pageSize)
            };

            return Json(new { isValid = true, url = @Url.Action("Index", "Ads", new { page = Environment.GetEnvironmentVariable("page") }) });
        }

        private bool AdExists(Guid id)
        {
            return _context.Ads.Any(e => e.AdId == id);
        }
    }
}
