-- ============================================
-- SCRIPT DE BASE DE DATOS - BOSKO E-COMMERCE
-- ============================================

-- Crear la base de datos (si no existe)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BoskoDB')
BEGIN
    CREATE DATABASE BoskoDB;
END
GO

USE BoskoDB;
GO

-- ============================================
-- TABLA: Categories
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        Image NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ============================================
-- TABLA: Products
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Price DECIMAL(10,2) NOT NULL,
        Stock INT NOT NULL DEFAULT 0,
        Image NVARCHAR(500) NULL,
        CategoryId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) 
            REFERENCES Categories(Id) ON DELETE SET NULL
    );
END
GO

-- ============================================
-- ÍNDICES PARA MEJOR PERFORMANCE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryId')
BEGIN
    CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Name')
BEGIN
    CREATE INDEX IX_Products_Name ON Products(Name);
END
GO

-- ============================================
-- DATOS DE PRUEBA: Categories
-- ============================================
IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (Name, Description, Image) VALUES
    ('Camisas', 'Colección de camisas elegantes para hombre y mujer', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800'),
    ('Pantalones', 'Pantalones casuales y formales de alta calidad', 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=800'),
    ('Vestidos', 'Vestidos elegantes para toda ocasión', 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=800'),
    ('Chaquetas', 'Chaquetas y abrigos para toda temporada', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800'),
    ('Accesorios', 'Complementos perfectos para tu outfit', 'https://images.unsplash.com/photo-1523779917675-b6ed3a42a561?w=800');
END
GO

-- ============================================
-- DATOS DE PRUEBA: Products
-- ============================================
IF NOT EXISTS (SELECT * FROM Products)
BEGIN
    INSERT INTO Products (Name, Description, Price, Stock, Image, CategoryId) VALUES
    -- Camisas (CategoryId = 1)
    ('Camisa Oxford Azul', 'Camisa clásica de algodón 100%, perfecta para el día a día', 49.99, 25, 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=500', 1),
    ('Camisa Lino Blanca', 'Camisa de lino premium, ideal para verano', 59.99, 30, 'https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf?w=500', 1),
    ('Camisa Casual Rayas', 'Diseño moderno con rayas verticales', 39.99, 20, 'https://images.unsplash.com/photo-1603252109303-2751441dd157?w=500', 1),
    
    -- Pantalones (CategoryId = 2)
    ('Jean Slim Fit Oscuro', 'Pantalón jean ajustado de mezclilla premium', 79.99, 40, 'https://images.unsplash.com/photo-1542272604-787c3835535d?w=500', 2),
    ('Pantalón Chino Beige', 'Estilo casual-formal versátil', 69.99, 35, 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=500', 2),
    ('Jean Regular Negro', 'Clásico jean negro de corte regular', 74.99, 28, 'https://images.unsplash.com/photo-1605518216938-7c31b7b14ad0?w=500', 2),
    
    -- Vestidos (CategoryId = 3)
    ('Vestido Floral Primavera', 'Vestido ligero con estampado floral', 89.99, 15, 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=500', 3),
    ('Vestido Elegante Negro', 'Vestido de cóctel para eventos especiales', 129.99, 12, 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=500', 3),
    ('Vestido Casual Verano', 'Perfecto para días casuales de verano', 69.99, 20, 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=500', 3),
    
    -- Chaquetas (CategoryId = 4)
    ('Chaqueta Cuero Negra', 'Chaqueta de cuero genuino estilo biker', 249.99, 8, 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=500', 4),
    ('Blazer Formal Gris', 'Blazer elegante para look profesional', 159.99, 18, 'https://images.unsplash.com/photo-1507679799987-c73779587ccf?w=500', 4),
    ('Cazadora Denim', 'Chaqueta jean clásica atemporal', 89.99, 22, 'https://images.unsplash.com/photo-1576871337622-98d48d1cf531?w=500', 4),
    
    -- Accesorios (CategoryId = 5)
    ('Cinturón Cuero Marrón', 'Cinturón de cuero genuino con hebilla plateada', 34.99, 50, 'https://images.unsplash.com/photo-1624222247344-550fb60583aa?w=500', 5),
    ('Bufanda Lana Gris', 'Bufanda suave de lana merino', 29.99, 45, 'https://images.unsplash.com/photo-1520903920243-00d872a2d1c9?w=500', 5),
    ('Gorra Baseball Negra', 'Gorra ajustable de estilo deportivo', 24.99, 60, 'https://images.unsplash.com/photo-1588850561407-ed78c282e89b?w=500', 5);
END
GO

-- ============================================
-- VERIFICAR LOS DATOS
-- ============================================
SELECT 'Categories' AS Tabla, COUNT(*) AS TotalRegistros FROM Categories
UNION ALL
SELECT 'Products', COUNT(*) FROM Products;
GO

PRINT 'Base de datos BoskoDB creada exitosamente!';
PRINT 'Categorías insertadas: ' + CAST((SELECT COUNT(*) FROM Categories) AS NVARCHAR(10));
PRINT 'Productos insertados: ' + CAST((SELECT COUNT(*) FROM Products) AS NVARCHAR(10));
GO
