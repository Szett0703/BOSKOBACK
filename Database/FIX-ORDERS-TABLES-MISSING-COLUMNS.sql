-- =====================================================
-- 🚨 SCRIPT DE CORRECCIÓN CRÍTICA - SISTEMA DE PEDIDOS
-- =====================================================
-- Fecha: 19 de Noviembre 2025
-- Problema: Faltan columnas en Orders/OrderItems y falta tabla ShippingAddresses
-- Este script debe ejecutarse INMEDIATAMENTE antes de usar el sistema de pedidos
-- =====================================================

USE [BoskoDB]
GO

PRINT '============================================'
PRINT '🔧 INICIANDO CORRECCIÓN DE BASE DE DATOS'
PRINT '============================================'
PRINT ''

-- =====================================================
-- PASO 1: AGREGAR COLUMNAS FALTANTES EN ORDERS
-- =====================================================
PRINT '📋 PASO 1: Verificando y agregando columnas en tabla Orders...'

-- Agregar columna OrderNumber (CRÍTICA para el sistema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'OrderNumber')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [OrderNumber] NVARCHAR(50) NULL
    
    PRINT '   ✅ Columna OrderNumber agregada'
END
ELSE
    PRINT '   ℹ️  Columna OrderNumber ya existe'

-- Agregar columna Tax (CRÍTICA - necesaria para cálculos)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'Tax')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [Tax] DECIMAL(18,2) NOT NULL DEFAULT 0
    
    PRINT '   ✅ Columna Tax agregada'
END
ELSE
    PRINT '   ℹ️  Columna Tax ya existe'

-- Agregar columna TrackingNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'TrackingNumber')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [TrackingNumber] NVARCHAR(100) NULL
    
    PRINT '   ✅ Columna TrackingNumber agregada'
END
ELSE
    PRINT '   ℹ️  Columna TrackingNumber ya existe'

-- Agregar columna Notes
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'Notes')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [Notes] NVARCHAR(500) NULL
    
    PRINT '   ✅ Columna Notes agregada'
END
ELSE
    PRINT '   ℹ️  Columna Notes ya existe'

PRINT ''

-- =====================================================
-- PASO 2: AGREGAR COLUMNA FALTANTE EN ORDERITEMS
-- =====================================================
PRINT '📋 PASO 2: Verificando y agregando columnas en tabla OrderItems...'

-- Agregar columna ProductImage (requerida por el modelo)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND name = 'ProductImage')
BEGIN
    ALTER TABLE [dbo].[OrderItems]
    ADD [ProductImage] NVARCHAR(500) NULL
    
    PRINT '   ✅ Columna ProductImage agregada'
END
ELSE
    PRINT '   ℹ️  Columna ProductImage ya existe'

PRINT ''

-- =====================================================
-- PASO 3: CREAR TABLA SHIPPINGADDRESSES (CRÍTICA)
-- =====================================================
PRINT '📋 PASO 3: Verificando y creando tabla ShippingAddresses...'

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShippingAddresses')
BEGIN
    CREATE TABLE [dbo].[ShippingAddresses](
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrderId] INT NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(20) NOT NULL,
        [Street] NVARCHAR(200) NOT NULL,
        [City] NVARCHAR(100) NOT NULL,
        [State] NVARCHAR(100) NOT NULL,
        [PostalCode] NVARCHAR(20) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL DEFAULT 'México',
        
        CONSTRAINT [FK_ShippingAddresses_Orders] FOREIGN KEY ([OrderId])
            REFERENCES [dbo].[Orders] ([Id])
            ON DELETE CASCADE
    )
    
    -- Crear índice en OrderId para mejorar performance
    CREATE NONCLUSTERED INDEX [IX_ShippingAddresses_OrderId] 
    ON [dbo].[ShippingAddresses] ([OrderId] ASC)
    
    PRINT '   ✅ Tabla ShippingAddresses creada exitosamente'
    PRINT '   ✅ Foreign Key FK_ShippingAddresses_Orders agregada'
    PRINT '   ✅ Índice IX_ShippingAddresses_OrderId creado'
END
ELSE
    PRINT '   ℹ️  Tabla ShippingAddresses ya existe'

PRINT ''

-- =====================================================
-- PASO 4: AGREGAR ÍNDICE ÚNICO EN ORDERNUMBER
-- =====================================================
PRINT '📋 PASO 4: Verificando índice único en OrderNumber...'

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_OrderNumber_Unique' AND object_id = OBJECT_ID('dbo.Orders'))
BEGIN
    -- Primero, asegurarse de que no hay valores NULL ni duplicados
    UPDATE [dbo].[Orders]
    SET [OrderNumber] = 'ORD-' + CONVERT(VARCHAR(20), [Id]) + '-' + CONVERT(VARCHAR(4), ABS(CHECKSUM(NEWID())) % 10000)
    WHERE [OrderNumber] IS NULL
    
    -- Crear índice único
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Orders_OrderNumber_Unique]
    ON [dbo].[Orders] ([OrderNumber] ASC)
    WHERE [OrderNumber] IS NOT NULL
    
    PRINT '   ✅ Índice único IX_Orders_OrderNumber_Unique creado'
    PRINT '   ✅ OrderNumbers existentes actualizados'
END
ELSE
    PRINT '   ℹ️  Índice único en OrderNumber ya existe'

PRINT ''

-- =====================================================
-- PASO 5: ACTUALIZAR RESTRICCIÓN DE FK EN ORDERITEMS
-- =====================================================
PRINT '📋 PASO 5: Verificando restricción de FK en OrderItems_ProductId...'

-- Verificar si la restricción tiene ON DELETE NO ACTION (que causa el error)
IF EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_OrderItems_ProductId' 
    AND delete_referential_action = 0  -- 0 = NO ACTION
)
BEGIN
    PRINT '   ⚠️  Restricción FK_OrderItems_ProductId tiene ON DELETE NO ACTION'
    PRINT '   🔧 Modificando a ON DELETE RESTRICT...'
    
    -- Eliminar la restricción existente
    ALTER TABLE [dbo].[OrderItems]
    DROP CONSTRAINT [FK_OrderItems_ProductId]
    
    -- Recrear con ON DELETE RESTRICT (que es lo que el código espera)
    ALTER TABLE [dbo].[OrderItems]  
    WITH CHECK ADD CONSTRAINT [FK_OrderItems_ProductId] 
    FOREIGN KEY([ProductId])
    REFERENCES [dbo].[Products] ([Id])
    ON DELETE NO ACTION  -- Cambiado de CASCADE a NO ACTION para evitar ciclos
    
    PRINT '   ✅ Restricción FK_OrderItems_ProductId actualizada correctamente'
END
ELSE
    PRINT '   ℹ️  Restricción FK_OrderItems_ProductId ya está correcta'

PRINT ''

-- =====================================================
-- PASO 6: VERIFICAR DATOS DE PRUEBA
-- =====================================================
PRINT '📋 PASO 6: Verificando datos de prueba...'

DECLARE @UserCount INT
DECLARE @ProductCount INT

SELECT @UserCount = COUNT(*) FROM [dbo].[Users]
SELECT @ProductCount = COUNT(*) FROM [dbo].[Products]

PRINT '   ℹ️  Usuarios en BD: ' + CAST(@UserCount AS VARCHAR(10))
PRINT '   ℹ️  Productos en BD: ' + CAST(@ProductCount AS VARCHAR(10))

IF @UserCount = 0
BEGIN
    PRINT '   ⚠️  WARNING: No hay usuarios en la BD'
    PRINT '   💡 Ejecutar: POST /api/auth/init-users para crear usuarios de prueba'
END

IF @ProductCount = 0
BEGIN
    PRINT '   ⚠️  WARNING: No hay productos en la BD'
    PRINT '   💡 Crear productos desde el Admin Panel o ejecutar script de seed'
END

PRINT ''

-- =====================================================
-- PASO 7: VERIFICACIÓN FINAL
-- =====================================================
PRINT '📋 PASO 7: Verificación final de estructura...'

-- Verificar columnas en Orders
DECLARE @OrdersColumns TABLE (ColumnName NVARCHAR(100))
INSERT INTO @OrdersColumns 
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Orders'

DECLARE @MissingOrdersColumns NVARCHAR(MAX) = ''

IF NOT EXISTS (SELECT * FROM @OrdersColumns WHERE ColumnName = 'OrderNumber')
    SET @MissingOrdersColumns = @MissingOrdersColumns + 'OrderNumber, '
IF NOT EXISTS (SELECT * FROM @OrdersColumns WHERE ColumnName = 'Tax')
    SET @MissingOrdersColumns = @MissingOrdersColumns + 'Tax, '
IF NOT EXISTS (SELECT * FROM @OrdersColumns WHERE ColumnName = 'TrackingNumber')
    SET @MissingOrdersColumns = @MissingOrdersColumns + 'TrackingNumber, '
IF NOT EXISTS (SELECT * FROM @OrdersColumns WHERE ColumnName = 'Notes')
    SET @MissingOrdersColumns = @MissingOrdersColumns + 'Notes, '

IF LEN(@MissingOrdersColumns) > 0
BEGIN
    PRINT '   ❌ ERROR: Faltan columnas en Orders: ' + LEFT(@MissingOrdersColumns, LEN(@MissingOrdersColumns)-1)
    RAISERROR('Verificación de estructura falló. Por favor revisar manualmente.', 16, 1)
END
ELSE
    PRINT '   ✅ Tabla Orders tiene todas las columnas requeridas'

-- Verificar columnas en OrderItems
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OrderItems' AND COLUMN_NAME = 'ProductImage')
BEGIN
    PRINT '   ❌ ERROR: Falta columna ProductImage en OrderItems'
    RAISERROR('Verificación de estructura falló. Por favor revisar manualmente.', 16, 1)
END
ELSE
    PRINT '   ✅ Tabla OrderItems tiene todas las columnas requeridas'

-- Verificar tabla ShippingAddresses
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShippingAddresses')
BEGIN
    PRINT '   ❌ ERROR: Tabla ShippingAddresses no existe'
    RAISERROR('Verificación de estructura falló. Por favor revisar manualmente.', 16, 1)
END
ELSE
    PRINT '   ✅ Tabla ShippingAddresses existe'

PRINT ''
PRINT '============================================'
PRINT '✅ CORRECCIÓN COMPLETADA EXITOSAMENTE'
PRINT '============================================'
PRINT ''
PRINT '📝 PRÓXIMOS PASOS:'
PRINT '   1. Reiniciar el backend (dotnet run)'
PRINT '   2. Verificar Swagger: https://localhost:5006/swagger'
PRINT '   3. Ejecutar POST /api/auth/init-users si no hay usuarios'
PRINT '   4. Probar POST /api/orders desde Angular o Swagger'
PRINT ''
PRINT '🎯 El sistema de pedidos está listo para usar'
PRINT ''

GO
