using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BeautyManager.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class LazerSeansController : Controller
    {
        private readonly BeautyContext _context;

        public LazerSeansController(BeautyContext context)
        {
            _context = context;
        }

        private bool GirisYapildiMi()
        {
            return HttpContext.Session.GetString("KullaniciAdi") != null;
        }

        public IActionResult Index(string? durum)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var seanslar = _context.LazerSeanslar
                .Include(s => s.Musteri)
                .AsQueryable();

            if (!string.IsNullOrEmpty(durum))
                seanslar = seanslar.Where(s => s.Durum == durum);

            ViewBag.SecilenDurum = durum;
            return View(seanslar.OrderByDescending(s => s.BaslangicTarihi).ToList());
        }

        public IActionResult Ekle()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            ViewBag.Musteriler = _context.Musteriler.OrderBy(m => m.Ad).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(LazerSeans seans)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            _context.LazerSeanslar.Add(seans);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Seans tamamla (+1)
        public IActionResult SeansTamamla(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var seans = _context.LazerSeanslar.Find(id);
            if (seans != null && seans.TamamlananSeans < seans.ToplamSeans)
            {
                seans.TamamlananSeans++;
                if (seans.TamamlananSeans == seans.ToplamSeans)
                    seans.Durum = "Tamamlandı";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Ödeme ekle
        [HttpPost]
        public IActionResult OdemeEkle(int id, decimal tutar)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var seans = _context.LazerSeanslar.Find(id);
            if (seans != null)
            {
                seans.OdenenUcret += tutar;
                if (seans.OdenenUcret >= seans.ToplamUcret)
                    seans.OdenenUcret = seans.ToplamUcret;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Sonraki seans tarihi güncelle
        [HttpPost]
        public IActionResult TarihGuncelle(int id, DateTime tarih)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var seans = _context.LazerSeanslar.Find(id);
            if (seans != null)
            {
                seans.SonrakiSeansTarihi = tarih;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var seans = _context.LazerSeanslar.Find(id);
            if (seans != null)
            {
                _context.LazerSeanslar.Remove(seans);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}