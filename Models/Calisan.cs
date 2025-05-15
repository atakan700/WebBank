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
        public int Id { get; set; }
        public string? Ad { get; set; }
        public Rol? Rol { get; set; }

        // Foreign key
        public int SubeId { get; set; }

        // Navigation property
        [ValidateNever]
        public Sube Sube { get; set; }


        [ValidateNever]
        public ICollection<Islem> YaptigiIslemler { get; set; }
    }
}
