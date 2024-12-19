using Microsoft.EntityFrameworkCore;

namespace KuaforSalonuYonetim.Models;

public class KuaforContext:DbContext
{
    public DbSet<Kullanici> Kullanicilar { get; set; }
    public KuaforContext(DbContextOptions<KuaforContext> options):base(options)
    {
            
    }
}
