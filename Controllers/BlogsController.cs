using Instadvert.CZ.Data.Services;
using Instadvert.CZ.Data;
using Instadvert.CZ.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using Instadvert.CZ.Data.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Blazor;
using System.Data.Entity;
using System.Reflection.Metadata;

namespace Instadvert.CZ.Controllers
{
    public class BlogsController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public BlogsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;        
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }

        public async Task<IActionResult> Index()
        {
            var blogs =_context.Blogs.ToList();
            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new BlogVM();
            return View(model); 

        }
        public async Task<IActionResult> Create(BlogVM model)
        {
            var blog = new Blog();
            if(model != null)
            {
                if (model.CoverPhoto != null)
                {
                    string folder = "Images/cover/";
                    model.CoverImageUrl = await UploadImage(folder, model.CoverPhoto);
                }
                blog.Name = model.Name;
                blog.Text = model.Text;
                blog.CoverImageUrl = model.CoverImageUrl;
                _context.Add(blog);
                _context.SaveChanges();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == Id);
            if (blog == null)
            {
                ViewBag.Message = "Blog does not exist";
                return View("Error");
            }
            var model = new BlogVM()
            {
                Name = blog.Name,
                Text = blog.Text,
                
            };
            return View(model); 
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string Id, BlogVM blogVM)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == Id);
            if (blog == null)
            {
                ViewBag.Message = "Blog does not exist";
                return View("Error");
            }
            blog.Text = blogVM.Text;
            blog.Name = blogVM.Name;

            _context.Update(blog);  
            _context.SaveChanges();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == Id);
            if (blog == null)
            {
                ViewBag.Message = "Blog does not exist";
                return View("Error");
            }
            

            return View();
        }


    }
}





