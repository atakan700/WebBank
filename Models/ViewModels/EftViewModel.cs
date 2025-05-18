using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebBank.Models;

namespace WebBank.Models.ViewModels
{
    public class EftViewModel
    {
        [Required]
        [StringLength(26, MinimumLength = 26, ErrorMessage = "IBAN 26 karakter olmalıdır.")]
        [RegularExpression(@"^TR\d{24}$", ErrorMessage = "Geçerli bir IBAN giriniz. (TR ve ardından 24 rakam)")]
        public string AliciIban { get; set; }

        [Required(ErrorMessage = "Alıcı banka adı seçilmelidir.")]
        public string AliciBankaAdi { get; set; }

        [Required(ErrorMessage = "Tutar girilmelidir.")]
        [Range(typeof(decimal), "1", "150000", ErrorMessage = "Tutar pozitif ve 15000'den az olmalıdır olmalıdır.")]
        public decimal Tutar { get; set; }

        [Required(ErrorMessage = "Gönderen hesap seçilmelidir.")]
        public int GonderenHesapId { get; set; }

        // Kullanıcının kendi hesaplarını seçmesi için
        public List<Hesap>? KullaniciHesaplari { get; set; }

        [ValidateNever]
        public IslemTuru TransferTuru { get; set; } = IslemTuru.EFT;       

        // EFT için isteğe bağlı olarak ek bilgi alanları eklenebilir, örneğin açıklama gibi
        public string? Aciklama { get; set; }
    }
}
