using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautyManager.Controllers
{
    public class PersonelController : Controller
    {
        private readonly BeautyContext _context;

        public PersonelController(BeautyContext context)
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
            var personeller = _context.Personeller.ToList();
            return View(personeller);
        }

        public IActionResult Ekle()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Personel personel)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            _context.Personeller.Add(personel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var personel = _context.Personeller.Find(id);
            if (personel == null) return RedirectToAction("Index");
            return View(personel);
        }

        [HttpPost]
        public IActionResult Duzenle(Personel personel)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var mevcut = _context.Personeller.Find(personel.Id);
            if (mevcut != null)
            {
                mevcut.Ad = personel.Ad;
                mevcut.Soyad = personel.Soyad;
                mevcut.Telefon = personel.Telefon;
                mevcut.Uzmanlik = personel.Uzmanlik;
                mevcut.Aktif = personel.Aktif;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var personel = _context.Personeller.Find(id);
            if (personel != null)
            {
                _context.Personeller.Remove(personel);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}