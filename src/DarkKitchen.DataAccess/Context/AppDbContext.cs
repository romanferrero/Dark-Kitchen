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
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<DeliveryType> DeliveryTypes { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureProduct(modelBuilder);
        ConfigureProductImage(modelBuilder);
        ConfigurePromotion(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureOrderProduct(modelBuilder);
        ConfigureDeliveryType(modelBuilder);
        ConfigureAuditLog(modelBuilder);
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
                .HasMaxLength(100);

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
                .HasColumnType("nvarchar(max)");

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
                .ValueGeneratedNever();

            entity.Property(o => o.DeliveryName)
                .HasColumnName("DeliveryType")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(o => o.OrderStatus)
                .IsRequired();

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
                .WithOne()
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureOrderProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(op => new { op.OrderId, op.ProductId });

            entity.Property(op => op.Quantity)
                .IsRequired();

            entity.HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDeliveryType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeliveryType>(entity =>
        {
            entity.HasKey(dt => dt.Id);
            entity.Property(dt => dt.Id).ValueGeneratedOnAdd();

            entity.Property(dt => dt.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(dt => dt.Name)
                .IsUnique();

            entity.Property(dt => dt.ShippingCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        });
    }

    private static void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();

            entity.Property(a => a.Timestamp).IsRequired();

            entity.Property(a => a.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.EntityId).IsRequired();

            entity.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(a => a.ResponsibleUser)
                .IsRequired()
                .HasMaxLength(100);
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

            // BCrypt hash of "Admin@Passw0rd!!xx" (the seed must store a hash, not the raw password).
            Password = "$2a$11$mMk7V81c60wLjpLLrxZlR.kDQcSoubGtXYe5qIZ4//rnQCbJKoC5W",
            Role = UserRole.Admin
        });

        modelBuilder.Entity<DeliveryType>().HasData(
            new { Id = 1, Name = "Express", ShippingCost = 250m },
            new { Id = 2, Name = "SameDay", ShippingCost = 200m },
            new { Id = 3, Name = "NextDay", ShippingCost = 180m });
    }
}
