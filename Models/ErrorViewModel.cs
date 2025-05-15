namespace WebBank.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
/*
 * Hata Alaný: Hesap.IBAN - Hata: The IBAN field is required.
Hata Alaný: Hesap.Sube - Hata: The Sube field is required.
Hata Alaný: Hesap.Musteri - Hata: The Musteri field is required.
Hata Alaný: Hesap.Islemler - Hata: The Islemler field is required.
Hata Alaný: Kisiler.subeBilgisi - Hata: The subeBilgisi field is required.
 */