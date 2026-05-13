using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeautyManager.Controllers
{
    public class GirisController : Controller
    {
        private readonly BeautyContext _context;

        public GirisController(BeautyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("KullaniciAdi") != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Index(string kullaniciAdi, string sifre)
        {
            var kullanici = _context.Kullanicilar
                .FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi && k.Sifre == sifre);

            if (kullanici != null)
            {
                HttpContext.Session.SetString("KullaniciAdi", kullanici.KullaniciAdi);
                HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult KurulumYap()
        {
            if (_context.Kullanicilar.Any())
                return Content("Kullanıcı zaten mevcut!");

            _context.Kullanicilar.Add(new Kullanici
            {
                KullaniciAdi = "admin",
                Sifre = "1234",
                AdSoyad = "Salon Yöneticisi",
                Rol = "Admin"
            });
            _context.SaveChanges();
            return Content("Kurulum tamam! admin / 1234 ile giriş yapabilirsin.");
        }
    }
}