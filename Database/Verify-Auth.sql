USE BoskoDB;
GO

PRINT '============================================';
PRINT 'VERIFICACIÓN DE AUTENTICACIÓN';
PRINT '============================================';
PRINT '';

PRINT '1. VERIFICAR USUARIOS ADMIN';
PRINT '----------------------------';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    SELECT 
        Id,
        Name,
        Email,
        Role,
        IsActive,
        Provider,
        CreatedAt
    FROM Users
    WHERE Role = 'Admin'
    ORDER BY Id;
    
    DECLARE @AdminCount INT = (SELECT COUNT(*) FROM Users WHERE Role = 'Admin' AND IsActive = 1);
    
    PRINT '';
    IF @AdminCount = 0
    BEGIN
        PRINT '❌ ERROR: No hay usuarios Admin activos';
        PRINT '';
        PRINT 'Crea un usuario Admin:';
        PRINT 'INSERT INTO Users (Name, Email, PasswordHash, Role, Provider, IsActive, CreatedAt, UpdatedAt)';
        PRINT 'VALUES (''Admin User'', ''admin@test.com'', ''$2a$11$hashed_password'', ''Admin'', ''Local'', 1, GETUTCDATE(), GETUTCDATE());';
    END
    ELSE
    BEGIN
        PRINT '✅ Hay ' + CAST(@AdminCount AS VARCHAR) + ' usuario(s) Admin activo(s)';
    END
END
ELSE
BEGIN
    PRINT '❌ ERROR: Tabla Users no existe';
    PRINT 'Ejecuta: Database/Users-Authentication-Setup.sql';
END
PRINT '';

PRINT '2. VERIFICAR TODOS LOS USUARIOS';
PRINT '--------------------------------';
SELECT 
    Role AS Rol,
    COUNT(*) AS Cantidad,
    SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS Activos
FROM Users
GROUP BY Role
ORDER BY Role;
PRINT '';

PRINT '3. CREDENCIALES DE PRUEBA';
PRINT '--------------------------';
PRINT 'Email: admin@test.com';
PRINT 'Password: Admin123!';
PRINT '';
PRINT 'Endpoint de login:';
PRINT 'POST https://localhost:5006/api/auth/login';
PRINT '';
PRINT 'Body:';
PRINT '{';
PRINT '  "email": "admin@test.com",';
PRINT '  "password": "Admin123!"';
PRINT '}';
PRINT '';

PRINT '4. VERIFICAR PASSWORD HASH';
PRINT '---------------------------';
SELECT 
    Name,
    Email,
    Role,
    CASE 
        WHEN PasswordHash IS NULL THEN '❌ Sin password (OAuth only)'
        WHEN LEN(PasswordHash) < 50 THEN '⚠️ Hash muy corto (puede ser inválido)'
        ELSE '✅ Hash válido'
    END AS PasswordStatus,
    Provider
FROM Users
WHERE Role = 'Admin';
PRINT '';

PRINT '============================================';
PRINT 'PRUEBA DE AUTENTICACIÓN';
PRINT '============================================';
PRINT '';
PRINT 'PASO 1: Login';
PRINT 'POST https://localhost:5006/api/auth/login';
PRINT 'Body: {"email":"admin@test.com","password":"Admin123!"}';
PRINT '';
PRINT 'PASO 2: Copiar token de la respuesta';
PRINT '';
PRINT 'PASO 3: Usar token en requests';
PRINT 'GET https://localhost:5006/api/admin/orders';
PRINT 'Headers: Authorization: Bearer {token}';
PRINT '';
PRINT '============================================';
GO
