using BeautyManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautyManager.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class MusteriController : Controller
    {
        private readonly BeautyContext _context;

        public MusteriController(BeautyContext context)
        {
            _context = context;
        }

        private bool GirisYapildiMi()
        {
            return HttpContext.Session.GetString("KullaniciAdi") != null;
        }

        public IActionResult Index(string? ara)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var musteriler = _context.Musteriler.AsQueryable();
            if (!string.IsNullOrEmpty(ara))
                musteriler = musteriler.Where(m => m.Ad.Contains(ara) || m.Soyad.Contains(ara) || m.Telefon.Contains(ara));
            ViewBag.Ara = ara;
            return View(musteriler.OrderBy(m => m.Ad).ToList());
        }

        public IActionResult Ekle()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Musteri musteri)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            _context.Musteriler.Add(musteri);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var musteri = _context.Musteriler.Find(id);
            if (musteri == null) return RedirectToAction("Index");
            return View(musteri);
        }

        [HttpPost]
        public IActionResult Duzenle(Musteri musteri)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var mevcut = _context.Musteriler.Find(musteri.Id);
            if (mevcut != null)
            {
                mevcut.Ad = musteri.Ad;
                mevcut.Soyad = musteri.Soyad;
                mevcut.Telefon = musteri.Telefon;
                mevcut.Email = musteri.Email;
                mevcut.Notlar = musteri.Notlar;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var musteri = _context.Musteriler.Find(id);
            if (musteri != null)
            {
                _context.Musteriler.Remove(musteri);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Detay(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var musteri = _context.Musteriler
                .Include(m => m.Randevular).ThenInclude(r => r.Personel)
                .Include(m => m.LazerSeanslar)
                .FirstOrDefault(m => m.Id == id);
            if (musteri == null) return RedirectToAction("Index");
            return View(musteri);
        }
    }
}