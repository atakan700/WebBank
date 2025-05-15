using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBank.Models;
using System;

public class CalisanlarController : Controller
{
    private readonly BankDbContext _context;
    private readonly ILogger<CalisanlarController> _logger;

    public CalisanlarController(BankDbContext context, ILogger<CalisanlarController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Ekleme formunu gösterir
    public IActionResult Ekle()
    {
        ViewBag.Subeler = _context.Sube.ToList();
        return View();
    }

    // Form gönderildiğinde çağrılır
    [HttpPost]
    public IActionResult Ekle(Calisan calisan)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();

                _logger.LogInformation("Çalışan başarıyla eklendi: {@Calisan}", calisan);
                return RedirectToAction("Merhaba");  // başarılıysa başka sayfaya yönlendir
            }
            else
            {
                // Model hatalarını terminale yaz
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Model hatası: {entry.Key} - {error.ErrorMessage}");
                    }
                }

                _logger.LogWarning("Model geçerli değil. Hatalar kullanıcıya gösterilecek.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("HATA: " + ex.Message);
            _logger.LogError(ex, "Çalışan eklenirken bir hata oluştu.");
            ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
        }

        ViewBag.Subeler = _context.Sube.ToList();
        return View(calisan);
    }

}
