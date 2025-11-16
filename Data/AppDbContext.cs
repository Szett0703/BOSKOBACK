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

        // Tablas de Admin Panel (nuevas)
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Tabla antigua (se mantiene para compatibilidad durante migración)
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Product y Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de índices para mejor performance
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            // Configuración de User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
                
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.HasIndex(e => e.Role)
                    .HasDatabaseName("IX_Users_Role");
                
                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.HasIndex(e => e.Provider)
                    .HasDatabaseName("IX_Users_Provider");
            });

            // Configuración de PasswordResetToken
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetTokens");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => e.Token)
                    .HasDatabaseName("IX_PasswordResetTokens_Token");
            });

            // Configuración de Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("IX_Orders_CustomerId");
                
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Orders_Status");
                
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Orders_CreatedAt");
            });

            // Configuración de OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("IX_OrderItems_OrderId");
                
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("IX_OrderItems_ProductId");
            });

            // Configuración de OrderStatusHistory
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.ToTable("OrderStatusHistory");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.StatusHistory)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("IX_OrderStatusHistory_OrderId");
            });

            // Configuración de ActivityLog
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.ToTable("ActivityLogs");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName("IX_ActivityLogs_Timestamp");
                
                entity.HasIndex(e => e.Type)
                    .HasDatabaseName("IX_ActivityLogs_Type");
            });

            // Configuración de Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_Notifications_UserId");
                
                entity.HasIndex(e => e.IsRead)
                    .HasDatabaseName("IX_Notifications_IsRead");
            });

            // Configuración de tabla antigua Productos (se mantiene)
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Stock).IsRequired();
                entity.Property(e => e.Categoria).HasMaxLength(50);
                entity.Property(e => e.FechaCreacion).IsRequired();
            });
        }
    }
}
