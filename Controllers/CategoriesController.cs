using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Instadvert.CZ.Data;
using Instadvert.CZ.Models;
using Instadvert.CZ.Data.ViewModels;

namespace Instadvert.CZ.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();


            //Creating View model for List filtering
            var filterVM = new FilterVM()
            {
                CategoryList = categories
            };

            if (categories == null)
            {
                return NotFound();
            }

            //Sending data for checkboxes
            ViewBag.Categories = categories;
            return View(filterVM);
        }
        [HttpPost]
        public async Task<IActionResult> Index(FilterVM filterVM)
        {

            var categories = await _context.Categories.ToListAsync();

            if (categories == null)
            {
                return NotFound();
            }

            if (filterVM.searchString != null)
            {
                filterVM.searchString = filterVM.searchString.ToLower();
                categories = categories.Where(x => x.Name.ToLower().Contains(filterVM.searchString)).ToList();
            }
            else
            {
                return NotFound();
            }


            filterVM.CategoryList = categories;

            ViewBag.Categories = categories;

            return View(filterVM);


        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        
        

        // GET: Categories/Create
        public IActionResult Create() => View();
        

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Where(x=>x.CategoryId == id)
                .Include(x=>x.bloggerUserUsers).
                Include(x=>x.companyUsers).
                FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            
            var bloggers = category.bloggerUserUsers;
            var companies = category.companyUsers; 
            


            var model = new CategoryVM()
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                userBloggers = bloggers,
                userCompanies = companies
            };


            return View(model);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryVM model)
        {
            if (id != model.CategoryId)
            {
                return NotFound();
            }

            var category = _context.Categories.FirstOrDefault(x=>x.CategoryId == id);

            category.Name = model.Name;
            category.Description = model.Description;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
