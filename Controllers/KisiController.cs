using Microsoft.AspNetCore.Mvc;
using WebBank.Models;
using System;

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
        public IActionResult Ekle(Kisiler kisi)

        {
            if (ModelState.IsValid)
            {
                _context.Kisiler.Add(kisi);
                _context.SaveChanges();
                return RedirectToAction("Listele");
            }

            else
            {
                // ModelState geçersizse, hata mesajlarını konsola yazdırabilirsiniz
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            Random random = new Random();

            string Iban = "TR00 7313 0";
            for (int i = 0; i <= 9; i++)
            {
                 Iban += (random.Next()%10).ToString();

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
