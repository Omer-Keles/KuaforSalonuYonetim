using System.ComponentModel.DataAnnotations;

namespace KuaforSalonuYonetim.Models;

public class CalisanIslem
{
    [Key]
    public int Id { get; set; }
    public int CalisanId { get; set; }
    public int IslemId { get; set; }

    // Navigation Properties
    public Calisan? Calisan { get; set; }
    public Islem? Islem { get; set; }
}