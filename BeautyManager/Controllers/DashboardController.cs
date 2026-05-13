using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeautyManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BeautyContext _context;

        public DashboardController(BeautyContext context)
        {
            _context = context;
        }

        private bool GirisYapildiMi()
        {
            return HttpContext.Session.GetString("KullaniciAdi") != null;
        }

        public IActionResult Index()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            return View();
        }
    }
}