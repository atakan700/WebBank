using Microsoft.EntityFrameworkCore;

namespace WebBank.Models
{
    public class BankDbContext:DbContext
    {
   
            public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
            {
            }


            public DbSet<Kisiler> Kisiler { get; set; }
            public DbSet<Hesap> HesapBilgileri { get; set; }
            public DbSet<Calisan> Calisanlar { get; set; }
            public DbSet<Islem> Islemler { get; set; }
            public DbSet<IslemUcreti> IslemUcretleri { get; set; }
             public DbSet<Sube> Sube { get; set; }


    }
}
