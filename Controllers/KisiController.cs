using Microsoft.AspNetCore.Mvc;
using WebBank.Models.ViewModels;
using WebBank.Models;
using Microsoft.EntityFrameworkCore;

public class KisiController : Controller
{
    private readonly BankDbContext _context;

    public KisiController(BankDbContext context)
    {
        _context = context;
    }
    private string GenerateIban()
    {
        Random random = new Random();
        string iban = "TR00 7313 0";
        for (int i = 0; i <= 9; i++)
        {
            iban += random.Next(10).ToString();
        }
        return iban;
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
            model.Hesap.IBAN = GenerateIban();
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

}
