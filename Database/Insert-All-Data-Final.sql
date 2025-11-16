USE BoskoDB;
GO

SET IDENTITY_INSERT Categories ON;

IF NOT EXISTS (SELECT * FROM Categories WHERE Id = 1)
    INSERT INTO Categories (Id, Name, CreatedAt, UpdatedAt) 
    VALUES (1, 'Camisas', GETUTCDATE(), GETUTCDATE());

IF NOT EXISTS (SELECT * FROM Categories WHERE Id = 2)
    INSERT INTO Categories (Id, Name, CreatedAt, UpdatedAt) 
    VALUES (2, 'Pantalones', GETUTCDATE(), GETUTCDATE());

IF NOT EXISTS (SELECT * FROM Categories WHERE Id = 3)
    INSERT INTO Categories (Id, Name, CreatedAt, UpdatedAt) 
    VALUES (3, 'Chaquetas', GETUTCDATE(), GETUTCDATE());

IF NOT EXISTS (SELECT * FROM Categories WHERE Id = 4)
    INSERT INTO Categories (Id, Name, CreatedAt, UpdatedAt) 
    VALUES (4, 'Calzado', GETUTCDATE(), GETUTCDATE());

IF NOT EXISTS (SELECT * FROM Categories WHERE Id = 5)
    INSERT INTO Categories (Id, Name, CreatedAt, UpdatedAt) 
    VALUES (5, 'Accesorios', GETUTCDATE(), GETUTCDATE());

SET IDENTITY_INSERT Categories OFF;

IF NOT EXISTS (SELECT * FROM Products WHERE Name = 'Camisa Casual Bosko')
BEGIN
    INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Image, CreatedAt, UpdatedAt)
    VALUES 
    ('Camisa Casual Bosko', 'Camisa de algodón premium con corte moderno. Perfecta para uso diario.', 49.99, 150, 1, 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c', GETUTCDATE(), GETUTCDATE()),
    ('Camisa Formal Blanca', 'Camisa formal de algodón egipcio. Ideal para eventos especiales.', 59.99, 120, 1, 'https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf', GETUTCDATE(), GETUTCDATE()),
    ('Polo Bosko Premium', 'Polo de algodón pima con logo bordado. Estilo deportivo elegante.', 39.99, 200, 1, 'https://images.unsplash.com/photo-1586790170083-2f9ceadc732d', GETUTCDATE(), GETUTCDATE()),
    ('Camisa de Lino Verano', 'Camisa ligera de lino natural. Perfecta para el verano.', 54.99, 90, 1, 'https://images.unsplash.com/photo-1620012253295-c15cc3e65df4', GETUTCDATE(), GETUTCDATE()),
    ('Pantalón Slim Fit Negro', 'Pantalón entallado de corte moderno. Tela elástica y cómoda.', 69.99, 130, 2, 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a', GETUTCDATE(), GETUTCDATE()),
    ('Jeans Clásicos Bosko', 'Jeans de mezclilla premium con lavado especial. Durabilidad garantizada.', 79.99, 110, 2, 'https://images.unsplash.com/photo-1542272604-787c3835535d', GETUTCDATE(), GETUTCDATE()),
    ('Chino Beige Elegante', 'Pantalón chino de algodón. Versátil para cualquier ocasión.', 64.99, 95, 2, 'https://images.unsplash.com/photo-1624378439575-d8705ad7ae80', GETUTCDATE(), GETUTCDATE()),
    ('Jogger Deportivo', 'Pantalón jogger cómodo para actividades deportivas o casual.', 49.99, 160, 2, 'https://images.unsplash.com/photo-1611312449408-fcece27cdbb7', GETUTCDATE(), GETUTCDATE()),
    ('Chaqueta de Cuero Premium', 'Chaqueta de cuero genuino con forro interno. Estilo atemporal.', 189.99, 45, 3, 'https://images.unsplash.com/photo-1551028719-00167b16eac5', GETUTCDATE(), GETUTCDATE()),
    ('Blazer Formal Azul', 'Blazer de corte italiano. Elegancia para eventos formales.', 149.99, 60, 3, 'https://images.unsplash.com/photo-1507679799987-c73779587ccf', GETUTCDATE(), GETUTCDATE()),
    ('Bomber Jacket Moderna', 'Chaqueta bomber de nylon resistente. Estilo urbano moderno.', 99.99, 85, 3, 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea', GETUTCDATE(), GETUTCDATE()),
    ('Abrigo de Invierno', 'Abrigo largo con aislamiento térmico. Perfecto para el frío.', 169.99, 50, 3, 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3', GETUTCDATE(), GETUTCDATE()),
    ('Zapatillas Deportivas Bosko', 'Zapatillas de alto rendimiento con tecnología de amortiguación.', 89.99, 200, 4, 'https://images.unsplash.com/photo-1542291026-7eec264c27ff', GETUTCDATE(), GETUTCDATE()),
    ('Zapatos Formales Oxford', 'Zapatos de cuero para ocasiones formales. Fabricación artesanal.', 119.99, 75, 4, 'https://images.unsplash.com/photo-1614252235316-8c857d38b5f4', GETUTCDATE(), GETUTCDATE()),
    ('Botas de Cuero', 'Botas robustas de cuero genuino. Estilo casual elegante.', 139.99, 65, 4, 'https://images.unsplash.com/photo-1608256246200-53e635b5b65f', GETUTCDATE(), GETUTCDATE()),
    ('Sandalias de Verano', 'Sandalias cómodas para el verano. Material transpirable.', 34.99, 180, 4, 'https://images.unsplash.com/photo-1603487742131-4160ec999306', GETUTCDATE(), GETUTCDATE()),
    ('Cinturón de Cuero Negro', 'Cinturón de cuero genuino con hebilla metálica. Elegancia clásica.', 29.99, 220, 5, 'https://images.unsplash.com/photo-1624222247344-550fb60583f0', GETUTCDATE(), GETUTCDATE()),
    ('Cartera de Piel Bosko', 'Cartera compacta de piel con múltiples compartimentos.', 44.99, 150, 5, 'https://images.unsplash.com/photo-1627123424574-724758594e93', GETUTCDATE(), GETUTCDATE()),
    ('Gafas de Sol Polarizadas', 'Gafas de sol con protección UV400 y lentes polarizados.', 79.99, 100, 5, 'https://images.unsplash.com/photo-1572635196237-14b3f281503f', GETUTCDATE(), GETUTCDATE()),
    ('Gorra Bosko Signature', 'Gorra ajustable de algodón con logo bordado.', 24.99, 250, 5, 'https://images.unsplash.com/photo-1588850561407-ed78c282e89b', GETUTCDATE(), GETUTCDATE());
END
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    DECLARE @CustomerId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Customer' ORDER BY Id);
    DECLARE @CustomerName NVARCHAR(100) = (SELECT TOP 1 Name FROM Users WHERE Id = @CustomerId);
    DECLARE @CustomerEmail NVARCHAR(255) = (SELECT TOP 1 Email FROM Users WHERE Id = @CustomerId);
    
    IF @CustomerId IS NOT NULL AND NOT EXISTS (SELECT * FROM Orders)
    BEGIN
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Calle Mayor 123, Madrid, 28001, España', 269.97, 15.00, 284.97, 'delivered', 'credit_card', DATEADD(day, -5, GETUTCDATE()), DATEADD(day, -1, GETUTCDATE()));
        
        DECLARE @Order1 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        VALUES 
            (@Order1, 1, 'Camisa Casual Bosko', 2, 49.99, 99.98),
            (@Order1, 5, 'Pantalón Slim Fit Negro', 1, 69.99, 69.99),
            (@Order1, 13, 'Zapatillas Deportivas Bosko', 1, 89.99, 89.99);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order1, 'pending', 'Pedido recibido', DATEADD(day, -5, GETUTCDATE())),
            (@Order1, 'processing', 'En preparación en almacén', DATEADD(day, -4, GETUTCDATE())),
            (@Order1, 'processing', 'Enviado a transportista', DATEADD(day, -3, GETUTCDATE())),
            (@Order1, 'delivered', 'Entregado con éxito', DATEADD(day, -1, GETUTCDATE()));
        
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Avenida Libertad 45, Barcelona, 08001, España', 149.98, 12.00, 161.98, 'processing', 'paypal', DATEADD(day, -2, GETUTCDATE()), DATEADD(hour, -6, GETUTCDATE()));
        
        DECLARE @Order2 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        VALUES (@Order2, 10, 'Blazer Formal Azul', 1, 149.99, 149.99);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order2, 'pending', 'Pedido recibido', DATEADD(day, -2, GETUTCDATE())),
            (@Order2, 'processing', 'Verificando stock', DATEADD(hour, -6, GETUTCDATE()));
        
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Plaza España 12, Valencia, 46001, España', 214.96, 15.00, 229.96, 'pending', 'credit_card', DATEADD(hour, -3, GETUTCDATE()), DATEADD(hour, -3, GETUTCDATE()));
        
        DECLARE @Order3 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        VALUES 
            (@Order3, 6, 'Jeans Clásicos Bosko', 1, 79.99, 79.99),
            (@Order3, 11, 'Bomber Jacket Moderna', 1, 99.99, 99.99),
            (@Order3, 17, 'Cinturón de Cuero Negro', 1, 29.99, 29.99);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES (@Order3, 'pending', 'Pedido recibido, pendiente de procesamiento', DATEADD(hour, -3, GETUTCDATE()));
        
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Calle Mayor 123, Madrid, 28001, España', 189.99, 15.00, 204.99, 'delivered', 'credit_card', DATEADD(day, -10, GETUTCDATE()), DATEADD(day, -7, GETUTCDATE()));
        
        DECLARE @Order4 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        VALUES (@Order4, 9, 'Chaqueta de Cuero Premium', 1, 189.99, 189.99);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order4, 'pending', 'Pedido recibido', DATEADD(day, -10, GETUTCDATE())),
            (@Order4, 'processing', 'En preparación', DATEADD(day, -9, GETUTCDATE())),
            (@Order4, 'delivered', 'Entregado', DATEADD(day, -7, GETUTCDATE()));
        
        INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, Subtotal, Shipping, Total, Status, PaymentMethod, CreatedAt, UpdatedAt)
        VALUES (@CustomerId, @CustomerName, @CustomerEmail, 'Calle Mayor 123, Madrid, 28001, España', 119.99, 12.00, 131.99, 'cancelled', 'credit_card', DATEADD(day, -8, GETUTCDATE()), DATEADD(day, -7, GETUTCDATE()));
        
        DECLARE @Order5 INT = SCOPE_IDENTITY();
        
        INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
        VALUES (@Order5, 14, 'Zapatos Formales Oxford', 1, 119.99, 119.99);
        
        INSERT INTO OrderStatusHistory (OrderId, Status, Note, Timestamp)
        VALUES 
            (@Order5, 'pending', 'Pedido recibido', DATEADD(day, -8, GETUTCDATE())),
            (@Order5, 'cancelled', 'Cancelado por el cliente', DATEADD(day, -7, GETUTCDATE()));
    END
END
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ActivityLogs')
BEGIN
    IF NOT EXISTS (SELECT * FROM ActivityLogs)
    BEGIN
        INSERT INTO ActivityLogs (Type, Text, Timestamp)
        VALUES 
            ('order', 'Nuevo pedido #3 recibido', DATEADD(hour, -3, GETUTCDATE())),
            ('order', 'Pedido #2 actualizado a procesando', DATEADD(hour, -6, GETUTCDATE())),
            ('product', 'Producto "Camisa Casual Bosko" actualizado - stock ajustado', DATEADD(hour, -12, GETUTCDATE())),
            ('user', 'Nuevo cliente registrado en el sistema', DATEADD(hour, -18, GETUTCDATE())),
            ('product', 'Nueva colección de otoño agregada', DATEADD(day, -1, GETUTCDATE())),
            ('order', 'Pedido #1 marcado como entregado', DATEADD(day, -1, GETUTCDATE())),
            ('category', 'Categoría "Accesorios" actualizada', DATEADD(day, -2, GETUTCDATE())),
            ('product', 'Producto "Zapatillas Deportivas Bosko" en oferta', DATEADD(day, -3, GETUTCDATE())),
            ('order', 'Pedido #5 cancelado por solicitud del cliente', DATEADD(day, -7, GETUTCDATE())),
            ('product', 'Stock repuesto para productos más vendidos', DATEADD(day, -8, GETUTCDATE()));
    END
END
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    DECLARE @AdminId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Admin' ORDER BY Id);
    
    IF @AdminId IS NOT NULL AND NOT EXISTS (SELECT * FROM Notifications WHERE UserId = @AdminId)
    BEGIN
        INSERT INTO Notifications (UserId, Title, Message, Type, IsRead, CreatedAt)
        VALUES 
            (@AdminId, 'Nuevo pedido recibido', 'Pedido #3 de Cliente Test', 'order', 0, DATEADD(hour, -3, GETUTCDATE())),
            (@AdminId, 'Pedido listo para envío', 'Pedido #2 empaquetado y listo', 'order', 0, DATEADD(hour, -6, GETUTCDATE())),
            (@AdminId, 'Stock bajo', 'El producto "Sandalias de Verano" tiene stock bajo (menos de 50 unidades)', 'product', 0, DATEADD(hour, -24, GETUTCDATE())),
            (@AdminId, 'Nuevo cliente registrado', 'Se registró un nuevo cliente en la plataforma', 'user', 1, DATEADD(day, -2, GETUTCDATE())),
            (@AdminId, 'Producto más vendido', 'Las "Zapatillas Deportivas Bosko" son el producto más vendido esta semana', 'product', 1, DATEADD(day, -3, GETUTCDATE()));
    END
END
GO

SELECT 'Categories' AS Tabla, COUNT(*) AS Total FROM Categories;
SELECT 'Products' AS Tabla, COUNT(*) AS Total FROM Products;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    SELECT 'Orders' AS Tabla, COUNT(*) AS Total FROM Orders;
    SELECT 'OrderItems' AS Tabla, COUNT(*) AS Total FROM OrderItems;
    SELECT 'OrderStatusHistory' AS Tabla, COUNT(*) AS Total FROM OrderStatusHistory;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ActivityLogs')
    SELECT 'ActivityLogs' AS Tabla, COUNT(*) AS Total FROM ActivityLogs;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
    SELECT 'Notifications' AS Tabla, COUNT(*) AS Total FROM Notifications;

PRINT 'Datos insertados correctamente';
GO
