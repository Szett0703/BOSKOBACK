-- ============================================
-- SCRIPTS SQL PENDIENTES DE EJECUTAR
-- ============================================
-- Fecha: 19 de Noviembre 2025
-- Base de datos: BoskoDB
-- Descripción: Scripts necesarios para completar el sistema de perfil de usuario y direcciones

USE BoskoDB;
GO

-- ============================================
-- SCRIPT 1: Verificar tablas existentes
-- ============================================
PRINT '============================================';
PRINT 'Verificando tablas existentes...';
PRINT '============================================';

-- Verificar UserPreferences
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserPreferences')
    PRINT '✅ UserPreferences - YA EXISTE';
ELSE
    PRINT '❌ UserPreferences - NO EXISTE (ejecutar script)';

-- Verificar columna AvatarUrl
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'AvatarUrl')
    PRINT '✅ Users.AvatarUrl - YA EXISTE';
ELSE
    PRINT '❌ Users.AvatarUrl - NO EXISTE (ejecutar script)';

-- Verificar Addresses
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Addresses')
    PRINT '✅ Addresses - YA EXISTE';
ELSE
    PRINT '❌ Addresses - NO EXISTE (ejecutar script)';

-- Verificar ShippingAddresses (LA MÁS IMPORTANTE)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShippingAddresses')
    PRINT '✅ ShippingAddresses - YA EXISTE';
ELSE
    PRINT '❌ ShippingAddresses - NO EXISTE (CRÍTICO - ejecutar ahora)';

GO

-- ============================================
-- SCRIPT 2: Crear tabla ShippingAddresses
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'Creando tabla ShippingAddresses...';
PRINT '============================================';

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
        CONSTRAINT [FK_ShippingAddresses_Orders] FOREIGN KEY([OrderId]) 
            REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE
    ) ON [PRIMARY];

    -- Índice para búsquedas rápidas por OrderId
    CREATE NONCLUSTERED INDEX [IX_ShippingAddresses_OrderId] 
    ON [dbo].[ShippingAddresses]([OrderId] ASC);

    PRINT '✅ Tabla ShippingAddresses creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ La tabla ShippingAddresses ya existe';
END
GO

-- ============================================
-- SCRIPT 3: Insertar preferencias por defecto
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'Insertando preferencias por defecto...';
PRINT '============================================';

-- Solo si la tabla UserPreferences existe
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserPreferences')
BEGIN
    INSERT INTO UserPreferences (UserId, Notifications, Newsletter, Language, CreatedAt)
    SELECT 
        u.Id,
        1, -- Notifications habilitadas
        1, -- Newsletter habilitado
        'es', -- Idioma español
        GETUTCDATE()
    FROM Users u
    WHERE NOT EXISTS (
        SELECT 1 FROM UserPreferences up WHERE up.UserId = u.Id
    );

    DECLARE @InsertedCount INT = @@ROWCOUNT;
    PRINT '✅ Preferencias creadas para ' + CAST(@InsertedCount AS VARCHAR(10)) + ' usuario(s)';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla UserPreferences no existe, saltando este paso';
END
GO

-- ============================================
-- SCRIPT 4: Verificación final
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN FINAL';
PRINT '============================================';

DECLARE @TablesOK INT = 0;
DECLARE @TablesTotal INT = 4;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserPreferences')
    SET @TablesOK = @TablesOK + 1;
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'AvatarUrl')
    SET @TablesOK = @TablesOK + 1;
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Addresses')
    SET @TablesOK = @TablesOK + 1;
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShippingAddresses')
    SET @TablesOK = @TablesOK + 1;

PRINT 'Tablas configuradas: ' + CAST(@TablesOK AS VARCHAR(10)) + ' de ' + CAST(@TablesTotal AS VARCHAR(10));

IF @TablesOK = @TablesTotal
BEGIN
    PRINT '';
    PRINT '✅✅✅ ¡TODOS LOS SCRIPTS EJECUTADOS CORRECTAMENTE! ✅✅✅';
    PRINT '';
    PRINT 'Próximos pasos:';
    PRINT '1. Reiniciar el backend: dotnet run';
    PRINT '2. Probar endpoint: GET /api/admin/orders/22';
    PRINT '3. Probar endpoint: GET /api/addresses';
    PRINT '4. Probar endpoint: GET /api/users/me';
END
ELSE
BEGIN
    PRINT '';
    PRINT '⚠️⚠️⚠️ FALTAN SCRIPTS POR EJECUTAR ⚠️⚠️⚠️';
    PRINT '';
    PRINT 'Por favor revisa los mensajes anteriores';
END

GO

-- ============================================
-- QUERIES DE VERIFICACIÓN ADICIONALES
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'INFORMACIÓN DE LAS TABLAS';
PRINT '============================================';

-- Contar registros
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserPreferences')
BEGIN
    DECLARE @UserPrefsCount INT = (SELECT COUNT(*) FROM UserPreferences);
    PRINT 'UserPreferences: ' + CAST(@UserPrefsCount AS VARCHAR(10)) + ' registro(s)';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Addresses')
BEGIN
    DECLARE @AddressesCount INT = (SELECT COUNT(*) FROM Addresses);
    PRINT 'Addresses: ' + CAST(@AddressesCount AS VARCHAR(10)) + ' registro(s)';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShippingAddresses')
BEGIN
    DECLARE @ShippingCount INT = (SELECT COUNT(*) FROM ShippingAddresses);
    PRINT 'ShippingAddresses: ' + CAST(@ShippingCount AS VARCHAR(10)) + ' registro(s)';
END

PRINT '';
PRINT '============================================';
PRINT 'SCRIPT COMPLETADO';
PRINT '============================================';
