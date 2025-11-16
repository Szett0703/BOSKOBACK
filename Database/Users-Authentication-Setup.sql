-- ============================================
-- SCRIPT DE AUTENTICACIÓN - BOSKO E-COMMERCE
-- Sistema de Usuarios, Roles y Autenticación
-- ============================================

USE BoskoDB;
GO

-- ============================================
-- TABLA: Users
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NULL, -- Null para usuarios de Google
        Phone NVARCHAR(20) NULL,
        Role NVARCHAR(20) NOT NULL DEFAULT 'Customer',
        Provider NVARCHAR(20) NOT NULL DEFAULT 'Local',
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsActive BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Employee', 'Customer')),
        CONSTRAINT CK_Users_Provider CHECK (Provider IN ('Local', 'Google'))
    );
    
    PRINT 'Tabla Users creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Users ya existe';
END
GO

-- ============================================
-- ÍNDICES PARA MEJOR PERFORMANCE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
    PRINT 'Índice IX_Users_Email creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_Role ON Users(Role);
    PRINT 'Índice IX_Users_Role creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Provider' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_Provider ON Users(Provider);
    PRINT 'Índice IX_Users_Provider creado';
END
GO

-- ============================================
-- TABLA: PasswordResetTokens (para recuperación)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PasswordResetTokens')
BEGIN
    CREATE TABLE PasswordResetTokens (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        Token NVARCHAR(500) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        IsUsed BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId) 
            REFERENCES Users(Id) ON DELETE CASCADE
    );
    
    PRINT 'Tabla PasswordResetTokens creada exitosamente';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_Token' AND object_id = OBJECT_ID('PasswordResetTokens'))
BEGIN
    CREATE INDEX IX_PasswordResetTokens_Token ON PasswordResetTokens(Token);
    PRINT 'Índice IX_PasswordResetTokens_Token creado';
END
GO

-- ============================================
-- LIMPIAR USUARIOS EXISTENTES (OPCIONAL)
-- Descomenta estas líneas si quieres recrear los usuarios
-- ============================================
-- DELETE FROM PasswordResetTokens;
-- DELETE FROM Users WHERE Provider = 'Local';
-- GO

-- ============================================
-- CREAR/ACTUALIZAR USUARIOS CON PASSWORDS VÁLIDOS
-- Password: Bosko123! (para todos)
-- ============================================

-- IMPORTANTE: Este script usa el endpoint /api/auth/init-users
-- para generar hashes BCrypt válidos automáticamente.
-- Si prefieres usar SQL directo, descomenta la siguiente sección:

-- ============================================
-- OPCIÓN A: USAR ENDPOINT (RECOMENDADO)
-- ============================================
-- 1. Ejecuta el backend: dotnet run
-- 2. En Swagger: POST /api/auth/init-users
-- 3. Esto generará hashes BCrypt válidos automáticamente

-- ============================================
-- OPCIÓN B: INSERTAR CON SQL DIRECTO
-- ============================================
-- Nota: Estos usuarios se crean SIN password (NULL)
-- Luego debes ejecutar /api/auth/init-users para generar los hashes

-- Admin
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@bosko.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive) VALUES 
    ('Admin Bosko', 'admin@bosko.com', NULL, 'Admin', 'Local', '+34 600 000 001', GETUTCDATE(), GETUTCDATE(), 1);
    PRINT '? Usuario Admin creado (requiere /api/auth/init-users)';
END
ELSE
BEGIN
    PRINT '?? Usuario Admin ya existe';
END
GO

-- Employee
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'employee@bosko.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive) VALUES 
    ('Empleado Test', 'employee@bosko.com', NULL, 'Employee', 'Local', '+34 600 000 002', GETUTCDATE(), GETUTCDATE(), 1);
    PRINT '? Usuario Employee creado (requiere /api/auth/init-users)';
END
ELSE
BEGIN
    PRINT '?? Usuario Employee ya existe';
END
GO

-- Customer
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'customer@bosko.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive) VALUES 
    ('Cliente Test', 'customer@bosko.com', NULL, 'Customer', 'Local', '+34 600 000 003', GETUTCDATE(), GETUTCDATE(), 1);
    PRINT '? Usuario Customer creado (requiere /api/auth/init-users)';
END
ELSE
BEGIN
    PRINT '?? Usuario Customer ya existe';
END
GO

-- Usuario de Google (sin password)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'google.test@gmail.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive) VALUES 
    ('Usuario Google', 'google.test@gmail.com', NULL, 'Customer', 'Google', NULL, GETUTCDATE(), GETUTCDATE(), 1);
    PRINT '? Usuario Google Test creado';
END
ELSE
BEGIN
    PRINT '?? Usuario Google ya existe';
END
GO

-- ============================================
-- VERIFICAR USUARIOS CREADOS
-- ============================================
SELECT 
    'Users' AS Tabla,
    COUNT(*) AS Total,
    COUNT(CASE WHEN Role = 'Admin' THEN 1 END) AS Admins,
    COUNT(CASE WHEN Role = 'Employee' THEN 1 END) AS Employees,
    COUNT(CASE WHEN Role = 'Customer' THEN 1 END) AS Customers,
    COUNT(CASE WHEN Provider = 'Google' THEN 1 END) AS GoogleUsers,
    COUNT(CASE WHEN PasswordHash IS NULL AND Provider = 'Local' THEN 1 END) AS SinPassword
FROM Users;
GO

-- Mostrar todos los usuarios creados
SELECT 
    Id,
    Name,
    Email,
    Role,
    Provider,
    Phone,
    CASE 
        WHEN PasswordHash IS NULL AND Provider = 'Local' THEN '? SIN PASSWORD - Ejecutar /api/auth/init-users'
        WHEN PasswordHash IS NULL AND Provider = 'Google' THEN '? Google (sin password)'
        WHEN LEN(PasswordHash) >= 60 THEN '? Password configurado'
        WHEN LEN(PasswordHash) > 0 AND LEN(PasswordHash) < 60 THEN '?? Hash inválido'
        ELSE '? Sin configurar'
    END AS PasswordStatus,
    IsActive,
    CreatedAt
FROM Users
ORDER BY Role, Id;
GO

PRINT '';
PRINT '============================================';
PRINT 'USUARIOS CREADOS - REQUIEREN INICIALIZACIÓN';
PRINT '============================================';
PRINT '';
PRINT '?? IMPORTANTE: Los usuarios se crearon SIN passwords';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '----------------';
PRINT '1. Ejecuta el backend:';
PRINT '   > dotnet run';
PRINT '';
PRINT '2. Abre Swagger:';
PRINT '   https://localhost:5006/swagger';
PRINT '';
PRINT '3. Ejecuta el endpoint de inicialización:';
PRINT '   POST /api/auth/init-users';
PRINT '';
PRINT '4. Esto generará hashes BCrypt válidos para:';
PRINT '   - admin@bosko.com';
PRINT '   - employee@bosko.com';
PRINT '   - customer@bosko.com';
PRINT '';
PRINT '5. Password para todos: Bosko123!';
PRINT '';
PRINT '============================================';
PRINT 'ALTERNATIVA: Si prefieres actualizar via SQL:';
PRINT '';
PRINT 'Ejecuta este comando DESPUÉS de obtener un hash';
PRINT 'válido desde el endpoint /api/auth/register:';
PRINT '';
PRINT 'UPDATE Users SET PasswordHash = ''HASH_AQUI''';
PRINT 'WHERE Email = ''admin@bosko.com'';';
PRINT '';
PRINT '============================================';
GO
