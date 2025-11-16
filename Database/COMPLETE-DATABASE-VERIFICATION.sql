-- ============================================
-- VERIFICACIÓN COMPLETA DE BASE DE DATOS
-- Bosko E-Commerce Backend
-- ============================================

USE master;
GO

PRINT '============================================';
PRINT 'VERIFICACIÓN COMPLETA DE BASE DE DATOS';
PRINT '============================================';
PRINT '';

-- ============================================
-- 1. VERIFICAR QUE SQL SERVER ESTÁ CORRIENDO
-- ============================================
PRINT '1. Verificando SQL Server...';
SELECT @@VERSION AS 'SQL Server Version';
PRINT 'â Sql Server est corriendo';
PRINT '';

-- ============================================
-- 2. VERIFICAR SI EXISTE LA BASE DE DATOS
-- ============================================
PRINT '2. Verificando base de datos BoskoDB...';
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'BoskoDB')
BEGIN
    PRINT 'â BoskoDB existe';
    USE BoskoDB;
END
ELSE
BEGIN
    PRINT '❌ BoskoDB NO EXISTE';
    PRINT 'Ejecuta: Database/BoskoDB-Setup.sql';
    PRINT '';
    -- No continuar si no existe la BD
    RAISERROR('Base de datos BoskoDB no encontrada', 16, 1);
    RETURN;
END
PRINT '';

-- ============================================
-- 3. LISTAR TODAS LAS TABLAS
-- ============================================
PRINT '3. Tablas existentes:';
SELECT 
    TABLE_SCHEMA AS [Schema],
    TABLE_NAME AS [Table],
    TABLE_TYPE AS [Type]
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
PRINT '';

-- ============================================
-- 4. VERIFICAR TABLAS REQUERIDAS
-- ============================================
PRINT '4. Verificando tablas requeridas...';

DECLARE @MissingTables TABLE (TableName NVARCHAR(100));

-- Verificar Categories
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
    INSERT INTO @MissingTables VALUES ('Categories');
ELSE
    PRINT 'â Categories existe';

-- Verificar Products
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
    INSERT INTO @MissingTables VALUES ('Products');
ELSE
    PRINT 'â Products existe';

-- Verificar Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
    INSERT INTO @MissingTables VALUES ('Users');
ELSE
    PRINT 'â Users existe';

-- Verificar Orders
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
    INSERT INTO @MissingTables VALUES ('Orders');
ELSE
    PRINT 'â Orders existe';

-- Verificar OrderItems
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
    INSERT INTO @MissingTables VALUES ('OrderItems');
ELSE
    PRINT 'â OrderItems existe';

-- Verificar OrderStatusHistory
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderStatusHistory')
    INSERT INTO @MissingTables VALUES ('OrderStatusHistory');
ELSE
    PRINT 'â OrderStatusHistory existe';

-- Verificar ActivityLogs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ActivityLogs')
    INSERT INTO @MissingTables VALUES ('ActivityLogs');
ELSE
    PRINT 'â ActivityLogs existe';

-- Verificar Notifications
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
    INSERT INTO @MissingTables VALUES ('Notifications');
ELSE
    PRINT 'â Notifications existe';

-- Mostrar tablas faltantes
IF EXISTS (SELECT * FROM @MissingTables)
BEGIN
    PRINT '';
    PRINT '❌ TABLAS FALTANTES:';
    SELECT TableName FROM @MissingTables;
    PRINT '';
    PRINT 'Ejecuta: dotnet ef database update';
END
PRINT '';

-- ============================================
-- 5. VERIFICAR DATOS EN TABLAS
-- ============================================
PRINT '5. Verificando datos en tablas...';

-- Categories
DECLARE @CatCount INT = (SELECT COUNT(*) FROM Categories);
PRINT 'Categories: ' + CAST(@CatCount AS VARCHAR) + ' registros';
IF @CatCount = 0
    PRINT 'â ï¸ Tabla Categories está vacía';

-- Products
DECLARE @ProdCount INT = (SELECT COUNT(*) FROM Products);
PRINT 'Products: ' + CAST(@ProdCount AS VARCHAR) + ' registros';
IF @ProdCount = 0
    PRINT 'â ï¸ Tabla Products está vacía';

-- Users
DECLARE @UserCount INT = (SELECT COUNT(*) FROM Users);
PRINT 'Users: ' + CAST(@UserCount AS VARCHAR) + ' registros';
IF @UserCount = 0
    PRINT 'â ï¸ Tabla Users está vacía';

-- Orders
DECLARE @OrderCount INT = (SELECT COUNT(*) FROM Orders);
PRINT 'Orders: ' + CAST(@OrderCount AS VARCHAR) + ' registros';
IF @OrderCount = 0
    PRINT 'â ï¸ Tabla Orders está vacía';

-- OrderItems
DECLARE @ItemCount INT = (SELECT COUNT(*) FROM OrderItems);
PRINT 'OrderItems: ' + CAST(@ItemCount AS VARCHAR) + ' registros';

PRINT '';

-- ============================================
-- 6. VERIFICAR INTEGRIDAD DE DATOS
-- ============================================
PRINT '6. Verificando integridad de datos...';

-- Verificar Orders sin Items
DECLARE @OrdersWithoutItems INT = (
    SELECT COUNT(*) FROM Orders o
    WHERE NOT EXISTS (SELECT 1 FROM OrderItems oi WHERE oi.OrderId = o.Id)
);
IF @OrdersWithoutItems > 0
    PRINT 'â ï¸ Hay ' + CAST(@OrdersWithoutItems AS VARCHAR) + ' pedidos sin items';
ELSE
    PRINT 'â Todos los pedidos tienen items';

-- Verificar Products sin Category
DECLARE @ProductsWithoutCategory INT = (
    SELECT COUNT(*) FROM Products WHERE CategoryId IS NULL
);
IF @ProductsWithoutCategory > 0
    PRINT 'â ï¸ Hay ' + CAST(@ProductsWithoutCategory AS VARCHAR) + ' productos sin categoría';
ELSE
    PRINT 'â Todos los productos tienen categoría';

-- Verificar Users Admin
DECLARE @AdminCount INT = (SELECT COUNT(*) FROM Users WHERE Role = 'Admin');
IF @AdminCount = 0
    PRINT '❌ No hay usuarios Admin';
ELSE
    PRINT 'â Hay ' + CAST(@AdminCount AS VARCHAR) + ' usuario(s) Admin';

PRINT '';

-- ============================================
-- 7. VERIFICAR ÍNDICES
-- ============================================
PRINT '7. Verificando índices...';

SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
WHERE OBJECT_NAME(i.object_id) IN ('Categories', 'Products', 'Orders', 'OrderItems', 'Users')
AND i.name IS NOT NULL
ORDER BY TableName, IndexName;

PRINT '';

-- ============================================
-- 8. VERIFICAR FOREIGN KEYS
-- ============================================
PRINT '8. Verificando foreign keys...';

SELECT 
    OBJECT_NAME(f.parent_object_id) AS TableName,
    f.name AS FK_Name,
    OBJECT_NAME(f.referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys f
WHERE OBJECT_NAME(f.parent_object_id) IN ('Products', 'Orders', 'OrderItems', 'OrderStatusHistory', 'Notifications')
ORDER BY TableName;

PRINT '';

-- ============================================
-- 9. MOSTRAR DATOS DE EJEMPLO
-- ============================================
PRINT '9. Datos de ejemplo:';

-- Top 5 Categories
PRINT '';
PRINT 'Top 5 Categorías:';
SELECT TOP 5 Id, Name FROM Categories ORDER BY Id;

-- Top 5 Products
PRINT '';
PRINT 'Top 5 Productos:';
SELECT TOP 5 Id, Name, Price, Stock FROM Products ORDER BY Id;

-- Top 5 Users
PRINT '';
PRINT 'Top 5 Usuarios:';
SELECT TOP 5 Id, Name, Email, Role FROM Users ORDER BY Id;

-- Top 5 Orders
PRINT '';
PRINT 'Top 5 Pedidos:';
SELECT TOP 5 Id, CustomerName, Total, Status, CreatedAt FROM Orders ORDER BY CreatedAt DESC;

PRINT '';

-- ============================================
-- 10. RESUMEN FINAL
-- ============================================
PRINT '============================================';
PRINT 'RESUMEN DE VERIFICACIÓN';
PRINT '============================================';

DECLARE @TablesExist BIT = 
    CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name IN ('Categories', 'Products', 'Users', 'Orders', 'OrderItems'))
    THEN 1 ELSE 0 END;

DECLARE @HasData BIT = 
    CASE WHEN @CatCount > 0 AND @ProdCount > 0 AND @UserCount > 0
    THEN 1 ELSE 0 END;

DECLARE @HasAdmin BIT = CASE WHEN @AdminCount > 0 THEN 1 ELSE 0 END;

PRINT '';
PRINT 'Estado de las tablas:';
IF @TablesExist = 1
    PRINT 'â Todas las tablas requeridas existen';
ELSE
    PRINT '❌ Faltan tablas requeridas';

PRINT '';
PRINT 'Estado de los datos:';
IF @HasData = 1
    PRINT 'â Hay datos en las tablas principales';
ELSE
    PRINT 'â ï¸ Tablas vacías - Ejecuta: Database/Complete-Data-Insert-Clean.sql';

PRINT '';
PRINT 'Estado de autenticación:';
IF @HasAdmin = 1
    PRINT 'â Existe al menos un usuario Admin';
ELSE
    PRINT '❌ No hay usuarios Admin - Ejecuta: Database/Users-Authentication-Setup.sql';

PRINT '';
PRINT '============================================';
PRINT 'PRÓXIMOS PASOS';
PRINT '============================================';

IF @TablesExist = 0
BEGIN
    PRINT '1. Ejecutar migraciones:';
    PRINT '   dotnet ef database update';
    PRINT '';
END

IF @HasAdmin = 0
BEGIN
    PRINT '2. Crear usuarios:';
    PRINT '   Ejecutar: Database/Users-Authentication-Setup.sql';
    PRINT '';
END

IF @HasData = 0
BEGIN
    PRINT '3. Insertar datos de prueba:';
    PRINT '   Ejecutar: Database/Complete-Data-Insert-Clean.sql';
    PRINT '';
END

IF @TablesExist = 1 AND @HasData = 1 AND @HasAdmin = 1
BEGIN
    PRINT 'â ¡ï¸ BASE DE DATOS LISTA PARA USAR';
    PRINT '';
    PRINT 'Inicia el backend:';
    PRINT '   dotnet run';
    PRINT '';
    PRINT 'Prueba los endpoints:';
    PRINT '   https://localhost:5006/swagger';
END

PRINT '============================================';
GO
