# 🚀 GUÍA RÁPIDA - EJECUTAR CORRECCIÓN DE BASE DE DATOS

## ⚡ EJECUTAR AHORA (5 Minutos)

### **Paso 1: Abrir SQL Server Management Studio o Azure Data Studio**

```
🔹 Si usas SSMS:
   - Abrir SQL Server Management Studio
   - Conectar a: localhost (o tu servidor)
   - Click en "New Query"

🔹 Si usas Azure Data Studio:
   - Abrir Azure Data Studio
   - Conectar a: localhost
   - Click en "New Query"
```

---

### **Paso 2: Seleccionar la Base de Datos**

```sql
USE [BoskoDB]
GO
```

**O hacer click derecho en `BoskoDB` → "New Query"**

---

### **Paso 3: Abrir el Script de Corrección**

**Opción A: Copiar/Pegar**
```
1. Abrir archivo: Database/FIX-ORDERS-TABLES-MISSING-COLUMNS.sql
2. Seleccionar TODO el contenido (Ctrl+A)
3. Copiar (Ctrl+C)
4. Pegar en la ventana de Query (Ctrl+V)
```

**Opción B: Abrir archivo directamente**
```
File → Open → File
Navegar a: DBTest-BACK/Database/FIX-ORDERS-TABLES-MISSING-COLUMNS.sql
```

---

### **Paso 4: EJECUTAR el Script**

```
🔹 Click en el botón "Execute" o presionar F5
🔹 Esperar a que termine (toma ~10 segundos)
```

---

### **Paso 5: Verificar Resultados**

Deberías ver en el panel "Messages":

```
============================================
🔧 INICIANDO CORRECCIÓN DE BASE DE DATOS
============================================

📋 PASO 1: Verificando y agregando columnas en tabla Orders...
   ✅ Columna OrderNumber agregada
   ✅ Columna Tax agregada
   ✅ Columna TrackingNumber agregada
   ✅ Columna Notes agregada

📋 PASO 2: Verificando y agregando columnas en tabla OrderItems...
   ✅ Columna ProductImage agregada

📋 PASO 3: Verificando y creando tabla ShippingAddresses...
   ✅ Tabla ShippingAddresses creada exitosamente
   ✅ Foreign Key FK_ShippingAddresses_Orders agregada
   ✅ Índice IX_ShippingAddresses_OrderId creado

📋 PASO 4: Verificando índice único en OrderNumber...
   ✅ Índice único IX_Orders_OrderNumber_Unique creado

📋 PASO 5: Verificando restricción de FK en OrderItems_ProductId...
   ✅ Restricción FK_OrderItems_ProductId actualizada correctamente

📋 PASO 6: Verificando datos de prueba...
   ℹ️  Usuarios en BD: X
   ℹ️  Productos en BD: Y

📋 PASO 7: Verificación final de estructura...
   ✅ Tabla Orders tiene todas las columnas requeridas
   ✅ Tabla OrderItems tiene todas las columnas requeridas
   ✅ Tabla ShippingAddresses existe

============================================
✅ CORRECCIÓN COMPLETADA EXITOSAMENTE
============================================
```

---

### **Paso 6: Reiniciar el Backend**

```powershell
# En tu terminal de Visual Studio o PowerShell
# Detener el backend: Ctrl+C

# Reiniciar:
dotnet run
```

**O en Visual Studio:**
```
Debug → Stop Debugging (Shift+F5)
Debug → Start Debugging (F5)
```

---

### **Paso 7: Verificar en Swagger**

```
1. Abrir navegador: https://localhost:5006/swagger
2. Buscar: POST /api/orders
3. Click en "Try it out"
4. Pegar este JSON de prueba:
```

```json
{
  "customerId": 1,
  "items": [
    {
      "productId": 1,
      "productName": "Producto Test",
      "productImage": "https://via.placeholder.com/150",
      "quantity": 1,
      "unitPrice": 99.99
    }
  ],
  "shippingAddress": {
    "fullName": "Test User",
    "phone": "555-1234",
    "street": "Test Street 123",
    "city": "Test City",
    "state": "Test State",
    "postalCode": "12345",
    "country": "México"
  },
  "paymentMethod": "credit_card",
  "notes": "Test order"
}
```

```
5. Click en "Execute"
6. Verificar que retorna 201 Created (NO 400)
```

---

### **Paso 8: Probar desde Angular**

```typescript
// En tu componente de carrito
// Hacer click en "Finalizar Compra"
// Verificar que el pedido se crea exitosamente
```

---

## ✅ CHECKLIST FINAL

Marcar cuando esté completado:

- [ ] Script SQL ejecutado sin errores
- [ ] Backend reiniciado
- [ ] Swagger muestra POST /api/orders
- [ ] Test en Swagger retorna 201 Created
- [ ] Angular puede crear pedidos sin error 400
- [ ] Pedido aparece en la tabla Orders
- [ ] Dirección aparece en ShippingAddresses

---

## 🚨 SI HAY ERRORES

### **Error: "Cannot insert NULL into column 'Tax'"**

**Causa:** Hay pedidos existentes en la tabla Orders

**Solución:**
```sql
-- Actualizar pedidos existentes con Tax = 0
UPDATE Orders SET Tax = 0 WHERE Tax IS NULL;
```

---

### **Error: "Constraint violation on OrderNumber"**

**Causa:** Hay pedidos con OrderNumber duplicado o NULL

**Solución:**
```sql
-- Generar OrderNumber único para pedidos existentes
UPDATE Orders
SET OrderNumber = 'ORD-' + CONVERT(VARCHAR(20), Id) + '-' + CONVERT(VARCHAR(4), ABS(CHECKSUM(NEWID())) % 10000)
WHERE OrderNumber IS NULL;
```

---

### **Error: "Table ShippingAddresses already exists"**

**Causa:** La tabla ya fue creada anteriormente

**Solución:**
```
✅ Ignorar este error - la tabla ya existe
El script detecta esto y lo marca como "ya existe"
```

---

### **Error: "Foreign key constraint failed"**

**Causa:** Hay OrderItems con ProductId que no existe en Products

**Solución:**
```sql
-- Ver OrderItems huérfanos
SELECT oi.* FROM OrderItems oi
LEFT JOIN Products p ON oi.ProductId = p.Id
WHERE p.Id IS NULL;

-- Opción 1: Eliminar OrderItems huérfanos
DELETE FROM OrderItems WHERE ProductId NOT IN (SELECT Id FROM Products);

-- Opción 2: Actualizar ProductId a uno válido
UPDATE OrderItems SET ProductId = 1 WHERE ProductId NOT IN (SELECT Id FROM Products);
```

---

## 📞 SOPORTE

**Si el script falla:**

1. Copiar el mensaje de error completo
2. Verificar en qué PASO falló (1-7)
3. Revisar el archivo: `CRITICAL-DATABASE-FIX-REQUIRED.md`
4. Ejecutar los comandos de verificación manual

**Verificación manual:**
```sql
-- Ver estructura de Orders
EXEC sp_help 'Orders';

-- Ver estructura de OrderItems
EXEC sp_help 'OrderItems';

-- Ver si existe ShippingAddresses
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShippingAddresses';
```

---

## 🎯 RESULTADO ESPERADO

**Antes:**
```
POST /api/orders → 400 Bad Request
Error: "An error occurred while saving entity changes"
```

**Después:**
```
POST /api/orders → 201 Created
Response: {
  "success": true,
  "message": "Pedido creado exitosamente",
  "data": {
    "id": 1,
    "orderNumber": "ORD-20251119123456-7890",
    ...
  }
}
```

---

## 📋 COMANDOS RÁPIDOS

### **Verificar que todo está OK:**
```sql
USE BoskoDB;

-- Contar registros
SELECT 'Orders' AS Tabla, COUNT(*) AS Total FROM Orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM OrderItems
UNION ALL
SELECT 'ShippingAddresses', COUNT(*) FROM ShippingAddresses
UNION ALL
SELECT 'Products', COUNT(*) FROM Products
UNION ALL
SELECT 'Users', COUNT(*) FROM Users;

-- Ver últimos 5 pedidos
SELECT TOP 5 * FROM Orders ORDER BY CreatedAt DESC;

-- Ver pedidos con sus direcciones
SELECT 
    o.Id,
    o.OrderNumber,
    o.CustomerName,
    o.Total,
    sa.City,
    sa.State
FROM Orders o
LEFT JOIN ShippingAddresses sa ON o.Id = sa.OrderId
ORDER BY o.CreatedAt DESC;
```

---

**🎉 ¡Listo! El sistema de pedidos está funcionando.**

**Tiempo total estimado:** 5 minutos  
**Dificultad:** Fácil ⭐  
**Resultado:** Sistema de Orders 100% funcional ✅
