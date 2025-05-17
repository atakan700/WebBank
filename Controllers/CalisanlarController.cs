using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBank.Models;
using WebBank.Models.ViewModels;
using WebBank.Filters; 
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace WebBank.Controllers
{
    public class CalisanlarController : Controller
    {
        private readonly BankDbContext _context;
        private readonly ILogger<CalisanlarController> _logger;

        public CalisanlarController(BankDbContext context, ILogger<CalisanlarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Ekle()
        {
            ViewBag.Subeler = _context.Sube.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(Calisan calisan)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subeler = _context.Sube.ToList();
                return View(calisan);
            }

            try
            {
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();
                _logger.LogInformation("Yeni çalışan eklendi: {@Calisan}", calisan);
                return RedirectToAction("Merhaba");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışan eklenirken hata oluştu.");
                ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
                ViewBag.Subeler = _context.Sube.ToList();
                return View(calisan);
            }
        }   

        [HttpGet]
        public IActionResult GiseGiris()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GiseGiris(GirisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var calisan = _context.Calisanlar
                .Include(c => c.Sube)
                .FirstOrDefault(k => k.TcNo == model.TcNo && k.Parola == model.Parola);

            if (calisan == null)
            {
                ViewBag.Hata = "Kullanıcı adı veya parola hatalı.";
                return View(model);
            }

            HttpContext.Session.SetInt32("KullaniciId", calisan.Id);
            HttpContext.Session.SetString("SubeAd", calisan.Sube.Ad);

            return RedirectToAction("SubeHesapları");
        }

       [SessionAuthorize]
       [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult SubeHesapları()
        {
            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            string subeAd = HttpContext.Session.GetString("SubeAd");

            if (kullaniciId == null || string.IsNullOrEmpty(subeAd))
            {
                return RedirectToAction("GiseGiris");
            }

            var calisan = _context.Calisanlar
                .Include(c => c.Sube)
                .FirstOrDefault(c => c.Id == kullaniciId);

            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

            var hesaplar = _context.HesapBilgileri
                .Include(h => h.Musteri)
                .Where(h => h.SubeId == calisan.SubeId)
                .ToList();

            var viewModel = new SubeHesaplariViewModel
            {
                Calisan = calisan,
                Hesaplar = hesaplar
            };

            ViewBag.SubeAd = subeAd;
            return View(viewModel);
        }
        
        public IActionResult GiseCikis()
        {
            return View("GiseGiris");
        }
    }
}