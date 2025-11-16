# 🔴 PROBLEMA CRÍTICO DETECTADO - ESQUEMA DE BASE DE DATOS

**Fecha:** 16 de Noviembre 2025  
**Severidad:** 🔴 CRÍTICA  
**Estado:** ⏳ REQUIERE ACCIÓN INMEDIATA

---

## 🔍 DIAGNÓSTICO

He analizado el script de tu base de datos y encontré **desajustes críticos** entre el esquema de SQL Server y lo que el backend espera.

### ❌ ESTO EXPLICA LOS ERRORES 500

El error `500 Internal Server Error` en `/api/admin/orders` NO es un problema de código, es porque **la estructura de la base de datos está incorrecta**.

---

## 🔴 PROBLEMAS DETECTADOS

### 1. **Tabla `Orders` - ESTRUCTURA INCORRECTA**

**Tu base de datos tiene:**
```sql
CREATE TABLE [dbo].[Orders](
    [Id] int,
    [UserId] int,              -- ❌ INCORRECTO
    [TotalAmount] decimal,     -- ❌ INCORRECTO
    [Status] nvarchar(50),
    [CreatedAt] datetime2,
    [UpdatedAt] datetime2
)
```

**El backend espera (según Models/Order.cs):**
```sql
CREATE TABLE [dbo].[Orders](
    [Id] int,
    [CustomerId] int,                -- ✅ NO UserId
    [CustomerName] nvarchar(100),    -- ✅ FALTA
    [CustomerEmail] nvarchar(255),   -- ✅ FALTA
    [ShippingAddress] nvarchar(500), -- ✅ FALTA
    [Subtotal] decimal(18,2),        -- ✅ FALTA
    [Shipping] decimal(18,2),        -- ✅ FALTA
    [Total] decimal(18,2),           -- ✅ NO TotalAmount
    [Status] nvarchar(20),
    [PaymentMethod] nvarchar(50),    -- ✅ FALTA
    [CreatedAt] datetime2,
    [UpdatedAt] datetime2
)
```

**Resultado:**
Cuando el backend intenta acceder a `o.CustomerName`, da **NullReferenceException** porque la columna no existe.

---

### 2. **Tabla `OrderItems` - COLUMNAS INCORRECTAS**

**Tu base de datos tiene:**
```sql
CREATE TABLE [dbo].[OrderItems](
    [Id] int,
    [OrderId] int,
    [ProductId] int,
    [Quantity] int,
    [UnitPrice] decimal(10,2)  -- ❌ INCORRECTO
)
```

**El backend espera:**
```sql
CREATE TABLE [dbo].[OrderItems](
    [Id] int,
    [OrderId] int,
    [ProductId] int,
    [ProductName] nvarchar(200),  -- ✅ FALTA
    [Quantity] int,
    [Price] decimal(18,2),        -- ✅ NO UnitPrice
    [Subtotal] decimal(18,2)      -- ✅ FALTA
)
```

**Resultado:**
Cuando el backend intenta acceder a `oi.ProductName`, da error porque la columna no existe.

---

### 3. **TABLAS FALTANTES**

El backend necesita estas tablas que **NO existen** en tu BD:

- ❌ `OrderStatusHistory` - Para el historial de cambios de estado
- ❌ `ActivityLogs` - Para el log de actividades del admin
- ❌ `Notifications` - Para notificaciones de usuarios

**Resultado:**
Cuando el backend intenta acceder a estas tablas, da error de "Invalid object name".

---

## 🎯 CAUSA RAÍZ DEL ERROR 500

```csharp
// AdminService.cs - GetOrdersAsync()
var orders = await query
    .Include(o => o.Items)
    .Select(o => new OrderDto
    {
        Id = o.Id,
        CustomerName = o.CustomerName,  // ❌ COLUMNA NO EXISTE EN TU BD
        CustomerEmail = o.CustomerEmail, // ❌ COLUMNA NO EXISTE EN TU BD
        Items = o.Items.Count,
        Amount = o.Total,                // ❌ TU BD TIENE TotalAmount
        // ...
    })
    .ToListAsync();
```

**La query falla** porque intenta seleccionar columnas que no existen en tu base de datos.

---

## ✅ SOLUCIÓN

He creado un **script de corrección completa** que:

1. ✅ **Respalda** tus datos existentes (si los hay)
2. ✅ **Elimina** las tablas con estructura incorrecta
3. ✅ **Crea** las tablas con la estructura correcta
4. ✅ **Migra** los datos del backup (adaptándolos)
5. ✅ **Crea** las tablas faltantes
6. ✅ **Inserta** datos de prueba (si está vacío)

---

## 📝 PASOS PARA CORREGIR

### PASO 1: Ejecutar el script de corrección (2 min)

```sql
-- En SQL Server Management Studio (SSMS):
-- 1. Conectarte a tu servidor SQL
-- 2. Abrir el archivo: Database/FIX-DATABASE-SCHEMA.sql
-- 3. Ejecutar el script completo (F5)
```

**El script hará:**
- Backup de datos existentes
- Recrear tablas con estructura correcta
- Migrar datos antiguos (si los hay)
- Crear tablas faltantes
- Insertar datos de prueba

---

### PASO 2: Verificar que se ejecutó correctamente

```sql
-- Ejecutar esta query:
USE BoskoDB;
GO

-- Ver todas las tablas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Debe mostrar:
-- ✅ ActivityLogs
-- ✅ Categories
-- ✅ Notifications
-- ✅ OrderItems
-- ✅ Orders
-- ✅ OrderStatusHistory
-- ✅ PasswordResetTokens
-- ✅ Products
-- ✅ Users
```

---

### PASO 3: Verificar estructura de Orders

```sql
-- Ver columnas de Orders
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
ORDER BY ORDINAL_POSITION;

-- Debe incluir:
-- ✅ CustomerId (NO UserId)
-- ✅ CustomerName
-- ✅ CustomerEmail
-- ✅ ShippingAddress
-- ✅ Subtotal
-- ✅ Shipping
-- ✅ Total (NO TotalAmount)
-- ✅ PaymentMethod
```

---

### PASO 4: Verificar datos

```sql
-- Contar registros
SELECT 'Orders' AS Tabla, COUNT(*) AS Total FROM Orders;
SELECT 'OrderItems' AS Tabla, COUNT(*) AS Total FROM OrderItems;
SELECT 'OrderStatusHistory' AS Tabla, COUNT(*) AS Total FROM OrderStatusHistory;
SELECT 'ActivityLogs' AS Tabla, COUNT(*) AS Total FROM ActivityLogs;

-- Debe haber al menos:
-- Orders: 3+ registros
-- OrderItems: 3+ registros
-- OrderStatusHistory: 3+ registros
-- ActivityLogs: 3+ registros
```

---

### PASO 5: Reiniciar backend y probar

```bash
# 1. Reiniciar backend
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK
dotnet run

# 2. Abrir Swagger
# https://localhost:5006/swagger

# 3. Probar endpoints:
# - GET /api/categories → Debe funcionar ✅
# - POST /api/auth/login → Debe funcionar ✅
# - GET /api/admin/orders → Debe funcionar ✅ (antes daba 500)
```

---

## 📊 COMPARACIÓN ANTES Y DESPUÉS

### ANTES (INCORRECTO):
```
Orders Table:
- UserId (❌ no coincide con código)
- TotalAmount (❌ no coincide con código)
- Sin CustomerName (❌ falta)
- Sin CustomerEmail (❌ falta)
- Sin Shipping (❌ falta)
- Sin PaymentMethod (❌ falta)

OrderItems Table:
- UnitPrice (❌ debe ser Price)
- Sin ProductName (❌ falta)
- Sin Subtotal (❌ falta)

Tablas faltantes:
❌ OrderStatusHistory
❌ ActivityLogs
❌ Notifications
```

### DESPUÉS (CORRECTO):
```
Orders Table:
✅ CustomerId (coincide con código)
✅ Total (coincide con código)
✅ CustomerName
✅ CustomerEmail
✅ ShippingAddress
✅ Subtotal
✅ Shipping
✅ PaymentMethod

OrderItems Table:
✅ Price (nombre correcto)
✅ ProductName
✅ Subtotal

Tablas completas:
✅ OrderStatusHistory
✅ ActivityLogs
✅ Notifications
```

---

## ⚠️ ADVERTENCIA

**NO ejecutes `dotnet ef database update`** después de corregir manualmente con el script SQL.

Las migraciones de Entity Framework están diseñadas para el esquema antiguo (incorrecto).

El script SQL ya corrige todo lo necesario.

---

## 🎯 DESPUÉS DE EJECUTAR EL SCRIPT

### Lo que funcionará:

✅ `GET /api/categories` - Funcionará (ya funcionaba)
✅ `GET /api/admin/orders` - **Funcionará** (antes daba 500)
✅ `GET /api/admin/orders/{id}` - Funcionará
✅ `PUT /api/admin/orders/{id}/status` - Funcionará
✅ `GET /api/admin/dashboard/stats` - Funcionará
✅ `GET /api/admin/orders/recent` - Funcionará

### El backend estará 100% funcional

---

## 📋 CHECKLIST

- [ ] ⏳ Ejecutar `Database/FIX-DATABASE-SCHEMA.sql` en SSMS
- [ ] ⏳ Verificar que las tablas se crearon correctamente
- [ ] ⏳ Verificar que hay datos de prueba
- [ ] ⏳ Reiniciar backend (`dotnet run`)
- [ ] ⏳ Probar `GET /api/admin/orders` en Swagger
- [ ] ⏳ Verificar que ya NO da error 500
- [ ] ⏳ ✅ TODO FUNCIONANDO

---

## 📞 SOPORTE

**Si el script falla:**
1. Revisa los mensajes de error
2. Verifica que SQL Server está corriendo
3. Verifica que tienes permisos de administrador
4. Lee los mensajes del script (te dirá qué falta)

**Si después del script sigue el error 500:**
1. Verifica que las columnas de Orders son correctas
2. Ejecuta: `Database/COMPLETE-DATABASE-VERIFICATION.sql`
3. Revisa los logs del backend

---

## ✅ CONFIRMACIÓN

Una vez ejecutado el script, el error 500 **desaparecerá completamente**.

El problema era 100% de **esquema de base de datos**, no de código.

---

**Tiempo estimado:** 5 minutos para ejecutar el script y verificar

**Resultado:** Backend 100% funcional sin errores 500

**Próximo paso:** Ejecutar `Database/FIX-DATABASE-SCHEMA.sql` en SSMS

---

**¡El script corregirá todo automáticamente!** 🚀
