using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
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
        ConfigureProductImage(modelBuilder);
        ConfigurePromotion(modelBuilder);
        ConfigureOrder(modelBuilder);
        SeedData(modelBuilder);
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

    private static void ConfigureProductImage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(pi => pi.Id);
            entity.Property(pi => pi.Id)
                .ValueGeneratedOnAdd();

            entity.Property(pi => pi.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(pi => pi.SizeInKb)
                .HasColumnType("decimal(10,2)");
        });
    }

    private static void ConfigurePromotion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.DiscountPercentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            entity.Property(p => p.DateFrom)
                .IsRequired();

            entity.Property(p => p.DateTo)
                .IsRequired();

            entity.HasMany(p => p.Products)
                .WithMany()
                .UsingEntity(j => j.ToTable("PromotionProducts"));
        });
    }

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderId);
            entity.Property(o => o.OrderId)
                .ValueGeneratedOnAdd();

            entity.Property(o => o.OrderNumber)
                .ValueGeneratedOnAdd();

            entity.Property(o => o.DeliveryType)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(o => o.OrderStatus)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            entity.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)");

            entity.Property(o => o.TotalCost)
                .HasColumnType("decimal(18,2)");

            entity.Property(o => o.OrderDate)
                .IsRequired();

            entity.OwnsOne(o => o.Address, address =>
            {
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(200);

                address.Property(a => a.DoorNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                address.Property(a => a.Apartment)
                    .HasMaxLength(50);
            });

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(o => o.Products)
                .WithMany()
                .UsingEntity(j => j.ToTable("OrderProducts"));
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new
        {
            Id = 1,
            FirstName = "Admin",
            LastName = "AdminUser",
            Email = "admin@darkkitchen.com",
            Phone = "099111222",
            Password = "Admin@Passw0rd!!xx",
            Role = UserRole.Admin
        });
    }
}
