namespace WebBank.Helper
{
    public class HesapIslemleri
    {
        public static string IbanUret()
        {
            Random random = new Random();
            string iban = "TR00 7313 0";
            for (int i = 0; i <= 9; i++)
            {
                iban += random.Next(10).ToString();
            }
            return iban;
        }
    }
}
