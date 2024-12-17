using Microsoft.EntityFrameworkCore;

namespace KuaforSalonuYonetim.Models;

public class KuaforContext:DbContext
{
    public DbSet<Employee> Employees {  get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public KuaforContext(DbContextOptions<KuaforContext> options):base(options)
    {
            
    }
}
