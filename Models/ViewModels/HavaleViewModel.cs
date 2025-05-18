using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebBank.Models;

namespace WebBank.Models.ViewModels
{
    public class HavaleViewModel
    {
        [Required(ErrorMessage = "Alıcı IBAN girilmelidir.")]
        public string AliciIban { get; set; }

        [Required(ErrorMessage = "Tutar girilmelidir.")]
        [Range(1, double.MaxValue, ErrorMessage = "Tutar pozitif olmalıdır.")]
        public decimal Tutar { get; set; }

        [Required(ErrorMessage = "Gönderen hesap seçilmelidir.")]
        public int GonderenHesapId { get; set; }
        // Seçilebilir hesapların listesi
        public List<Hesap>? KullaniciHesaplari { get; set; }

        // View'da IBAN + HesapTuru gibi bilgileri göstermek için
        public List<SelectListItem>? HesapSelectListesi { get; set; }

        [ValidateNever]
        public IslemTuru TransferTuru { get; set; }
    }
}
