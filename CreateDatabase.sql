-- Script para crear la base de datos BOSKO manualmente
-- Alternativa a usar Entity Framework migrations

USE master;
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BOSKO')
BEGIN
    CREATE DATABASE BOSKO;
END
GO

USE BOSKO;
GO

-- Tabla Users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(255) NULL,
        [Provider] NVARCHAR(20) NULL DEFAULT 'Local',
        [ResetToken] NVARCHAR(255) NULL,
        [ResetTokenExpiry] DATETIME2 NULL
    );
END
GO

-- Tabla Categories
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Categories] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [ImageUrl] NVARCHAR(500) NOT NULL
    );
END
GO

-- Tabla Products
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Products] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(1000) NOT NULL,
        [Price] DECIMAL(18,2) NOT NULL,
        [ImageUrl] NVARCHAR(500) NOT NULL,
        [CategoryId] INT NOT NULL,
        CONSTRAINT FK_Products_Categories FOREIGN KEY ([CategoryId]) 
            REFERENCES [Categories]([Id]) ON DELETE CASCADE
    );
END
GO

-- Tabla Orders
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Orders] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrderDate] DATETIME2 NOT NULL,
        [Total] DECIMAL(18,2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [UserId] INT NOT NULL,
        CONSTRAINT FK_Orders_Users FOREIGN KEY ([UserId]) 
            REFERENCES [Users]([Id]) ON DELETE CASCADE
    );
END
GO

-- Tabla OrderItems
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [OrderItems] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrderId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY ([OrderId]) 
            REFERENCES [Orders]([Id]) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Products FOREIGN KEY ([ProductId]) 
            REFERENCES [Products]([Id]) ON DELETE NO ACTION
    );
END
GO

-- Insertar datos de categorías
IF NOT EXISTS (SELECT * FROM [Categories])
BEGIN
    SET IDENTITY_INSERT [Categories] ON;
    
    INSERT INTO [Categories] ([Id], [Name], [Description], [ImageUrl]) VALUES
    (1, N'Men', N'Fashion for Men', N'https://images.unsplash.com/photo-1490114538077-0a7f8cb49891?w=400&h=300&fit=crop'),
    (2, N'Women', N'Fashion for Women', N'https://images.unsplash.com/photo-1483985988355-763728e1935b?w=400&h=300&fit=crop'),
    (3, N'Kids', N'Fashion for Kids', N'https://images.unsplash.com/photo-1503919545889-aef636e10ad4?w=400&h=300&fit=crop'),
    (4, N'Accessories', N'Fashion Accessories', N'https://images.unsplash.com/photo-1523779917675-b6ed3a42a561?w=400&h=300&fit=crop'),
    (5, N'Shoes', N'Footwear Collection', N'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=300&fit=crop'),
    (6, N'Sports', N'Sports & Active Wear', N'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400&h=300&fit=crop');
    
    SET IDENTITY_INSERT [Categories] OFF;
END
GO

-- Insertar datos de productos
IF NOT EXISTS (SELECT * FROM [Products])
BEGIN
    SET IDENTITY_INSERT [Products] ON;
    
    INSERT INTO [Products] ([Id], [Name], [Description], [Price], [ImageUrl], [CategoryId]) VALUES
    (1, N'Classic Winter Jacket', N'Stay warm in style with our premium winter jacket', 129.99, N'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400&h=500&fit=crop', 1),
    (2, N'Elegant Summer Dress', N'Perfect for any summer occasion', 89.99, N'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=400&h=500&fit=crop', 2),
    (3, N'Designer Sneakers', N'Comfortable and stylish everyday wear', 149.99, N'https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?w=400&h=500&fit=crop', 5),
    (4, N'Luxury Watch', N'Timeless elegance for your wrist', 299.99, N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=500&fit=crop', 4),
    (5, N'Kids Casual T-Shirt', N'Comfortable cotton t-shirt for active kids', 24.99, N'https://images.unsplash.com/photo-1503944583220-79d8926ad5e2?w=400&h=500&fit=crop', 3),
    (6, N'Sports Running Shoes', N'High-performance running shoes', 119.99, N'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=500&fit=crop', 6),
    (7, N'Leather Handbag', N'Elegant leather handbag for any occasion', 179.99, N'https://images.unsplash.com/photo-1564422170194-896b89110ef8?w=400&h=500&fit=crop', 4),
    (8, N'Denim Jeans', N'Classic fit denim jeans', 79.99, N'https://images.unsplash.com/photo-1542272604-787c3835535d?w=400&h=500&fit=crop', 1);
    
    SET IDENTITY_INSERT [Products] OFF;
END
GO

PRINT 'Base de datos BOSKO creada exitosamente con datos iniciales';
GO
