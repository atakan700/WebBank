using System.ComponentModel.DataAnnotations;

namespace WebBank.Models
{
    public enum IslemTuru
    {
        Havale,
        EFT,
        ParaCekme,
        ParaYatirma
    }

    public class Islem
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public IslemTuru Tur { get; set; }
        public decimal Miktar { get; set; }
        public decimal Ucret { get; set; }
        public bool InternetUzerindenMi { get; set; }

        public string Aciklama {  get; set; }
        public int HesapId { get; set; }
        public Hesap Hesap { get; set; }

        public int? GiseMemuruId { get; set; }
        public Calisan? GiseMemuru { get; set; }
    }

}
