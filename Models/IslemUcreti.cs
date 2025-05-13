using System.ComponentModel.DataAnnotations;
namespace WebBank.Models
{
    public class IslemUcreti
    {
        public int Id { get; set; }
        public IslemTuru Tur { get; set; }
        public double UcretInternet { get; set; }
        public double UcretGise { get; set; }
    }

}
