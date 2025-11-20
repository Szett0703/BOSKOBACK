using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace DBTest_BACK.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            Console.WriteLine("=== INICIANDO DATABASE SEEDER ===");

            // ============================================
            // 1. VERIFICAR SI YA EXISTE DATA
            // ============================================
            if (await db.Users.AnyAsync())
            {
                Console.WriteLine("⚠️  La base de datos ya contiene datos. Seeder cancelado para evitar duplicados.");
                return;
            }

            Console.WriteLine("✅ Base de datos vacía. Iniciando seed...");

            // ============================================
            // 2. CREAR USUARIOS (1 Admin + 5 Clientes)
            // ============================================
            Console.WriteLine("📋 Creando usuarios...");

            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@Bosko2025");
            var clientPasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente@123");

            var users = new List<User>
            {
                // Usuario Admin
                new User
                {
                    Name = "Santiago Admin",
                    Email = "admin@bosko.com",
                    PasswordHash = adminPasswordHash,
                    Phone = "+34 612 345 678",
                    Role = "Admin",
                    Provider = "local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                // Cliente 1
                new User
                {
                    Name = "María García",
                    Email = "cliente1@bosko.com",
                    PasswordHash = clientPasswordHash,
                    Phone = "+34 611 111 111",
                    Role = "Customer",
                    Provider = "local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-5),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                // Cliente 2
                new User
                {
                    Name = "Juan Pérez",
                    Email = "cliente2@bosko.com",
                    PasswordHash = clientPasswordHash,
                    Phone = "+34 622 222 222",
                    Role = "Customer",
                    Provider = "local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                // Cliente 3
                new User
                {
                    Name = "Ana Rodríguez",
                    Email = "cliente3@bosko.com",
                    PasswordHash = clientPasswordHash,
                    Phone = "+34 633 333 333",
                    Role = "Customer",
                    Provider = "local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                // Cliente 4
                new User
                {
                    Name = "Carlos López",
                    Email = "cliente4@bosko.com",
                    PasswordHash = clientPasswordHash,
                    Phone = "+34 644 444 444",
                    Role = "Customer",
                    Provider = "local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                // Cliente 5
                new User
                {
                    Name = "Laura Martínez",
                    Email = "cliente5@bosko.com",
                    PasswordHash = clientPasswordHash,
                    Phone = "+34 655 555 555",
                    Role = "Customer",
                    Provider = "local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-1)
                }
            };

            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {users.Count} usuarios creados");

            // ============================================
            // 3. CREAR CATEGORÍAS
            // ============================================
            Console.WriteLine("📋 Creando categorías...");

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Hombres",
                    Description = "Ropa y accesorios para hombre",
                    Image = "https://images.unsplash.com/photo-1490578474895-699cd4e2cf59",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Category
                {
                    Name = "Mujeres",
                    Description = "Ropa y accesorios para mujer",
                    Image = "https://images.unsplash.com/photo-1483985988355-763728e1935b",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Category
                {
                    Name = "Niños",
                    Description = "Ropa y accesorios para niños",
                    Image = "https://images.unsplash.com/photo-1514090458221-65bb69cf63e3",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Category
                {
                    Name = "Accesorios",
                    Description = "Complementos y accesorios de moda",
                    Image = "https://images.unsplash.com/photo-1523779105320-d1cd346ff52b",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Category
                {
                    Name = "Calzado",
                    Description = "Zapatos y calzado deportivo",
                    Image = "https://images.unsplash.com/photo-1549298916-b41d501d3772",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Category
                {
                    Name = "Deportiva",
                    Description = "Ropa deportiva y fitness",
                    Image = "https://images.unsplash.com/photo-1517836357463-d25dfeac3438",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                }
            };

            await db.Categories.AddRangeAsync(categories);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {categories.Count} categorías creadas");

            // ============================================
            // 4. CREAR PRODUCTOS (5 por categoría = 30)
            // ============================================
            Console.WriteLine("📋 Creando productos...");

            var products = new List<Product>
            {
                // CATEGORÍA: HOMBRES (5 productos)
                new Product
                {
                    Name = "Camisa Formal Blanca",
                    Description = "Camisa de vestir 100% algodón para hombre",
                    Price = 39.99m,
                    Stock = 50,
                    CategoryId = categories[0].Id,
                    Image = "https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Product
                {
                    Name = "Jeans Azul Oscuro",
                    Description = "Pantalón vaquero ajustado para hombre",
                    Price = 49.99m,
                    Stock = 40,
                    CategoryId = categories[0].Id,
                    Image = "https://images.unsplash.com/photo-1542272604-787c3835535d",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Product
                {
                    Name = "Chaqueta de Cuero",
                    Description = "Chaqueta de cuero sintético estilo biker",
                    Price = 129.99m,
                    Stock = 20,
                    CategoryId = categories[0].Id,
                    Image = "https://images.unsplash.com/photo-1551028719-00167b16eac5",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Polo Básico Gris",
                    Description = "Polo clásico de manga corta",
                    Price = 24.99m,
                    Stock = 60,
                    CategoryId = categories[0].Id,
                    Image = "https://images.unsplash.com/photo-1586790170083-2f9ceadc732d",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Suéter de Lana",
                    Description = "Suéter tejido de lana merino",
                    Price = 69.99m,
                    Stock = 30,
                    CategoryId = categories[0].Id,
                    Image = "https://images.unsplash.com/photo-1620799140408-edc6dcb6d633",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },

                // CATEGORÍA: MUJERES (5 productos)
                new Product
                {
                    Name = "Vestido Floral Verano",
                    Description = "Vestido ligero con estampado floral",
                    Price = 59.99m,
                    Stock = 35,
                    CategoryId = categories[1].Id,
                    Image = "https://images.unsplash.com/photo-1595777457583-95e059d581b8",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Product
                {
                    Name = "Blusa de Seda Blanca",
                    Description = "Blusa elegante de seda natural",
                    Price = 79.99m,
                    Stock = 25,
                    CategoryId = categories[1].Id,
                    Image = "https://images.unsplash.com/photo-1618932260643-eee4a2f652a6",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Pantalón de Vestir Negro",
                    Description = "Pantalón de corte recto para oficina",
                    Price = 54.99m,
                    Stock = 45,
                    CategoryId = categories[1].Id,
                    Image = "https://images.unsplash.com/photo-1594633312681-425c7b97ccd1",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Abrigo Largo Beige",
                    Description = "Abrigo elegante de invierno",
                    Price = 149.99m,
                    Stock = 15,
                    CategoryId = categories[1].Id,
                    Image = "https://images.unsplash.com/photo-1539533018447-63fcce2678e3",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Falda Plisada Negra",
                    Description = "Falda midi plisada elegante",
                    Price = 44.99m,
                    Stock = 40,
                    CategoryId = categories[1].Id,
                    Image = "https://images.unsplash.com/photo-1583496661160-fb5886a0aaaa",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },

                // CATEGORÍA: NIÑOS (5 productos)
                new Product
                {
                    Name = "Camiseta Dinosaurios",
                    Description = "Camiseta divertida con estampado de dinosaurios",
                    Price = 14.99m,
                    Stock = 70,
                    CategoryId = categories[2].Id,
                    Image = "https://images.unsplash.com/photo-1519238263530-99bdd11df2ea",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Sudadera con Capucha",
                    Description = "Sudadera cómoda para niños",
                    Price = 29.99m,
                    Stock = 50,
                    CategoryId = categories[2].Id,
                    Image = "https://images.unsplash.com/photo-1503342217505-b0a15ec3261c",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Pantalón Deportivo Azul",
                    Description = "Pantalón deportivo para actividades",
                    Price = 24.99m,
                    Stock = 55,
                    CategoryId = categories[2].Id,
                    Image = "https://images.unsplash.com/photo-1560857792-215f9e3534ed",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Vestido Princesa Rosa",
                    Description = "Vestido de fiesta para niñas",
                    Price = 39.99m,
                    Stock = 30,
                    CategoryId = categories[2].Id,
                    Image = "https://images.unsplash.com/photo-1518831959646-742c3a14ebf7",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Chaqueta Acolchada",
                    Description = "Chaqueta de invierno para niños",
                    Price = 49.99m,
                    Stock = 40,
                    CategoryId = categories[2].Id,
                    Image = "https://images.unsplash.com/photo-1514846326710-096e4a8bcb05",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },

                // CATEGORÍA: ACCESORIOS (5 productos)
                new Product
                {
                    Name = "Bolso de Mano Negro",
                    Description = "Bolso elegante de cuero sintético",
                    Price = 79.99m,
                    Stock = 25,
                    CategoryId = categories[3].Id,
                    Image = "https://images.unsplash.com/photo-1590874103328-eac38a683ce7",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Product
                {
                    Name = "Gafas de Sol Aviador",
                    Description = "Gafas de sol estilo aviador con protección UV",
                    Price = 34.99m,
                    Stock = 60,
                    CategoryId = categories[3].Id,
                    Image = "https://images.unsplash.com/photo-1511499767150-a48a237f0083",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Cinturón de Cuero Marrón",
                    Description = "Cinturón clásico de cuero genuino",
                    Price = 29.99m,
                    Stock = 45,
                    CategoryId = categories[3].Id,
                    Image = "https://images.unsplash.com/photo-1624222247344-550fb60583bb",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Bufanda de Lana Gris",
                    Description = "Bufanda cálida de lana merino",
                    Price = 24.99m,
                    Stock = 50,
                    CategoryId = categories[3].Id,
                    Image = "https://images.unsplash.com/photo-1520903920243-00d872a2d1c9",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Reloj Digital Deportivo",
                    Description = "Reloj digital resistente al agua",
                    Price = 89.99m,
                    Stock = 35,
                    CategoryId = categories[3].Id,
                    Image = "https://images.unsplash.com/photo-1523275335684-37898b6baf30",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },

                // CATEGORÍA: CALZADO (5 productos)
                new Product
                {
                    Name = "Zapatillas Running Negras",
                    Description = "Zapatillas deportivas para correr",
                    Price = 89.99m,
                    Stock = 40,
                    CategoryId = categories[4].Id,
                    Image = "https://images.unsplash.com/photo-1542291026-7eec264c27ff",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Product
                {
                    Name = "Botas de Cuero Marrón",
                    Description = "Botas elegantes de cuero genuino",
                    Price = 119.99m,
                    Stock = 25,
                    CategoryId = categories[4].Id,
                    Image = "https://images.unsplash.com/photo-1608256246200-53e635b5b65f",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Sandalias de Verano",
                    Description = "Sandalias cómodas para el verano",
                    Price = 34.99m,
                    Stock = 50,
                    CategoryId = categories[4].Id,
                    Image = "https://images.unsplash.com/photo-1603808033587-e9a2ba417d8e",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Zapatos Formales Negros",
                    Description = "Zapatos de vestir para ocasiones especiales",
                    Price = 99.99m,
                    Stock = 30,
                    CategoryId = categories[4].Id,
                    Image = "https://images.unsplash.com/photo-1533867617858-e7b97e060509",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Zapatillas Casual Blancas",
                    Description = "Zapatillas versátiles para uso diario",
                    Price = 69.99m,
                    Stock = 45,
                    CategoryId = categories[4].Id,
                    Image = "https://images.unsplash.com/photo-1549298916-b41d501d3772",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },

                // CATEGORÍA: DEPORTIVA (5 productos)
                new Product
                {
                    Name = "Camiseta Técnica Running",
                    Description = "Camiseta transpirable para correr",
                    Price = 29.99m,
                    Stock = 60,
                    CategoryId = categories[5].Id,
                    Image = "https://images.unsplash.com/photo-1571902943202-507ec2618e8f",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Mallas de Yoga",
                    Description = "Mallas elásticas para yoga y pilates",
                    Price = 39.99m,
                    Stock = 50,
                    CategoryId = categories[5].Id,
                    Image = "https://images.unsplash.com/photo-1506629082955-511b1aa562c8",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Product
                {
                    Name = "Chaqueta Deportiva",
                    Description = "Chaqueta ligera para entrenamientos",
                    Price = 59.99m,
                    Stock = 35,
                    CategoryId = categories[5].Id,
                    Image = "https://images.unsplash.com/photo-1556821840-3a63f95609a7",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Short Deportivo",
                    Description = "Short cómodo para gimnasio",
                    Price = 24.99m,
                    Stock = 55,
                    CategoryId = categories[5].Id,
                    Image = "https://images.unsplash.com/photo-1591195853828-11db59a44f6b",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Product
                {
                    Name = "Top Deportivo Mujer",
                    Description = "Top de soporte para actividades deportivas",
                    Price = 34.99m,
                    Stock = 45,
                    CategoryId = categories[5].Id,
                    Image = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                }
            };

            await db.Products.AddRangeAsync(products);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {products.Count} productos creados");

            // ============================================
            // 5. CREAR DIRECCIONES (2 por usuario)
            // ============================================
            Console.WriteLine("📋 Creando direcciones de usuarios...");

            var addresses = new List<Address>
            {
                // Direcciones María García
                new Address
                {
                    UserId = users[1].Id,
                    Label = "Casa",
                    Street = "Calle Gran Vía 123, 3º A",
                    City = "Madrid",
                    State = "Madrid",
                    PostalCode = "28013",
                    Country = "España",
                    Phone = "+34 611 111 111",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-5),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new Address
                {
                    UserId = users[1].Id,
                    Label = "Trabajo",
                    Street = "Paseo de la Castellana 200",
                    City = "Madrid",
                    State = "Madrid",
                    PostalCode = "28046",
                    Country = "España",
                    Phone = "+34 611 111 111",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-4)
                },

                // Direcciones Juan Pérez
                new Address
                {
                    UserId = users[2].Id,
                    Label = "Casa",
                    Street = "Avenida Diagonal 450, 2º B",
                    City = "Barcelona",
                    State = "Cataluña",
                    PostalCode = "08006",
                    Country = "España",
                    Phone = "+34 622 222 222",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Address
                {
                    UserId = users[2].Id,
                    Label = "Casa de Campo",
                    Street = "Camino Rural 45",
                    City = "Sitges",
                    State = "Cataluña",
                    PostalCode = "08870",
                    Country = "España",
                    Phone = "+34 622 222 222",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-3)
                },

                // Direcciones Ana Rodríguez
                new Address
                {
                    UserId = users[3].Id,
                    Label = "Casa",
                    Street = "Calle Colón 78, 5º C",
                    City = "Valencia",
                    State = "Valencia",
                    PostalCode = "46004",
                    Country = "España",
                    Phone = "+34 633 333 333",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Address
                {
                    UserId = users[3].Id,
                    Label = "Oficina",
                    Street = "Avenida del Puerto 200",
                    City = "Valencia",
                    State = "Valencia",
                    PostalCode = "46023",
                    Country = "España",
                    Phone = "+34 633 333 333",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-2)
                },

                // Direcciones Carlos López
                new Address
                {
                    UserId = users[4].Id,
                    Label = "Casa",
                    Street = "Calle Real 56, 1º D",
                    City = "Sevilla",
                    State = "Andalucía",
                    PostalCode = "41001",
                    Country = "España",
                    Phone = "+34 644 444 444",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                new Address
                {
                    UserId = users[4].Id,
                    Label = "Padres",
                    Street = "Plaza Mayor 12",
                    City = "Córdoba",
                    State = "Andalucía",
                    PostalCode = "14003",
                    Country = "España",
                    Phone = "+34 644 444 444",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-1)
                },

                // Direcciones Laura Martínez
                new Address
                {
                    UserId = users[5].Id,
                    Label = "Casa",
                    Street = "Calle Larios 89, 4º A",
                    City = "Málaga",
                    State = "Andalucía",
                    PostalCode = "29015",
                    Country = "España",
                    Phone = "+34 655 555 555",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-1)
                },
                new Address
                {
                    UserId = users[5].Id,
                    Label = "Apartamento Playa",
                    Street = "Paseo Marítimo 34",
                    City = "Marbella",
                    State = "Andalucía",
                    PostalCode = "29600",
                    Country = "España",
                    Phone = "+34 655 555 555",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                }
            };

            await db.Addresses.AddRangeAsync(addresses);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {addresses.Count} direcciones creadas");

            // ============================================
            // 6. CREAR ÓRDENES (15 órdenes)
            // ============================================
            Console.WriteLine("📋 Creando órdenes...");

            var random = new Random();
            var orders = new List<Order>();

            var orderStatuses = new[] { "pending", "processing", "shipped", "delivered", "cancelled" };
            var paymentMethods = new[] { "credit_card", "debit_card", "paypal" };

            // Generar 15 órdenes distribuidas entre los 5 clientes
            for (int i = 0; i < 15; i++)
            {
                var customerIndex = (i % 5) + 1; // Clientes del 1 al 5
                var customer = users[customerIndex];
                var daysAgo = random.Next(10, 30);
                var createdAt = DateTime.UtcNow.AddDays(-daysAgo);
                var updatedDays = random.Next(1, 4);
                var updatedAt = createdAt.AddDays(updatedDays);

                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderNumber = $"ORD-2024-{(1000 + i):D4}",
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email,
                    ShippingAddress = $"{addresses[customerIndex * 2 - 2].Street}, {addresses[customerIndex * 2 - 2].City}, {addresses[customerIndex * 2 - 2].PostalCode}",
                    Subtotal = 0, // Se calculará después
                    Tax = 0m,
                    Shipping = random.Next(5, 21) * 1.0m,
                    Total = 0, // Se calculará después
                    Status = orderStatuses[random.Next(orderStatuses.Length)],
                    PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                    TrackingNumber = i % 3 == 0 ? $"TRK-2024-{(5000 + i):D4}" : null,
                    Notes = i % 4 == 0 ? "Entregar por la tarde" : null,
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt
                };

                orders.Add(order);
            }

            await db.Orders.AddRangeAsync(orders);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {orders.Count} órdenes creadas");

            // ============================================
            // 7. CREAR ORDER ITEMS (2-4 items por orden)
            // ============================================
            Console.WriteLine("📋 Creando items de órdenes...");

            var orderItems = new List<OrderItem>();

            foreach (var order in orders)
            {
                var itemCount = random.Next(2, 5); // 2 a 4 items por orden
                var selectedProductIndices = new HashSet<int>();

                for (int i = 0; i < itemCount; i++)
                {
                    int productIndex;
                    do
                    {
                        productIndex = random.Next(products.Count);
                    } while (selectedProductIndices.Contains(productIndex));

                    selectedProductIndices.Add(productIndex);
                    var product = products[productIndex];
                    var quantity = random.Next(1, 4);
                    var price = product.Price;
                    var subtotal = price * quantity;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductImage = product.Image,
                        Quantity = quantity,
                        Price = price,
                        Subtotal = subtotal
                    };

                    orderItems.Add(orderItem);
                    order.Subtotal += subtotal;
                }

                order.Total = order.Subtotal + order.Tax + order.Shipping;
            }

            await db.OrderItems.AddRangeAsync(orderItems);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {orderItems.Count} items de órdenes creados");

            // Actualizar totales de órdenes
            db.Orders.UpdateRange(orders);
            await db.SaveChangesAsync();
            Console.WriteLine("✅ Totales de órdenes actualizados");

            // ============================================
            // 8. CREAR SHIPPING ADDRESSES (1:1 con Order)
            // ============================================
            Console.WriteLine("📋 Creando direcciones de envío...");

            var shippingAddresses = new List<ShippingAddress>();
            var countries = new[] { "España", "México", "Colombia" };

            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                var customerIndex = (i % 5) + 1;
                var userAddress = addresses[customerIndex * 2 - 2];

                var shippingAddress = new ShippingAddress
                {
                    OrderId = order.Id,
                    FullName = order.CustomerName,
                    Phone = userAddress.Phone ?? "+34 600 000 000",
                    Street = userAddress.Street,
                    City = userAddress.City,
                    State = userAddress.State ?? "N/A",
                    PostalCode = userAddress.PostalCode,
                    Country = countries[random.Next(countries.Length)]
                };

                shippingAddresses.Add(shippingAddress);
            }

            await db.ShippingAddresses.AddRangeAsync(shippingAddresses);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {shippingAddresses.Count} direcciones de envío creadas");

            // ============================================
            // 9. CREAR ORDER STATUS HISTORY
            // ============================================
            Console.WriteLine("📋 Creando historial de estados...");

            var orderStatusHistories = new List<OrderStatusHistory>();

            foreach (var order in orders)
            {
                var currentStatus = order.Status;
                var baseDate = order.CreatedAt;

                // Historial según el estado final
                switch (currentStatus)
                {
                    case "pending":
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        break;

                    case "processing":
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "processing",
                            Note = "Pedido en preparación",
                            Timestamp = baseDate.AddHours(6)
                        });
                        break;

                    case "shipped":
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "processing",
                            Note = "Pedido en preparación",
                            Timestamp = baseDate.AddHours(6)
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "shipped",
                            Note = "Pedido enviado",
                            Timestamp = baseDate.AddDays(1)
                        });
                        break;

                    case "delivered":
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "processing",
                            Note = "Pedido en preparación",
                            Timestamp = baseDate.AddHours(6)
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "shipped",
                            Note = "Pedido enviado",
                            Timestamp = baseDate.AddDays(1)
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "delivered",
                            Note = "Pedido entregado con éxito",
                            Timestamp = baseDate.AddDays(3)
                        });
                        break;

                    case "cancelled":
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        orderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "cancelled",
                            Note = "Pedido cancelado por el cliente",
                            Timestamp = baseDate.AddHours(12)
                        });
                        break;
                }
            }

            await db.OrderStatusHistory.AddRangeAsync(orderStatusHistories);
            await db.SaveChangesAsync();
            Console.WriteLine($"✅ {orderStatusHistories.Count} registros de historial creados");

            // ============================================
            // 10. RESUMEN FINAL
            // ============================================
            Console.WriteLine("\n=== SEED COMPLETADO CON ÉXITO ===");
            Console.WriteLine($"✅ {users.Count} usuarios");
            Console.WriteLine($"✅ {categories.Count} categorías");
            Console.WriteLine($"✅ {products.Count} productos");
            Console.WriteLine($"✅ {addresses.Count} direcciones");
            Console.WriteLine($"✅ {orders.Count} órdenes");
            Console.WriteLine($"✅ {orderItems.Count} items de órdenes");
            Console.WriteLine($"✅ {shippingAddresses.Count} direcciones de envío");
            Console.WriteLine($"✅ {orderStatusHistories.Count} registros de historial");
            Console.WriteLine("==================================\n");
        }
    }
}
