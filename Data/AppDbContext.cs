using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablas principales de Bosko E-Commerce
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        // Tablas de autenticación
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        // Tablas de Admin Panel
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Tabla antigua (se mantiene para compatibilidad durante migración)
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // CONFIGURACIÓN DE PRODUCT Y CATEGORY
            // ============================================
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.Stock)
                    .IsRequired();
                
                entity.Property(e => e.Image)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                // Relación con Category
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                // Índices
                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("IX_Products_CategoryId");
                
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Products_Name");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.Image)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
            });

            // ============================================
            // CONFIGURACIÓN DE USER
            // ============================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150); // Actualizado para coincidir con DB
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150); // Actualizado para coincidir con DB
                
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Phone)
                    .HasMaxLength(50); // Actualizado para coincidir con DB
                
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50); // Actualizado para coincidir con DB
                
                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(50); // Actualizado para coincidir con DB
                
                entity.Property(e => e.IsActive)
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Índice único en Email
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
            });

            // ============================================
            // CONFIGURACIÓN DE PASSWORDRESETTOKEN
            // ============================================
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetTokens");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(255); // Actualizado para coincidir con DB
                
                entity.Property(e => e.ExpiresAt)
                    .IsRequired();
                
                entity.Property(e => e.IsUsed)
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                // Relación con User
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // CONFIGURACIÓN DE ORDER
            // ============================================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ShippingAddress)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.Subtotal)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Shipping)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Total)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Relación con Customer (User)
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Índices
                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("IX_Orders_CustomerId");
                
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Orders_Status");
                
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Orders_CreatedAt");
            });

            // ============================================
            // CONFIGURACIÓN DE ORDERITEM
            // ============================================
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Quantity)
                    .IsRequired();
                
                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Subtotal)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                // Relaciones
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Índices
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("IX_OrderItems_OrderId");
                
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("IX_OrderItems_ProductId");
            });

            // ============================================
            // CONFIGURACIÓN DE ORDERSTATUSHISTORY
            // ============================================
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.ToTable("OrderStatusHistory");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.Note)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Timestamp)
                    .IsRequired();
                
                // Relación
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.StatusHistory)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Índice
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("IX_OrderStatusHistory_OrderId");
            });

            // ============================================
            // CONFIGURACIÓN DE SHIPPINGADDRESS
            // ============================================
            modelBuilder.Entity<ShippingAddress>(entity =>
            {
                entity.ToTable("ShippingAddresses");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(100);
                
                // Relación uno a uno con Order
                entity.HasOne(e => e.Order)
                    .WithOne(o => o.ShippingAddressDetails)
                    .HasForeignKey<ShippingAddress>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // CONFIGURACIÓN DE PRODUCTOS (TABLA ANTIGUA)
            // ============================================
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();
                
                entity.Property(e => e.Stock)
                    .IsRequired();
                
                entity.Property(e => e.Categoria)
                    .HasMaxLength(50);
                
                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
            });
        }
    }
}
