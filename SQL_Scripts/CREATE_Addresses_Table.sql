-- Script para crear tabla Addresses
USE BoskoDB;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Addresses](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [UserId] [int] NOT NULL,
        [Label] [nvarchar](100) NULL,
        [Street] [nvarchar](200) NOT NULL,
        [City] [nvarchar](100) NOT NULL,
        [State] [nvarchar](100) NULL,
        [PostalCode] [nvarchar](20) NOT NULL,
        [Country] [nvarchar](100) NOT NULL,
        [Phone] [nvarchar](20) NULL,
        [IsDefault] [bit] NOT NULL DEFAULT 0,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Addresses_Users] FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
    ) ON [PRIMARY];

    CREATE INDEX [IX_Addresses_UserId] ON [dbo].[Addresses]([UserId]);
    CREATE INDEX [IX_Addresses_IsDefault] ON [dbo].[Addresses]([UserId], [IsDefault]);

    PRINT 'Tabla Addresses creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Addresses ya existe';
END
GO
