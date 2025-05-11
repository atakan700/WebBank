using Microsoft.AspNetCore.Mvc;
using WebBank.Models;

namespace WebBank.Controllers
{
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
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Kisi kisi)
        {
            if (ModelState.IsValid)
            {
                _context.Kisiler.Add(kisi);
                _context.SaveChanges();
                return RedirectToAction("Listele");
            }
            return View(kisi);
        }

        public IActionResult Listele()
        {
            var kisiler = _context.Kisiler.ToList();
            return View(kisiler);
        }
    }
}
