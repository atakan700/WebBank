using WebBank.Models; 
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebBank.Models.ViewModels
{
    public class GiseEftViewModel
    {
        [Required(ErrorMessage = "Müşteri seçilmelidir.")]
        public int MusteriId { get; set; }

        [Required(ErrorMessage = "Gönderen hesap seçilmelidir.")]
        public int GonderenHesapId { get; set; }

        [Required(ErrorMessage = "Alıcı IBAN girilmelidir.")]
        [StringLength(26, MinimumLength = 26, ErrorMessage = "IBAN 26 karakter olmalıdır.")]
        [RegularExpression(@"^TR\d{24}$", ErrorMessage = "IBAN 'TR' ile başlamalı ve 24 rakam içermelidir.")]
        public string AliciIban { get; set; }

        public string? AliciBankaAdi { get; set; }          

        [Required(ErrorMessage = "Tutar girilmelidir.")]
        [Range(typeof(decimal), "1", "79228162514264337593543950335", ErrorMessage = "Tutar pozitif olmalıdır.")]
        public decimal Tutar { get; set; }

        public string? Aciklama { get; set; }

        public List<Kisiler> Musteriler { get; set; } = new();
        public List<Hesap> MusteriHesaplari { get; set; } = new();
        public List<SelectListItem> MusteriHesaplariSelectList { get; set; } = new();
    }
}