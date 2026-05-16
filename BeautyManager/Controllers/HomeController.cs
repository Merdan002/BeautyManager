using Microsoft.AspNetCore.Mvc;

namespace BeautyManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}