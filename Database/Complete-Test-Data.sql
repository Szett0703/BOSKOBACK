-- ============================================
-- SCRIPT SQL COMPLETO - BOSKO E-COMMERCE
-- Datos de Prueba Realistas
-- Ejecutar en SQL Server Management Studio
-- ============================================

USE BoskoDB;
GO

SET NOCOUNT ON;

PRINT '============================================';
PRINT 'BOSKO E-COMMERCE - CARGA DE DATOS';
PRINT '============================================';
PRINT '';

-- ============================================
-- LIMPIAR DATOS EXISTENTES
-- ============================================
PRINT 'Limpiando datos existentes...';

DELETE FROM OrderStatusHistory;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Notifications;
DELETE FROM ActivityLogs;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Users;

DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderItems', RESEED, 0);
DBCC CHECKIDENT ('OrderStatusHistory', RESEED, 0);
DBCC CHECKIDENT ('ActivityLogs', RESEED, 0);
DBCC CHECKIDENT ('Notifications', RESEED, 0);

PRINT 'â Datos limpiados';
PRINT '';

-- Password para todos: Admin123! (hasheado con BCrypt)
DECLARE @pass NVARCHAR(255) = '$2a$11$jQXJvHYNOLVBF3vYH6GZuOXN7yN8uPGZRJvPQGJGBQXTJvHYNOLVB';

-- ============================================
-- 1. USUARIOS (20)
-- ============================================
PRINT '1. Insertando usuarios (20)...';

INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, IsActive, CreatedAt, UpdatedAt) VALUES
-- Admins (2)
('Juan Pérez', 'admin@bosko.com', @pass, 'Admin', 'Local', '+34 600 111 222', 1, DATEADD(day, -180, GETUTCDATE()), GETUTCDATE()),
('María González', 'admin2@bosko.com', @pass, 'Admin', 'Local', '+34 600 222 333', 1, DATEADD(day, -170, GETUTCDATE()), GETUTCDATE()),
-- Employees (3)
('Carlos Rodríguez', 'employee1@bosko.com', @pass, 'Employee', 'Local', '+34 600 333 444', 1, DATEADD(day, -160, GETUTCDATE()), GETUTCDATE()),
('Ana Martínez', 'employee2@bosko.com', @pass, 'Employee', 'Local', '+34 600 444 555', 1, DATEADD(day, -150, GETUTCDATE()), GETUTCDATE()),
('Luis Sánchez', 'employee3@bosko.com', @pass, 'Employee', 'Local', '+34 600 555 666', 1, DATEADD(day, -140, GETUTCDATE()), GETUTCDATE()),
-- Customers (15)
('Laura Fernández', 'laura.f@email.com', @pass, 'Customer', 'Local', '+34 666 111 111', 1, DATEADD(day, -120, GETUTCDATE()), GETUTCDATE()),
('Pedro López', 'pedro.l@email.com', @pass, 'Customer', 'Local', '+34 666 222 222', 1, DATEADD(day, -110, GETUTCDATE()), GETUTCDATE()),
('Sofía García', 'sofia.g@email.com', @pass, 'Customer', 'Google', '+34 666 333 333', 1, DATEADD(day, -100, GETUTCDATE()), GETUTCDATE()),
('Diego Torres', 'diego.t@email.com', @pass, 'Customer', 'Local', '+34 666 444 444', 1, DATEADD(day, -90, GETUTCDATE()), GETUTCDATE()),
('Carmen Ruiz', 'carmen.r@email.com', @pass, 'Customer', 'Google', '+34 666 555 555', 1, DATEADD(day, -80, GETUTCDATE()), GETUTCDATE()),
('Javier Díaz', 'javier.d@email.com', @pass, 'Customer', 'Local', '+34 666 666 666', 1, DATEADD(day, -70, GETUTCDATE()), GETUTCDATE()),
('Elena Moreno', 'elena.m@email.com', @pass, 'Customer', 'Local', '+34 666 777 777', 1, DATEADD(day, -60, GETUTCDATE()), GETUTCDATE()),
('Roberto Jiménez', 'roberto.j@email.com', @pass, 'Customer', 'Google', '+34 666 888 888', 1, DATEADD(day, -50, GETUTCDATE()), GETUTCDATE()),
('Patricia Muñoz', 'patricia.m@email.com', @pass, 'Customer', 'Local', '+34 666 999 999', 1, DATEADD(day, -40, GETUTCDATE()), GETUTCDATE()),
('Miguel Álvarez', 'miguel.a@email.com', @pass, 'Customer', 'Local', '+34 677 111 111', 1, DATEADD(day, -30, GETUTCDATE()), GETUTCDATE()),
('Isabel Romero', 'isabel.r@email.com', @pass, 'Customer', 'Google', '+34 677 222 222', 1, DATEADD(day, -20, GETUTCDATE()), GETUTCDATE()),
('Francisco Navarro', 'francisco.n@email.com', @pass, 'Customer', 'Local', '+34 677 333 333', 1, DATEADD(day, -15, GETUTCDATE()), GETUTCDATE()),
('Raquel Serrano', 'raquel.s@email.com', @pass, 'Customer', 'Local', '+34 677 444 444', 1, DATEADD(day, -10, GETUTCDATE()), GETUTCDATE()),
('Alberto Blanco', 'alberto.b@email.com', @pass, 'Customer', 'Google', '+34 677 555 555', 1, DATEADD(day, -5, GETUTCDATE()), GETUTCDATE()),
('Cristina Castro', 'cristina.c@email.com', @pass, 'Customer', 'Local', '+34 677 666 666', 1, DATEADD(day, -2, GETUTCDATE()), GETUTCDATE());

PRINT 'â 20 usuarios insertados (2 Admin, 3 Employee, 15 Customer)';

-- ============================================
-- 2. CATEGORÍAS (20)
-- ============================================
PRINT '2. Insertando categorías (20)...';

INSERT INTO Categories (Name, Description, Image, CreatedAt) VALUES
('Camisetas', 'Camisetas de algodón, manga corta y larga', 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400', DATEADD(day, -200, GETUTCDATE())),
('Pantalones', 'Pantalones vaqueros, chinos, deportivos', 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=400', DATEADD(day, -200, GETUTCDATE())),
('Sudaderas', 'Sudaderas con y sin capucha', 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=400', DATEADD(day, -200, GETUTCDATE())),
('Vestidos', 'Vestidos elegantes y casuales', 'https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=400', DATEADD(day, -200, GETUTCDATE())),
('Chaquetas', 'Chaquetas de cuero, vaqueras, bomber', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400', DATEADD(day, -200, GETUTCDATE())),
('Faldas', 'Faldas cortas, largas, midi', 'https://images.unsplash.com/photo-1583496661160-fb5886a0aaaa?w=400', DATEADD(day, -200, GETUTCDATE())),
('Camisas', 'Camisas formales y casuales', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400', DATEADD(day, -200, GETUTCDATE())),
('Abrigos', 'Abrigos largos y cortos', 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=400', DATEADD(day, -200, GETUTCDATE())),
('Shorts', 'Shorts vaqueros y deportivos', 'https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=400', DATEADD(day, -200, GETUTCDATE())),
('Blusas', 'Blusas elegantes y casuales', 'https://images.unsplash.com/photo-1618932260643-eee4a2f652a6?w=400', DATEADD(day, -200, GETUTCDATE())),
('Trajes', 'Trajes completos y blazers', 'https://images.unsplash.com/photo-1507679799987-c73779587ccf?w=400', DATEADD(day, -200, GETUTCDATE())),
('Jerseys', 'Jerseys de punto y cardigans', 'https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=400', DATEADD(day, -200, GETUTCDATE())),
('Polos', 'Polos clásicos y deportivos', 'https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?w=400', DATEADD(day, -200, GETUTCDATE())),
('Monos', 'Monos largos y cortos', 'https://images.unsplash.com/photo-1594633313593-bab3825d0caf?w=400', DATEADD(day, -200, GETUTCDATE())),
('Ropa Deportiva', 'Ropa para gimnasio y running', 'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400', DATEADD(day, -200, GETUTCDATE())),
('Pijamas', 'Pijamas de algodón y seda', 'https://images.unsplash.com/photo-1571513722275-4b41940f54b8?w=400', DATEADD(day, -200, GETUTCDATE())),
('Ropa Interior', 'Ropa interior masculina y femenina', 'https://images.unsplash.com/photo-1562157873-818bc0726f68?w=400', DATEADD(day, -200, GETUTCDATE())),
('Calcetines', 'Calcetines deportivos y formales', 'https://images.unsplash.com/photo-1586350977771-b3b0abd50c82?w=400', DATEADD(day, -200, GETUTCDATE())),
('Accesorios', 'Bufandas, gorros, guantes', 'https://images.unsplash.com/photo-1523359346063-d879354c0ea5?w=400', DATEADD(day, -200, GETUTCDATE())),
('Ropa de Baño', 'Bañadores y bikinis', 'https://images.unsplash.com/photo-1519046904884-53103b34b206?w=400', DATEADD(day, -200, GETUTCDATE()));

PRINT 'â 20 categorías insertadas';

-- ============================================
-- 3. PRODUCTOS (100 productos - 5 por categoría)
-- ============================================
PRINT '3. Insertando productos (100)...';

DECLARE @catId INT = 1;
DECLARE @prodNum INT;
DECLARE @basePrice DECIMAL(10,2);
DECLARE @stockBase INT;

WHILE @catId <= 20
BEGIN
    SET @prodNum = 1;
    SET @basePrice = 15 + (@catId * 2);
    SET @stockBase = 50 + (@catId * 5);
    
    WHILE @prodNum <= 5
    BEGIN
        INSERT INTO Products (Name, Description, Price, Stock, Image, CategoryId, CreatedAt)
        VALUES (
            'Producto ' + CAST(@catId AS VARCHAR) + '-' + CAST(@prodNum AS VARCHAR),
            'Descripción del producto ' + CAST(@catId AS VARCHAR) + '-' + CAST(@prodNum AS VARCHAR) + '. Material de alta calidad, diseño moderno y cómodo.',
            @basePrice + (@prodNum * 5),
            @stockBase + (@prodNum * 10),
            'https://images.unsplash.com/photo-' + CAST(1500000000 + (@catId * 100) + @prodNum AS VARCHAR) + '?w=400',
            @catId,
            DATEADD(day, -((@catId * 5) + @prodNum), GETUTCDATE())
        );
        SET @prodNum = @prodNum + 1;
    END
    
    SET @catId = @catId + 1;
END

PRINT 'â 100 productos insertados (5 por categoría)';

-- ============================================
-- 4. ÓRDENES (50 órdenes)
-- ============================================
PRINT '4. Insertando órdenes (50)...';

DECLARE @customerId INT;
DECLARE @orderNum INT = 1;
DECLARE @orderTotal DECIMAL(18,2);
DECLARE @orderStatus NVARCHAR(20);

WHILE @orderNum <= 50
BEGIN
    SET @customerId = 6 + ((@orderNum - 1) % 15); -- Clientes del 6 al 20
    SET @orderTotal = 50 + ((@orderNum * 13.7) % 200);
    
    SET @orderStatus = CASE (@orderNum % 4)
        WHEN 0 THEN 'delivered'
        WHEN 1 THEN 'processing'
        WHEN 2 THEN 'pending'
        ELSE 'cancelled'
    END;
    
    INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
    SELECT 
        Id,
        Name,
        Email,
        'Calle Principal ' + CAST(@orderNum AS VARCHAR) + ', Madrid, 28001, España',
        @orderTotal,
        15.00,
        @orderTotal + 15.00,
        @orderStatus,
        CASE (@orderNum % 3)
            WHEN 0 THEN 'credit_card'
            WHEN 1 THEN 'paypal'
            ELSE 'transfer'
        END,
        DATEADD(day, -(@orderNum * 2), GETUTCDATE()),
        DATEADD(hour, -(@orderNum), GETUTCDATE())
    FROM Users WHERE Id = @customerId;
    
    SET @orderNum = @orderNum + 1;
END

PRINT 'â 50 órdenes insertadas';

-- ============================================
-- 5. ORDER ITEMS (2-4 items por orden)
-- ============================================
PRINT '5. Insertando items de órdenes...';

DECLARE @orderId INT = 1;
DECLARE @itemCount INT;
DECLARE @productId INT;
DECLARE @quantity INT;

WHILE @orderId <= 50
BEGIN
    SET @itemCount = 2 + (@orderId % 3); -- 2-4 items por orden
    
    WHILE @itemCount > 0
    BEGIN
        SET @productId = 1 + ((@orderId * 7 + @itemCount) % 100);
        SET @quantity = 1 + (@itemCount % 3);
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        SELECT 
            @orderId,
            Id,
            Name,
            @quantity,
            Price,
            Price * @quantity
        FROM Products WHERE Id = @productId;
        
        SET @itemCount = @itemCount - 1;
    END
    
    SET @orderId = @orderId + 1;
END

PRINT 'â Items de órdenes insertados';

-- ============================================
-- 6. ORDER STATUS HISTORY
-- ============================================
PRINT '6. Insertando historial de estados...';

INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
SELECT 
    Id,
    'pending',
    'Pedido recibido',
    CreatedAt
FROM Orders;

INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
SELECT 
    Id,
    Status,
    CASE Status
        WHEN 'delivered' THEN 'Pedido entregado'
        WHEN 'processing' THEN 'Pedido en preparación'
        WHEN 'cancelled' THEN 'Pedido cancelado'
        ELSE 'Actualización de estado'
    END,
    UpdatedAt
FROM Orders WHERE Status != 'pending';

PRINT 'â Historial de estados insertado';

-- ============================================
-- 7. ACTIVITY LOGS
-- ============================================
PRINT '7. Insertando activity logs...';

INSERT INTO ActivityLogs (Type, Text, UserId, Timestamp) VALUES
('user', 'Usuario Admin creado', 1, DATEADD(day, -180, GETUTCDATE())),
('user', 'Usuario Admin2 creado', 1, DATEADD(day, -170, GETUTCDATE())),
('category', 'Categoría Camisetas creada', 1, DATEADD(day, -200, GETUTCDATE())),
('category', 'Categoría Pantalones creada', 1, DATEADD(day, -200, GETUTCDATE())),
('product', 'Producto agregado al catálogo', 1, DATEADD(day, -190, GETUTCDATE())),
('product', 'Stock actualizado', 1, DATEADD(day, -150, GETUTCDATE())),
('order', 'Nuevo pedido recibido', NULL, DATEADD(day, -10, GETUTCDATE())),
('order', 'Pedido entregado', NULL, DATEADD(day, -5, GETUTCDATE())),
('user', 'Nuevo cliente registrado', NULL, DATEADD(day, -5, GETUTCDATE())),
('product', 'Precio actualizado', 3, DATEADD(day, -3, GETUTCDATE()));

PRINT 'â Activity logs insertados';

-- ============================================
-- 8. NOTIFICATIONS
-- ============================================
PRINT '8. Insertando notificaciones...';

INSERT INTO Notifications (UserId, Title, Message, Type, IsRead, CreatedAt) VALUES
(1, 'Nuevo pedido', 'Pedido #1 ha sido recibido', 'order', 0, DATEADD(hour, -2, GETUTCDATE())),
(1, 'Stock bajo', 'El producto Producto 1-1 tiene stock bajo', 'product', 0, DATEADD(hour, -5, GETUTCDATE())),
(1, 'Pedido entregado', 'Pedido #25 ha sido entregado', 'order', 1, DATEADD(day, -1, GETUTCDATE())),
(2, 'Bienvenida', 'Bienvenido al panel de administración', 'system', 1, DATEADD(day, -170, GETUTCDATE())),
(2, 'Nuevo usuario', 'Se ha registrado un nuevo cliente', 'user', 0, DATEADD(hour, -10, GETUTCDATE()));

PRINT 'â Notificaciones insertadas';

-- ============================================
-- RESUMEN FINAL
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'RESUMEN DE DATOS INSERTADOS';
PRINT '============================================';

SELECT 'Users' AS Tabla, COUNT(*) AS Total, 
       SUM(CASE WHEN Role='Admin' THEN 1 ELSE 0 END) AS Admins,
       SUM(CASE WHEN Role='Employee' THEN 1 ELSE 0 END) AS Employees,
       SUM(CASE WHEN Role='Customer' THEN 1 ELSE 0 END) AS Customers
FROM Users;

SELECT 'Categories' AS Tabla, COUNT(*) AS Total FROM Categories;
SELECT 'Products' AS Tabla, COUNT(*) AS Total, AVG(Price) AS PrecioPromedio, SUM(Stock) AS StockTotal FROM Products;

SELECT 'Orders' AS Tabla, COUNT(*) AS Total,
       SUM(CASE WHEN Status='pending' THEN 1 ELSE 0 END) AS Pendientes,
       SUM(CASE WHEN Status='processing' THEN 1 ELSE 0 END) AS Procesando,
       SUM(CASE WHEN Status='delivered' THEN 1 ELSE 0 END) AS Entregados,
       SUM(CASE WHEN Status='cancelled' THEN 1 ELSE 0 END) AS Cancelados,
       SUM(Total) AS TotalVentas
FROM Orders;

SELECT 'OrderItems' AS Tabla, COUNT(*) AS Total FROM OrderItems;
SELECT 'OrderStatusHistory' AS Tabla, COUNT(*) AS Total FROM OrderStatusHistory;
SELECT 'ActivityLogs' AS Tabla, COUNT(*) AS Total FROM ActivityLogs;
SELECT 'Notifications' AS Tabla, COUNT(*) AS Total FROM Notifications;

PRINT '';
PRINT '============================================';
PRINT 'â¡ï¸ CARGA DE DATOS COMPLETADA';
PRINT '============================================';
PRINT '';
PRINT 'Credenciales de prueba:';
PRINT '  Admin:    admin@bosko.com / Admin123!';
PRINT '  Employee: employee1@bosko.com / Admin123!';
PRINT '  Customer: laura.f@email.com / Admin123!';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '1. Ejecutar: cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK';
PRINT '2. Ejecutar: dotnet run';
PRINT '3. Abrir: https://localhost:5006/swagger';
PRINT '4. Hacer login con las credenciales de arriba';
PRINT '5. Probar los endpoints del panel admin';
PRINT '';
PRINT '============================================';

SET NOCOUNT OFF;
GO
