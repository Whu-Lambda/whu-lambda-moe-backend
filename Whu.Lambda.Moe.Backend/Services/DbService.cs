using Whu.Lambda.Moe.Backend.Dao;

namespace Whu.Lambda.Moe.Backend.Services;

public class DbService : DbContext
{
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Article> Articles => Set<Article>();

    public DbService(DbContextOptions options) : base(options) =>
        Database.EnsureCreated();
}
