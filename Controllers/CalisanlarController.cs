using Microsoft.AspNetCore.Mvc;
using WebBank.Models;

namespace WebBank.Controllers
{
    public class CalisanlarController : Controller
    {
        private readonly BankDbContext _context;

        public CalisanlarController(BankDbContext context)
        {
            _context = context;
        }



        public IActionResult Ekle()
        {
            var Subeler = _context.Sube.ToList();
            return View(Subeler);
        }
    }
}
