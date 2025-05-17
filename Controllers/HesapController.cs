using Microsoft.AspNetCore.Mvc;
using WebBank.Models.ViewModels;
using WebBank.Models;
using WebBank.Helper;
using Microsoft.EntityFrameworkCore;

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
    }
}
