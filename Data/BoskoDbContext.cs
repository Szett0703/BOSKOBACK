using Microsoft.EntityFrameworkCore;
using BOSKOBACK.Models;

namespace BOSKOBACK.Data
{
    public class BoskoDbContext : DbContext
    {
        public BoskoDbContext(DbContextOptions<BoskoDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint on Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure decimal precision for Product Price
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Configure decimal precision for Order Total
            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            // Configure decimal precision for OrderItem UnitPrice
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            // Category-Product relationship (one-to-many)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // User-Order relationship (one-to-many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order-OrderItem relationship (one-to-many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product-OrderItem relationship (one-to-many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Men", Description = "Fashion for Men", ImageUrl = "https://images.unsplash.com/photo-1490114538077-0a7f8cb49891?w=400&h=300&fit=crop" },
                new Category { Id = 2, Name = "Women", Description = "Fashion for Women", ImageUrl = "https://images.unsplash.com/photo-1483985988355-763728e1935b?w=400&h=300&fit=crop" },
                new Category { Id = 3, Name = "Kids", Description = "Fashion for Kids", ImageUrl = "https://images.unsplash.com/photo-1503919545889-aef636e10ad4?w=400&h=300&fit=crop" },
                new Category { Id = 4, Name = "Accessories", Description = "Fashion Accessories", ImageUrl = "https://images.unsplash.com/photo-1523779917675-b6ed3a42a561?w=400&h=300&fit=crop" },
                new Category { Id = 5, Name = "Shoes", Description = "Footwear Collection", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=300&fit=crop" },
                new Category { Id = 6, Name = "Sports", Description = "Sports & Active Wear", ImageUrl = "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400&h=300&fit=crop" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Classic Winter Jacket",
                    Description = "Stay warm in style with our premium winter jacket",
                    Price = 129.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400&h=500&fit=crop",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Elegant Summer Dress",
                    Description = "Perfect for any summer occasion",
                    Price = 89.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=400&h=500&fit=crop",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Designer Sneakers",
                    Description = "Comfortable and stylish everyday wear",
                    Price = 149.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?w=400&h=500&fit=crop",
                    CategoryId = 5
                },
                new Product
                {
                    Id = 4,
                    Name = "Luxury Watch",
                    Description = "Timeless elegance for your wrist",
                    Price = 299.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=500&fit=crop",
                    CategoryId = 4
                },
                new Product
                {
                    Id = 5,
                    Name = "Kids Casual T-Shirt",
                    Description = "Comfortable cotton t-shirt for active kids",
                    Price = 24.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1503944583220-79d8926ad5e2?w=400&h=500&fit=crop",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 6,
                    Name = "Sports Running Shoes",
                    Description = "High-performance running shoes",
                    Price = 119.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=500&fit=crop",
                    CategoryId = 6
                },
                new Product
                {
                    Id = 7,
                    Name = "Leather Handbag",
                    Description = "Elegant leather handbag for any occasion",
                    Price = 179.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1564422170194-896b89110ef8?w=400&h=500&fit=crop",
                    CategoryId = 4
                },
                new Product
                {
                    Id = 8,
                    Name = "Denim Jeans",
                    Description = "Classic fit denim jeans",
                    Price = 79.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400&h=500&fit=crop",
                    CategoryId = 1
                }
            );
        }
    }
}
