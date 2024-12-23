namespace KuaforSalonuYonetim.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

public class RandevuAlViewModel
{
    [Required(ErrorMessage = "Lütfen bir işlem seçiniz.")]
    public int? IslemId { get; set; }

    [Required(ErrorMessage = "Lütfen bir çalışan seçiniz.")]
    public int? CalisanId { get; set; }

    [Required(ErrorMessage = "Lütfen bir tarih seçiniz.")]
    [DataType(DataType.Date)]
    public DateTime? RandevuTarihi { get; set; }

    [Required(ErrorMessage = "Lütfen başlangıç saati seçiniz.")]
    [DataType(DataType.Time)]
    public TimeSpan? BaslangicSaati { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan? BitisSaati { get; set; }

    public decimal Ucret { get; set; }  // Otomatik dolacak

    // Dropdownlar ve/veya ek veriler
    public List<Islem>? Islemler { get; set; }
    public List<Calisan>? Calisanlar { get; set; }
}