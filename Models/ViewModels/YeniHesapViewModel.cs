using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebBank.Models;
namespace WebBank.Models.ViewModels
    {
        public class YeniHesapViewModel
        {
            public  HesapTuru HesapTuru { get; set; }
            public int SubeId { get; set; }

            [ValidateNever]
            public List<Sube> Subeler { get; set; }
        }
    }


