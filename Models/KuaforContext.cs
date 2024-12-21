using Microsoft.EntityFrameworkCore;

namespace KuaforSalonuYonetim.Models;

public class KuaforContext:DbContext
{
    public DbSet<Kullanici> Kullanicilar { get; set; }
    public DbSet<Salon> Salonlar { get; set; }
    public DbSet<CalismaSaatleri> CalismaSaatleri { get; set; }

    public KuaforContext(DbContextOptions<KuaforContext> options):base(options)
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
            new CalismaSaatleri { CalismaSaatleriId = 1, SalonId = 1, Gun = "Monday", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri { CalismaSaatleriId = 2, SalonId = 1, Gun = "Tuesday", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri { CalismaSaatleriId = 3, SalonId = 1, Gun = "Wednesday", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri { CalismaSaatleriId = 4, SalonId = 1, Gun = "Thursday", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri { CalismaSaatleriId = 5, SalonId = 1, Gun = "Friday", BaslangicSaati = "09:00", BitisSaati = "21:00" },
            new CalismaSaatleri { CalismaSaatleriId = 6, SalonId = 1, Gun = "Saturday", BaslangicSaati = "09:00", BitisSaati = "19:00" },
            new CalismaSaatleri { CalismaSaatleriId = 7, SalonId = 1, Gun = "Sunday", BaslangicSaati = "Closed", BitisSaati = "Closed" }
        );
    }

}
