-- ============================================
-- SCRIPT ADMIN PANEL - BOSKO E-COMMERCE
-- Agregar tablas para gestión de pedidos y admin
-- VERSIÓN: 1.0
-- FECHA: 16 de Noviembre 2025
-- ============================================

USE BoskoDB;
GO

PRINT '';
PRINT '============================================';
PRINT 'AGREGANDO TABLAS DEL ADMIN PANEL';
PRINT '============================================';
PRINT '';

-- ============================================
-- TABLA: Orders
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerId INT NOT NULL,
        CustomerName NVARCHAR(100) NOT NULL,
        CustomerEmail NVARCHAR(255) NOT NULL,
        ShippingAddress NVARCHAR(500) NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL,
        Shipping DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'pending',
        PaymentMethod NVARCHAR(50) NOT NULL DEFAULT 'credit_card',
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Orders_CustomerId FOREIGN KEY (CustomerId) 
            REFERENCES Users(Id) ON DELETE NO ACTION,
        CONSTRAINT CK_Orders_Status CHECK (Status IN ('pending', 'processing', 'delivered', 'cancelled'))
    );
    
    -- Índices para Orders
    CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
    CREATE INDEX IX_Orders_Status ON Orders(Status);
    CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);
    
    PRINT '✅ Tabla Orders creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla Orders ya existe';
END
GO

-- ============================================
-- TABLA: OrderItems
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Quantity INT NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL,
        
        CONSTRAINT FK_OrderItems_OrderId FOREIGN KEY (OrderId) 
            REFERENCES Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_ProductId FOREIGN KEY (ProductId) 
            REFERENCES Products(Id) ON DELETE NO ACTION
    );
    
    -- Índices para OrderItems
    CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
    CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
    
    PRINT '✅ Tabla OrderItems creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla OrderItems ya existe';
END
GO

-- ============================================
-- TABLA: OrderStatusHistory
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderStatusHistory')
BEGIN
    CREATE TABLE OrderStatusHistory (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        Status NVARCHAR(20) NOT NULL,
        Note NVARCHAR(500) NULL,
        Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_OrderStatusHistory_OrderId FOREIGN KEY (OrderId) 
            REFERENCES Orders(Id) ON DELETE CASCADE
    );
    
    -- Índice para OrderStatusHistory
    CREATE INDEX IX_OrderStatusHistory_OrderId ON OrderStatusHistory(OrderId);
    
    PRINT '✅ Tabla OrderStatusHistory creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla OrderStatusHistory ya existe';
END
GO

-- ============================================
-- TABLA: ActivityLogs
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ActivityLogs')
BEGIN
    CREATE TABLE ActivityLogs (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Type NVARCHAR(50) NOT NULL,
        Text NVARCHAR(500) NOT NULL,
        UserId INT NULL,
        Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_ActivityLogs_UserId FOREIGN KEY (UserId) 
            REFERENCES Users(Id) ON DELETE SET NULL,
        CONSTRAINT CK_ActivityLogs_Type CHECK (Type IN ('order', 'product', 'user', 'category'))
    );
    
    -- Índices para ActivityLogs
    CREATE INDEX IX_ActivityLogs_Timestamp ON ActivityLogs(Timestamp);
    CREATE INDEX IX_ActivityLogs_Type ON ActivityLogs(Type);
    
    PRINT '✅ Tabla ActivityLogs creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla ActivityLogs ya existe';
END
GO

-- ============================================
-- TABLA: Notifications
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    CREATE TABLE Notifications (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Message NVARCHAR(500) NOT NULL,
        Type NVARCHAR(50) NOT NULL,
        IsRead BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Notifications_UserId FOREIGN KEY (UserId) 
            REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT CK_Notifications_Type CHECK (Type IN ('order', 'product', 'user', 'system'))
    );
    
    -- Índices para Notifications
    CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
    CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);
    
    PRINT '✅ Tabla Notifications creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla Notifications ya existe';
END
GO

-- ============================================
-- DATOS DE PRUEBA
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'INSERTANDO DATOS DE PRUEBA';
PRINT '============================================';
PRINT '';

-- Insertar pedidos de ejemplo solo si no existen
IF NOT EXISTS (SELECT * FROM Orders)
BEGIN
    -- Obtener IDs de usuarios existentes
    DECLARE @AdminId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Admin');
    DECLARE @CustomerId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Customer');
    
    IF @CustomerId IS NOT NULL
    BEGIN
        -- Pedido 1
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt)
        VALUES (@CustomerId, 'Cliente Test', 'customer@bosko.com', 'Calle Principal 123, Madrid, 28001', 1200.00, 50.00, 1250.00, 'delivered', 'credit_card', DATEADD(day, -5, GETUTCDATE()));
        
        DECLARE @Order1Id INT = SCOPE_IDENTITY();
        
        -- Items del Pedido 1
        DECLARE @ProductId INT = (SELECT TOP 1 Id FROM Products);
        IF @ProductId IS NOT NULL
        BEGIN
            INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
            VALUES (@Order1Id, @ProductId, 'Camisa Casual Bosko', 2, 50.00, 100.00);
            
            -- Historial del Pedido 1
            INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
            VALUES 
                (@Order1Id, 'pending', 'Pedido creado', DATEADD(day, -5, GETUTCDATE())),
                (@Order1Id, 'processing', 'Pedido en preparación', DATEADD(day, -4, GETUTCDATE())),
                (@Order1Id, 'delivered', 'Pedido entregado', DATEADD(day, -1, GETUTCDATE()));
        END
        
        -- Pedido 2
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt)
        VALUES (@CustomerId, 'Cliente Test', 'customer@bosko.com', 'Calle Principal 123, Madrid, 28001', 840.00, 50.50, 890.50, 'processing', 'paypal', DATEADD(day, -2, GETUTCDATE()));
        
        DECLARE @Order2Id INT = SCOPE_IDENTITY();
        
        -- Historial del Pedido 2
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order2Id, 'pending', 'Pedido creado', DATEADD(day, -2, GETUTCDATE())),
            (@Order2Id, 'processing', 'Pedido en preparación', DATEADD(day, -1, GETUTCDATE()));
        
        -- Pedido 3
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt)
        VALUES (@CustomerId, 'Cliente Test', 'customer@bosko.com', 'Calle Principal 123, Madrid, 28001', 2050.00, 50.00, 2100.00, 'pending', 'credit_card', GETUTCDATE());
        
        DECLARE @Order3Id INT = SCOPE_IDENTITY();
        
        -- Historial del Pedido 3
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES (@Order3Id, 'pending', 'Pedido creado', GETUTCDATE());
        
        PRINT '✅ 3 pedidos de prueba insertados';
    END
    ELSE
    BEGIN
        PRINT '⚠️ No se encontró un usuario Customer para crear pedidos de prueba';
    END
END
ELSE
BEGIN
    PRINT '⚠️ Ya existen pedidos en la base de datos';
END
GO

-- Insertar actividad de ejemplo
IF NOT EXISTS (SELECT * FROM ActivityLogs)
BEGIN
    INSERT INTO ActivityLogs (Type, Text, Timestamp)
    VALUES 
        ('order', 'Nuevo pedido #1 creado', DATEADD(hour, -2, GETUTCDATE())),
        ('product', 'Producto actualizado: Camisa Casual', DATEADD(hour, -4, GETUTCDATE())),
        ('user', 'Nuevo cliente registrado', DATEADD(hour, -6, GETUTCDATE())),
        ('order', 'Pedido #2 marcado como procesando', DATEADD(hour, -8, GETUTCDATE())),
        ('product', 'Nuevo producto agregado', DATEADD(hour, -10, GETUTCDATE()));
    
    PRINT '✅ 5 actividades de prueba insertadas';
END
ELSE
BEGIN
    PRINT '⚠️ Ya existen actividades en la base de datos';
END
GO

-- Insertar notificación de ejemplo para Admin
DECLARE @AdminIdNotif INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Admin');
IF @AdminIdNotif IS NOT NULL AND NOT EXISTS (SELECT * FROM Notifications)
BEGIN
    INSERT INTO Notifications (UserId, Title, Message, Type, IsRead, CreatedAt)
    VALUES 
        (@AdminIdNotif, 'Nuevo pedido recibido', 'Pedido #1 de Cliente Test', 'order', 0, DATEADD(hour, -1, GETUTCDATE())),
        (@AdminIdNotif, 'Stock bajo', 'El producto X tiene stock bajo', 'product', 0, DATEADD(hour, -3, GETUTCDATE())),
        (@AdminIdNotif, 'Nuevo cliente', 'Se registró un nuevo cliente', 'user', 1, DATEADD(hour, -5, GETUTCDATE()));
    
    PRINT '✅ 3 notificaciones de prueba insertadas para Admin';
END
GO

-- ============================================
-- VERIFICACIÓN FINAL
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN DE TABLAS';
PRINT '============================================';
PRINT '';

SELECT 
    'Orders' AS Tabla,
    COUNT(*) AS Total,
    COUNT(CASE WHEN Status = 'pending' THEN 1 END) AS Pendientes,
    COUNT(CASE WHEN Status = 'processing' THEN 1 END) AS Procesando,
    COUNT(CASE WHEN Status = 'delivered' THEN 1 END) AS Entregados,
    COUNT(CASE WHEN Status = 'cancelled' THEN 1 END) AS Cancelados
FROM Orders;

SELECT 
    'OrderItems' AS Tabla,
    COUNT(*) AS Total
FROM OrderItems;

SELECT 
    'OrderStatusHistory' AS Tabla,
    COUNT(*) AS Total
FROM OrderStatusHistory;

SELECT 
    'ActivityLogs' AS Tabla,
    COUNT(*) AS Total
FROM ActivityLogs;

SELECT 
    'Notifications' AS Tabla,
    COUNT(*) AS Total,
    COUNT(CASE WHEN IsRead = 0 THEN 1 END) AS NoLeidas
FROM Notifications;

PRINT '';
PRINT '============================================';
PRINT '✅ ADMIN PANEL - INSTALACIÓN COMPLETADA';
PRINT '============================================';
PRINT '';
PRINT 'Tablas creadas:';
PRINT '  ✅ Orders';
PRINT '  ✅ OrderItems';
PRINT '  ✅ OrderStatusHistory';
PRINT '  ✅ ActivityLogs';
PRINT '  ✅ Notifications';
PRINT '';
PRINT 'Datos de prueba insertados:';
PRINT '  ✅ 3 pedidos de ejemplo';
PRINT '  ✅ 5 actividades de ejemplo';
PRINT '  ✅ 3 notificaciones de ejemplo';
PRINT '';
PRINT '============================================';
PRINT 'PRÓXIMOS PASOS:';
PRINT '1. Ejecuta: dotnet ef migrations add AddAdminPanelTables';
PRINT '2. Ejecuta: dotnet run';
PRINT '3. Abre: https://localhost:5006/swagger';
PRINT '4. Prueba: GET /api/admin/dashboard/stats';
PRINT '============================================';
GO
