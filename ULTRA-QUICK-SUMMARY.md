-- ============================================
-- SCRIPT DE CORRECCIÓN COMPLETA DE BASE DE DATOS
-- Bosko E-Commerce - Actualización de Esquema
-- ============================================

USE [BoskoDB];
GO

PRINT '============================================';
PRINT 'CORRECCIÓN DE ESQUEMA DE BASE DE DATOS';
PRINT '============================================';
PRINT '';

-- ============================================
-- PASO 1: GUARDAR DATOS EXISTENTES (SI LOS HAY)
-- ============================================
PRINT 'PASO 1: Respaldando datos existentes...';

-- Crear tablas temporales para backup
IF OBJECT_ID('tempdb..#OrdersBackup') IS NOT NULL DROP TABLE #OrdersBackup;
IF OBJECT_ID('tempdb..#OrderItemsBackup') IS NOT NULL DROP TABLE #OrderItemsBackup;

-- Backup Orders (si tiene datos)
IF EXISTS (SELECT * FROM Orders)
BEGIN
    SELECT 
        Id,
        UserId,
        TotalAmount,
        Status,
        CreatedAt,
        UpdatedAt
    INTO #OrdersBackup
    FROM Orders;
    
    DECLARE @OrdersCount INT = @@ROWCOUNT;
    PRINT 'â Orders respaldados: ' + CAST(@OrdersCount AS VARCHAR) + ' registros';
END
ELSE
BEGIN
    PRINT 'â ï¸ Tabla Orders está vacía';
END

-- Backup OrderItems (si tiene datos)
IF EXISTS (SELECT * FROM OrderItems)
BEGIN
    SELECT 
        Id,
        OrderId,
        ProductId,
        Quantity,
        UnitPrice
    INTO #OrderItemsBackup
    FROM OrderItems;
    
    DECLARE @ItemsCount INT = @@ROWCOUNT;
    PRINT 'â OrderItems respaldados: ' + CAST(@ItemsCount AS VARCHAR) + ' registros';
END
ELSE
BEGIN
    PRINT 'â ï¸ Tabla OrderItems está vacía';
END

PRINT '';

-- ============================================
-- PASO 2: ELIMINAR TABLAS INCORRECTAS
-- ============================================
PRINT 'PASO 2: Eliminando tablas con estructura incorrecta...';

-- Eliminar FKs primero
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Orders')
BEGIN
    ALTER TABLE OrderItems DROP CONSTRAINT FK_OrderItems_Orders;
    PRINT 'â FK_OrderItems_Orders eliminada';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Products')
BEGIN
    ALTER TABLE OrderItems DROP CONSTRAINT FK_OrderItems_Products;
    PRINT 'â FK_OrderItems_Products eliminada';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Users')
BEGIN
    ALTER TABLE Orders DROP CONSTRAINT FK_Orders_Users;
    PRINT 'â FK_Orders_Users eliminada';
END

-- Eliminar tablas
IF OBJECT_ID('OrderItems', 'U') IS NOT NULL
BEGIN
    DROP TABLE OrderItems;
    PRINT 'â Tabla OrderItems eliminada';
END

IF OBJECT_ID('Orders', 'U') IS NOT NULL
BEGIN
    DROP TABLE Orders;
    PRINT 'â Tabla Orders eliminada';
END

PRINT '';

-- ============================================
-- PASO 3: CREAR TABLAS CORRECTAS
-- ============================================
PRINT 'PASO 3: Creando tablas con estructura correcta...';

-- ============================================
-- Tabla: Orders (NUEVA ESTRUCTURA)
-- ============================================
CREATE TABLE [dbo].[Orders] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [CustomerId] INT NOT NULL,
    [CustomerName] NVARCHAR(100) NOT NULL,
    [CustomerEmail] NVARCHAR(255) NOT NULL,
    [ShippingAddress] NVARCHAR(500) NOT NULL,
    [Subtotal] DECIMAL(18,2) NOT NULL,
    [Shipping] DECIMAL(18,2) NOT NULL,
    [Total] DECIMAL(18,2) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'pending',
    [PaymentMethod] NVARCHAR(50) NOT NULL DEFAULT 'credit_card',
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Orders_CustomerId FOREIGN KEY (CustomerId) 
        REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('pending', 'processing', 'delivered', 'cancelled'))
);

CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);

PRINT 'â Tabla Orders creada con estructura correcta';

-- ============================================
-- Tabla: OrderItems (NUEVA ESTRUCTURA)
-- ============================================
CREATE TABLE [dbo].[OrderItems] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [OrderId] INT NOT NULL,
    [ProductId] INT NOT NULL,
    [ProductName] NVARCHAR(200) NOT NULL,
    [Quantity] INT NOT NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [Subtotal] DECIMAL(18,2) NOT NULL,
    
    CONSTRAINT FK_OrderItems_OrderId FOREIGN KEY (OrderId) 
        REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_ProductId FOREIGN KEY (ProductId) 
        REFERENCES Products(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);

PRINT 'â Tabla OrderItems creada con estructura correcta';

-- ============================================
-- Tabla: OrderStatusHistory (NUEVA)
-- ============================================
IF OBJECT_ID('OrderStatusHistory', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderStatusHistory] (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [OrderId] INT NOT NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [Note] NVARCHAR(500) NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_OrderStatusHistory_OrderId FOREIGN KEY (OrderId) 
            REFERENCES Orders(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_OrderStatusHistory_OrderId ON OrderStatusHistory(OrderId);
    
    PRINT 'â Tabla OrderStatusHistory creada';
END
ELSE
BEGIN
    PRINT 'â ï¸ Tabla OrderStatusHistory ya existe';
END

-- ============================================
-- Tabla: ActivityLogs (NUEVA)
-- ============================================
IF OBJECT_ID('ActivityLogs', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ActivityLogs] (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [Type] NVARCHAR(50) NOT NULL,
        [Text] NVARCHAR(500) NOT NULL,
        [UserId] INT NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_ActivityLogs_UserId FOREIGN KEY (UserId) 
            REFERENCES Users(Id) ON DELETE SET NULL,
        CONSTRAINT CK_ActivityLogs_Type CHECK (Type IN ('order', 'product', 'user', 'category'))
    );
    
    CREATE INDEX IX_ActivityLogs_Timestamp ON ActivityLogs(Timestamp);
    CREATE INDEX IX_ActivityLogs_Type ON ActivityLogs(Type);
    
    PRINT 'â Tabla ActivityLogs creada';
END
ELSE
BEGIN
    PRINT 'â ï¸ Tabla ActivityLogs ya existe';
END

-- ============================================
-- Tabla: Notifications (NUEVA)
-- ============================================
IF OBJECT_ID('Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Notifications] (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Message] NVARCHAR(500) NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Notifications_UserId FOREIGN KEY (UserId) 
            REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT CK_Notifications_Type CHECK (Type IN ('order', 'product', 'user', 'system'))
    );
    
    CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
    CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);
    
    PRINT 'â Tabla Notifications creada';
END
ELSE
BEGIN
    PRINT 'â ï¸ Tabla Notifications ya existe';
END

PRINT '';

-- ============================================
-- PASO 4: MIGRAR DATOS DEL BACKUP (SI LOS HAY)
-- ============================================
PRINT 'PASO 4: Intentando migrar datos existentes...';

-- Migrar Orders (si hay backup)
IF OBJECT_ID('tempdb..#OrdersBackup') IS NOT NULL
BEGIN
    -- Intentar migrar con valores predeterminados para campos nuevos
    INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
    SELECT 
        ob.UserId AS CustomerId,
        u.Name AS CustomerName,
        u.Email AS CustomerEmail,
        'Dirección pendiente de actualizar' AS ShippingAddress,
        ob.TotalAmount AS Subtotal,
        0 AS Shipping,
        ob.TotalAmount AS Total,
        CASE 
            WHEN ob.Status = 'Pending' THEN 'pending'
            WHEN ob.Status = 'Processing' THEN 'processing'
            WHEN ob.Status = 'Delivered' THEN 'delivered'
            WHEN ob.Status = 'Cancelled' THEN 'cancelled'
            ELSE 'pending'
        END AS Status,
        'credit_card' AS PaymentMethod,
        ob.CreatedAt,
        ob.UpdatedAt
    FROM #OrdersBackup ob
    INNER JOIN Users u ON ob.UserId = u.Id;
    
    DECLARE @MigratedOrders INT = @@ROWCOUNT;
    
    IF @MigratedOrders > 0
        PRINT 'â Pedidos migrados: ' + CAST(@MigratedOrders AS VARCHAR);
    ELSE
        PRINT 'â ï¸ No se pudieron migrar pedidos (verifica que existan usuarios)';
END

-- Migrar OrderItems (si hay backup)
IF OBJECT_ID('tempdb..#OrderItemsBackup') IS NOT NULL
BEGIN
    -- Crear mapping de IDs antiguos a nuevos
    DECLARE @OrderMapping TABLE (OldId INT, NewId INT);
    
    INSERT INTO @OrderMapping (OldId, NewId)
    SELECT ob.Id, o.Id
    FROM #OrdersBackup ob
    INNER JOIN Orders o ON o.CreatedAt = ob.CreatedAt AND o.Total = ob.TotalAmount;
    
    -- Migrar items
    INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
    SELECT 
        om.NewId AS OrderId,
        oib.ProductId,
        p.Name AS ProductName,
        oib.Quantity,
        oib.UnitPrice AS Price,
        (oib.Quantity * oib.UnitPrice) AS Subtotal
    FROM #OrderItemsBackup oib
    INNER JOIN @OrderMapping om ON oib.OrderId = om.OldId
    INNER JOIN Products p ON oib.ProductId = p.Id;
    
    DECLARE @MigratedItems INT = @@ROWCOUNT;
    
    IF @MigratedItems > 0
        PRINT 'â Items migrados: ' + CAST(@MigratedItems AS VARCHAR);
    ELSE
        PRINT 'â ï¸ No se pudieron migrar items';
END

PRINT '';

-- ============================================
-- PASO 5: INSERTAR DATOS DE PRUEBA (SI ESTÁ VACÍO)
-- ============================================
PRINT 'PASO 5: Verificando si necesita datos de prueba...';

DECLARE @HasOrders INT = (SELECT COUNT(*) FROM Orders);

IF @HasOrders = 0
BEGIN
    PRINT 'Insertando datos de prueba...';
    
    -- Obtener un usuario Customer
    DECLARE @CustomerId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Customer' ORDER BY Id);
    
    IF @CustomerId IS NOT NULL
    BEGIN
        DECLARE @CustomerName NVARCHAR(100) = (SELECT Name FROM Users WHERE Id = @CustomerId);
        DECLARE @CustomerEmail NVARCHAR(255) = (SELECT Email FROM Users WHERE Id = @CustomerId);
        
        -- Pedido 1: Entregado
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Calle Mayor 123, Madrid, 28001, España', 269.97, 15.00, 284.97, 'delivered', 'credit_card', DATEADD(day, -5, GETUTCDATE()), DATEADD(day, -1, GETUTCDATE()));
        
        DECLARE @Order1 INT = SCOPE_IDENTITY();
        
        -- Items del Pedido 1
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        SELECT @Order1, Id, Name, 2, Price, (2 * Price) FROM Products WHERE Id = (SELECT TOP 1 Id FROM Products ORDER BY Id);
        
        -- Historial del Pedido 1
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order1, 'pending', 'Pedido recibido', DATEADD(day, -5, GETUTCDATE())),
            (@Order1, 'processing', 'En preparación', DATEADD(day, -4, GETUTCDATE())),
            (@Order1, 'delivered', 'Entregado', DATEADD(day, -1, GETUTCDATE()));
        
        -- Pedido 2: Procesando
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Avenida Libertad 45, Barcelona, 08001, España', 149.99, 12.00, 161.99, 'processing', 'paypal', DATEADD(day, -2, GETUTCDATE()), DATEADD(hour, -6, GETUTCDATE()));
        
        DECLARE @Order2 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        SELECT @Order2, Id, Name, 1, Price, Price FROM Products WHERE Id = (SELECT TOP 1 Id FROM Products ORDER BY Id OFFSET 1 ROWS);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order2, 'pending', 'Pedido recibido', DATEADD(day, -2, GETUTCDATE())),
            (@Order2, 'processing', 'Verificando stock', DATEADD(hour, -6, GETUTCDATE()));
        
        -- Pedido 3: Pendiente
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Plaza España 12, Valencia, 46001, España', 209.97, 15.00, 224.97, 'pending', 'credit_card', DATEADD(hour, -3, GETUTCDATE()), DATEADD(hour, -3, GETUTCDATE()));
        
        DECLARE @Order3 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        SELECT @Order3, Id, Name, 1, Price, Price FROM Products WHERE Id IN (SELECT TOP 2 Id FROM Products ORDER BY Id OFFSET 2 ROWS);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES (@Order3, 'pending', 'Pedido recibido', DATEADD(hour, -3, GETUTCDATE()));
        
        PRINT 'â 3 pedidos de prueba creados';
        PRINT 'â Items insertados en pedidos';
        PRINT 'â Historial de estados creado';
    END
    ELSE
    BEGIN
        PRINT 'â ï¸ No hay usuarios Customer para crear pedidos de prueba';
        PRINT 'Ejecuta: Database/Users-Authentication-Setup.sql primero';
    END
    
    -- Activity Logs de prueba
    IF NOT EXISTS (SELECT * FROM ActivityLogs)
    BEGIN
        INSERT INTO ActivityLogs (Type, Text, Timestamp)
        VALUES 
            ('order', 'Nuevo pedido #1 creado', DATEADD(hour, -2, GETUTCDATE())),
            ('product', 'Producto actualizado', DATEADD(hour, -4, GETUTCDATE())),
            ('user', 'Nuevo cliente registrado', DATEADD(hour, -6, GETUTCDATE()));
        
        PRINT 'â Activity logs de prueba creados';
    END
    
    -- Notifications de prueba
    DECLARE @AdminId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Admin');
    IF @AdminId IS NOT NULL AND NOT EXISTS (SELECT * FROM Notifications)
    BEGIN
        INSERT INTO Notifications (UserId, Title, Message, Type, IsRead, CreatedAt)
        VALUES 
            (@AdminId, 'Nuevo pedido', 'Pedido #1 recibido', 'order', 0, DATEADD(hour, -1, GETUTCDATE())),
            (@AdminId, 'Stock bajo', 'Producto X tiene stock bajo', 'product', 0, DATEADD(hour, -3, GETUTCDATE()));
        
        PRINT 'â Notifications de prueba creadas';
    END
END
ELSE
BEGIN
    PRINT 'â Tabla Orders ya tiene datos (' + CAST(@HasOrders AS VARCHAR) + ' pedidos)';
END

PRINT '';

-- ============================================
-- PASO 6: VERIFICACIÓN FINAL
-- ============================================
PRINT 'PASO 6: Verificación final...';
PRINT '';

-- Verificar estructura de Orders
PRINT 'Estructura de Orders:';
SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS Tipo,
    CHARACTER_MAXIMUM_LENGTH AS Longitud,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT 'Estructura de OrderItems:';
SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS Tipo,
    CHARACTER_MAXIMUM_LENGTH AS Longitud,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'OrderItems'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- Contar registros
SELECT 
    'Orders' AS Tabla, COUNT(*) AS Total,
    SUM(CASE WHEN Status = 'pending' THEN 1 ELSE 0 END) AS Pending,
    SUM(CASE WHEN Status = 'processing' THEN 1 ELSE 0 END) AS Processing,
    SUM(CASE WHEN Status = 'delivered' THEN 1 ELSE 0 END) AS Delivered,
    SUM(CASE WHEN Status = 'cancelled' THEN 1 ELSE 0 END) AS Cancelled
FROM Orders;

SELECT 'OrderItems' AS Tabla, COUNT(*) AS Total FROM OrderItems;
SELECT 'OrderStatusHistory' AS Tabla, COUNT(*) AS Total FROM OrderStatusHistory;
SELECT 'ActivityLogs' AS Tabla, COUNT(*) AS Total FROM ActivityLogs;
SELECT 'Notifications' AS Tabla, COUNT(*) AS Total FROM Notifications;

PRINT '';
PRINT '============================================';
PRINT 'â CORRECCIÓN COMPLETADA EXITOSAMENTE';
PRINT '============================================';
PRINT '';
PRINT 'Cambios realizados:';
PRINT 'â Orders: Estructura actualizada con campos correctos';
PRINT 'â OrderItems: Columnas renombradas (UnitPrice â Price)';
PRINT 'â OrderStatusHistory: Tabla creada';
PRINT 'â ActivityLogs: Tabla creada';
PRINT 'â Notifications: Tabla creada';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '1. Reinicia el backend: dotnet run';
PRINT '2. Prueba los endpoints en Swagger';
PRINT '3. â¡ï¸ Backend debería funcionar 100%';
PRINT '============================================';
GO
