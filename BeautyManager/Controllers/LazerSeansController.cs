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
        public IActionResult Ekle(int MusteriId, string MusteriAdi, string Bolge,
    int ToplamSeans, decimal ToplamUcret, DateTime BaslangicTarihi,
    DateTime? SonrakiSeansTarihi)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");

            // Müşteri seçilmediyse ama isim yazıldıysa yeni müşteri oluştur
            if (MusteriId == 0 && !string.IsNullOrEmpty(MusteriAdi))
            {
                var adParcalar = MusteriAdi.Trim().Split(' ');
                var yeniMusteri = new Musteri
                {
                    Ad = adParcalar[0],
                    Soyad = adParcalar.Length > 1 ? string.Join(" ", adParcalar.Skip(1)) : "",
                    Telefon = "",
                    KayitTarihi = DateTime.Now
                };
                _context.Musteriler.Add(yeniMusteri);
                _context.SaveChanges();
                MusteriId = yeniMusteri.Id;
            }

            if (MusteriId == 0)
            {
                ViewBag.Hata = "Lütfen müşteri bilgisi giriniz!";
                ViewBag.Musteriler = _context.Musteriler.OrderBy(m => m.Ad).ToList();
                return View();
            }

            var seans = new LazerSeans
            {
                MusteriId = MusteriId,
                Bolge = Bolge ?? "",
                ToplamSeans = ToplamSeans,
                ToplamUcret = ToplamUcret,
                BaslangicTarihi = BaslangicTarihi,
                SonrakiSeansTarihi = SonrakiSeansTarihi,
                TamamlananSeans = 0,
                OdenenUcret = 0,
                Durum = "Devam Ediyor"
            };

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