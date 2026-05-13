using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautyManager.Controllers
{
    public class RandevuController : Controller
    {
        private readonly BeautyContext _context;

        public RandevuController(BeautyContext context)
        {
            _context = context;
        }

        private bool GirisYapildiMi()
        {
            return HttpContext.Session.GetString("KullaniciAdi") != null;
        }

        // Randevuları listele
        public IActionResult Index(string? personel, string? durum, string? tarih)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");

            var randevular = _context.Randevular
                .Include(r => r.Musteri)
                .Include(r => r.Personel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(personel))
                randevular = randevular.Where(r => r.PersonelId == int.Parse(personel));

            if (!string.IsNullOrEmpty(durum))
                randevular = randevular.Where(r => r.Durum == durum);

            if (!string.IsNullOrEmpty(tarih))
            {
                var tarihDate = DateTime.Parse(tarih);
                randevular = randevular.Where(r => r.RandevuTarihi.Date == tarihDate.Date);
            }

            ViewBag.Personeller = _context.Personeller.Where(p => p.Aktif).ToList();
            ViewBag.SecilenPersonel = personel;
            ViewBag.SecilenDurum = durum;
            ViewBag.SecilenTarih = tarih;

            return View(randevular.OrderByDescending(r => r.RandevuTarihi).ToList());
        }

        // Yeni randevu sayfası
        public IActionResult Ekle()
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            ViewBag.Musteriler = _context.Musteriler.OrderBy(m => m.Ad).ToList();
            ViewBag.Personeller = _context.Personeller.Where(p => p.Aktif).ToList();
            ViewBag.FiyatListesi = _context.FiyatListeleri.Where(f => f.Aktif).ToList();
            return View();
        }

        // Randevuyu kaydet
        [HttpPost]
        public IActionResult Ekle(Randevu randevu)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            _context.Randevular.Add(randevu);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Randevu düzenle
        public IActionResult Duzenle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var randevu = _context.Randevular.Find(id);
            if (randevu == null) return RedirectToAction("Index");
            ViewBag.Musteriler = _context.Musteriler.OrderBy(m => m.Ad).ToList();
            ViewBag.Personeller = _context.Personeller.Where(p => p.Aktif).ToList();
            return View(randevu);
        }

        [HttpPost]
        public IActionResult Duzenle(Randevu randevu)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var mevcut = _context.Randevular.Find(randevu.Id);
            if (mevcut != null)
            {
                mevcut.MusteriId = randevu.MusteriId;
                mevcut.PersonelId = randevu.PersonelId;
                mevcut.RandevuTarihi = randevu.RandevuTarihi;
                mevcut.IslemAciklamasi = randevu.IslemAciklamasi;
                mevcut.Durum = randevu.Durum;
                mevcut.Ucret = randevu.Ucret;
                mevcut.OdendiMi = randevu.OdendiMi;
                mevcut.Notlar = randevu.Notlar;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Durum güncelle
        public IActionResult DurumGuncelle(int id, string durum)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var randevu = _context.Randevular.Find(id);
            if (randevu != null)
            {
                randevu.Durum = durum;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Ödeme güncelle
        public IActionResult OdemeGuncelle(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var randevu = _context.Randevular.Find(id);
            if (randevu != null)
            {
                randevu.OdendiMi = !randevu.OdendiMi;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Randevu sil
        public IActionResult Sil(int id)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");
            var randevu = _context.Randevular.Find(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}