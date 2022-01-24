using GrpcServer.DAO;

namespace GrpcServer.Services;

public class DbService : DbContext
{
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Article> Articles => Set<Article>();

    public DbService(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}
