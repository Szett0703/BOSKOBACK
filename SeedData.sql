-- Script SQL para poblar base de datos de desarrollo con datos de prueba
-- IMPORTANTE: Este script debe ejecutarse DESPUÉS de aplicar las migraciones con 'dotnet ef database update'

USE BOSKO;
GO

-- Los Roles ya fueron insertados por el seed en la migración (Admin=1, Employee=2, Customer=3)

-- Insertar usuario Admin por defecto
-- Contraseña: Admin123 (hash BCrypt generado)
INSERT INTO Users (Name, Email, PasswordHash, Provider, RoleId, ResetToken, ResetTokenExpiry)
VALUES 
('Admin', 'admin@bosko.com', '$2a$11$xvqZr7Z8LQXGxKzN4YhPa.YqE5bZB8HFqL5o4VHxFLvJxYqXqK7oi', 'Local', 1, NULL, NULL);

-- Insertar algunos usuarios de prueba
-- Contraseña para todos: Password123
INSERT INTO Users (Name, Email, PasswordHash, Provider, RoleId, ResetToken, ResetTokenExpiry)
VALUES 
('Juan Employee', 'employee@bosko.com', '$2a$11$Qy4VZ8L5v0qL8Y9Q0gqXN.E6H4v7k8Lz0VYqXqK7oiWqXqK7oiWqX', 'Local', 2, NULL, NULL),
('Maria Customer', 'customer@bosko.com', '$2a$11$Qy4VZ8L5v0qL8Y9Q0gqXN.E6H4v7k8Lz0VYqXqK7oiWqXqK7oiWqX', 'Local', 3, NULL, NULL),
('Pedro García', 'pedro@example.com', '$2a$11$Qy4VZ8L5v0qL8Y9Q0gqXN.E6H4v7k8Lz0VYqXqK7oiWqXqK7oiWqX', 'Local', 3, NULL, NULL);

-- Insertar categorías de ejemplo
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (Id, Name, Description, ImageUrl)
VALUES 
(1, 'Hombre', 'Ropa y accesorios para hombre', 'https://images.unsplash.com/photo-1490114538077-0a7f8cb49891?w=400'),
(2, 'Mujer', 'Ropa y accesorios para mujer', 'https://images.unsplash.com/photo-1483985988355-763728e1935b?w=400'),
(3, 'Niños', 'Ropa para niños', 'https://images.unsplash.com/photo-1503919545889-aef636e10ad4?w=400'),
(4, 'Accesorios', 'Complementos de moda', 'https://images.unsplash.com/photo-1523779917675-b6ed3a42a561?w=400'),
(5, 'Calzado', 'Zapatos y zapatillas', 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400'),
(6, 'Deportes', 'Ropa y accesorios deportivos', 'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400');
SET IDENTITY_INSERT Categories OFF;

-- Insertar productos de ejemplo
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (Id, Name, Description, Price, ImageUrl, CategoryId)
VALUES 
(1, 'Chaqueta de Invierno', 'Chaqueta térmica para clima frío', 129.99, 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400', 1),
(2, 'Vestido de Verano', 'Vestido elegante para ocasiones especiales', 89.99, 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=400', 2),
(3, 'Zapatillas Deportivas', 'Cómodas para uso diario', 149.99, 'https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?w=400', 5),
(4, 'Reloj de Lujo', 'Elegancia atemporal', 299.99, 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400', 4),
(5, 'Camiseta Infantil', 'Camiseta de algodón para niños', 24.99, 'https://images.unsplash.com/photo-1503944583220-79d8926ad5e2?w=400', 3),
(6, 'Zapatillas Running', 'Alto rendimiento deportivo', 119.99, 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400', 6),
(7, 'Bolso de Cuero', 'Bolso elegante para cualquier ocasión', 179.99, 'https://images.unsplash.com/photo-1564422170194-896b89110ef8?w=400', 4),
(8, 'Jeans Clásicos', 'Jeans de corte clásico', 79.99, 'https://images.unsplash.com/photo-1542272604-787c3835535d?w=400', 1),
(9, 'Blusa Elegante', 'Blusa para eventos formales', 59.99, 'https://images.unsplash.com/photo-1564859228273-274232fdb516?w=400', 2),
(10, 'Sudadera Deportiva', 'Perfecta para entrenar', 69.99, 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=400', 6);
SET IDENTITY_INSERT Products OFF;

-- Insertar algunas direcciones de ejemplo para customer@bosko.com (ID 3)
INSERT INTO Addresses (UserId, Street, City, State, PostalCode, Country)
VALUES 
(3, 'Calle Principal 123', 'Madrid', 'Madrid', '28001', 'España'),
(3, 'Avenida Central 456', 'Barcelona', 'Barcelona', '08001', 'España');

-- Insertar direcciones para pedro@example.com (ID 4)
INSERT INTO Addresses (UserId, Street, City, State, PostalCode, Country)
VALUES 
(4, 'Calle Secundaria 789', 'Valencia', 'Valencia', '46001', 'España');

-- Insertar algunos pedidos de ejemplo
-- Pedido para Maria Customer (UserId=3)
DECLARE @OrderId1 INT;
INSERT INTO Orders (UserId, OrderDate, Total, Status)
VALUES (3, GETDATE(), 339.97, 'Completed');
SET @OrderId1 = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES 
(@OrderId1, 1, 1, 129.99),
(@OrderId1, 3, 1, 149.99),
(@OrderId1, 5, 2, 24.99);

-- Pedido para Pedro (UserId=4)
DECLARE @OrderId2 INT;
INSERT INTO Orders (UserId, OrderDate, Total, Status)
VALUES (4, DATEADD(day, -2, GETDATE()), 229.98, 'Shipped');
SET @OrderId2 = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES 
(@OrderId2, 6, 1, 119.99),
(@OrderId2, 8, 1, 79.99);

-- Pedido pendiente
DECLARE @OrderId3 INT;
INSERT INTO Orders (UserId, OrderDate, Total, Status)
VALUES (3, DATEADD(day, -1, GETDATE()), 89.99, 'Pending');
SET @OrderId3 = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES 
(@OrderId3, 2, 1, 89.99);

-- Insertar algunas reseñas
INSERT INTO Reviews (ProductId, UserId, Rating, Comment, ReviewDate)
VALUES 
(1, 3, 5, 'Excelente chaqueta, muy cálida y cómoda', DATEADD(day, -5, GETDATE())),
(3, 4, 4, 'Muy buenas zapatillas, muy cómodas', DATEADD(day, -3, GETDATE())),
(6, 3, 5, 'Perfectas para correr, las recomiendo', DATEADD(day, -1, GETDATE())),
(2, 4, 4, 'Bonito vestido, tela de calidad', DATEADD(day, -7, GETDATE()));

-- Insertar items en wishlist
INSERT INTO WishlistItems (UserId, ProductId)
VALUES 
(3, 4),  -- Maria quiere el reloj
(3, 7),  -- Maria quiere el bolso
(4, 1),  -- Pedro quiere la chaqueta
(4, 9);  -- Pedro quiere la blusa (regalo)

PRINT 'Datos de prueba insertados correctamente.';
PRINT '';
PRINT 'CREDENCIALES DE PRUEBA:';
PRINT '========================';
PRINT 'Admin:';
PRINT '  Email: admin@bosko.com';
PRINT '  Password: Admin123';
PRINT '';
PRINT 'Employee:';
PRINT '  Email: employee@bosko.com';
PRINT '  Password: Password123';
PRINT '';
PRINT 'Customer 1:';
PRINT '  Email: customer@bosko.com';
PRINT '  Password: Password123';
PRINT '';
PRINT 'Customer 2:';
PRINT '  Email: pedro@example.com';
PRINT '  Password: Password123';
GO
