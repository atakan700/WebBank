using Microsoft.AspNetCore.Mvc;
using WebBank.Models.ViewModels;
using WebBank.Models;
using WebBank.Helper;
using Microsoft.EntityFrameworkCore;
using WebBank.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace WebBank.Controllers
{
    public class HesapController : Controller
    {
        private readonly BankDbContext _context;

        public HesapController(BankDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult HesapEkle()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
                return RedirectToAction("Giris");

            var model = new YeniHesapViewModel
            {
                Subeler = _context.Sube.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult HesapEkle(YeniHesapViewModel model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
                return RedirectToAction("Giris");

            if (ModelState.IsValid)
            {
                var yeniHesap = new Hesap
                {
                    MusteriId = kullaniciId.Value,
                    SubeId = model.SubeId,
                    HesapTuru = model.HesapTuru,
                    IBAN = HesapIslemleri.IbanUret(),
                    Bakiye = 0
                };

                _context.HesapBilgileri.Add(yeniHesap);
                _context.SaveChanges();

                return RedirectToAction("HesapSayfasi", "Kisi");

            }
            else
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Hata - Alan: {key}, Mesaj: {error.ErrorMessage}");
                    }
                }


                // Subeler tekrar yollanmalı çünkü model hatalıysa sayfa yeniden yüklenecek
                model.Subeler = _context.Sube.ToList();
                return View(model);
            }

        }
        [HttpGet]
        public IActionResult Havale()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
                return RedirectToAction("Giris", "Kisi");

            var hesaplar = _context.HesapBilgileri
                .Where(h => h.MusteriId == kullaniciId && h.HesapTuru == HesapTuru.TL)
                .Select(h => new
                {
                    h.Id,
                    Gosterim = h.IBAN + " - " + h.Sube.Ad
                })
                .ToList();

            ViewBag.HesapSecenekleri = hesaplar
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Gosterim
                })
                .ToList();

            var model = new HavaleViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Havale(HavaleViewModel model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
                return RedirectToAction("Giris", "Kisi");
            model.TransferTuru = IslemTuru.Havale;

            if (!ModelState.IsValid)
            {
                // Dropdown listesini tekrar doldur:
                var hesaplar = _context.HesapBilgileri
                    .Where(h => h.MusteriId == kullaniciId && h.HesapTuru == HesapTuru.TL)
                    .Select(h => new
                    {
                        h.Id,
                        Gosterim = h.IBAN + " - " + h.Sube.Ad
                    })
                    .ToList();

                ViewBag.HesapSecenekleri = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Gosterim
                }).ToList();

                // Hataları logla
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"[ModelState Hatası] Alan: {key}, Mesaj: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            var gonderenHesap = _context.HesapBilgileri
                .FirstOrDefault(h => h.Id == model.GonderenHesapId && h.MusteriId == kullaniciId);

            if (gonderenHesap == null)
            {
                Console.WriteLine("[HATA] Gönderen hesap bulunamadı veya kullanıcıya ait değil.");
                ModelState.AddModelError("", "Seçilen gönderen hesap bulunamadı veya size ait değil.");
                model.KullaniciHesaplari = _context.HesapBilgileri
                    .Where(h => h.MusteriId == kullaniciId)
                    .ToList();
                return View(model);
            }

            if (gonderenHesap.Bakiye < model.Tutar)
            {
                Console.WriteLine($"[HATA] Yetersiz bakiye. Mevcut: {gonderenHesap.Bakiye}, İstenen: {model.Tutar}");
                ModelState.AddModelError("Tutar", "Yetersiz bakiye.");
                model.KullaniciHesaplari = _context.HesapBilgileri
                    .Where(h => h.MusteriId == kullaniciId)
                    .ToList();
                return View(model);
            }
            if (15000 < model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Limiti aşıldı.");
                model.KullaniciHesaplari = _context.HesapBilgileri
                     .Where(h => h.MusteriId == kullaniciId)
                     .ToList();
                return View(model);
                
            }

            var aliciHesap = _context.HesapBilgileri
                .FirstOrDefault(h => h.IBAN == model.AliciIban);

            if (aliciHesap == null)
            {
                Console.WriteLine($"[HATA] Alıcı IBAN bulunamadı: {model.AliciIban}");
                ModelState.AddModelError("AliciIban", "Alıcı IBAN bulunamadı.");
                model.KullaniciHesaplari = _context.HesapBilgileri
                    .Where(h => h.MusteriId == kullaniciId)
                    .ToList();
                return View(model);
            }

            // İşlem - bakiye güncelleme
            gonderenHesap.Bakiye -= model.Tutar;
            aliciHesap.Bakiye += model.Tutar;

            _context.Islemler.Add(new Islem
            {
                HesapId = gonderenHesap.Id,
                Miktar = -model.Tutar,
                Aciklama = $"Havale ile gönderildi: {model.AliciIban}",
                Tarih = DateTime.Now
            });

            _context.Islemler.Add(new Islem
            {
                HesapId = aliciHesap.Id,
                Miktar = model.Tutar,
                Aciklama = $"Gelen Havale: {gonderenHesap.IBAN}",
                Tarih = DateTime.Now
            });

            _context.SaveChanges();

            Console.WriteLine("[BAŞARILI] Havale işlemi tamamlandı.");
            TempData["BasariliMesaj"] = "Havale işlemi başarıyla gerçekleştirildi.";
            return RedirectToAction("HesapSayfasi", "Kisi");
        }
        [HttpGet]
        // EFT için benzer işlemleri buraya ekleyebilirsin

        private int? GetMemurSubeId()
        {
            return HttpContext.Session.GetInt32(SessionKeys.GiseMemuruSubeId);
        }

        private int? GetCalisanId()
        {
            return HttpContext.Session.GetInt32(SessionKeys.GiseMemuruId);
        }


        [HttpGet]
        public IActionResult Eft()
        {
            var musteriId = HttpContext.Session.GetInt32(SessionKeys.KullaniciId);
            if (musteriId == null)
                return RedirectToAction("Giris", "Kisi");

            var hesaplar = _context.HesapBilgileri
                .Where(h => h.MusteriId == musteriId)
                .Select(h => new
                {
                    h.Id,
                    Gosterim = h.IBAN + " - " + h.Sube.Ad
                })
                .ToList();

            ViewBag.HesapSecenekleri = hesaplar.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Gosterim
            }).ToList();

            var model = new EftViewModel();
            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eft(EftViewModel model)
        {
            var musteriId = HttpContext.Session.GetInt32(SessionKeys.KullaniciId);
            if (musteriId == null)
                return RedirectToAction("Giris", "Kisi");

            // Ortak: hesap seçeneklerini dolduran fonksiyon
            void HesapSecenekleriYukle()
            {
                var hesaplar = _context.HesapBilgileri
                    .Where(h => h.MusteriId == musteriId)
                    .Select(h => new
                    {
                        h.Id,
                        Gosterim = h.IBAN + " - " + h.Sube.Ad
                    })
                    .ToList();

                ViewBag.HesapSecenekleri = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Gosterim
                }).ToList();
            }

            if (!ModelState.IsValid)
            {
                HesapSecenekleriYukle();
                return View(model);
            }

            var gonderenHesap = _context.HesapBilgileri
                .FirstOrDefault(h => h.Id == model.GonderenHesapId && h.MusteriId == musteriId);

            if (gonderenHesap == null)
            {
                ModelState.AddModelError("", "Gönderen hesap size ait değil.");
                HesapSecenekleriYukle();
                return View(model);
            }

            if (gonderenHesap.Bakiye < model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Yetersiz bakiye.");
                HesapSecenekleriYukle();
                return View(model);
            }
            if (15000 < model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Limiti aşıldı.");
                HesapSecenekleriYukle();
                return View(model);
            }

            // EFT işlemi
            gonderenHesap.Bakiye -= model.Tutar;

            _context.Islemler.Add(new Islem
            {
                HesapId = gonderenHesap.Id,
                Miktar = -model.Tutar,
                Aciklama = $"Müşteri EFT: {model.AliciIban}",
                Tarih = DateTime.Now
            });

            _context.SaveChanges();

            TempData["BasariliMesaj"] = "EFT işlemi başarıyla tamamlandı.";
            return RedirectToAction("HesapSayfasi", "Kisi");
        }


    }


}



