namespace WebBank.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
/*
 * Hata Alan�: Hesap.IBAN - Hata: The IBAN field is required.
Hata Alan�: Hesap.Sube - Hata: The Sube field is required.
Hata Alan�: Hesap.Musteri - Hata: The Musteri field is required.
Hata Alan�: Hesap.Islemler - Hata: The Islemler field is required.
Hata Alan�: Kisiler.subeBilgisi - Hata: The subeBilgisi field is required.
 */