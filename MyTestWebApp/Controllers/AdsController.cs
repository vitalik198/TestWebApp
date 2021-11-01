using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Context;
using MyTestWebApp.Helpers;
using MyTestWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyTestWebApp.Helpers.AttributeNoDirectAccess;

namespace MyTestWebApp.Controllers
{
    public class AdsController : Controller
    {
        private readonly int _pageSize = 3;

        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _webHost;

        public AdsController(ApplicationContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? sort, int page = 1)
        {
            IEnumerable<Ad> result = _context.Ads;
            var count = result.Count<Ad>();

            if (search != null && search.Length > 0)
                result = result.Where(x => x.Text.ToLower().Contains(search.ToLower()));

            if (sort != null)
                switch (sort)
                {
                    case "number":
                        result = result.OrderBy(x => x.Number);
                        break;
                    case "text":
                        result = result.OrderBy(x => x.Text);
                        break;
                    case "rating":
                        result = result.OrderBy(x => x.Rating);
                        break;
                    case "user":
                        result = result.OrderBy(x => x.UserName);
                        break;
                    default:
                        break;
                }

            var items = result.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, _pageSize);
            IndexPaginationViewModel viewModel = new IndexPaginationViewModel
            {
                PageViewModel = pageViewModel,
                ads = items
            };

            Environment.SetEnvironmentVariable("page", page.ToString());
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details([Required] Guid id)
        {
            var ad = await _context.Ads.FirstOrDefaultAsync(m => m.AdId == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        [NoDirectAccess]
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdCreateModel ad, IFormFile image)
        {
            string? imagePath = await ImageHelper.SaveImage(image, _webHost, this);

            Ad adResult = new Ad();
            if (imagePath != null)
                adResult.Image = imagePath;
            else
                ModelState.AddModelError("Image", "Не удалось загрузить изображение");

            if (ModelState.IsValid)
            {
                adResult.UserName = User.Identity.Name;
                adResult.CreateTime = DateTime.Now;
                adResult.DropTime = DateTime.Now.AddDays(90);
                adResult.Number = ad.Number;
                adResult.Text = ad.Text;
                _context.Add(adResult);
                await _context.SaveChangesAsync();

                int count = _context.Ads.Count();
                int page = Convert.ToInt32(Environment.GetEnvironmentVariable("page"));
                var ads = GetAdsOfPage(page);
                IndexPaginationViewModel model = new IndexPaginationViewModel()
                {
                    ads = ads,
                    PageViewModel = new PageViewModel(count, page, _pageSize)
                };

                return View("Index", model);
            }
            return View(ad);
        }

        [NoDirectAccess]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit([Required] Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                return RedirectToAction("Index");
            }

            AdCreateModel model = new AdCreateModel()
            {
                Number = ad.Number,
                Text = ad.Text
            };

            return PartialView(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdCreateModel ad, IFormFile image)
        {
            Ad adResult = new Ad();
            string? newImage = null;
            var old = _context.Ads.AsNoTracking<Ad>().Where(x => x.AdId == id).ToList()[0];

            //old image return
            if (image == null)
            {
                adResult.Image = old.Image;
            }
            else if (image != null || image.Length > 0)
            {
                newImage = await ImageHelper.SaveImage(image, _webHost, this);
            }

            adResult.AdId = old.AdId;
            adResult.DropTime = old.DropTime;
            adResult.CreateTime = old.CreateTime;
            adResult.Rating = old.Rating;
            adResult.UserName = old.UserName;
            adResult.Text = ad.Text;
            adResult.Number = ad.Number;

            if (User.Identity.Name != adResult.UserName && !User.IsInRole("admin"))
            {
                ModelState.AddModelError("", "Только admin может вносить измнеия в чужие записи");
            }

            if (ModelState.IsValid)
            {

                if (newImage != null)
                {
                    adResult.Image = newImage;
                    ImageHelper.DeleteImage(old.Image,_webHost);
                }

                try
                {
                    _context.Update(adResult);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (newImage != null)
                        ImageHelper.DeleteImage(newImage,_webHost);

                    RedirectToAction("Index");
                }

                int count = _context.Ads.Count();
                int page = Convert.ToInt32(Environment.GetEnvironmentVariable("page"));
                var ads = GetAdsOfPage(page);
                IndexPaginationViewModel model = new IndexPaginationViewModel()
                {
                    ads = ads,
                    PageViewModel = new PageViewModel(count, page, _pageSize)
                };

                return View("Index", model);
            }

            if (newImage != null)
                ImageHelper.DeleteImage(newImage,_webHost);  
            
            return View(ad);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete([Required] Guid id)
        {
            var ad = await _context.Ads.FirstOrDefaultAsync(m => m.AdId == id);
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

            ImageHelper.DeleteImage(ad.Image, _webHost);
            _context.Ads.Remove(ad);

            await _context.SaveChangesAsync();

            int count = _context.Ads.Count();
            int page = Convert.ToInt32(Environment.GetEnvironmentVariable("page"));
            var ads = GetAdsOfPage(page);
            IndexPaginationViewModel model = new IndexPaginationViewModel()
            {
                ads = ads,
                PageViewModel = new PageViewModel(count, page, _pageSize)
            };

            return View("Index", model);
        }

        private IEnumerable<Ad> GetAdsOfPage(int page)
        {
            IEnumerable<Ad> result = _context.Ads;
            result = result.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();

            return result;
        }
    }
}
