using System.ComponentModel.DataAnnotations;
using WebBank.Models;


namespace WebBank.Models.ViewModels
{
    public class SubeHesaplariViewModel
    {
        public Calisan Calisan { get; set; }
        public List<Hesap> Hesaplar { get; set; }
    }
}
