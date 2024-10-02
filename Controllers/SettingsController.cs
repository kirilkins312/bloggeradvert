using Microsoft.AspNetCore.Mvc;

namespace Instadvert.CZ.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
