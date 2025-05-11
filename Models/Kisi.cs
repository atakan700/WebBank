using System.ComponentModel.DataAnnotations;

namespace WebBank.Models
{
    public class Kisi
    {
            [Key]
            public int Id { get; set; }

            [Required]
            public string Ad { get; set; }

            [Required]
            public string Soyad { get; set; }

            public bool InternetBank { get; set; }
        
    }
}
