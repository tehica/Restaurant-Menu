using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DishesMenu.Data;
using DishesMenu.Models;
using DishesMenu.Models.ViewModels;
using DishesMenu.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DishesMenu.Areas.Admin.Controllers
{
    // define what roles are authorized
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        // it's a property we use through methods to display Error Message
        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // get index
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        // GET CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the sub category exists with in the same Category

                // we retrieve from db all the SubCategories and we also incluse Categori there: .Include(s => s.Category)
                // and then we use Where()
                // we want to retrieve all the SubCategories where the Name matches that is the name inside the 
                // model: (SubCategoryAndCategoryViewModel model)
                // &&
                // we also want to make sure that it exist only with in the same Category

                // this will retrieve all the records in whitch the Name matches with the exact same Name in the
                // same Category
                var doesSubCategoryExists = _db.SubCategory
                                            .Include(s => s.Category)
                                            .Where(s => s.Name == model.SubCategory.Name &&
                                                   s.Category.Id == model.SubCategory.CategoryId);

                // then we will check how many records it retrieved
                if(doesSubCategoryExists.Count() > 0)
                {
                    // Error
                    StatusMessage = "Error : Sub Category exists under " + 
                                    doesSubCategoryExists.First().Category.Name + 
                                    " category. Please use another name.";              
                }
                else
                {
                    // if there is not an error we'll add that to the db
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // if model state is not valid
            // we need to return back to the view and we need to create view model
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            // Generate list of SubCategory
            List<SubCategory> subCategories = new List<SubCategory>();

            // there we have a list of SubCategories where CategoryId matches with
            // id what we pass as parameter
            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();

            // this will pass Json object
            // IActionResult works with Json because it is one of the implementations
            // we dont worry about return type
            return Json(new SelectList(subCategories, "Id", "Name"));
        }



        // GET EDIT
        // (int? id) recived id of the SubCategory that needs to be edited
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            // (m => m.Id == id) based on that id it will find the SubCategory what we want Edit
            // and add to the variable: var subCategory
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if(subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {

                var doesSubCategoryExists = _db.SubCategory
                                            .Include(s => s.Category)
                                            .Where(s => s.Name == model.SubCategory.Name &&
                                                   s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    // Error
                    StatusMessage = "Error : Sub Category exists under " +
                                    doesSubCategoryExists.First().Category.Name +
                                    " category. Please use another name.";
                }
                else
                {
                    var subCategoryFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCategoryFromDb.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }


            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        // get DETAILS SUB CATEGORY
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }


        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        
        // POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategory.FirstOrDefaultAsync(m => m.Id == id);
            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
    }
}