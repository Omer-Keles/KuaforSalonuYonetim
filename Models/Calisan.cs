using System.ComponentModel.DataAnnotations;

namespace KuaforSalonuYonetim.Models;

public class Calisan
{
    [Key]
    public int CalisanId { get; set; }
    public string CalisanAdi { get; set; } = null!;
    public string CalisanSoyadi { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    // Navigation Properties
    public List<CalisanIslem> CalisanIslemler { get; set; } = new(); // Çalışanın yapabildiği işlemler
    public List<CalisanUygunSaat> UygunSaatler { get; set; } = new();
}