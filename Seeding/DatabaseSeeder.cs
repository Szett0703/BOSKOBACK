using DBTest_BACK.Data;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Diagnostics;

namespace DBTest_BACK.Seeding
{
    /// <summary>
    /// Database Seeder profesional para Bosko E-Commerce
    /// .NET 8 + EF Core + PostgreSQL + Railway
    /// 100% Idempotente, Async y Production-Ready
    /// </summary>
    public static class DatabaseSeeder
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Método principal para ejecutar el seeding completo de la base de datos
        /// </summary>
        public static async Task SeedAsync(AppDbContext db)
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("================================================================================");
            Console.WriteLine("[SEED] INICIANDO DATABASE SEEDER - BOSKO E-COMMERCE");
            Console.WriteLine("================================================================================");

            try
            {
                // ==================================================
                // NIVEL 1: Entidades sin dependencias
                // ==================================================
                var users = await SeedUsersAsync(db);
                var categories = await SeedCategoriesAsync(db);

                // ==================================================
                // NIVEL 2: Entidades que dependen de nivel 1
                // ==================================================
                var products = await SeedProductsAsync(db, categories);
                var addresses = await SeedAddressesAsync(db, users);

                // ==================================================
                // NIVEL 3: Entidades que dependen de nivel 1-2
                // ==================================================
                var orders = await SeedOrdersAsync(db, users, addresses);

                // ==================================================
                // NIVEL 4: Entidades que dependen de nivel 3
                // ==================================================
                await SeedOrderItemsAsync(db, orders, products);
                await SeedShippingAddressesAsync(db, orders, addresses);
                await SeedOrderStatusHistoryAsync(db, orders);

                stopwatch.Stop();

                // ==================================================
                // RESUMEN FINAL
                // ==================================================
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("[SEED] ✅ SEEDING COMPLETADO CON ÉXITO");
                Console.WriteLine($"[SEED] ⏱️  Tiempo total: {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine("================================================================================\n");

                // Crear archivo de documentación
                await CreateSeedInfoFileAsync(stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine("\n================================================================================");
                Console.WriteLine($"[SEED] ❌ ERROR DURANTE EL SEEDING: {ex.Message}");
                Console.WriteLine($"[SEED] Stack Trace: {ex.StackTrace}");
                Console.WriteLine("================================================================================\n");
                throw;
            }
        }

        #region USERS

        private static async Task<List<User>> SeedUsersAsync(AppDbContext db)
        {
            if (await db.Users.AnyAsync())
            {
                Console.WriteLine("[SKIP] Usuarios ya existen en la base de datos");
                return await db.Users.OrderBy(u => u.Id).ToListAsync();
            }

            Console.WriteLine("[SEED] Creando usuarios...");
            var users = CreateUsers();
            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {users.Count} usuarios creados (1 Admin + 5 Clientes)");

            return users;
        }

        private static List<User> CreateUsers()
        {
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@Bosko2025");
            var clientPasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente@123");

            return new List<User>
            {
                // ADMIN
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
                // CLIENTE 1
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
                // CLIENTE 2
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
                // CLIENTE 3
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
                // CLIENTE 4
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
                // CLIENTE 5
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
        }

        #endregion

        #region CATEGORIES

        private static async Task<List<Category>> SeedCategoriesAsync(AppDbContext db)
        {
            if (await db.Categories.AnyAsync())
            {
                Console.WriteLine("[SKIP] Categorías ya existen en la base de datos");
                return await db.Categories.OrderBy(c => c.Id).ToListAsync();
            }

            Console.WriteLine("[SEED] Creando categorías...");
            var categories = CreateCategories();
            await db.Categories.AddRangeAsync(categories);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {categories.Count} categorías creadas");

            return categories;
        }

        private static List<Category> CreateCategories()
        {
            return new List<Category>
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
        }

        #endregion

        #region PRODUCTS

        private static async Task<List<Product>> SeedProductsAsync(AppDbContext db, List<Category> categories)
        {
            if (await db.Products.AnyAsync())
            {
                Console.WriteLine("[SKIP] Productos ya existen en la base de datos");
                return await db.Products.OrderBy(p => p.Id).ToListAsync();
            }

            Console.WriteLine("[SEED] Creando productos...");
            var products = CreateProducts(categories);
            await db.Products.AddRangeAsync(products);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {products.Count} productos creados (5 por categoría)");

            return products;
        }

        private static List<Product> CreateProducts(List<Category> categories)
        {
            var products = new List<Product>();

            // CATEGORÍA: HOMBRES (5 productos)
            products.AddRange(new List<Product>
            {
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
                }
            });

            // CATEGORÍA: MUJERES (5 productos)
            products.AddRange(new List<Product>
            {
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
                }
            });

            // CATEGORÍA: NIÑOS (5 productos)
            products.AddRange(new List<Product>
            {
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
                }
            });

            // CATEGORÍA: ACCESORIOS (5 productos)
            products.AddRange(new List<Product>
            {
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
                }
            });

            // CATEGORÍA: CALZADO (5 productos)
            products.AddRange(new List<Product>
            {
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
                }
            });

            // CATEGORÍA: DEPORTIVA (5 productos)
            products.AddRange(new List<Product>
            {
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
            });

            return products;
        }

        #endregion

        #region ADDRESSES

        private static async Task<List<Address>> SeedAddressesAsync(AppDbContext db, List<User> users)
        {
            if (await db.Addresses.AnyAsync())
            {
                Console.WriteLine("[SKIP] Direcciones ya existen en la base de datos");
                return await db.Addresses.OrderBy(a => a.Id).ToListAsync();
            }

            Console.WriteLine("[SEED] Creando direcciones...");
            var addresses = CreateAddresses(users);
            await db.Addresses.AddRangeAsync(addresses);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {addresses.Count} direcciones creadas (2 por cliente)");

            return addresses;
        }

        private static List<Address> CreateAddresses(List<User> users)
        {
            return new List<Address>
            {
                // Direcciones María García (users[1])
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

                // Direcciones Juan Pérez (users[2])
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

                // Direcciones Ana Rodríguez (users[3])
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

                // Direcciones Carlos López (users[4])
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

                // Direcciones Laura Martínez (users[5])
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
        }

        #endregion

        #region ORDERS

        private static async Task<List<Order>> SeedOrdersAsync(AppDbContext db, List<User> users, List<Address> addresses)
        {
            if (await db.Orders.AnyAsync())
            {
                Console.WriteLine("[SKIP] Órdenes ya existen en la base de datos");
                return await db.Orders.OrderBy(o => o.Id).ToListAsync();
            }

            Console.WriteLine("[SEED] Creando órdenes...");
            var orders = CreateOrders(users, addresses);
            await db.Orders.AddRangeAsync(orders);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {orders.Count} órdenes creadas");

            return orders;
        }

        private static List<Order> CreateOrders(List<User> users, List<Address> addresses)
        {
            var orders = new List<Order>();
            var orderStatuses = new[] { "pending", "processing", "shipped", "delivered", "cancelled" };
            var paymentMethods = new[] { "credit_card", "debit_card", "paypal" };

            // Generar 15 órdenes distribuidas entre los 5 clientes
            for (int i = 0; i < 15; i++)
            {
                var customerIndex = (i % 5) + 1; // Clientes del 1 al 5
                var customer = users[customerIndex];
                var daysAgo = _random.Next(10, 30);
                var createdAt = DateTime.UtcNow.AddDays(-daysAgo);
                var updatedDays = _random.Next(1, 4);
                var updatedAt = createdAt.AddDays(updatedDays);

                // Obtener la dirección default del cliente
                var defaultAddress = addresses.FirstOrDefault(a => a.UserId == customer.Id && a.IsDefault);

                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderNumber = $"ORD-2024-{(1000 + i):D4}",
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email,
                    ShippingAddress = defaultAddress != null 
                        ? $"{defaultAddress.Street}, {defaultAddress.City}, {defaultAddress.PostalCode}" 
                        : "Dirección no disponible",
                    Subtotal = 0, // Se calculará después con OrderItems
                    Tax = 0m,
                    Shipping = _random.Next(5, 21) * 1.0m,
                    Total = 0, // Se calculará después
                    Status = orderStatuses[_random.Next(orderStatuses.Length)],
                    PaymentMethod = paymentMethods[_random.Next(paymentMethods.Length)],
                    TrackingNumber = i % 3 == 0 ? $"TRK-2024-{(5000 + i):D4}" : null,
                    Notes = i % 4 == 0 ? "Entregar por la tarde" : null,
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt
                };

                orders.Add(order);
            }

            return orders;
        }

        #endregion

        #region ORDER ITEMS

        private static async Task SeedOrderItemsAsync(AppDbContext db, List<Order> orders, List<Product> products)
        {
            if (await db.OrderItems.AnyAsync())
            {
                Console.WriteLine("[SKIP] Items de órdenes ya existen en la base de datos");
                return;
            }

            Console.WriteLine("[SEED] Creando items de órdenes...");
            var orderItems = CreateOrderItems(orders, products);
            await db.OrderItems.AddRangeAsync(orderItems);
            await db.SaveChangesAsync();

            // Actualizar totales de órdenes
            foreach (var order in orders)
            {
                var items = orderItems.Where(oi => oi.OrderId == order.Id).ToList();
                order.Subtotal = items.Sum(oi => oi.Subtotal);
                order.Total = order.Subtotal + order.Tax + order.Shipping;
            }

            db.Orders.UpdateRange(orders);
            await db.SaveChangesAsync();

            Console.WriteLine($"[SEED] ✅ {orderItems.Count} items de órdenes creados");
            Console.WriteLine("[SEED] ✅ Totales de órdenes actualizados");
        }

        private static List<OrderItem> CreateOrderItems(List<Order> orders, List<Product> products)
        {
            var orderItems = new List<OrderItem>();

            foreach (var order in orders)
            {
                var itemCount = _random.Next(2, 5); // 2 a 4 items por orden
                var selectedProductIndices = new HashSet<int>();

                for (int i = 0; i < itemCount; i++)
                {
                    int productIndex;
                    do
                    {
                        productIndex = _random.Next(products.Count);
                    } while (selectedProductIndices.Contains(productIndex));

                    selectedProductIndices.Add(productIndex);
                    var product = products[productIndex];
                    var quantity = _random.Next(1, 4); // 1 a 3 unidades
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
                }
            }

            return orderItems;
        }

        #endregion

        #region SHIPPING ADDRESSES

        private static async Task SeedShippingAddressesAsync(AppDbContext db, List<Order> orders, List<Address> addresses)
        {
            if (await db.ShippingAddresses.AnyAsync())
            {
                Console.WriteLine("[SKIP] Direcciones de envío ya existen en la base de datos");
                return;
            }

            Console.WriteLine("[SEED] Creando direcciones de envío...");
            var shippingAddresses = CreateShippingAddresses(orders, addresses);
            await db.ShippingAddresses.AddRangeAsync(shippingAddresses);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {shippingAddresses.Count} direcciones de envío creadas");
        }

        private static List<ShippingAddress> CreateShippingAddresses(List<Order> orders, List<Address> addresses)
        {
            var shippingAddresses = new List<ShippingAddress>();
            var countries = new[] { "España", "México", "Colombia" };

            foreach (var order in orders)
            {
                // Obtener la dirección default del cliente
                var userAddress = addresses.FirstOrDefault(a => a.UserId == order.CustomerId && a.IsDefault);

                if (userAddress != null)
                {
                    var shippingAddress = new ShippingAddress
                    {
                        OrderId = order.Id,
                        FullName = order.CustomerName,
                        Phone = userAddress.Phone ?? "+34 600 000 000",
                        Street = userAddress.Street,
                        City = userAddress.City,
                        State = userAddress.State ?? "N/A",
                        PostalCode = userAddress.PostalCode,
                        Country = countries[_random.Next(countries.Length)]
                    };

                    shippingAddresses.Add(shippingAddress);
                }
            }

            return shippingAddresses;
        }

        #endregion

        #region ORDER STATUS HISTORY

        private static async Task SeedOrderStatusHistoryAsync(AppDbContext db, List<Order> orders)
        {
            if (await db.OrderStatusHistory.AnyAsync())
            {
                Console.WriteLine("[SKIP] Historial de estados ya existe en la base de datos");
                return;
            }

            Console.WriteLine("[SEED] Creando historial de estados...");
            var statusHistory = CreateStatusHistory(orders);
            await db.OrderStatusHistory.AddRangeAsync(statusHistory);
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ {statusHistory.Count} registros de historial creados");
        }

        private static List<OrderStatusHistory> CreateStatusHistory(List<Order> orders)
        {
            var statusHistory = new List<OrderStatusHistory>();

            foreach (var order in orders)
            {
                var currentStatus = order.Status;
                var baseDate = order.CreatedAt;

                // Crear historial según el estado final
                switch (currentStatus)
                {
                    case "pending":
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        break;

                    case "processing":
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "processing",
                            Note = "Pedido en preparación",
                            Timestamp = baseDate.AddHours(6)
                        });
                        break;

                    case "shipped":
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "processing",
                            Note = "Pedido en preparación",
                            Timestamp = baseDate.AddHours(6)
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "shipped",
                            Note = "Pedido enviado",
                            Timestamp = baseDate.AddDays(1)
                        });
                        break;

                    case "delivered":
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "processing",
                            Note = "Pedido en preparación",
                            Timestamp = baseDate.AddHours(6)
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "shipped",
                            Note = "Pedido enviado",
                            Timestamp = baseDate.AddDays(1)
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "delivered",
                            Note = "Pedido entregado con éxito",
                            Timestamp = baseDate.AddDays(3)
                        });
                        break;

                    case "cancelled":
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "pending",
                            Note = "Pedido recibido",
                            Timestamp = baseDate
                        });
                        statusHistory.Add(new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            Status = "cancelled",
                            Note = "Pedido cancelado por el cliente",
                            Timestamp = baseDate.AddHours(12)
                        });
                        break;
                }
            }

            return statusHistory;
        }

        #endregion

        #region DOCUMENTATION

        private static async Task CreateSeedInfoFileAsync(long elapsedMilliseconds)
        {
            try
            {
                var docsPath = Path.Combine(Directory.GetCurrentDirectory(), "Docs");
                if (!Directory.Exists(docsPath))
                {
                    Directory.CreateDirectory(docsPath);
                }

                var filePath = Path.Combine(docsPath, "DatabaseSeedInfo.txt");

                var content = $@"================================================================================
DATABASE SEEDER - INFORMACIÓN DE EJECUCIÓN
================================================================================
Proyecto: BOSKO E-Commerce Backend
Framework: .NET 8
Base de Datos: PostgreSQL (Railway)
Fecha de Ejecución: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
Tiempo de Ejecución: {elapsedMilliseconds}ms
================================================================================

ENTIDADES CREADAS:
────────────────────────────────────────────────────────────────────────────
✅ Users: 6 (1 Admin + 5 Clientes)
✅ Categories: 6
✅ Products: 30 (5 por categoría)
✅ Addresses: 10 (2 por cliente)
✅ Orders: 15 (distribuidas entre clientes)
✅ OrderItems: Variable (2-4 items por orden)
✅ ShippingAddresses: 15 (1 por orden)
✅ OrderStatusHistory: Variable (según estado de orden)

CREDENCIALES DE ACCESO:
────────────────────────────────────────────────────────────────────────────
ADMIN:
  Email: admin@bosko.com
  Password: Admin@Bosko2025
  
CLIENTES:
  Email: cliente1@bosko.com | Password: Cliente@123 | Nombre: María García
  Email: cliente2@bosko.com | Password: Cliente@123 | Nombre: Juan Pérez
  Email: cliente3@bosko.com | Password: Cliente@123 | Nombre: Ana Rodríguez
  Email: cliente4@bosko.com | Password: Cliente@123 | Nombre: Carlos López
  Email: cliente5@bosko.com | Password: Cliente@123 | Nombre: Laura Martínez

CATEGORÍAS CREADAS:
────────────────────────────────────────────────────────────────────────────
1. Hombres
2. Mujeres
3. Niños
4. Accesorios
5. Calzado
6. Deportiva

CARACTERÍSTICAS DEL SEEDER:
────────────────────────────────────────────────────────────────────────────
✅ 100% Idempotente (no crea datos duplicados)
✅ 100% Asíncrono (async/await)
✅ Orden correcto de dependencias FK
✅ Logs profesionales
✅ Compatible con PostgreSQL en Railway
✅ Production-ready

ORDEN DE EJECUCIÓN:
────────────────────────────────────────────────────────────────────────────
1. Users
2. Categories
3. Products (requiere Categories)
4. Addresses (requiere Users)
5. Orders (requiere Users + Addresses)
6. OrderItems (requiere Orders + Products)
7. ShippingAddresses (requiere Orders + Addresses)
8. OrderStatusHistory (requiere Orders)

NOTAS IMPORTANTES:
────────────────────────────────────────────────────────────────────────────
- Todas las contraseñas están hasheadas con BCrypt
- Las fechas usan DateTime.UtcNow
- Los totales de órdenes se calculan automáticamente
- Las imágenes apuntan a Unsplash
- El seeder solo se ejecuta si las tablas están vacías

================================================================================
FIN DEL REPORTE
================================================================================
";

                await File.WriteAllTextAsync(filePath, content);
                Console.WriteLine($"[SEED] 📄 Archivo de documentación creado: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SEED] ⚠️  No se pudo crear el archivo de documentación: {ex.Message}");
            }
        }

        #endregion
    }
}
