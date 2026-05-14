using BeautyManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeautyManager.Controllers
{
    public class AyarlarController : Controller
    {
        private readonly BeautyContext _context;

        public AyarlarController(BeautyContext context)
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
            var kullaniciAdi = HttpContext.Session.GetString("KullaniciAdi");
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);
            return View(kullanici);
        }

        [HttpPost]
        public IActionResult SifreDegistir(string eskiSifre, string yeniSifre, string yeniSifreTekrar)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");

            var kullaniciAdi = HttpContext.Session.GetString("KullaniciAdi");
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı!";
                return RedirectToAction("Index");
            }

            if (kullanici.Sifre != eskiSifre)
            {
                TempData["Hata"] = "Mevcut şifre hatalı!";
                return RedirectToAction("Index");
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                TempData["Hata"] = "Yeni şifreler eşleşmiyor!";
                return RedirectToAction("Index");
            }

            if (yeniSifre.Length < 4)
            {
                TempData["Hata"] = "Şifre en az 4 karakter olmalı!";
                return RedirectToAction("Index");
            }

            kullanici.Sifre = yeniSifre;
            _context.SaveChanges();
            TempData["Basari"] = "Şifre başarıyla değiştirildi!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ProfilGuncelle(string adSoyad, string yeniKullaniciAdi)
        {
            if (!GirisYapildiMi()) return RedirectToAction("Index", "Giris");

            var kullaniciAdi = HttpContext.Session.GetString("KullaniciAdi");
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici != null)
            {
                // Kullanıcı adı başkası tarafından alınmış mı kontrol et
                if (!string.IsNullOrEmpty(yeniKullaniciAdi) && yeniKullaniciAdi != kullaniciAdi)
                {
                    var mevcutMu = _context.Kullanicilar.Any(k => k.KullaniciAdi == yeniKullaniciAdi);
                    if (mevcutMu)
                    {
                        TempData["Hata"] = "Bu kullanıcı adı zaten kullanılıyor!";
                        return RedirectToAction("Index");
                    }
                    kullanici.KullaniciAdi = yeniKullaniciAdi;
                    HttpContext.Session.SetString("KullaniciAdi", yeniKullaniciAdi);
                }

                kullanici.AdSoyad = adSoyad;
                _context.SaveChanges();
                HttpContext.Session.SetString("AdSoyad", adSoyad);
                TempData["Basari"] = "Profil güncellendi!";
            }

            return RedirectToAction("Index");
        }
    }
}