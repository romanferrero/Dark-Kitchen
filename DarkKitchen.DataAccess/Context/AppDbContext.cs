using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Phone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(25);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>();
        });
    }
}
