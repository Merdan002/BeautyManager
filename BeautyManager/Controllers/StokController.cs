using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeautyManager.Controllers
{
    public class StokController : Controller
    {
        private readonly BeautyContext _context;

        public StokController(BeautyContext context)
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
            var stoklar = _context.MalzemeStoklar.AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
                stoklar = stoklar.Where(s => s.Kategori == kategori);

            ViewBag.Kategoriler = _context.MalzemeStoklar
                .Select(s => s.Kategori).Distinct().ToList();
            ViewBag.SecilenKategori = kategori;
            ViewBag.DusukStokSayisi = _context.MalzemeStoklar
                .Count(s => s.Miktar <= s.MinimumStok);

            return View(stoklar.OrderBy(s => s.Kategori).ThenBy(s => s.UrunAdi).ToList());
        }

        public IActionResult Ekle()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(MalzemeStok stok)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            stok.SonGuncelleme = DateTime.Now;
            _context.MalzemeStoklar.Add(stok);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var stok = _context.MalzemeStoklar.Find(id);
            if (stok == null) return RedirectToAction("Index");
            return View(stok);
        }

        [HttpPost]
        public IActionResult Duzenle(MalzemeStok stok)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var mevcut = _context.MalzemeStoklar.Find(stok.Id);
            if (mevcut != null)
            {
                mevcut.UrunAdi = stok.UrunAdi;
                mevcut.Kategori = stok.Kategori;
                mevcut.Miktar = stok.Miktar;
                mevcut.Birim = stok.Birim;
                mevcut.MinimumStok = stok.MinimumStok;
                mevcut.BirimFiyat = stok.BirimFiyat;
                mevcut.SonGuncelleme = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Hızlı stok güncelle
        [HttpPost]
        public IActionResult StokGuncelle(int id, int miktar)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var stok = _context.MalzemeStoklar.Find(id);
            if (stok != null)
            {
                stok.Miktar += miktar;
                if (stok.Miktar < 0) stok.Miktar = 0;
                stok.SonGuncelleme = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var stok = _context.MalzemeStoklar.Find(id);
            if (stok != null)
            {
                _context.MalzemeStoklar.Remove(stok);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}