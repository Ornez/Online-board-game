using Microsoft.EntityFrameworkCore;
using API.Entities;

namespace API.Data;
public class DataContext : DbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Language> Language { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Statistics> Statistics { get; set; }
    
    public DataContext(DbContextOptions options) : base(options)
    {
    }
}
