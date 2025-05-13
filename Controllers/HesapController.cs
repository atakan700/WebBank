using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using WebBank.Models;

namespace WebBank.Controllers
{
    public class HesapController
    {
        public IActionResult Ekle()
        {
            ViewBag.HesapTurleri = Enum.GetValues(typeof(HesapTuru))
                .Cast<HesapTuru>()
                .Select(h => new SelectListItem
                {
                    Value = ((int)h).ToString(),
                    Text = h.ToString()
                }).ToList();

            return View();
        }

    }
}
