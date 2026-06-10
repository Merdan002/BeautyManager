using BeautyManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautyManager.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class FiyatController : Controller
    {
        private readonly BeautyContext _context;

        public FiyatController(BeautyContext context)
        {
            _context = context;
        }

        private bool GirisYapildiMi()
        {
            return HttpContext.Session.GetString("KullaniciAdi") != null;
        }

        public IActionResult Index(string? kategori)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var fiyatlar = _context.FiyatListeleri.AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
                fiyatlar = fiyatlar.Where(f => f.Kategori == kategori);

            ViewBag.Kategoriler = _context.FiyatListeleri
                .Select(f => f.Kategori).Distinct().ToList();
            ViewBag.SecilenKategori = kategori;
            ViewBag.ToplamIslem = _context.FiyatListeleri.Count();
            ViewBag.AktifIslem = _context.FiyatListeleri.Count(f => f.Aktif);

            return View(fiyatlar.OrderBy(f => f.Kategori).ThenBy(f => f.IslemAdi).ToList());
        }

        public IActionResult Ekle()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            return View(new FiyatListesi());
        }

        [HttpPost]
        public IActionResult Ekle(string IslemAdi, string Kategori, decimal Fiyat, int Sure, bool Aktif = true)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var fiyat = new FiyatListesi
            {
                IslemAdi = IslemAdi,
                Kategori = Kategori,
                Fiyat = Fiyat,
                Sure = Sure,
                Aktif = Aktif
            };
            _context.FiyatListeleri.Add(fiyat);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var fiyat = _context.FiyatListeleri.Find(id);
            if (fiyat == null) return RedirectToAction("Index");
            return View(fiyat);
        }

        [HttpPost]
        public IActionResult Duzenle(int id, string IslemAdi, string Kategori, decimal Fiyat, int Sure, bool Aktif = true)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var mevcut = _context.FiyatListeleri.Find(id);
            if (mevcut != null)
            {
                mevcut.IslemAdi = IslemAdi;
                mevcut.Kategori = Kategori;
                mevcut.Fiyat = Fiyat;
                mevcut.Sure = Sure;
                mevcut.Aktif = Aktif;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult AktifToggle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var fiyat = _context.FiyatListeleri.Find(id);
            if (fiyat != null)
            {
                fiyat.Aktif = !fiyat.Aktif;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var fiyat = _context.FiyatListeleri.Find(id);
            if (fiyat != null)
            {
                _context.FiyatListeleri.Remove(fiyat);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}