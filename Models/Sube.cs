using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebBank.Models
{
    public class Sube
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public double ParaMiktari { get; set; }

        [ValidateNever]
        public ICollection<Calisan> Calisanlar { get; set; }
        public ICollection<Hesap> Hesaplar { get; set; }
    }

}
