-- ============================================
-- SCRIPT SQL COMPLETO - BOSKO E-COMMERCE
-- Base de Datos de Prueba con Datos Realistas
-- ============================================

USE BoskoDB;
GO

PRINT '============================================';
PRINT 'BOSKO E-COMMERCE - CARGA DE DATOS DE PRUEBA';
PRINT '============================================';
PRINT '';

-- ============================================
-- LIMPIAR DATOS EXISTENTES (OPCIONAL)
-- ============================================
PRINT 'Limpiando datos existentes...';

DELETE FROM OrderStatusHistory;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Notifications;
DELETE FROM ActivityLogs;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM PasswordResetTokens;
DELETE FROM Users;

-- Resetear contadores de identidad
DBCC CHECKIDENT ('OrderStatusHistory', RESEED, 0);
DBCC CHECKIDENT ('OrderItems', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('Notifications', RESEED, 0);
DBCC CHECKIDENT ('ActivityLogs', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);

PRINT 'â Datos limpiados';
PRINT '';

-- ============================================
-- 1. INSERTAR USUARIOS (20 usuarios)
-- ============================================
PRINT '1. Insertando usuarios...';

-- Password para todos: Admin123! (hasheado con BCrypt)
DECLARE @passwordHash NVARCHAR(255) = '$2a$11$jQXJvHYNOLVBF3vYH6GZuOXN7yN8uPGZRJvPQGJGBQXTJvHYNOLVB';

-- Admins (2)
INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, IsActive, CreatedAt, UpdatedAt)
VALUES 
('Juan Pérez', 'admin@bosko.com', @passwordHash, 'Admin', 'Local', '+34 600 111 222', 1, DATEADD(day, -180, GETUTCDATE()), GETUTCDATE()),
('María González', 'admin2@bosko.com', @passwordHash, 'Admin', 'Local', '+34 600 222 333', 1, DATEADD(day, -170, GETUTCDATE()), GETUTCDATE());

-- Employees (3)
INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, IsActive, CreatedAt, UpdatedAt)
VALUES 
('Carlos Rodríguez', 'employee1@bosko.com', @passwordHash, 'Employee', 'Local', '+34 600 333 444', 1, DATEADD(day, -160, GETUTCDATE()), GETUTCDATE()),
('Ana Martínez', 'employee2@bosko.com', @passwordHash, 'Employee', 'Local', '+34 600 444 555', 1, DATEADD(day, -150, GETUTCDATE()), GETUTCDATE()),
('Luis Sánchez', 'employee3@bosko.com', @passwordHash, 'Employee', 'Local', '+34 600 555 666', 1, DATEADD(day, -140, GETUTCDATE()), GETUTCDATE());

-- Customers (15)
INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, IsActive, CreatedAt, UpdatedAt)
VALUES 
('Laura Fernández', 'laura.fernandez@email.com', @passwordHash, 'Customer', 'Local', '+34 666 111 111', 1, DATEADD(day, -120, GETUTCDATE()), GETUTCDATE()),
('Pedro López', 'pedro.lopez@email.com', @passwordHash, 'Customer', 'Local', '+34 666 222 222', 1, DATEADD(day, -110, GETUTCDATE()), GETUTCDATE()),
('Sofía García', 'sofia.garcia@email.com', @passwordHash, 'Customer', 'Google', '+34 666 333 333', 1, DATEADD(day, -100, GETUTCDATE()), GETUTCDATE()),
('Diego Torres', 'diego.torres@email.com', @passwordHash, 'Customer', 'Local', '+34 666 444 444', 1, DATEADD(day, -90, GETUTCDATE()), GETUTCDATE()),
('Carmen Ruiz', 'carmen.ruiz@email.com', @passwordHash, 'Customer', 'Google', '+34 666 555 555', 1, DATEADD(day, -80, GETUTCDATE()), GETUTCDATE()),
('Javier Díaz', 'javier.diaz@email.com', @passwordHash, 'Customer', 'Local', '+34 666 666 666', 1, DATEADD(day, -70, GETUTCDATE()), GETUTCDATE()),
('Elena Moreno', 'elena.moreno@email.com', @passwordHash, 'Customer', 'Local', '+34 666 777 777', 1, DATEADD(day, -60, GETUTCDATE()), GETUTCDATE()),
('Roberto Jiménez', 'roberto.jimenez@email.com', @passwordHash, 'Customer', 'Google', '+34 666 888 888', 1, DATEADD(day, -50, GETUTCDATE()), GETUTCDATE()),
('Patricia Muñoz', 'patricia.munoz@email.com', @passwordHash, 'Customer', 'Local', '+34 666 999 999', 1, DATEADD(day, -40, GETUTCDATE()), GETUTCDATE()),
('Miguel Álvarez', 'miguel.alvarez@email.com', @passwordHash, 'Customer', 'Local', '+34 677 111 111', 1, DATEADD(day, -30, GETUTCDATE()), GETUTCDATE()),
('Isabel Romero', 'isabel.romero@email.com', @passwordHash, 'Customer', 'Google', '+34 677 222 222', 1, DATEADD(day, -20, GETUTCDATE()), GETUTCDATE()),
('Francisco Navarro', 'francisco.navarro@email.com', @passwordHash, 'Customer', 'Local', '+34 677 333 333', 1, DATEADD(day, -15, GETUTCDATE()), GETUTCDATE()),
('Raquel Serrano', 'raquel.serrano@email.com', @passwordHash, 'Customer', 'Local', '+34 677 444 444', 1, DATEADD(day, -10, GETUTCDATE()), GETUTCDATE()),
('Alberto Blanco', 'alberto.blanco@email.com', @passwordHash, 'Customer', 'Google', '+34 677 555 555', 1, DATEADD(day, -5, GETUTCDATE()), GETUTCDATE()),
('Cristina Castro', 'cristina.castro@email.com', @passwordHash, 'Customer', 'Local', '+34 677 666 666', 1, DATEADD(day, -2, GETUTCDATE()), GETUTCDATE());

PRINT 'â 20 usuarios insertados (2 Admin, 3 Employee, 15 Customer)';
PRINT '';

-- ============================================
-- 2. INSERTAR CATEGORÍAS (20 categorías de ropa)
-- ============================================
PRINT '2. Insertando categorías de ropa...';

INSERT INTO Categories (Name, Description, Image, CreatedAt)
VALUES 
('Camisetas', 'Camisetas de algodón, manga corta y larga, para hombre y mujer', 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400', DATEADD(day, -200, GETUTCDATE())),
('Pantalones', 'Pantalones vaqueros, chinos, deportivos para todo tipo de ocasiones', 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=400', DATEADD(day, -200, GETUTCDATE())),
('Sudaderas', 'Sudaderas con capucha, sin capucha, deportivas y casuales', 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=400', DATEADD(day, -200, GETUTCDATE())),
('Vestidos', 'Vestidos elegantes, casuales, de fiesta para mujer', 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=400', DATEADD(day, -200, GETUTCDATE())),
('Chaquetas', 'Chaquetas de cuero, vaqueras, bomber, impermeables', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400', DATEADD(day, -200, GETUTCDATE())),
('Faldas', 'Faldas cortas, largas, midi, plisadas para mujer', 'https://images.unsplash.com/photo-1583496661160-fb5886a0aaaa?w=400', DATEADD(day, -200, GETUTCDATE())),
('Camisas', 'Camisas formales, casuales, de vestir para hombre y mujer', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400', DATEADD(day, -200, GETUTCDATE())),
('Abrigos', 'Abrigos largos, cortos, de lana, impermeables', 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=400', DATEADD(day, -200, GETUTCDATE())),
('Shorts', 'Shorts vaqueros, deportivos, bermudas para verano', 'https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=400', DATEADD(day, -200, GETUTCDATE())),
('Blusas', 'Blusas elegantes, casuales, de seda, estampadas', 'https://images.unsplash.com/photo-1618932260643-eee4a2f652a6?w=400', DATEADD(day, -200, GETUTCDATE())),
('Trajes', 'Trajes completos, blazers, americanas formales', 'https://images.unsplash.com/photo-1507679799987-c73779587ccf?w=400', DATEADD(day, -200, GETUTCDATE())),
('Jerseys', 'Jerseys de punto, cuellos altos, cardigans', 'https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=400', DATEADD(day, -200, GETUTCDATE())),
('Polos', 'Polos clásicos, deportivos, de algodón', 'https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?w=400', DATEADD(day, -200, GETUTCDATE())),
('Monos', 'Monos largos, cortos, elegantes, casuales', 'https://images.unsplash.com/photo-1594633313593-bab3825d0caf?w=400', DATEADD(day, -200, GETUTCDATE())),
('Ropa Deportiva', 'Ropa para gimnasio, running, yoga, fitness', 'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400', DATEADD(day, -200, GETUTCDATE())),
('Pijamas', 'Pijamas de algodón, seda, conjuntos de noche', 'https://images.unsplash.com/photo-1571513722275-4b41940f54b8?w=400', DATEADD(day, -200, GETUTCDATE())),
('Ropa Interior', 'Ropa interior masculina y femenina, lencería', 'https://images.unsplash.com/photo-1562157873-818bc0726f68?w=400', DATEADD(day, -200, GETUTCDATE())),
('Calcetines', 'Calcetines deportivos, formales, térmicos', 'https://images.unsplash.com/photo-1586350977771-b3b0abd50c82?w=400', DATEADD(day, -200, GETUTCDATE())),
('Accesorios', 'Bufandas, gorros, guantes, cinturones', 'https://images.unsplash.com/photo-1523359346063-d879354c0ea5?w=400', DATEADD(day, -200, GETUTCDATE())),
('Ropa de Baño', 'Bañadores, bikinis, tankinis para playa y piscina', 'https://images.unsplash.com/photo-1519046904884-53103b34b206?w=400', DATEADD(day, -200, GETUTCDATE()));

PRINT 'â 20 categorías de ropa insertadas';
PRINT '';

-- ============================================
-- 3. INSERTAR PRODUCTOS (400 productos = 20 por categoría)
-- ============================================
PRINT '3. Insertando productos (400 productos)...';
PRINT 'Esto puede tardar unos segundos...';

-- CATEGORÍA 1: Camisetas (20 productos)
INSERT INTO Products (Name, Description, Price, Stock, Image, CategoryId, CreatedAt)
VALUES 
('Camiseta Básica Blanca', 'Camiseta de algodón 100% blanca, manga corta, corte regular', 14.99, 150, 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400', 1, DATEADD(day, -190, GETUTCDATE())),
('Camiseta Negra Oversize', 'Camiseta negra de corte holgado, estilo urbano', 17.99, 120, 'https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?w=400', 1, DATEADD(day, -190, GETUTCDATE())),
('Camiseta Rayas Marinera', 'Camiseta con rayas horizontales blancas y azules', 16.50, 100, 'https://images.unsplash.com/photo-1527719327859-c6ce80353573?w=400', 1, DATEADD(day, -190, GETUTCDATE())),
('Camiseta Logo Bosko', 'Camiseta con logo bordado en el pecho', 19.99, 200, 'https://images.unsplash.com/photo-1562157873-818bc0726f68?w=400', 1, DATEADD(day, -190, GETUTCDATE())),
('Camiseta Manga Larga Gris', 'Camiseta gris de manga larga, ideal para entretiempo', 21.99, 80, 'https://images.unsplash.com/photo-1618354691373-d851c5c3a990?w=400', 1, DATEADD(day, -185, GETUTCDATE())),
('Camiseta Estampado Tropical', 'Camiseta con estampado de hojas tropicales', 18.99, 90, 'https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=400', 1, DATEADD(day, -185, GETUTCDATE())),
('Camiseta Deportiva Técnica', 'Camiseta técnica de secado rápido para deporte', 24.99, 110, 'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400', 1, DATEADD(day, -180, GETUTCDATE())),
('Camiseta Cuello Pico Azul', 'Camiseta azul con cuello de pico, estilo clásico', 15.99, 130, 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=400', 1, DATEADD(day, -180, GETUTCDATE())),
('Camiseta Vintage Lavada', 'Camiseta con efecto vintage desgastado', 22.99, 70, 'https://images.unsplash.com/photo-1554568218-0f1715e72254?w=400', 1, DATEADD(day, -175, GETUTCDATE())),
('Camiseta Tie-Dye Multicolor', 'Camiseta con estampado tie-dye único', 19.99, 60, 'https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?w=400', 1, DATEADD(day, -175, GETUTCDATE())),
('Camiseta Premium Algodón', 'Camiseta de algodón premium, suave al tacto', 26.99, 95, 'https://images.unsplash.com/photo-1581655353564-df123a1eb820?w=400', 1, DATEADD(day, -170, GETUTCDATE())),
('Camiseta Bolsillo Frontal', 'Camiseta con bolsillo frontal estampado', 17.50, 105, 'https://images.unsplash.com/photo-1586350977771-b3b0abd50c82?w=400', 1, DATEADD(day, -170, GETUTCDATE())),
('Camiseta Cuello Redondo Rosa', 'Camiseta rosa pastel de cuello redondo', 16.99, 85, 'https://images.unsplash.com/photo-1618932260643-eee4a2f652a6?w=400', 1, DATEADD(day, -165, GETUTCDATE())),
('Camiseta Crop Top Mujer', 'Camiseta crop top ajustada para mujer', 14.50, 140, 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=400', 1, DATEADD(day, -165, GETUTCDATE())),
('Camiseta Gráfico Abstracto', 'Camiseta con diseño gráfico abstracto moderno', 20.99, 75, 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400', 1, DATEADD(day, -160, GETUTCDATE())),
('Camiseta Henley Botones', 'Camiseta tipo henley con botones decorativos', 23.99, 100, 'https://images.unsplash.com/photo-1594633313593-bab3825d0caf?w=400', 1, DATEADD(day, -160, GETUTCDATE())),
('Camiseta Algodón Orgánico', 'Camiseta eco-friendly de algodón orgánico', 28.99, 50, 'https://images.unsplash.com/photo-1571513722275-4b41940f54b8?w=400', 1, DATEADD(day, -155, GETUTCDATE())),
('Camiseta Degradado Sunset', 'Camiseta con degradado tipo atardecer', 18.50, 90, 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400', 1, DATEADD(day, -155, GETUTCDATE())),
('Camiseta Lino Verano', 'Camiseta ligera de lino para verano', 29.99, 65, 'https://images.unsplash.com/photo-1583496661160-fb5886a0aaaa?w=400', 1, DATEADD(day, -150, GETUTCDATE())),
('Camiseta Tank Top Deportivo', 'Tank top sin mangas para gimnasio', 15.99, 115, 'https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=400', 1, DATEADD(day, -150, GETUTCDATE()));

-- Continúa en el siguiente archivo...
PRINT 'â Categoría 1: Camisetas (20 productos) â';
