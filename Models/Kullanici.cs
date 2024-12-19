using System.ComponentModel.DataAnnotations;

namespace KuaforSalonuYonetim.Models;

public class Kullanici
{
    [Key] // Primary Key olarak işaretleniyor
    public int Kullanici_Id { get; set; }

    public string Kullanici_Adi { get; set; } = null!;
    public string Kullanici_Soyad { get; set; } = null!;
    public string Kullanici_Email { get; set; } = null!;
    public string Kullanici_Telefon { get; set; } = null!;
    public string Kullanici_Parola { get; set; } = null!;
    public string Kullanici_Rol { get; set; } = "Normal";
}
