using WebBank.Models;
namespace WebBank.Models.ViewModels;

public class KisiHesapViewModel
{
    public Kisiler Kisiler { get; set; }
    public Hesap Hesap { get; set; }    
    public int SubeId { get; set; }          // Veritabanıyla ilişki için lazım
}
