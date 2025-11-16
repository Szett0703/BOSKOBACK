-- ============================================
-- SCRIPT DE CORRECCIÓN: Agregar campo IsUsed
-- Tabla: PasswordResetTokens
-- Fecha: 16 de Noviembre 2025
-- ============================================

USE [BoskoDB];
GO

PRINT '============================================';
PRINT 'AGREGANDO CAMPO IsUsed A PasswordResetTokens';
PRINT '============================================';
PRINT '';

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PasswordResetTokens' 
    AND COLUMN_NAME = 'IsUsed'
)
BEGIN
    -- Agregar la columna
    ALTER TABLE [dbo].[PasswordResetTokens]
    ADD [IsUsed] BIT NOT NULL DEFAULT 0;
    
    PRINT '✅ Columna IsUsed agregada exitosamente';
    PRINT '   - Tipo: BIT (boolean)';
    PRINT '   - Default: 0 (false)';
    PRINT '   - NOT NULL';
END
ELSE
BEGIN
    PRINT '⚠️ La columna IsUsed ya existe';
    PRINT '   - No se requieren cambios';
END

PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN DE ESTRUCTURA';
PRINT '============================================';
PRINT '';

-- Mostrar la estructura actual de PasswordResetTokens
SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS Tipo,
    CHARACTER_MAXIMUM_LENGTH AS Longitud,
    IS_NULLABLE AS Nullable,
    COLUMN_DEFAULT AS ValorDefault
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PasswordResetTokens'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '============================================';
PRINT '✅ ACTUALIZACIÓN COMPLETADA';
PRINT '============================================';
PRINT '';
PRINT '📝 Próximos pasos:';
PRINT '1. Reinicia el backend: dotnet run';
PRINT '2. El error de compilación debería desaparecer';
PRINT '3. La funcionalidad de reset password funcionará correctamente';
PRINT '';
GO
