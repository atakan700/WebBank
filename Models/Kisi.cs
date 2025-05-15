using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebBank.Models
{
    public class Kisiler
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı gereklidir.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı gereklidir.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = " alanı gereklidir.")]
        public string Parola {  get; set; }
        public bool InternetBank { get; set; }
        public string? subeBilgisi { get; set; }

        [ValidateNever]
        public ICollection<Hesap> Hesaplar { get; set; }
    }



}
