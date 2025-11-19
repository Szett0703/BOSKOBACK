-- ============================================
-- SCRIPT DE ACTUALIZACIÓN - SISTEMA DE PEDIDOS
-- Agregar campos faltantes y tabla ShippingAddresses
-- ============================================

USE BoskoDB;
GO

PRINT '============================================';
PRINT '📦 ACTUALIZACIÓN DEL SISTEMA DE PEDIDOS';
PRINT '============================================';
PRINT '';

-- ============================================
-- 1. ACTUALIZAR TABLA ORDERS
-- ============================================
PRINT '1. Actualizando tabla Orders...';

-- Agregar OrderNumber si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'OrderNumber')
BEGIN
    ALTER TABLE Orders ADD OrderNumber NVARCHAR(50) NULL;
    PRINT '   ✅ Columna OrderNumber agregada';
END
ELSE
BEGIN
    PRINT '   ℹ️  OrderNumber ya existe';
END

-- Agregar Tax si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'Tax')
BEGIN
    ALTER TABLE Orders ADD Tax DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT '   ✅ Columna Tax agregada';
END
ELSE
BEGIN
    PRINT '   ℹ️  Tax ya existe';
END

-- Agregar TrackingNumber si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'TrackingNumber')
BEGIN
    ALTER TABLE Orders ADD TrackingNumber NVARCHAR(100) NULL;
    PRINT '   ✅ Columna TrackingNumber agregada';
END
ELSE
BEGIN
    PRINT '   ℹ️  TrackingNumber ya existe';
END

-- Agregar Notes si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'Notes')
BEGIN
    ALTER TABLE Orders ADD Notes NVARCHAR(500) NULL;
    PRINT '   ✅ Columna Notes agregada';
END
ELSE
BEGIN
    PRINT '   ℹ️  Notes ya existe';
END

PRINT '';

-- ============================================
-- 2. ACTUALIZAR TABLA ORDERITEMS
-- ============================================
PRINT '2. Actualizando tabla OrderItems...';

-- Agregar ProductImage si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrderItems') AND name = 'ProductImage')
BEGIN
    ALTER TABLE OrderItems ADD ProductImage NVARCHAR(500) NULL;
    PRINT '   ✅ Columna ProductImage agregada';
END
ELSE
BEGIN
    PRINT '   ℹ️  ProductImage ya existe';
END

-- Renombrar Price a UnitPrice si existe Price
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrderItems') AND name = 'Price')
   AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrderItems') AND name = 'UnitPrice')
BEGIN
    EXEC sp_rename 'OrderItems.Price', 'UnitPrice', 'COLUMN';
    PRINT '   ✅ Columna Price renombrada a UnitPrice';
END
ELSE
BEGIN
    PRINT '   ℹ️  UnitPrice ya existe o Price no encontrado';
END

PRINT '';

-- ============================================
-- 3. CREAR TABLA SHIPPINGADDRESSES
-- ============================================
PRINT '3. Creando tabla ShippingAddresses...';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShippingAddresses')
BEGIN
    CREATE TABLE ShippingAddresses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        Phone NVARCHAR(20) NOT NULL,
        Street NVARCHAR(200) NOT NULL,
        City NVARCHAR(100) NOT NULL,
        State NVARCHAR(100) NOT NULL,
        PostalCode NVARCHAR(20) NOT NULL,
        Country NVARCHAR(100) NOT NULL DEFAULT 'México',
        
        CONSTRAINT FK_ShippingAddresses_Orders FOREIGN KEY (OrderId)
            REFERENCES Orders(Id) ON DELETE CASCADE
    );
    
    -- Índice para búsquedas por OrderId
    CREATE INDEX IX_ShippingAddresses_OrderId ON ShippingAddresses(OrderId);
    
    PRINT '   ✅ Tabla ShippingAddresses creada exitosamente';
END
ELSE
BEGIN
    PRINT '   ℹ️  Tabla ShippingAddresses ya existe';
END

PRINT '';

-- ============================================
-- 4. GENERAR ORDERNUMBER PARA PEDIDOS EXISTENTES
-- ============================================
PRINT '4. Generando OrderNumber para pedidos existentes...';

DECLARE @UpdatedOrders INT = 0;

UPDATE Orders
SET OrderNumber = 'ORD-' + RIGHT('00000' + CAST(Id AS VARCHAR), 5)
WHERE OrderNumber IS NULL;

SET @UpdatedOrders = @@ROWCOUNT;

IF @UpdatedOrders > 0
BEGIN
    PRINT '   ✅ ' + CAST(@UpdatedOrders AS VARCHAR) + ' pedidos actualizados con OrderNumber';
END
ELSE
BEGIN
    PRINT '   ℹ️  Todos los pedidos ya tienen OrderNumber';
END

PRINT '';

-- ============================================
-- 5. MIGRAR SHIPPINGADDRESS A SHIPPINGADDRESSES
-- ============================================
PRINT '5. Migrando direcciones de envío...';

DECLARE @MigratedAddresses INT = 0;

-- Insertar direcciones de envío desde Orders.ShippingAddress
INSERT INTO ShippingAddresses (OrderId, FullName, Phone, Street, City, State, PostalCode, Country)
SELECT 
    o.Id,
    o.CustomerName,
    ISNULL(u.Phone, ''),
    o.ShippingAddress,
    'Ciudad', -- Valor por defecto
    'Estado', -- Valor por defecto
    '00000', -- Valor por defecto
    'México'
FROM Orders o
LEFT JOIN Users u ON o.CustomerId = u.Id
WHERE NOT EXISTS (
    SELECT 1 FROM ShippingAddresses sa WHERE sa.OrderId = o.Id
)
AND o.ShippingAddress IS NOT NULL
AND o.ShippingAddress <> '';

SET @MigratedAddresses = @@ROWCOUNT;

IF @MigratedAddresses > 0
BEGIN
    PRINT '   ✅ ' + CAST(@MigratedAddresses AS VARCHAR) + ' direcciones migradas';
END
ELSE
BEGIN
    PRINT '   ℹ️  No hay direcciones nuevas para migrar';
END

PRINT '';

-- ============================================
-- 6. VERIFICAR RESULTADOS
-- ============================================
PRINT '============================================';
PRINT '📊 VERIFICACIÓN DE RESULTADOS';
PRINT '============================================';
PRINT '';

-- Contar pedidos con OrderNumber
DECLARE @OrdersWithNumber INT = (SELECT COUNT(*) FROM Orders WHERE OrderNumber IS NOT NULL);
DECLARE @TotalOrders INT = (SELECT COUNT(*) FROM Orders);

PRINT 'Pedidos con OrderNumber: ' + CAST(@OrdersWithNumber AS VARCHAR) + ' de ' + CAST(@TotalOrders AS VARCHAR);

-- Contar OrderItems con ProductImage
DECLARE @ItemsWithImage INT = (SELECT COUNT(*) FROM OrderItems WHERE ProductImage IS NOT NULL);
DECLARE @TotalItems INT = (SELECT COUNT(*) FROM OrderItems);

PRINT 'Items con ProductImage: ' + CAST(@ItemsWithImage AS VARCHAR) + ' de ' + CAST(@TotalItems AS VARCHAR);

-- Contar direcciones de envío
DECLARE @TotalShippingAddresses INT = (SELECT COUNT(*) FROM ShippingAddresses);

PRINT 'Direcciones de envío: ' + CAST(@TotalShippingAddresses AS VARCHAR);

PRINT '';

-- Mostrar estructura actualizada de Orders
PRINT 'Columnas de Orders:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- Mostrar estructura de OrderItems
PRINT 'Columnas de OrderItems:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'OrderItems'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- Mostrar estructura de ShippingAddresses
PRINT 'Columnas de ShippingAddresses:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ShippingAddresses'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '============================================';
PRINT '✅ ACTUALIZACIÓN COMPLETADA';
PRINT '============================================';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '1. Reiniciar el backend (dotnet run)';
PRINT '2. Verificar endpoints en Swagger';
PRINT '3. Probar creación de pedidos con dirección de envío';
PRINT '';

GO
