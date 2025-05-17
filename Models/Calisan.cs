using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBank.Models
{
    public enum Rol
    {
        SubeMuduru = 0,
        GiseMemuru = 1
    }

    public class Calisan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "TC No 11 haneli olmalıdır.")]
        public string TcNo { get; set; }

        [Required(ErrorMessage = "Ad alanı gereklidir.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı gereklidir.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = " alanı gereklidir.")]
        public string Parola { get; set; }

        // Foreign key
        public int SubeId { get; set; }

        // Navigation property
        [ValidateNever]
        public Sube Sube { get; set; }


        [ValidateNever]
        public ICollection<Islem> YaptigiIslemler { get; set; }
    }
}
