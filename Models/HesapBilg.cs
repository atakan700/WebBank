using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBank.Models
{
    public enum HesapTuru
    {
        TL,
        USD,
        EUR
    }

    public class Hesap
    {
        public int Id { get; set; }
        public string? IBAN { get; set; }
        public double Bakiye { get; set; }
        public HesapTuru HesapTuru { get; set; }

        public int MusteriId { get; set; }
        [ValidateNever]
        public Kisiler? Musteri { get; set; }

        public int? SubeId { get; set; }
        public Sube? Sube { get; set; }

        public ICollection<Islem>? Islemler { get; set; }
    }

}
