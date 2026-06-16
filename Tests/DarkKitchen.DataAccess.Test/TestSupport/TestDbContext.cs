using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.TestSupport;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> Entities { get; set; } = null!;
}
