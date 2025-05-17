using Microsoft.AspNetCore.Mvc;
using WebBank.Models.ViewModels;
using WebBank.Models;
using WebBank.Helper;
using Microsoft.EntityFrameworkCore;

public class KisiController : Controller
{
    private readonly BankDbContext _context;

    public KisiController(BankDbContext context)
    {
        _context = context;
    }
   

    [HttpGet]
    public IActionResult Ekle()
    {
        ViewBag.Subeler = _context.Sube.ToList();
        var viewModel = new KisiHesapViewModel();
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Ekle(KisiHesapViewModel model)
    {
        ViewBag.Subeler = _context.Sube.ToList();

        if (ModelState.IsValid)
        {
            model.Kisiler.subeBilgisi = _context.Sube.Find(model.SubeId)?.Ad;
            _context.Kisiler.Add(model.Kisiler);
            _context.SaveChanges();

            model.Hesap.MusteriId = model.Kisiler.Id;
            model.Hesap.SubeId = model.SubeId;
            model.Hesap.IBAN = HesapIslemleri.IbanUret();
            model.Hesap.Bakiye = 0;

            _context.HesapBilgileri.Add(model.Hesap);
            _context.SaveChanges();

            return RedirectToAction("Listele");
        }

        return View(model);
    }




    public IActionResult Listele()
    {
        // Kisiler ve ilgili hesap bilgilerini birlikte almak için Include kullanıyoruz
        var kisiler = _context.Kisiler.Include(k => k.Hesaplar).ToList();
        return View(kisiler);
    }



    [HttpGet]
    public IActionResult Giris()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Giris(GirisViewModel model)
    {
        if (ModelState.IsValid)
        {
            var kisi = _context.Kisiler
                .Include(k => k.Hesaplar)
                .ThenInclude(h => h.Sube)
                .FirstOrDefault(k => k.TcNo == model.TcNo && k.Parola == model.Parola);

            if (kisi != null)
            {
                HttpContext.Session.SetInt32("KullaniciId", kisi.Id);
                return RedirectToAction("HesapSayfasi");
            }
            else
            {
                ViewBag.Hata = "TC No veya parola hatalı.";
            }
        }

        return View(model);
    }

    public IActionResult HesapSayfasi()
    {
        int? kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

        if (kullaniciId == null)
            return RedirectToAction("Giris");

        var kisi = _context.Kisiler
            .Include(k => k.Hesaplar)
            .ThenInclude(h => h.Sube)
            .FirstOrDefault(k => k.Id == kullaniciId);

        return View(kisi);
    }


    [HttpGet]
    public IActionResult HesapEkle()
    {
        // Kullanıcının oturum açıp açmadığını kontrol edebilirsin
        var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
        if (kullaniciId == null)
            return RedirectToAction("Giris", "Kisiler");

        // Şube listesini viewmodel olarak ekleyebilirsin (dilersen)
        return View();
    }

    [HttpPost]
    public IActionResult HesapEkle(Hesap hesap)
    {
        var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
        if (kullaniciId == null)
            return RedirectToAction("Giris", "Kisiler");

        hesap.MusteriId = kullaniciId.Value;
        hesap.IBAN = HesapIslemleri.IbanUret(); // IBAN üretme metodunu unutma!

        _context.HesapBilgileri.Add(hesap);
        _context.SaveChanges();

        return RedirectToAction("HesapSayfasi");
    }





}