using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Context;
using MyTestWebApp.Helpers;
using MyTestWebApp.Models;
using MyTestWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyTestWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAdsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _webHost;

        public ApiAdsController(ApplicationContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }


        // GET: api/<ApiAdsController>
        [HttpGet]
        public IAsyncEnumerable<Ad> Get()
        {
            return _context.Ads.AsAsyncEnumerable();
        }

        // GET api/<ApiAdsController>/5
        [HttpGet("{id}")]
        public Task<Ad> Get(Guid id)
        {
            return _context.Ads.FirstOrDefaultAsync(x => x.AdId == id);
        }

        // POST api/<ApiAdsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] AdCreateApiModel value)
        {
            //authorization check
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Must be authenticated for use");
                return BadRequest(ModelState);
            }

            string image = await ImageHelper.SaveImage(value.Image, _webHost,this);

            //model state validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ad ad = new Ad()
            {
                Number = value.Number,
                Text = value.Text,
                UserName = User.Identity.Name,
                CreateTime = DateTime.Now,
                DropTime = DateTime.Now.AddDays(90),
                Rating = 0,
                Image = image
            };

            _context.Add(ad);
            _context.SaveChanges();

            return Ok();
        }

        // PUT api/<ApiAdsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] AdEditApiModel value)
        {
            //authorization check
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Must be authenticated for use");
                return BadRequest(ModelState);
            }

            //check ad object
            Ad old;
            try
            {
                old = _context.Ads.AsNoTracking<Ad>().Where(x => x.AdId == id).ToList()[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                ModelState.AddModelError("", "Ad not exist");
                return BadRequest(ModelState);
            }

            //check user name           
            if (User.Identity.Name != old.UserName && !User.IsInRole("admin"))
            {
                ModelState.AddModelError("", "Только админ или владелец записи может редактировать эту запись");
                return BadRequest(ModelState);
            }

            //image check
            string? newImage = null;
            if (value.Image!=null)
                newImage = await ImageHelper.SaveImage(value.Image, _webHost, this);

            //model state validation
            if (!ModelState.IsValid)
            {
                if (newImage != null)
                    ImageHelper.DeleteImage(newImage,_webHost);
                return BadRequest(ModelState);
            }


            Ad ad = new Ad();
            ad.AdId = id;
            ad.Text = value.Text;
            ad.Number = value.Number;
            ad.CreateTime = old.CreateTime;
            ad.DropTime = old.DropTime;
            ad.Rating = old.Rating;
            ad.UserName = old.UserName;

            if (newImage == null)
                ad.Image = old.Image;
            else
            {
                ad.Image = newImage;
                ImageHelper.DeleteImage(old.Image,_webHost);
            }

            _context.Update(ad);
            _context.SaveChanges();

            return Ok();
        }

        // DELETE api/<ApiAdsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            //authorization check
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Must be authenticated for use");
                return BadRequest(ModelState);
            }

            //check ad object
            Ad ad;
            try
            {
                ad = _context.Ads.AsNoTracking<Ad>().Where(x => x.AdId == id).ToList()[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                ModelState.AddModelError("", "Ad not exist");
                return BadRequest(ModelState);
            }

            //check user name           
            if (User.Identity.Name != ad.UserName && !User.IsInRole("admin"))
            {
                ModelState.AddModelError("", "Только админ или владелец записи может удалить эту запись");
                return BadRequest(ModelState);
            }

            ImageHelper.DeleteImage(ad.Image,_webHost);
            _context.Remove(ad);
            _context.SaveChanges();
            return Ok();
        }
    }
}
