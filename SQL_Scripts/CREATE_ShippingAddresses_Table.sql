-- Script para crear tabla ShippingAddresses
USE BoskoDB;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShippingAddresses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ShippingAddresses](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [OrderId] [int] NOT NULL,
        [FullName] [nvarchar](100) NOT NULL,
        [Phone] [nvarchar](20) NOT NULL,
        [Street] [nvarchar](200) NOT NULL,
        [City] [nvarchar](100) NOT NULL,
        [State] [nvarchar](100) NOT NULL,
        [PostalCode] [nvarchar](20) NOT NULL,
        [Country] [nvarchar](100) NOT NULL,
        CONSTRAINT [PK_ShippingAddresses] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ShippingAddresses_Orders] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE
    ) ON [PRIMARY];

    -- Crear índice en OrderId para mejorar performance
    CREATE NONCLUSTERED INDEX [IX_ShippingAddresses_OrderId] ON [dbo].[ShippingAddresses]
    (
        [OrderId] ASC
    );

    PRINT 'Tabla ShippingAddresses creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla ShippingAddresses ya existe';
END
GO
