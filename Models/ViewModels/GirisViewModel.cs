using System.ComponentModel.DataAnnotations;
using WebBank.Models;
namespace WebBank.Models.ViewModels;
public class GirisViewModel
{
    [Required]
    public string TcNo { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Parola { get; set; }
}

