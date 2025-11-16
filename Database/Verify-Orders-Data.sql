USE BoskoDB;
GO

PRINT '===========================================';
PRINT 'VERIFICACIÓN DE DATOS - GESTIÓN DE PEDIDOS';
PRINT '===========================================';
PRINT '';

PRINT '1. VERIFICAR TABLA ORDERS';
PRINT '--------------------------';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    DECLARE @OrderCount INT = (SELECT COUNT(*) FROM Orders);
    PRINT '✅ Tabla Orders existe';
    PRINT 'Total de pedidos: ' + CAST(@OrderCount AS VARCHAR);
    
    IF @OrderCount = 0
    BEGIN
        PRINT '⚠️ WARNING: No hay pedidos en la tabla';
        PRINT 'Ejecuta: Database/Complete-Data-Insert-Clean.sql';
    END
    ELSE
    BEGIN
        PRINT '';
        PRINT 'Detalle de pedidos:';
        SELECT 
            o.Id,
            o.CustomerName,
            o.CustomerEmail,
            o.Status,
            o.Total AS Amount,
            COUNT(oi.Id) AS Items,
            o.CreatedAt,
            o.UpdatedAt
        FROM Orders o
        LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
        GROUP BY o.Id, o.CustomerName, o.CustomerEmail, o.Status, o.Total, o.CreatedAt, o.UpdatedAt
        ORDER BY o.CreatedAt DESC;
    END
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla Orders no existe';
    PRINT 'Ejecuta: Database/Admin-Panel-Setup.sql';
END
PRINT '';

PRINT '2. VERIFICAR TABLA ORDERITEMS';
PRINT '------------------------------';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    DECLARE @ItemCount INT = (SELECT COUNT(*) FROM OrderItems);
    PRINT '✅ Tabla OrderItems existe';
    PRINT 'Total de items: ' + CAST(@ItemCount AS VARCHAR);
    
    IF @ItemCount = 0
    BEGIN
        PRINT '⚠️ WARNING: No hay items en los pedidos';
    END
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla OrderItems no existe';
END
PRINT '';

PRINT '3. VERIFICAR RELACIONES';
PRINT '------------------------';
SELECT 
    'Pedido #' + CAST(o.Id AS VARCHAR) AS Pedido,
    COUNT(oi.Id) AS CantidadItems,
    SUM(oi.Subtotal) AS SumaItems,
    o.Subtotal AS SubtotalPedido,
    CASE 
        WHEN SUM(oi.Subtotal) = o.Subtotal THEN '✅ CORRECTO'
        ELSE '❌ DESCUADRADO'
    END AS Validacion
FROM Orders o
LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
GROUP BY o.Id, o.Subtotal
ORDER BY o.Id;
PRINT '';

PRINT '4. VERIFICAR USUARIOS';
PRINT '---------------------';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    SELECT 
        Role AS Rol,
        COUNT(*) AS Cantidad
    FROM Users
    GROUP BY Role;
    
    DECLARE @CustomerCount INT = (SELECT COUNT(*) FROM Users WHERE Role = 'Customer');
    DECLARE @AdminCount INT = (SELECT COUNT(*) FROM Users WHERE Role = 'Admin');
    
    IF @CustomerCount = 0
    BEGIN
        PRINT '';
        PRINT '⚠️ WARNING: No hay usuarios Customer';
        PRINT 'Los pedidos necesitan un CustomerId válido';
    END
    
    IF @AdminCount = 0
    BEGIN
        PRINT '';
        PRINT '⚠️ WARNING: No hay usuarios Admin';
        PRINT 'Ejecuta: Database/Users-Authentication-Setup.sql';
    END
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla Users no existe';
END
PRINT '';

PRINT '5. VERIFICAR PRODUCTOS';
PRINT '----------------------';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    DECLARE @ProductCount INT = (SELECT COUNT(*) FROM Products);
    PRINT '✅ Tabla Products existe';
    PRINT 'Total de productos: ' + CAST(@ProductCount AS VARCHAR);
    
    IF @ProductCount = 0
    BEGIN
        PRINT '⚠️ WARNING: No hay productos';
        PRINT 'Ejecuta: Database/Complete-Data-Insert-Clean.sql';
    END
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla Products no existe';
END
PRINT '';

PRINT '6. PRUEBA DE QUERY DEL ENDPOINT';
PRINT '--------------------------------';
PRINT 'Esta es la query que ejecuta GET /api/admin/orders:';
PRINT '';

DECLARE @page INT = 1;
DECLARE @limit INT = 10;

SELECT 
    o.Id,
    o.CustomerName,
    o.CustomerEmail,
    COUNT(oi.Id) AS Items,
    o.Total AS Amount,
    o.Status,
    o.CreatedAt,
    o.UpdatedAt
FROM Orders o
LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
GROUP BY o.Id, o.CustomerName, o.CustomerEmail, o.Total, o.Status, o.CreatedAt, o.UpdatedAt
ORDER BY o.CreatedAt DESC
OFFSET (@page - 1) * @limit ROWS
FETCH NEXT @limit ROWS ONLY;
PRINT '';

PRINT '===========================================';
PRINT 'RESUMEN DE VERIFICACIÓN';
PRINT '===========================================';

DECLARE @Summary NVARCHAR(MAX) = '';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
    SET @Summary = @Summary + '✅ Tabla Orders existe' + CHAR(13);
ELSE
    SET @Summary = @Summary + '❌ Tabla Orders NO existe' + CHAR(13);

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
    SET @Summary = @Summary + '✅ Tabla OrderItems existe' + CHAR(13);
ELSE
    SET @Summary = @Summary + '❌ Tabla OrderItems NO existe' + CHAR(13);

IF EXISTS (SELECT * FROM Orders)
    SET @Summary = @Summary + '✅ Hay pedidos en la BD' + CHAR(13);
ELSE
    SET @Summary = @Summary + '⚠️ NO hay pedidos en la BD' + CHAR(13);

IF EXISTS (SELECT * FROM OrderItems)
    SET @Summary = @Summary + '✅ Hay items en los pedidos' + CHAR(13);
ELSE
    SET @Summary = @Summary + '⚠️ NO hay items en los pedidos' + CHAR(13);

IF EXISTS (SELECT * FROM Users WHERE Role = 'Customer')
    SET @Summary = @Summary + '✅ Existen usuarios Customer' + CHAR(13);
ELSE
    SET @Summary = @Summary + '⚠️ NO existen usuarios Customer' + CHAR(13);

IF EXISTS (SELECT * FROM Users WHERE Role = 'Admin')
    SET @Summary = @Summary + '✅ Existen usuarios Admin' + CHAR(13);
ELSE
    SET @Summary = @Summary + '⚠️ NO existen usuarios Admin' + CHAR(13);

PRINT @Summary;
PRINT '';
PRINT '===========================================';

IF EXISTS (SELECT * FROM Orders) AND EXISTS (SELECT * FROM OrderItems)
BEGIN
    PRINT '✅ BASE DE DATOS LISTA PARA USAR';
    PRINT '';
    PRINT 'Puedes probar:';
    PRINT '  GET https://localhost:5006/api/admin/orders';
END
ELSE
BEGIN
    PRINT '⚠️ FALTAN DATOS EN LA BASE DE DATOS';
    PRINT '';
    PRINT 'Ejecuta estos scripts en orden:';
    PRINT '  1. Database/BoskoDB-Setup.sql';
    PRINT '  2. Database/Users-Authentication-Setup.sql';
    PRINT '  3. Database/Admin-Panel-Setup.sql';
    PRINT '  4. Database/Complete-Data-Insert-Clean.sql';
END

PRINT '===========================================';
GO
