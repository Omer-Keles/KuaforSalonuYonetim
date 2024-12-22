namespace KuaforSalonuYonetim.Models;

public class Islem
{
    public int IslemId { get; set; } // İşlem ID
    public string IslemAdi { get; set; } // İşlem Adı
    public string IslemAciklama { get; set; } // İşlem Açıklaması
    public decimal Ucret { get; set; } // İşlem Ücreti
    public int Sure { get; set; } // İşlem Süresi (dakika)
}