using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // Bugünün randevuları
            var bugun = DateTime.Today;
            var bugunRandevular = _context.Randevular
                .Include(r => r.Musteri)
                .Include(r => r.Personel)
                .Where(r => r.RandevuTarihi.Date == bugun)
                .OrderBy(r => r.RandevuTarihi)
                .ToList();

            // İstatistikler
            ViewBag.BugunRandevuSayisi = bugunRandevular.Count;
            ViewBag.HaftalikRandevu = _context.Randevular
                .Count(r => r.RandevuTarihi >= DateTime.Today.AddDays(-7));
            var odenenRandevular = _context.Randevular
    .Where(r => r.RandevuTarihi.Month == DateTime.Now.Month && r.OdendiMi)
    .ToList();
            ViewBag.AylikGelir = odenenRandevular.Any() ? odenenRandevular.Sum(r => r.Ucret) : 0;
            ViewBag.BekleyenOdeme = _context.Randevular
                .Count(r => !r.OdendiMi && r.Durum != "İptal");
            ViewBag.ToplamMusteri = _context.Musteriler.Count();
            ViewBag.DusukStok = _context.MalzemeStoklar
                .Count(s => s.Miktar <= s.MinimumStok);

            // Doluluk oranı (günlük kapasite: 10)
            ViewBag.DolulukOrani = Math.Min((bugunRandevular.Count * 100) / 10, 100);

            // Haftalık müşteri sayıları (grafik için)
            var haftaBaslangic = DateTime.Today.AddDays(-6);
            var haftalikData = Enumerable.Range(0, 7)
                .Select(i => new {
                    Gun = DateTime.Today.AddDays(-6 + i).ToString("ddd"),
                    Sayi = _context.Randevular
                        .Count(r => r.RandevuTarihi.Date == DateTime.Today.AddDays(-6 + i))
                })
                .ToList();

            ViewBag.GrafikGunler = string.Join(",", haftalikData.Select(h => $"'{h.Gun}'"));
            ViewBag.GrafikSayilar = string.Join(",", haftalikData.Select(h => h.Sayi));

            // Aylık karşılaştırma
            var aylikData = Enumerable.Range(0, 4)
                .Select(i => new {
                    Hafta = $"Hafta {i + 1}",
                    BuAy = _context.Randevular.Count(r =>
                        r.RandevuTarihi.Month == DateTime.Now.Month &&
                        r.RandevuTarihi.Day >= i * 7 + 1 &&
                        r.RandevuTarihi.Day <= (i + 1) * 7),
                    GecenAy = _context.Randevular.Count(r =>
                        r.RandevuTarihi.Month == DateTime.Now.AddMonths(-1).Month &&
                        r.RandevuTarihi.Day >= i * 7 + 1 &&
                        r.RandevuTarihi.Day <= (i + 1) * 7)
                })
                .ToList();

            ViewBag.AylikHaftalar = string.Join(",", aylikData.Select(a => $"'{a.Hafta}'"));
            ViewBag.BuAyData = string.Join(",", aylikData.Select(a => a.BuAy));
            ViewBag.GecenAyData = string.Join(",", aylikData.Select(a => a.GecenAy));

            // Düşük stok uyarıları
            ViewBag.DusukStoklar = _context.MalzemeStoklar
                .Where(s => s.Miktar <= s.MinimumStok)
                .Take(5)
                .ToList();

            return View(bugunRandevular);
        }
    }
}