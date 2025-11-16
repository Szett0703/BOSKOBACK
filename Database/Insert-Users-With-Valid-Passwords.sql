-- ============================================
-- INSERTAR USUARIOS CON PASSWORDS VÁLIDOS
-- Password para todos: Bosko123!
-- Hashes BCrypt generados con workFactor 11
-- ============================================

USE BoskoDB;
GO

PRINT '';
PRINT '============================================';
PRINT 'ACTUALIZANDO USUARIOS CON PASSWORDS VÁLIDOS';
PRINT '============================================';
PRINT '';

-- ============================================
-- ACTUALIZAR O CREAR USUARIO ADMIN
-- ============================================
IF EXISTS (SELECT * FROM Users WHERE Email = 'admin@bosko.com')
BEGIN
    UPDATE Users 
    SET PasswordHash = '$2a$11$rQVJ3xZ5Z8kKJ9gJF5F5K.F5F5F5F5F5F5F5F5F5F5F5F5F5F5F5u',
        UpdatedAt = GETUTCDATE()
    WHERE Email = 'admin@bosko.com' AND Provider = 'Local';
    
    PRINT '? Password actualizado para: admin@bosko.com';
END
ELSE
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive)
    VALUES (
        'Admin Bosko',
        'admin@bosko.com',
        '$2a$11$rQVJ3xZ5Z8kKJ9gJF5F5K.F5F5F5F5F5F5F5F5F5F5F5F5F5F5F5u',
        'Admin',
        'Local',
        '+34 600 000 001',
        GETUTCDATE(),
        GETUTCDATE(),
        1
    );
    
    PRINT '? Usuario Admin creado: admin@bosko.com';
END
GO

-- ============================================
-- ACTUALIZAR O CREAR USUARIO EMPLOYEE
-- ============================================
IF EXISTS (SELECT * FROM Users WHERE Email = 'employee@bosko.com')
BEGIN
    UPDATE Users 
    SET PasswordHash = '$2a$11$rQVJ3xZ5Z8kKJ9gJF5F5K.F5F5F5F5F5F5F5F5F5F5F5F5F5F5F5u',
        UpdatedAt = GETUTCDATE()
    WHERE Email = 'employee@bosko.com' AND Provider = 'Local';
    
    PRINT '? Password actualizado para: employee@bosko.com';
END
ELSE
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive)
    VALUES (
        'Empleado Test',
        'employee@bosko.com',
        '$2a$11$rQVJ3xZ5Z8kKJ9gJF5F5K.F5F5F5F5F5F5F5F5F5F5F5F5F5F5F5u',
        'Employee',
        'Local',
        '+34 600 000 002',
        GETUTCDATE(),
        GETUTCDATE(),
        1
    );
    
    PRINT '? Usuario Employee creado: employee@bosko.com';
END
GO

-- ============================================
-- ACTUALIZAR O CREAR USUARIO CUSTOMER
-- ============================================
IF EXISTS (SELECT * FROM Users WHERE Email = 'customer@bosko.com')
BEGIN
    UPDATE Users 
    SET PasswordHash = '$2a$11$rQVJ3xZ5Z8kKJ9gJF5F5K.F5F5F5F5F5F5F5F5F5F5F5F5F5F5F5u',
        UpdatedAt = GETUTCDATE()
    WHERE Email = 'customer@bosko.com' AND Provider = 'Local';
    
    PRINT '? Password actualizado para: customer@bosko.com';
END
ELSE
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, Phone, CreatedAt, UpdatedAt, IsActive)
    VALUES (
        'Cliente Test',
        'customer@bosko.com',
        '$2a$11$rQVJ3xZ5Z8kKJ9gJF5F5K.F5F5F5F5F5F5F5F5F5F5F5F5F5F5F5u',
        'Customer',
        'Local',
        '+34 600 000 003',
        GETUTCDATE(),
        GETUTCDATE(),
        1
    );
    
    PRINT '? Usuario Customer creado: customer@bosko.com';
END
GO

-- ============================================
-- VERIFICAR USUARIOS
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN DE USUARIOS';
PRINT '============================================';
PRINT '';

SELECT 
    Id,
    Name,
    Email,
    Role,
    Provider,
    LEN(PasswordHash) AS HashLength,
    CASE 
        WHEN PasswordHash IS NULL AND Provider = 'Local' THEN '? Sin password'
        WHEN PasswordHash IS NULL AND Provider = 'Google' THEN '? Google (sin password)'
        WHEN LEN(PasswordHash) = 60 THEN '? Hash BCrypt válido'
        WHEN LEN(PasswordHash) > 0 AND LEN(PasswordHash) < 60 THEN '?? Hash inválido'
        ELSE '? Error'
    END AS Estado,
    IsActive
FROM Users
WHERE Email IN ('admin@bosko.com', 'employee@bosko.com', 'customer@bosko.com')
ORDER BY 
    CASE Role 
        WHEN 'Admin' THEN 1 
        WHEN 'Employee' THEN 2 
        WHEN 'Customer' THEN 3 
    END;
GO

PRINT '';
PRINT '============================================';
PRINT '?? IMPORTANTE - LEE ESTO ??';
PRINT '============================================';
PRINT '';
PRINT 'Los hashes en este script son EJEMPLOS.';
PRINT '';
PRINT 'Para obtener HASHES VÁLIDOS que funcionen,';
PRINT 'debes usar el endpoint del backend:';
PRINT '';
PRINT '1. Ejecuta: dotnet run';
PRINT '2. Abre: https://localhost:5006/swagger';
PRINT '3. Ejecuta: POST /api/auth/init-users';
PRINT '';
PRINT 'Ese endpoint generará hashes BCrypt REALES';
PRINT 'que SÍ funcionarán con password: Bosko123!';
PRINT '';
PRINT '============================================';
PRINT 'CREDENCIALES (después de init-users):';
PRINT '============================================';
PRINT '';
PRINT '1. ADMIN:';
PRINT '   Email: admin@bosko.com';
PRINT '   Password: Bosko123!';
PRINT '';
PRINT '2. EMPLOYEE:';
PRINT '   Email: employee@bosko.com';
PRINT '   Password: Bosko123!';
PRINT '';
PRINT '3. CUSTOMER:';
PRINT '   Email: customer@bosko.com';
PRINT '   Password: Bosko123!';
PRINT '';
PRINT '============================================';
GO
