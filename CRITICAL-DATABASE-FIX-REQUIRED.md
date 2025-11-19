# 🚨 ERROR 400 - PROBLEMA CRÍTICO IDENTIFICADO

**Fecha:** 19 de Noviembre 2025  
**Status:** ❌ PROBLEMA CRÍTICO - ESTRUCTURA DE BD INCOMPLETA  
**Prioridad:** 🔴 ALTA - REQUIERE ACCIÓN INMEDIATA

---

## 🎯 PROBLEMA IDENTIFICADO

### **Error Recibido:**
```
Status: 400
Message: "Error al crear el pedido"
Error: "An error occurred while saving the entity changes. See the inner exception for details."
```

### **Causa Raíz:**
La estructura de la base de datos **NO coincide con el código C#**. Faltan columnas y una tabla completa.

---

## 🔍 ANÁLISIS DETALLADO

### **1. Tabla `Orders` - FALTAN 4 COLUMNAS**

**Lo que el código espera:**
```csharp
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? OrderNumber { get; set; }        // ❌ FALTA EN BD
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string ShippingAddress { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }                // ❌ FALTA EN BD
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public string PaymentMethod { get; set; }
    public string? TrackingNumber { get; set; }     // ❌ FALTA EN BD
    public string? Notes { get; set; }              // ❌ FALTA EN BD
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Lo que hay en la BD actualmente:**
```sql
CREATE TABLE Orders (
    Id INT,
    CustomerId INT,
    -- ❌ OrderNumber NO EXISTE
    CustomerName NVARCHAR(100),
    CustomerEmail NVARCHAR(255),
    ShippingAddress NVARCHAR(500),
    Subtotal DECIMAL(18,2),
    -- ❌ Tax NO EXISTE
    Shipping DECIMAL(18,2),
    Total DECIMAL(18,2),
    Status NVARCHAR(20),
    PaymentMethod NVARCHAR(50),
    -- ❌ TrackingNumber NO EXISTE
    -- ❌ Notes NO EXISTE
    CreatedAt DATETIME2,
    UpdatedAt DATETIME2
)
```

---

### **2. Tabla `OrderItems` - FALTA 1 COLUMNA**

**Lo que el código espera:**
```csharp
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductImage { get; set; }       // ❌ FALTA EN BD
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}
```

**Lo que hay en la BD:**
```sql
CREATE TABLE OrderItems (
    Id INT,
    OrderId INT,
    ProductId INT,
    ProductName NVARCHAR(200),
    -- ❌ ProductImage NO EXISTE
    Quantity INT,
    Price DECIMAL(18,2),
    Subtotal DECIMAL(18,2)
)
```

---

### **3. Tabla `ShippingAddresses` - NO EXISTE**

**Lo que el código espera:**
```csharp
public class ShippingAddress
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}
```

**Lo que hay en la BD:**
```
❌ LA TABLA NO EXISTE
```

Esto causa que cuando el código intenta hacer:
```csharp
_context.ShippingAddresses.Add(shippingAddress);
await _context.SaveChangesAsync();
```

Entity Framework no puede crear la relación porque **la tabla no existe**.

---

## 🔧 SOLUCIÓN

### **PASO 1: Ejecutar Script SQL**

He creado el archivo `Database/FIX-ORDERS-TABLES-MISSING-COLUMNS.sql` que contiene:

1. **ALTER TABLE Orders** - Agregar columnas faltantes:
   - `OrderNumber` NVARCHAR(50)
   - `Tax` DECIMAL(18,2)
   - `TrackingNumber` NVARCHAR(100)
   - `Notes` NVARCHAR(500)

2. **ALTER TABLE OrderItems** - Agregar columna:
   - `ProductImage` NVARCHAR(500)

3. **CREATE TABLE ShippingAddresses** - Crear tabla completa con:
   - Todas las columnas requeridas
   - Foreign Key a Orders
   - Índice en OrderId

4. **CREATE UNIQUE INDEX** en Orders.OrderNumber

5. **Verificaciones finales** de integridad

### **PASO 2: Ejecutar el Script**

```powershell
# En SQL Server Management Studio o Azure Data Studio
# Abrir el archivo: Database/FIX-ORDERS-TABLES-MISSING-COLUMNS.sql
# Ejecutar todo el script (F5)
```

O desde PowerShell:
```powershell
sqlcmd -S localhost -d BoskoDB -i "Database\FIX-ORDERS-TABLES-MISSING-COLUMNS.sql"
```

### **PASO 3: Reiniciar Backend**

```bash
# Detener el backend (Ctrl+C)
dotnet run
```

### **PASO 4: Probar nuevamente**

```typescript
// Desde Angular, volver a intentar crear el pedido
orderService.createOrder(orderData).subscribe(...)
```

---

## 📋 CHECKLIST DE VERIFICACIÓN

Después de ejecutar el script SQL:

- [ ] Script ejecutado sin errores
- [ ] Tabla `Orders` tiene columna `OrderNumber`
- [ ] Tabla `Orders` tiene columna `Tax`
- [ ] Tabla `Orders` tiene columna `TrackingNumber`
- [ ] Tabla `Orders` tiene columna `Notes`
- [ ] Tabla `OrderItems` tiene columna `ProductImage`
- [ ] Tabla `ShippingAddresses` existe
- [ ] Tabla `ShippingAddresses` tiene FK a Orders
- [ ] Backend reiniciado
- [ ] Swagger muestra los endpoints correctamente
- [ ] POST /api/orders funciona sin error 400

---

## 🧪 VERIFICACIÓN MANUAL

### **1. Verificar en SQL Server:**

```sql
-- Verificar columnas en Orders
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
ORDER BY ORDINAL_POSITION;

-- Debe mostrar: OrderNumber, Tax, TrackingNumber, Notes

-- Verificar columnas en OrderItems
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'OrderItems'
ORDER BY ORDINAL_POSITION;

-- Debe mostrar: ProductImage

-- Verificar tabla ShippingAddresses
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'ShippingAddresses';

-- Debe retornar 1 fila
```

### **2. Verificar en Swagger:**

```
1. Abrir: https://localhost:5006/swagger
2. Buscar: POST /api/orders
3. Click en "Try it out"
4. Pegar el JSON de prueba:
```

```json
{
  "customerId": 22,
  "items": [
    {
      "productId": 21,
      "productName": "Tenis Blancos Guess",
      "productImage": "https://m.media-amazon.com/images/I/61-2ap5dmJL._AC_SY575_.jpg",
      "quantity": 2,
      "unitPrice": 10.99
    }
  ],
  "shippingAddress": {
    "fullName": "Camilo",
    "phone": "555-0000",
    "street": "Dirección temporal",
    "city": "Ciudad",
    "state": "Estado",
    "postalCode": "00000",
    "country": "México"
  },
  "paymentMethod": "credit_card",
  "notes": "Pedido de prueba"
}
```

```
5. Click en "Execute"
6. Verificar response 201 (NO 400)
```

---

## 🎯 POR QUÉ OCURRIÓ ESTE ERROR

### **Problema de Sincronización:**

1. El código C# fue creado con los modelos completos
2. La base de datos fue creada con un script antiguo o incompleto
3. **NO se ejecutó Entity Framework Migrations**
4. Resultado: Código y BD desincronizados

### **Solución a Futuro:**

**Opción 1: Usar EF Migrations (Recomendado)**
```bash
# Crear migración inicial
dotnet ef migrations add InitialCreate

# Aplicar a BD
dotnet ef database update
```

**Opción 2: Mantener Scripts SQL Manualmente**
- Cada vez que cambies un modelo en C#, actualizar el script SQL
- Mantener un script "master" con la estructura completa

---

## 📊 IMPACTO DEL ERROR

### **Qué NO funcionaba:**
- ❌ POST /api/orders - Error 400
- ❌ Crear pedidos desde Angular
- ❌ Guardar direcciones de envío
- ❌ Sistema de tracking (TrackingNumber)
- ❌ Notas de pedidos

### **Qué SÍ funcionaba:**
- ✅ GET /api/products
- ✅ GET /api/categories
- ✅ POST /api/auth/login
- ✅ Cualquier endpoint que NO use Orders

---

## 🚀 DESPUÉS DE LA CORRECCIÓN

### **Lo que funcionará:**
- ✅ Crear pedidos completos con dirección de envío
- ✅ Guardar notas del cliente
- ✅ Generar OrderNumber único automáticamente
- ✅ Calcular Tax (16% IVA) correctamente
- ✅ Sistema de tracking con TrackingNumber
- ✅ Imágenes de productos en OrderItems
- ✅ Relación 1:1 entre Order y ShippingAddress

---

## 📞 SI EL PROBLEMA PERSISTE

### **1. Verificar logs del backend:**
```
Visual Studio → Output → Debug
Buscar líneas con ❌ o ERROR
```

### **2. Verificar que customerId y productId existen:**
```sql
-- Verificar usuario
SELECT * FROM Users WHERE Id = 22;

-- Verificar producto
SELECT * FROM Products WHERE Id = 21;
```

Si no existen, crear usuarios/productos de prueba.

### **3. Verificar foreign keys:**
```sql
-- Ver todas las FK
SELECT 
    fk.name AS FK_Name,
    tp.name AS Parent_Table,
    cp.name AS Parent_Column,
    tr.name AS Referenced_Table,
    cr.name AS Referenced_Column
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
WHERE tp.name IN ('Orders', 'OrderItems', 'ShippingAddresses')
ORDER BY tp.name;
```

---

## ✅ RESUMEN

**Problema:** Base de datos incompleta (4 columnas faltantes + 1 tabla faltante)  
**Solución:** Ejecutar script `FIX-ORDERS-TABLES-MISSING-COLUMNS.sql`  
**Tiempo estimado:** 2 minutos  
**Impacto:** Sin este fix, el sistema de pedidos NO funciona  
**Próximo paso:** Ejecutar el script SQL AHORA

---

**Status Final:** ⚠️ ESPERANDO EJECUCIÓN DEL SCRIPT SQL  
**Última Actualización:** 19 de Noviembre 2025  
**Creado por:** Backend Team - DBTest-BACK
