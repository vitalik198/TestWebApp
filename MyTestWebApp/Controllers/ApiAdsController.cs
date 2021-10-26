using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Context;
using MyTestWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyTestWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAdsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ApiAdsController(ApplicationContext context)
        {
            this._context = context;
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
        public IActionResult Post([FromBody] AdCreateModel value)
        {
            //authorization check
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Must be authenticated for use");
                return BadRequest(ModelState);
            }

            //image validation
            try
            {
                var img = System.Drawing.Image.FromStream(new MemoryStream(value.Image));
                if (value.Image.Length > 5242880)
                    ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");
            }
            catch
            {
                ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
                return BadRequest(ModelState);
            }

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
                Image = value.Image
            };

            _context.Add(ad);
            _context.SaveChanges();

            return Ok();
        }

        // PUT api/<ApiAdsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] AdCreateModel value)
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

            //image validation
            try
            {
                var img = System.Drawing.Image.FromStream(new MemoryStream(value.Image));
                if (value.Image.Length > 5242880)
                    ModelState.AddModelError("Image", "Картинка не должна быть больше 5 мб");
            }
            catch
            {
                ModelState.AddModelError("Image", "Загруженный файл не является изображением или поврежден");
                return BadRequest(ModelState);
            }

            //model state validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            Ad ad = new Ad();
            ad.AdId = id;
            ad.Text = value.Text;
            ad.Number = value.Number;
            ad.Image = value.Image;
            ad.CreateTime = old.CreateTime;
            ad.DropTime = old.DropTime;
            ad.Rating = old.Rating;
            ad.UserName = old.UserName;

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

            _context.Remove(ad);
            _context.SaveChanges();
            return Ok();
        }
    }
}
