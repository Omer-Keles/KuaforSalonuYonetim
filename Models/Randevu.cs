using System.ComponentModel.DataAnnotations;
namespace KuaforSalonuYonetim.Models;

public class Randevu
{
    [Key]
    public int RandevuId { get; set; }

    public int CalisanId { get; set; } // Çalışan ID
    public int IslemId { get; set; } // İşlem ID
    public DateTime RandevuTarihi { get; set; } // Randevu Tarihi
    public TimeSpan BaslangicSaati { get; set; } // Başlangıç Saati
    public TimeSpan BitisSaati { get; set; } // Bitiş Saati
    public decimal Ucret { get; set; } // Ücret
    public string Durum { get; set; } = "Beklemede";

    // Kullanıcı ile ilişki
    public int KullaniciId { get; set; }
    public Kullanici Kullanici { get; set; } = null!;

    // Çalışan ve İşlem ile ilişki
    public Calisan Calisan { get; set; } = null!;
    public Islem Islem { get; set; } = null!;
}
