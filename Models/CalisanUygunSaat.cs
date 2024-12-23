using System.ComponentModel.DataAnnotations;

namespace KuaforSalonuYonetim.Models;

public class CalisanUygunSaat
{
    [Key]
    public int Id { get; set; }
    public int CalisanId { get; set; }
    public string Gun { get; set; } = null!; // Örneğin: "Pazartesi", "Salı"
    public TimeSpan BaslangicSaati { get; set; } // Başlangıç saati
    public TimeSpan BitisSaati { get; set; } // Bitiş saati

    // Navigation Property
    public Calisan? Calisan { get; set; }
}