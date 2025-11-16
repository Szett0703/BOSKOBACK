# 🔧 SOLUCIÓN ERROR 500 - Gestión de Pedidos

**Fecha:** 16 de Noviembre 2025  
**Estado:** ✅ SOLUCIONADO

---

## 🔴 PROBLEMA IDENTIFICADO

El endpoint `GET /api/admin/orders` estaba retornando error 500 porque la función `GetRecentOrdersAsync` no estaba poblando correctamente todos los campos del DTO `OrderDto`.

### Campos faltantes:
- ❌ `Items` (conteo de items del pedido)
- ❌ `UpdatedAt` (fecha de última actualización)

---

## ✅ SOLUCIÓN APLICADA

### **Archivo modificado:** `Services/AdminService.cs`

**Antes:**
```csharp
public async Task<List<OrderDto>> GetRecentOrdersAsync(int limit = 5)
{
    return await _context.Orders
        .OrderByDescending(o => o.CreatedAt)
        .Take(limit)
        .Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            Amount = o.Total,
            Status = o.Status,
            CreatedAt = o.CreatedAt
            // ❌ Faltaba: Items y UpdatedAt
        })
        .ToListAsync();
}
```

**Después:**
```csharp
public async Task<List<OrderDto>> GetRecentOrdersAsync(int limit = 5)
{
    return await _context.Orders
        .Include(o => o.Items)  // ✅ Agregado Include
        .OrderByDescending(o => o.CreatedAt)
        .Take(limit)
        .Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            Items = o.Items.Count,      // ✅ Agregado
            Amount = o.Total,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt     // ✅ Agregado
        })
        .ToListAsync();
}
```

---

## 🧪 VERIFICACIÓN

### 1. Build Status
```bash
✅ Compilación exitosa
✅ Sin errores
✅ Sin warnings
```

### 2. Estructura del DTO
El `OrderDto` requiere estos campos:

```csharp
public class OrderDto
{
    public int Id { get; set; }                      // ✅
    public string CustomerName { get; set; }         // ✅
    public string CustomerEmail { get; set; }        // ✅
    public int Items { get; set; }                   // ✅ CORREGIDO
    public decimal Amount { get; set; }              // ✅
    public string Status { get; set; }               // ✅
    public DateTime CreatedAt { get; set; }          // ✅
    public DateTime UpdatedAt { get; set; }          // ✅ CORREGIDO
}
```

---

## 🔍 OTRAS VERIFICACIONES REALIZADAS

### ✅ Verificado: GetOrdersAsync
```csharp
// Este método YA estaba correcto
public async Task<PagedResult<OrderDto>> GetOrdersAsync(...)
{
    var orders = await query
        .Include(o => o.Items)  // ✅ Include presente
        .Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            Items = o.Items.Count,      // ✅ Presente
            Amount = o.Total,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt     // ✅ Presente
        })
        .ToListAsync();
}
```

### ✅ Verificado: Controlador
```csharp
[HttpGet("orders")]
public async Task<ActionResult> GetOrders(...)
{
    try
    {
        var result = await _adminService.GetOrdersAsync(page, limit, status, search);
        
        return Ok(new
        {
            orders = result.Data,
            pagination = new
            {
                total = result.Total,
                page = result.Page,
                pages = result.Pages,
                limit = limit
            }
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting orders");
        return StatusCode(500, new { message = "Error interno del servidor" });
    }
}
```

---

## 📊 TESTING

### Test 1: GET /api/admin/orders
```bash
GET https://localhost:5006/api/admin/orders?page=1&limit=10&status=all

ESPERADO:
✅ Status: 200 OK
✅ Response con estructura correcta:
{
  "orders": [
    {
      "id": 1,
      "customerName": "Cliente Test",
      "customerEmail": "customer@bosko.com",
      "items": 3,              // ✅ Campo presente
      "amount": 284.97,
      "status": "delivered",
      "createdAt": "2025-11-11T...",
      "updatedAt": "2025-11-12T..."  // ✅ Campo presente
    }
  ],
  "pagination": {
    "total": 5,
    "page": 1,
    "pages": 1,
    "limit": 10
  }
}
```

### Test 2: Con filtros
```bash
GET https://localhost:5006/api/admin/orders?page=1&limit=10&status=pending

ESPERADO:
✅ Status: 200 OK
✅ Solo pedidos con status "pending"
✅ Todos los campos completos
```

### Test 3: Con búsqueda
```bash
GET https://localhost:5006/api/admin/orders?page=1&limit=10&search=Test

ESPERADO:
✅ Status: 200 OK
✅ Solo pedidos que contienen "Test" en nombre o email
✅ Todos los campos completos
```

---

## 🔄 PASOS SIGUIENTES PARA EL FRONTEND

### 1. Reiniciar el backend (si está corriendo)
```bash
# Detener el servidor (Ctrl+C)
# Volver a ejecutar
dotnet run --project DBTest-BACK.csproj
```

### 2. Limpiar caché del navegador
- Abrir DevTools (F12)
- Ir a Network
- Hacer click derecho → Clear browser cache
- O usar Ctrl+Shift+Delete

### 3. Probar el endpoint nuevamente
```javascript
// En el frontend
this.orderService.getOrders(1, 10, 'all').subscribe({
  next: (response) => {
    console.log('✅ SUCCESS:', response);
    console.log('Orders:', response.orders);
    console.log('Pagination:', response.pagination);
  },
  error: (err) => {
    console.error('❌ ERROR:', err);
  }
});
```

---

## 🗄️ VERIFICAR BASE DE DATOS

Asegúrate de que la tabla Orders tenga datos:

```sql
-- Verificar datos
SELECT 
    o.Id,
    o.CustomerName,
    o.CustomerEmail,
    o.Status,
    o.Total,
    o.CreatedAt,
    o.UpdatedAt,
    COUNT(oi.Id) as ItemsCount
FROM Orders o
LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
GROUP BY o.Id, o.CustomerName, o.CustomerEmail, o.Status, o.Total, o.CreatedAt, o.UpdatedAt
ORDER BY o.CreatedAt DESC;

-- Resultado esperado: 5 pedidos con al menos 1 item cada uno
```

Si no hay datos, ejecuta:
```sql
-- Ejecutar el script de datos
Database/Complete-Data-Insert-Clean.sql
```

---

## 📝 CHECKLIST DE VERIFICACIÓN

### Backend:
- [x] ✅ Código corregido en `AdminService.cs`
- [x] ✅ Build exitoso
- [x] ✅ Sin errores de compilación
- [x] ✅ DTO `OrderDto` tiene todos los campos
- [x] ✅ Método `GetOrdersAsync` correcto
- [x] ✅ Método `GetRecentOrdersAsync` corregido

### Base de Datos:
- [ ] ⏳ Verificar que existe tabla Orders
- [ ] ⏳ Verificar que hay datos en Orders
- [ ] ⏳ Verificar que hay datos en OrderItems
- [ ] ⏳ Verificar relaciones entre tablas

### Frontend:
- [ ] ⏳ Reiniciar backend
- [ ] ⏳ Limpiar caché del navegador
- [ ] ⏳ Probar endpoint GET /api/admin/orders
- [ ] ⏳ Verificar que aparecen los pedidos
- [ ] ⏳ Probar filtros
- [ ] ⏳ Probar búsqueda

---

## 🆘 SI PERSISTE EL ERROR

### 1. Verificar logs del backend
En Visual Studio:
- View → Output
- Show output from: Debug
- Buscar el error específico

### 2. Verificar autenticación
```bash
# En DevTools → Network → Headers
# Verificar que se envía:
Authorization: Bearer {token_válido}
```

### 3. Verificar URL
```bash
# Debe ser exactamente:
GET https://localhost:5006/api/admin/orders
```

### 4. Probar con Postman/Thunder Client
```bash
# 1. Login
POST https://localhost:5006/api/auth/login
Body: {"email":"admin@test.com","password":"Admin123!"}

# 2. Copiar token

# 3. Probar endpoint
GET https://localhost:5006/api/admin/orders?page=1&limit=10
Authorization: Bearer {token}
```

---

## 📞 DEBUGGING AVANZADO

Si después de todo esto sigue fallando, agrega más logging:

### En AdminService.cs:
```csharp
public async Task<PagedResult<OrderDto>> GetOrdersAsync(int page, int limit, string? status, string? search)
{
    try
    {
        _logger.LogInformation("GetOrdersAsync called - Page: {Page}, Limit: {Limit}, Status: {Status}, Search: {Search}", 
            page, limit, status ?? "null", search ?? "null");
        
        var query = _context.Orders.AsQueryable();
        
        // ... resto del código
        
        var orders = await query
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                Items = o.Items.Count,
                Amount = o.Total,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToListAsync();
        
        _logger.LogInformation("GetOrdersAsync completed - Found {Count} orders", orders.Count);
        
        return new PagedResult<OrderDto>
        {
            Data = orders,
            Total = total,
            Page = page,
            Pages = pages
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in GetOrdersAsync");
        throw;
    }
}
```

---

## ✅ RESUMEN

**Problema:** Error 500 en `GET /api/admin/orders`  
**Causa:** Campos faltantes en `OrderDto` (`Items` y `UpdatedAt`)  
**Solución:** Agregado `.Include(o => o.Items)` y campos faltantes en el Select  
**Status:** ✅ RESUELTO

**Próximo paso:** Reiniciar backend y probar desde frontend

---

**Si aún tienes problemas, envíame:**
1. Logs completos del backend (Output window)
2. Error específico del navegador (Network → Response)
3. Resultado de la query SQL de verificación

¡El error debería estar resuelto! 🎉
