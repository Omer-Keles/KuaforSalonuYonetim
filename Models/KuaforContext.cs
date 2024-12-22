using Microsoft.EntityFrameworkCore;

namespace KuaforSalonuYonetim.Models;

public class KuaforContext : DbContext
{
    public DbSet<Kullanici> Kullanicilar { get; set; }
    public DbSet<Salon> Salonlar { get; set; }
    public DbSet<CalismaSaatleri> CalismaSaatleri { get; set; }

    public DbSet<Islem> Islemler { get; set; }

    public KuaforContext(DbContextOptions<KuaforContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Varsayılan Salon Verisi
        modelBuilder.Entity<Salon>().HasData(new Salon
        {
            SalonId = 1,
            SalonAdi = "SAÜ Kuaför",
            Adres = "Sakarya Üniversitesi",
            Telefon = "+905545455454"
        });

        // Varsayılan Çalışma Saatleri Verisi
        modelBuilder.Entity<CalismaSaatleri>().HasData(
            new CalismaSaatleri
            {
                CalismaSaatleriId = 1, SalonId = 1, Gun = "Pazartesi", BaslangicSaati = "09:00", BitisSaati = "21:00"
            },
            new CalismaSaatleri
                { CalismaSaatleriId = 2, SalonId = 1, Gun = "Salı", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri
            {
                CalismaSaatleriId = 3, SalonId = 1, Gun = "Çarşamba", BaslangicSaati = "09:00", BitisSaati = "21:00"
            },
            new CalismaSaatleri
            {
                CalismaSaatleriId = 4, SalonId = 1, Gun = "Perşembe", BaslangicSaati = "09:00", BitisSaati = "21:00"
            },
            new CalismaSaatleri
                { CalismaSaatleriId = 5, SalonId = 1, Gun = "Cuma", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri
            {
                CalismaSaatleriId = 6, SalonId = 1, Gun = "Cumartesi", BaslangicSaati = "09:00", BitisSaati = "19:00"
            }
        );

        modelBuilder.Entity<Islem>().HasData(
            new Islem
            {
                IslemId = 1, IslemAdi = "Saç Kesim",
                IslemAciklama =
                    "Makas veya makineyle yapılan klasik, modern ve trend kesimler yapılır. Saç isteğe göre şekillendirilir.",
                Sure = 60, Ucret = 150
            },
            new Islem
            {
                IslemId = 2, IslemAdi = "Sakal Kesim",
                IslemAciklama =
                    "Ustura veya makineyle sakal ve bıyık kesimi yapılır.İsteğe göre sakal şekillendirilir ve bıyık düzeltilir",
                Sure = 30, Ucret = 100
            },
            new Islem
            {
                IslemId = 3, IslemAdi = "Saç ve Sakal Bakım",
                IslemAciklama = "Saç yıkama, masaj yapılır, saç maskeleri, keratin bakımı yapılır", Sure = 30,
                Ucret = 100
            },
            new Islem
            {
                IslemId = 4, IslemAdi = "Saç Boyama",
                IslemAciklama = "Doğal veya modern renklerle saç boyama, beyaz kapatma yapılır.", Sure = 90,
                Ucret = 500
            }
        );
    }
}