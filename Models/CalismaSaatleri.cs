namespace KuaforSalonuYonetim.Models;

public class CalismaSaatleri
{
    public int CalismaSaatleriId { get; set; }
    public int SalonId { get; set; }
    public string Gun { get; set; } // Gün adı (Pazartesi, Salı, vb.)
    public string BaslangicSaati { get; set; } // Başlangıç saati
    public string BitisSaati { get; set; } // Bitiş saati
}
