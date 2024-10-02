using Instadvert.CZ.Data;
using Instadvert.CZ.Data.ViewModels;
using Instadvert.CZ.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.Identity;
using System.Diagnostics;


namespace Instadvert.CZ.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;




        public HomeController(IConfiguration configuration,ILogger<HomeController> logger, Microsoft.AspNetCore.Identity.UserManager<User> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }


        public async Task<IActionResult> Index()
        {
            var id = _userManager.GetUserId(User);
       

            var user = await _userManager.FindByIdAsync(id) as BloggerUser;
          

            return View(user);

            
        }

        public IActionResult AboutUs()
        {
            return View();
        }



    }
}
