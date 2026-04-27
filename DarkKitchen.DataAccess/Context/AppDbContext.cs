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
        ConfigureProduct(modelBuilder);
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

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(p => p.Code)
                .IsUnique();

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(p => p.Line)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(p => p.Active)
                .IsRequired();

            entity.HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
