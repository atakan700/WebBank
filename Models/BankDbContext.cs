using Microsoft.EntityFrameworkCore;

namespace WebBank.Models
{
    public class BankDbContext:DbContext
    {
   
            public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
            {
            }

            public DbSet<Kisi> Kisiler { get; set; }
        
    }
}
