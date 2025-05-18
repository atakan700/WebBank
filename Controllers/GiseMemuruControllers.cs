using Microsoft.AspNetCore.Mvc;
using WebBank.Models.ViewModels;
using WebBank.Models;
using Microsoft.EntityFrameworkCore;
using WebBank.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebBank.Controllers
{
    public class GiseMemuruController : Controller
    {
        private readonly BankDbContext _context;

        public GiseMemuruController(BankDbContext context)
        {
            _context = context;
        }

        private int? GetMemurSubeId()
        {
            return HttpContext.Session.GetInt32(SessionKeys.GiseMemuruSubeId);
        }

        private int? GetCalisanId()
        {
            return HttpContext.Session.GetInt32(SessionKeys.GiseMemuruId);
        }
        [HttpGet]
        public IActionResult MusteriHesaplariGetir(int musteriId)
        {
            var hesaplar = _context.HesapBilgileri
                .Where(h => h.MusteriId == musteriId)
                .Select(h => new
                {
                    id = h.Id,
                    ad = $"{h.IBAN} - {h.Bakiye} ₺"
                }).ToList();

            return Json(hesaplar);
        }

        [HttpGet]
        public IActionResult Havale()
        {
            var calisanId = GetCalisanId();
            var subeId = GetMemurSubeId();
            Console.WriteLine($"Session CalisanId: {calisanId}, SubeId: {subeId}");

            if (calisanId == null || subeId == null)
            {
                Console.WriteLine("çalışanıd veya subeID gelmedi");
                return RedirectToAction("Giris", "Kisi");
            }

            var hesaplar = _context.HesapBilgileri
                .Where(h => h.SubeId == subeId)
                .ToList();

            var hesapSelectList = hesaplar.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = $"{h.IBAN} - {h.Bakiye} {h.HesapTuru}"
            }).ToList();

            var model = new HavaleViewModel
            {
                KullaniciHesaplari = hesaplar,
                HesapSelectListesi = hesapSelectList
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Havale(HavaleViewModel model)
        {
            var subeId = GetMemurSubeId();
            if (subeId == null)
                return RedirectToAction("Giris", "Kisi");

            if (!ModelState.IsValid)
            {
                var hesaplar = _context.HesapBilgileri
                    .Where(h => h.SubeId == subeId)
                    .ToList();

                model.KullaniciHesaplari = hesaplar;
                model.HesapSelectListesi = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.IBAN} - {h.Bakiye} {h.HesapTuru}"
                }).ToList();

                return View(model);
            }

            var gonderenHesap = _context.HesapBilgileri
                .FirstOrDefault(h => h.Id == model.GonderenHesapId && h.SubeId == subeId);

            if (gonderenHesap == null)
            {
                ModelState.AddModelError("", "Seçilen gönderen hesap bulunamadı veya sizin şubenize ait değil.");

                var hesaplar = _context.HesapBilgileri
                    .Where(h => h.SubeId == subeId)
                    .ToList();

                model.KullaniciHesaplari = hesaplar;
                model.HesapSelectListesi = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.IBAN} - {h.Bakiye} {h.HesapTuru}"
                }).ToList();

                return View(model);
            }

            if (gonderenHesap.Bakiye < model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Yetersiz bakiye.");

                var hesaplar = _context.HesapBilgileri
                    .Where(h => h.SubeId == subeId)
                    .ToList();

                model.KullaniciHesaplari = hesaplar;
                model.HesapSelectListesi = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.IBAN} - {h.Bakiye} {h.HesapTuru}"
                }).ToList();

                return View(model);
            }
            if (15000 < model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Limiti aşıldı.");
                var hesaplar = _context.HesapBilgileri
                     .Where(h => h.SubeId == subeId)
                     .ToList();

                model.KullaniciHesaplari = hesaplar;
                model.HesapSelectListesi = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.IBAN} - {h.Bakiye} {h.HesapTuru}"
                }).ToList();
                return View(model);

            }

            var aliciHesap = _context.HesapBilgileri
                .FirstOrDefault(h => h.IBAN == model.AliciIban);

            if (aliciHesap == null)
            {
                ModelState.AddModelError("AliciIban", "Alıcı IBAN bulunamadı.");

                var hesaplar = _context.HesapBilgileri
                    .Where(h => h.SubeId == subeId)
                    .ToList();

                model.KullaniciHesaplari = hesaplar;
                model.HesapSelectListesi = hesaplar.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.IBAN} - {h.Bakiye} {h.HesapTuru}"
                }).ToList();

                return View(model);
            }

            // Bakiye aktarımı ve işlem kayıtları
            gonderenHesap.Bakiye -= model.Tutar;
            aliciHesap.Bakiye += model.Tutar;

            var memurId = GetCalisanId();
            if (memurId == null)
            {
                Console.WriteLine("", "Gişe memuru bilgisi alınamadı.");
                return View(model);
            }
            _context.Islemler.Add(new Islem
            {
                HesapId = gonderenHesap.Id,
                Miktar = -model.Tutar,
                Aciklama = $"Gişe memuru havalesi: {model.AliciIban}",
                Tarih = DateTime.Now,
                GiseMemuruId=memurId

            });

            _context.Islemler.Add(new Islem
            {
                HesapId = aliciHesap.Id,
                Miktar = model.Tutar,
                Aciklama = $"Gişe memurundan gelen havale: {gonderenHesap.IBAN}",
                Tarih = DateTime.Now,
                GiseMemuruId = memurId
            });

            _context.SaveChanges();

            TempData["BasariliMesaj"] = "Havale işlemi başarıyla gerçekleştirildi.";
            return RedirectToAction("SubeHesapları", "Calisanlar");
        }


        [HttpGet]
        public IActionResult Eft()
        {
            var model = new GiseEftViewModel();
            model.Musteriler = _context.Kisiler.ToList();

            // İlk açılışta boş bırakabilir veya default hesaplar gösterebilirsin
            model.MusteriHesaplariSelectList = new List<SelectListItem>();

            return View(model);
        }


        [HttpPost]
        public IActionResult Eft(GiseEftViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // ViewModel güncelle
                model.Musteriler = _context.Kisiler.ToList();
                model.MusteriHesaplariSelectList = _context.HesapBilgileri
                    .Where(h => h.MusteriId == model.MusteriId)
                    .Select(h => new SelectListItem
                    {
                        Value = h.Id.ToString(),
                        Text = $"{h.IBAN} - {h.Bakiye} ₺"
                    })
                    .ToList();
                Console.WriteLine("################buada patlıyoruz########################");
                return View(model);
            }

            var gonderenHesap = _context.HesapBilgileri
                .FirstOrDefault(h => h.Id == model.GonderenHesapId && h.MusteriId == model.MusteriId);
            Console.WriteLine($"GonderenHesap bulundu mu? {(gonderenHesap == null ? "Hayır" : "Evet")}");

            if (gonderenHesap == null)
            {
                ModelState.AddModelError("", "Seçilen gönderen hesap geçersiz.");
                model.Musteriler = _context.Kisiler.ToList();
                model.MusteriHesaplariSelectList = _context.HesapBilgileri
                    .Where(h => h.MusteriId == model.MusteriId)
                    .Select(h => new SelectListItem
                    {
                        Value = h.Id.ToString(),
                        Text = $"{h.IBAN} - {h.Bakiye} ₺"
                    })
                    .ToList();
                Console.WriteLine($"MusteriId: {model.MusteriId}");
                Console.WriteLine($"GonderenHesapId: {model.GonderenHesapId}");
                Console.WriteLine($"AliciIban: {model.AliciIban}");
                Console.WriteLine($"Tutar: {model.Tutar}");
                return View(model);
            }

            if (gonderenHesap.Bakiye < model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Yetersiz bakiye.");
                model.Musteriler = _context.Kisiler.ToList();
                model.MusteriHesaplariSelectList = _context.HesapBilgileri
                    .Where(h => h.MusteriId == model.MusteriId)
                    .Select(h => new SelectListItem
                    {
                        Value = h.Id.ToString(),
                        Text = $"{h.IBAN} - {h.Bakiye} ₺"
                    })
                    .ToList();
                return View(model);
            }
            if (15000<model.Tutar)
            {
                ModelState.AddModelError("Tutar", "Limiti aşıldı.");
                model.Musteriler = _context.Kisiler.ToList();
                model.MusteriHesaplariSelectList = _context.HesapBilgileri
                    .Where(h => h.MusteriId == model.MusteriId)
                    .Select(h => new SelectListItem
                    {
                        Value = h.Id.ToString(),
                        Text = $"{h.IBAN} - {h.Bakiye} ₺"
                    })
                    .ToList();

                return View (model);
            }

          
            var aliciHesap = _context.HesapBilgileri.FirstOrDefault(h => h.IBAN == model.AliciIban);
            if (aliciHesap != null)
            {
                TempData["BilgiMesaji"] = "Bu IBAN banka içinde kayıtlı. Lütfen EFT yerine havale işlemi yapınız.";
                return RedirectToAction("SubeHesapları", "GiseMemuru");
            }

            // ✅ EFT işlemi (alıcı banka dışında)
            gonderenHesap.Bakiye -= model.Tutar;
            var memurId = GetCalisanId();

            _context.Islemler.Add(new Islem
            {
                HesapId = gonderenHesap.Id,
                Miktar = -model.Tutar,
                Aciklama = $"Gişe EFT ile gönderildi: {model.AliciIban} - {model.AliciBankaAdi}" +
                           (string.IsNullOrWhiteSpace(model.Aciklama) ? "" : $" ({model.Aciklama})"),
                Tarih = DateTime.Now,
                GiseMemuruId = memurId
            });

            _context.SaveChanges();

            TempData["BasariliMesaj"] = "EFT işlemi başarıyla gerçekleştirildi.";
            return RedirectToAction("SubeHesapları", "Calisanlar");
        }

        // GiseMemuruController.cs
        [HttpGet]
        public IActionResult HesapSil()
        {
            var hesaplar = _context.HesapBilgileri
                .Include(h => h.Musteri)
                .ToList();

            var viewModel = new HesapSilViewModel
            {
                Hesaplar = hesaplar
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult HesapSil(HesapSilViewModel model)
        {
            if (model.SecilenHesapId != null)
            {
                var hesap = _context.HesapBilgileri.FirstOrDefault(h => h.Id == model.SecilenHesapId);
                if (hesap != null)
                {
                    _context.HesapBilgileri.Remove(hesap);
                    _context.SaveChanges();
                    TempData["Mesaj"] = "Hesap başarıyla silindi.";
                }
            }

            return RedirectToAction("HesapSil","GiseMemuru");
        }


    }
}