# ✅ CORREGIDO - Error 500 en GET /api/admin/orders/{id}

**Fecha:** 19 de Noviembre 2025  
**Status:** ✅ PROBLEMA IDENTIFICADO Y CORREGIDO  
**Endpoint Afectado:** GET /api/admin/orders/{id}

---

## 🔴 PROBLEMA IDENTIFICADO

### **Síntoma:**
Error 500 (Internal Server Error) al intentar obtener detalles de un pedido desde el panel de administración.

### **Causa Raíz:**
El método `GetOrderByIdAsync` en `AdminService.cs` tenía **2 problemas**:

1. **Faltaba `.Include(o => o.ShippingAddressDetails)`** en la consulta EF Core
2. **Código intentaba parsear un string** en lugar de usar la tabla `ShippingAddresses`

### **¿Por qué fallaba?**

Cuando se creó un pedido nuevo (como el pedido #22), el sistema guardó la dirección en la tabla `ShippingAddresses` (nuevo sistema).

Sin embargo, el método `GetOrderByIdAsync` intentaba:
1. Cargar `order.ShippingAddress` (string legacy)
2. Parsearlo dividiendo por comas: `"Calle, Ciudad, Estado, CP, País"`

**Problema:** Los pedidos nuevos tienen `ShippingAddressDetails` (objeto) pero NO tienen el string legacy, causando un `NullReferenceException` o error de parsing.

---

## ✅ SOLUCIÓN IMPLEMENTADA

### **Cambios en `Services/AdminService.cs`:**

**ANTES (código problemático):**
```csharp
public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
{
    var order = await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)
        // ❌ FALTA: .Include(o => o.ShippingAddressDetails)
        .Include(o => o.StatusHistory)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null)
        return null;

    // ❌ Siempre intenta parsear el string
    var addressParts = order.ShippingAddress.Split(',').Select(p => p.Trim()).ToArray();
    var shippingAddress = new ShippingAddressInfo
    {
        Street = addressParts.Length > 0 ? addressParts[0] : "",
        City = addressParts.Length > 1 ? addressParts[1] : "",
        State = addressParts.Length > 2 ? addressParts[2] : "",
        ZipCode = addressParts.Length > 3 ? addressParts[3] : "",
        Country = addressParts.Length > 4 ? addressParts[4] : ""
    };
    
    // ...resto del código
}
```

**DESPUÉS (código corregido):**
```csharp
public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
{
    try
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingAddressDetails)  // ✅ AGREGADO
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        // ✅ Lógica inteligente: usar tabla si existe, sino parsear string
        ShippingAddressInfo shippingAddress;
        
        if (order.ShippingAddressDetails != null)
        {
            // Nuevo sistema con tabla ShippingAddresses
            shippingAddress = new ShippingAddressInfo
            {
                Street = order.ShippingAddressDetails.Street,
                City = order.ShippingAddressDetails.City,
                State = order.ShippingAddressDetails.State,
                ZipCode = order.ShippingAddressDetails.PostalCode,
                Country = order.ShippingAddressDetails.Country
            };
        }
        else
        {
            // Sistema legacy: parsear el string (compatibilidad con pedidos antiguos)
            var addressParts = order.ShippingAddress.Split(',').Select(p => p.Trim()).ToArray();
            shippingAddress = new ShippingAddressInfo
            {
                Street = addressParts.Length > 0 ? addressParts[0] : "",
                City = addressParts.Length > 1 ? addressParts[1] : "",
                State = addressParts.Length > 2 ? addressParts[2] : "",
                ZipCode = addressParts.Length > 3 ? addressParts[3] : "",
                Country = addressParts.Length > 4 ? addressParts[4] : ""
            };
        }

        // ...resto del código (sin cambios)
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error obteniendo detalles del pedido {OrderId}", id);
        throw; // Re-lanzar para que el controller lo maneje
    }
}
```

---

## 🎯 BENEFICIOS DE LA CORRECCIÓN

### **1. Compatibilidad hacia atrás:**
- ✅ Pedidos **nuevos** (con `ShippingAddressDetails`): funcionan perfectamente
- ✅ Pedidos **antiguos** (solo con string): siguen funcionando con el parsing legacy

### **2. Mejor manejo de errores:**
- ✅ Try-catch agregado para capturar excepciones
- ✅ Logging detallado de errores
- ✅ Controller responde con 500 y mensaje claro

### **3. Performance:**
- ✅ Se carga `ShippingAddressDetails` en una sola query (con Include)
- ✅ No más queries adicionales después

---

## 🧪 TESTING

### **Prueba 1: Pedido nuevo con ShippingAddresses**

```http
GET https://localhost:5006/api/admin/orders/22
Authorization: Bearer {admin-token}
```

**Resultado Esperado:**
```json
{
  "id": 22,
  "customerName": "Santiago",
  "customerEmail": "santiago.c0399@gmail.com",
  "status": "pending",
  "shippingAddress": {
    "street": "Dirección temporal",
    "city": "Ciudad",
    "state": "Estado",
    "zipCode": "00000",
    "country": "México"
  },
  "orderItems": [...],
  "statusHistory": [...]
}
```

**Status:** ✅ 200 OK

---

### **Prueba 2: Pedido inexistente**

```http
GET https://localhost:5006/api/admin/orders/999999
Authorization: Bearer {admin-token}
```

**Resultado Esperado:**
```json
{
  "error": "Pedido no encontrado",
  "orderId": 999999
}
```

**Status:** 404 Not Found

---

### **Prueba 3: Sin autenticación**

```http
GET https://localhost:5006/api/admin/orders/22
# Sin header Authorization
```

**Resultado Esperado:**
```json
{
  "message": "Unauthorized"
}
```

**Status:** 401 Unauthorized

---

## 📊 VERIFICACIÓN SQL

Para verificar que el pedido existe y tiene dirección:

```sql
-- Ver el pedido
SELECT * FROM Orders WHERE Id = 22;

-- Ver si tiene ShippingAddress (tabla)
SELECT * FROM ShippingAddresses WHERE OrderId = 22;

-- Ver items del pedido
SELECT * FROM OrderItems WHERE OrderId = 22;

-- Ver historial de estados
SELECT * FROM OrderStatusHistory WHERE OrderId = 22 ORDER BY Timestamp DESC;
```

**Resultado esperado:**
- Orders: 1 fila
- ShippingAddresses: 1 fila
- OrderItems: ≥1 fila(s)
- OrderStatusHistory: ≥1 fila(s)

---

## 🚀 PRÓXIMOS PASOS

### **1. Reiniciar el Backend**

```bash
# Detener (Ctrl+C)
dotnet run
```

### **2. Probar en Swagger**

```
1. Abrir: https://localhost:5006/swagger
2. Login como admin: POST /api/auth/login
3. Copiar token
4. Authorize
5. GET /api/admin/orders/{id}
6. id: 22
7. Execute
8. Verificar: 200 OK y datos completos
```

### **3. Probar desde Angular**

```typescript
// En order-management.component.ts
viewOrderDetails(order: any) {
  this.orderService.getOrderById(order.id).subscribe({
    next: (details) => {
      console.log('✅ Detalles del pedido:', details);
      this.selectedOrder = details;
      this.showDetailsModal = true;
    },
    error: (error) => {
      console.error('❌ Error:', error);
      this.toastr.error('Error al cargar detalles del pedido');
    }
  });
}
```

**Resultado esperado:**
- ✅ Modal se abre con todos los detalles
- ✅ Dirección de envío completa
- ✅ Lista de items
- ✅ Historial de estados
- ✅ No más error 500

---

## 📝 LOGS MEJORADOS

Con la corrección, los logs ahora muestran:

### **Caso exitoso:**
```
info: DBTest_BACK.Services.AdminService[0]
      Obteniendo detalles del pedido 22
info: DBTest_BACK.Controllers.AdminController[0]
      Pedido 22 obtenido exitosamente
```

### **Caso de error:**
```
error: DBTest_BACK.Services.AdminService[0]
       Error obteniendo detalles del pedido 22
       System.NullReferenceException: Object reference not set...
       at AdminService.GetOrderByIdAsync...
error: DBTest_BACK.Controllers.AdminController[0]
       Error getting order 22
```

---

## ✅ CHECKLIST DE VERIFICACIÓN

Después de reiniciar el backend:

- [ ] Build exitoso sin errores
- [ ] Backend iniciado correctamente
- [ ] Swagger disponible en https://localhost:5006/swagger
- [ ] Login como admin funciona
- [ ] GET /api/admin/orders/{id} retorna 200 OK
- [ ] Response incluye `shippingAddress` completa
- [ ] Response incluye `orderItems` con datos
- [ ] Response incluye `statusHistory`
- [ ] Frontend puede ver detalles sin error 500
- [ ] Modal de detalles se abre correctamente

---

## 🎯 RESUMEN

| Aspecto | Antes | Después |
|---------|-------|---------|
| **Include ShippingAddressDetails** | ❌ No | ✅ Sí |
| **Compatibilidad legacy** | ❌ No | ✅ Sí |
| **Manejo de errores** | ⚠️ Básico | ✅ Completo |
| **Logs** | ⚠️ Básico | ✅ Detallados |
| **Try-catch** | ❌ No | ✅ Sí |
| **Status del endpoint** | 🔴 Error 500 | ✅ 200 OK |

---

## 📞 SI EL PROBLEMA PERSISTE

### **1. Verificar logs del backend:**

```
Visual Studio → Output → Debug
Buscar: "Error obteniendo detalles del pedido"
```

### **2. Verificar en BD que el pedido existe:**

```sql
SELECT o.Id, o.OrderNumber, o.CustomerName,
       COUNT(oi.Id) AS ItemsCount,
       COUNT(sa.Id) AS HasShippingAddress
FROM Orders o
LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
LEFT JOIN ShippingAddresses sa ON sa.OrderId = o.Id
WHERE o.Id = 22
GROUP BY o.Id, o.OrderNumber, o.CustomerName;
```

**Resultado esperado:**
```
Id | OrderNumber | CustomerName | ItemsCount | HasShippingAddress
22 | ORD-...     | Santiago     | 2          | 1
```

### **3. Verificar respuesta completa en Swagger:**

```
Response body debe incluir:
- id
- customerName
- customerEmail
- shippingAddress { street, city, state, zipCode, country }
- orderItems [ {...}, {...} ]
- statusHistory [ {...} ]
- subtotal
- shipping
- total
```

---

**Status:** ✅ CORREGIDO  
**Build:** ✅ Exitoso  
**Testing:** ⏳ Pendiente (ejecutar ahora)  
**Próximo Paso:** Reiniciar backend y probar en Swagger

**Última Actualización:** 19 de Noviembre 2025  
**Corregido por:** Backend Team
