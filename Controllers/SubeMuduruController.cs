using Microsoft.AspNetCore.Mvc;
using WebBank.Models.ViewModels;
using WebBank.Models;
using WebBank.Helper;
using Microsoft.EntityFrameworkCore;
using WebBank.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace WebBank.Controllers

{
    public class SubeMuduruController : Controller
    {
        private readonly BankDbContext _context;

        public SubeMuduruController(BankDbContext context)
        {
            _context = context;
        }
        public IActionResult SubeMuduruHesabı()
        {
            return View();
        }

        
        public IActionResult CalisanCikar()
        {
            // Giriş yapan şube müdürünün şube Id'si alınıyor (örnek için sabit yazıldı)
            int subeId = Convert.ToInt32(HttpContext.Session.GetInt32("SubeId"));

            var calisanlar = _context.Calisanlar
                .Where(c => c.SubeId == subeId && c.Rol == Rol.GiseMemuru)
                .ToList();

            return View(calisanlar);
        }

        [HttpPost]
        public IActionResult CalisanCikar(string tcNo)
        {
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.TcNo == tcNo);
            if (calisan != null)
            {
                _context.Calisanlar.Remove(calisan);
                _context.SaveChanges();
                TempData["Mesaj"] = "Çalışan başarıyla çıkarıldı.";
            }
            else
            {
                TempData["Hata"] = "Belirtilen TC No ile çalışan bulunamadı.";
            }

            return RedirectToAction("CalisanCikar");
        }

        public IActionResult MudurGiris()
        {
            return View("MudurGiris");
        }

        [HttpPost]
        public IActionResult MudurGiris(string tcNo, string parola)
        {
            var mudur = _context.Calisanlar
                .Include(c => c.Sube) // Şubeyi dahil et
                .FirstOrDefault(c => c.TcNo == tcNo && c.Parola == parola && c.Rol == Rol.SubeMuduru);

            if (mudur != null)
            {
                HttpContext.Session.SetString("SubeAd", mudur.Sube.Ad); // Şube adı
                HttpContext.Session.SetString("AdSoyad", $"{mudur.Ad} {mudur.Soyad}");
                HttpContext.Session.SetString("Rol", mudur.Rol.ToString());

                HttpContext.Session.SetInt32("SubeId", mudur.SubeId);

                return RedirectToAction("SubeMuduruHesabı", "SubeMuduru");
            }

            TempData["Hata"] = "TC No, parola veya rol hatalı.";
            return View("MudurGiris");
        }
        public IActionResult IslemGecmisi()
        {
            int? subeId = HttpContext.Session.GetInt32("SubeId");
            if (subeId == null)
            {
                Console.WriteLine("burda patlıyor çünkü SubeID:" + subeId);
                // Giriş yapılmamışsa veya session yoksa giriş sayfasına yönlendir
                return RedirectToAction("MudurGiris");
            }

            // İlgili şubeye ait hesapların işlemleri
            var islemler = _context.Islemler
                .Include(i => i.Hesap)
                .ThenInclude(h => h.Sube)
                .Include(i => i.GiseMemuru)
                .Where(i => i.Hesap.SubeId == subeId)
                .OrderByDescending(i => i.Tarih)
                .ToList();

            return View(islemler);
        }

    }

}
