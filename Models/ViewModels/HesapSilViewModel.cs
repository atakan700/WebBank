// Models/ViewModels/HesapSilViewModel.cs
using System.Collections.Generic;
using WebBank.Models;

namespace WebBank.Models.ViewModels
{
    public class HesapSilViewModel
    {
        public List<Hesap> Hesaplar { get; set; } = new();
        public int? SecilenHesapId { get; set; }
    }
}
