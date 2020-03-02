using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DishesMenu.Data;
using DishesMenu.Models;
using DishesMenu.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishesMenu.Areas.Admin.Controllers
{
    // define what roles are authorized
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;


        [BindProperty]
        public Coupon CouponModel { get; set; }


        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View( await _db.Coupon.ToListAsync() );
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    // when we save the image in db 
                    // in Coupon model propety type must be byte[] streams
                    // --> public byte[] Picture { get; set; }
                    // we convert image to byte array
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    CouponModel.Picture = p1;
                }
                _db.Coupon.Add(CouponModel);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(CouponModel);
        }


        // GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST EDIT
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if( id == null)
            {
                return NotFound();
            }

            var couponFromDb = await _db.Coupon.Where(c => c.Id == CouponModel.Id).FirstOrDefaultAsync();

            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDb.Picture = p1;
                }

                couponFromDb.MinimumAmount = CouponModel.MinimumAmount;
                couponFromDb.Name = CouponModel.Name;
                couponFromDb.Discount = CouponModel.Discount;
                couponFromDb.CouponType = CouponModel.CouponType;
                couponFromDb.IsActive = CouponModel.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(CouponModel);
        }

        // GET DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if( id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.FirstOrDefaultAsync(m => m.Id == id);
            if( coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.FirstOrDefaultAsync(m => m.Id == id);
            if(coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var coupon = await _db.Coupon.FirstOrDefaultAsync(c => c.Id == id);
            if( coupon == null )
            {
                return NotFound();
            }
            _db.Coupon.Remove(coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}