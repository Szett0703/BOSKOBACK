# 🚨 Guía Rápida de Troubleshooting - Sistema de Pedidos

## ❌ Error 400 Bad Request

### **Síntoma:**
```json
{
  "success": false,
  "message": "Error de validación",
  "errors": ["Campo X es requerido"]
}
```

### **Causas Comunes:**
1. **Items vacío** - El carrito debe tener al menos 1 item
2. **customerId faltante** - Debe enviarse el ID del cliente
3. **shippingAddress incompleta** - Todos los campos son requeridos
4. **paymentMethod inválido** - Debe ser string válido
5. **quantity <= 0** - La cantidad debe ser mayor a 0
6. **unitPrice <= 0** - El precio debe ser mayor a 0

### **Solución:**
Verificar que el request body contenga todos los campos requeridos:
```json
{
  "customerId": 1,  // ✅ Requerido
  "items": [        // ✅ Requerido, mínimo 1
    {
      "productId": 5,        // ✅ Requerido
      "productName": "X",    // ✅ Requerido
      "quantity": 2,         // ✅ Requerido, > 0
      "unitPrice": 599.99    // ✅ Requerido, > 0
    }
  ],
  "shippingAddress": {  // ✅ Requerido
    "fullName": "...",  // ✅ Requerido
    "phone": "...",     // ✅ Requerido
    "street": "...",    // ✅ Requerido
    "city": "...",      // ✅ Requerido
    "state": "...",     // ✅ Requerido
    "postalCode": "...", // ✅ Requerido
    "country": "..."    // ✅ Requerido
  },
  "paymentMethod": "credit_card" // ✅ Requerido
}
```

---

## ❌ Error 401 Unauthorized

### **Síntoma:**
```json
{
  "message": "Usuario no autenticado"
}
```

### **Causas:**
1. Token JWT no enviado en el header
2. Token JWT expirado
3. Token JWT inválido/corrupto

### **Solución:**
```typescript
// Angular - Agregar interceptor
headers: {
  'Authorization': `Bearer ${jwtToken}`
}
```

```bash
# Postman - Agregar header
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Nota:** Obtener nuevo token con `POST /api/auth/login`

---

## ❌ Error 403 Forbidden

### **Síntoma:**
```
Acceso denegado
```

### **Causas:**
1. Customer intentando ver pedido de otro usuario
2. Customer intentando acceder a endpoint de Admin/Employee
3. Customer intentando editar pedido de otro usuario

### **Solución:**
- **Customer:** Solo puede acceder a sus propios pedidos
- **Employee:** Puede ver todos los pedidos pero no editarlos
- **Admin:** Acceso total

**Endpoints por Rol:**
```
Customer:
  ✅ POST   /api/orders (crear propio pedido)
  ✅ GET    /api/orders/my-orders
  ✅ GET    /api/orders/{id} (solo sus pedidos)
  ✅ PUT    /api/orders/{id} (solo sus pedidos pendientes)
  ✅ POST   /api/orders/{id}/cancel (solo sus pedidos)
  ❌ GET    /api/orders (todos los pedidos)
  ❌ GET    /api/orders/stats
  ❌ PUT    /api/orders/{id}/status

Employee:
  ✅ Todos los de Customer
  ✅ GET    /api/orders (todos los pedidos)
  ✅ GET    /api/orders/stats
  ✅ PUT    /api/orders/{id}/status
  
Admin:
  ✅ Todo lo anterior sin restricciones
```

---

## ❌ Error 404 Not Found

### **Síntoma:**
```json
{
  "success": false,
  "message": "Pedido con ID X no encontrado"
}
```

### **Causas:**
1. El ID del pedido no existe en la base de datos
2. Pedido fue eliminado
3. Typo en el ID

### **Solución:**
1. Verificar que el ID sea correcto
2. Usar `GET /api/orders/my-orders` para obtener IDs válidos
3. Si el pedido fue eliminado, crear uno nuevo

---

## ❌ Error al Editar Pedido (400)

### **Síntoma:**
```json
{
  "success": false,
  "message": "Solo puedes editar pedidos en estado Pendiente"
}
```

### **Causa:**
Intentando editar un pedido que ya no está en estado "pending"

### **Solución:**
- ✅ Solo pedidos con `status: "pending"` pueden editarse
- ❌ No se pueden editar pedidos "processing", "delivered", "cancelled"

**Flujo correcto:**
```
1. Crear pedido → status: "pending" ✅ Editable
2. Admin cambia a "processing" → ❌ Ya no editable
3. Admin cambia a "delivered" → ❌ Completado, inmutable
```

---

## ❌ Error al Cancelar Pedido (400)

### **Síntoma:**
```json
{
  "success": false,
  "message": "No se puede cancelar un pedido ya entregado"
}
```

### **Causa:**
Intentando cancelar un pedido que ya fue entregado

### **Solución:**
- ✅ Solo "pending" y "processing" pueden cancelarse
- ❌ No se pueden cancelar "delivered" ni "cancelled"

**Estados Válidos para Cancelación:**
```
pending → ✅ Cancelable
processing → ✅ Cancelable
delivered → ❌ NO cancelable (ya completado)
cancelled → ❌ Ya está cancelado
```

---

## ❌ Error al Crear Pedido: Cliente no encontrado

### **Síntoma:**
```json
{
  "success": false,
  "message": "Cliente no encontrado"
}
```

### **Causa:**
El `customerId` enviado no existe en la tabla `Users`

### **Solución:**
1. Verificar que el usuario esté registrado
2. Usar el `userId` del token JWT:
```typescript
// En el frontend, obtener userId del token
const userId = jwtDecode(token).nameid;

// Usarlo en el request
orderService.createOrder({
  customerId: userId,  // ✅ Usar ID del usuario autenticado
  items: [...],
  shippingAddress: {...}
});
```

---

## ❌ Error 500 Internal Server Error

### **Síntoma:**
```json
{
  "success": false,
  "message": "Error al crear el pedido",
  "error": "Exception message..."
}
```

### **Causas Comunes:**
1. **Base de datos no disponible** - SQL Server no está corriendo
2. **ConnectionString incorrecta** - Verificar `appsettings.json`
3. **Tablas no creadas** - Ejecutar migraciones
4. **Foreign Key constraint** - ProductId no existe en Products

### **Solución:**

#### 1. Verificar SQL Server
```bash
# Verificar que SQL Server esté corriendo
Get-Service MSSQLSERVER
```

#### 2. Verificar ConnectionString
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BoskoDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

#### 3. Verificar Tablas
```sql
-- Ejecutar en SSMS o Azure Data Studio
USE BoskoDB;
GO

SELECT * FROM Orders;
SELECT * FROM OrderItems;
SELECT * FROM ShippingAddresses;
SELECT * FROM Products;
```

#### 4. Verificar Logs
```bash
# Visual Studio → Output → Debug
# Buscar líneas con ❌ o ERROR
```

---

## ❌ CORS Error (desde Angular)

### **Síntoma:**
```
Access to fetch at 'https://localhost:5006/api/orders' from origin 'http://localhost:4300'
has been blocked by CORS policy
```

### **Causa:**
El backend no permite requests desde el origin del frontend

### **Solución:**
Ya está configurado en `Program.cs` para puertos 4200 y 4300:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "http://localhost:4300",
            "https://localhost:4200",
            "https://localhost:4300"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

Si usas otro puerto, agregarlo a la lista y reiniciar el backend.

---

## ❌ Cálculos Incorrectos

### **Problema:**
Los totales no coinciden con lo esperado

### **Fórmulas Correctas:**
```csharp
Subtotal = Σ(quantity × unitPrice) de todos los items
Tax = Subtotal × 0.16 (IVA México 16%)
ShippingCost = Subtotal >= 500 ? 0 : 100
Total = Subtotal + Tax + ShippingCost
```

### **Ejemplo:**
```
Items:
  - Product A: $100 × 2 = $200
  - Product B: $150 × 3 = $450

Subtotal: $200 + $450 = $650
Tax: $650 × 0.16 = $104
ShippingCost: $0 (subtotal >= $500)
Total: $650 + $104 + $0 = $754
```

**Nota:** El backend calcula automáticamente, no enviar estos valores desde el frontend.

---

## ❌ OrderNumber Duplicado

### **Síntoma:**
```
Error: Duplicate key violation
```

### **Causa:**
Muy raro, pero puede ocurrir si dos requests se procesan exactamente al mismo milisegundo

### **Solución:**
El `OrderNumber` se genera con:
```csharp
$"ORD-{timestamp}-{random}"
// Ejemplo: ORD-20251118143025-5678
```

La probabilidad de colisión es < 0.01%. Si ocurre, el backend retornará error 500. Simplemente reintentar el request.

---

## 🛠️ Comandos de Diagnóstico

### **Verificar Backend Corriendo**
```bash
# PowerShell
Test-NetConnection -ComputerName localhost -Port 5006
```

### **Verificar Base de Datos**
```sql
-- SQL Server Management Studio
SELECT COUNT(*) FROM Orders;
SELECT COUNT(*) FROM Users;
SELECT COUNT(*) FROM Products;
```

### **Logs del Backend**
```bash
# Visual Studio → Output → Debug
# Buscar líneas con:
📨 POST /api/orders
✅ POST /api/orders → 201
❌ POST /api/orders → 400
```

### **Testing Manual con curl**
```bash
# Login
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@bosko.com","password":"Bosko123!"}'

# Crear Orden (reemplazar {JWT_TOKEN})
curl -X POST https://localhost:5006/api/orders \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "customerId": 1,
    "items": [{"productId":1,"productName":"Test","quantity":1,"unitPrice":100}],
    "shippingAddress": {
      "fullName":"Test","phone":"1234","street":"Test",
      "city":"CDMX","state":"CDMX","postalCode":"12345","country":"México"
    },
    "paymentMethod":"credit_card"
  }'
```

---

## 📞 Puntos de Contacto

### **Si el problema persiste:**

1. **Verificar documentación:**
   - `ORDERS-SYSTEM-COMPLETE-SUMMARY.md`
   - `Rules.md`
   - `ORDERS-SYSTEM.md`

2. **Verificar Swagger:**
   ```
   https://localhost:5006/swagger
   ```
   Probar endpoints directamente en Swagger UI

3. **Revisar logs del backend:**
   - Visual Studio → Output → Debug
   - Buscar líneas con ❌ o ERROR

4. **Verificar base de datos:**
   - SQL Server corriendo
   - Tablas existen
   - Datos de prueba presentes

---

## ✅ Checklist de Verificación Rápida

Antes de reportar un error, verificar:

- [ ] Backend está corriendo (`dotnet run`)
- [ ] SQL Server está corriendo
- [ ] Base de datos `BoskoDB` existe
- [ ] Usuario está autenticado (token JWT válido)
- [ ] Request body tiene todos los campos requeridos
- [ ] CustomerId existe en la tabla Users
- [ ] ProductIds existen en la tabla Products
- [ ] Rol del usuario tiene permisos para el endpoint
- [ ] Estado del pedido permite la acción (editar/cancelar)
- [ ] CORS está configurado para el puerto del frontend

---

**Última Actualización:** 18 de Noviembre 2025  
**Status:** ✅ Guía Completa
