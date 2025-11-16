# 🧪 PLAN DE PRUEBAS - GESTIÓN DE PEDIDOS

## 📋 Resumen

Este documento detalla todas las pruebas que deben realizarse para validar la funcionalidad completa del sistema de gestión de pedidos.

---

## 🎯 Objetivos de las Pruebas

- ✅ Verificar que todos los endpoints respondan correctamente
- ✅ Validar que las autorizaciones funcionen apropiadamente
- ✅ Confirmar que los filtros y búsquedas sean precisos
- ✅ Asegurar que las actualizaciones de estado se registren correctamente
- ✅ Verificar el rendimiento con múltiples registros

---

## 🔧 Configuración Previa

### 1. Base de Datos

Ejecutar los siguientes scripts en orden:

```sql
-- 1. Crear estructura
Database/BoskoDB-Setup.sql

-- 2. Agregar tablas de autenticación
Database/Users-Authentication-Setup.sql

-- 3. Agregar tablas de admin panel
Database/Admin-Panel-Setup.sql

-- 4. Insertar datos de prueba
Database/Insert-All-Data-Final.sql
```

### 2. Backend

```bash
# Iniciar el backend
dotnet run --project DBTest-BACK.csproj
```

Debe estar corriendo en: `https://localhost:5006`

### 3. Obtener Token de Autenticación

```bash
# Login como Admin
POST https://localhost:5006/api/auth/login
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

Guardar el token de la respuesta para usar en todas las pruebas.

---

## 📝 Casos de Prueba

### GRUPO 1: Autenticación y Autorización

#### TEST-AUTH-01: Login exitoso como Admin
**Objetivo:** Verificar que un admin puede hacer login

```bash
POST /api/auth/login
Body: { "email": "admin@test.com", "password": "Admin123!" }

Resultado esperado:
✅ Status: 200 OK
✅ Response incluye token JWT
✅ Response incluye role: "Admin"
```

#### TEST-AUTH-02: Acceso con token válido
**Objetivo:** Verificar que se puede acceder con token

```bash
GET /api/admin/orders
Authorization: Bearer {token_valido}

Resultado esperado:
✅ Status: 200 OK
✅ Response incluye lista de pedidos
```

#### TEST-AUTH-03: Acceso sin token
**Objetivo:** Verificar que se requiere autenticación

```bash
GET /api/admin/orders
(Sin header Authorization)

Resultado esperado:
❌ Status: 401 Unauthorized
```

#### TEST-AUTH-04: Acceso con rol Customer
**Objetivo:** Verificar que un cliente no puede acceder

```bash
# Login como Customer
POST /api/auth/login
Body: { "email": "customer@test.com", "password": "Customer123!" }

# Intentar acceder a admin
GET /api/admin/orders
Authorization: Bearer {token_customer}

Resultado esperado:
❌ Status: 403 Forbidden
```

---

### GRUPO 2: Listar Pedidos (GET /api/admin/orders)

#### TEST-LIST-01: Listar todos los pedidos
**Objetivo:** Obtener lista completa sin filtros

```bash
GET /api/admin/orders?page=1&limit=10
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response.orders es un array
✅ Response.pagination.total >= 5 (datos de prueba)
✅ Response.pagination.page = 1
✅ Response.pagination.pages >= 1
```

#### TEST-LIST-02: Paginación - Primera página
**Objetivo:** Verificar paginación correcta

```bash
GET /api/admin/orders?page=1&limit=2
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response.orders.length = 2
✅ Response.pagination.limit = 2
✅ Response.pagination.page = 1
```

#### TEST-LIST-03: Paginación - Segunda página
**Objetivo:** Verificar navegación entre páginas

```bash
GET /api/admin/orders?page=2&limit=2
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response.orders.length >= 1
✅ Response.pagination.page = 2
✅ IDs diferentes a la página 1
```

#### TEST-LIST-04: Límite máximo
**Objetivo:** Verificar que se respeta el límite máximo

```bash
GET /api/admin/orders?page=1&limit=200
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response.orders.length <= 100 (límite máximo)
```

#### TEST-LIST-05: Filtrar por estado "pending"
**Objetivo:** Obtener solo pedidos pendientes

```bash
GET /api/admin/orders?status=pending
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los orders tienen status: "pending"
✅ No aparecen pedidos con otros estados
```

#### TEST-LIST-06: Filtrar por estado "processing"
**Objetivo:** Obtener solo pedidos en proceso

```bash
GET /api/admin/orders?status=processing
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los orders tienen status: "processing"
```

#### TEST-LIST-07: Filtrar por estado "delivered"
**Objetivo:** Obtener solo pedidos entregados

```bash
GET /api/admin/orders?status=delivered
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los orders tienen status: "delivered"
✅ Al menos 2 pedidos (según datos de prueba)
```

#### TEST-LIST-08: Filtrar por estado "cancelled"
**Objetivo:** Obtener solo pedidos cancelados

```bash
GET /api/admin/orders?status=cancelled
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los orders tienen status: "cancelled"
✅ Al menos 1 pedido (según datos de prueba)
```

#### TEST-LIST-09: Búsqueda por nombre de cliente
**Objetivo:** Buscar pedidos por nombre

```bash
GET /api/admin/orders?search=Test
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los orders contienen "Test" en customerName
```

#### TEST-LIST-10: Búsqueda por email
**Objetivo:** Buscar pedidos por email del cliente

```bash
GET /api/admin/orders?search=test.com
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los orders contienen "test.com" en customerEmail
```

#### TEST-LIST-11: Búsqueda por ID
**Objetivo:** Buscar un pedido específico por su ID

```bash
GET /api/admin/orders?search=1
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response incluye el pedido con ID = 1
```

#### TEST-LIST-12: Búsqueda sin resultados
**Objetivo:** Manejar búsquedas sin coincidencias

```bash
GET /api/admin/orders?search=xxxxxxxxx
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response.orders = []
✅ Response.pagination.total = 0
```

#### TEST-LIST-13: Combinación de filtros
**Objetivo:** Usar múltiples filtros simultáneamente

```bash
GET /api/admin/orders?status=pending&search=Test&page=1&limit=5
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Todos los pedidos son "pending" Y contienen "Test"
```

#### TEST-LIST-14: Validación de estructura de datos
**Objetivo:** Verificar que cada pedido tiene todos los campos

```bash
GET /api/admin/orders?page=1&limit=1
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ order.id existe y es número
✅ order.customerName existe y es string
✅ order.customerEmail existe y es string válido
✅ order.items existe y es número > 0
✅ order.amount existe y es decimal > 0
✅ order.status existe y es válido
✅ order.createdAt existe y es fecha ISO
✅ order.updatedAt existe y es fecha ISO
```

---

### GRUPO 3: Detalles del Pedido (GET /api/admin/orders/{id})

#### TEST-DETAIL-01: Obtener pedido existente
**Objetivo:** Recuperar detalles completos de un pedido

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Response.id = 1
✅ Response.customer existe con id, name, email
✅ Response.shippingAddress existe y está parseada
✅ Response.orderItems es array con al menos 1 item
✅ Response.statusHistory es array
✅ Response.subtotal + shipping = total
```

#### TEST-DETAIL-02: Pedido no existente
**Objetivo:** Manejar IDs que no existen

```bash
GET /api/admin/orders/99999
Authorization: Bearer {token}

Resultado esperado:
❌ Status: 404 Not Found
✅ Response.error = "Pedido no encontrado"
✅ Response.orderId = 99999
```

#### TEST-DETAIL-03: Validar estructura de customer
**Objetivo:** Verificar información del cliente

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ Response.customer.id existe
✅ Response.customer.name existe
✅ Response.customer.email existe
✅ Response.customer.phone existe (puede ser null)
```

#### TEST-DETAIL-04: Validar estructura de shippingAddress
**Objetivo:** Verificar dirección de envío parseada

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ Response.shippingAddress.street existe
✅ Response.shippingAddress.city existe
✅ Response.shippingAddress.state existe
✅ Response.shippingAddress.zipCode existe
✅ Response.shippingAddress.country existe
```

#### TEST-DETAIL-05: Validar orderItems
**Objetivo:** Verificar items del pedido

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ Response.orderItems.length >= 1
✅ Cada item tiene: productId, name, quantity, price, subtotal
✅ item.quantity * item.price = item.subtotal
✅ item.imageUrl existe (puede ser null)
```

#### TEST-DETAIL-06: Validar statusHistory
**Objetivo:** Verificar historial de estados

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ Response.statusHistory.length >= 1
✅ Cada entrada tiene: status, timestamp
✅ Ordenado del más reciente al más antiguo
✅ note puede ser null o string
```

#### TEST-DETAIL-07: Validar cálculos
**Objetivo:** Verificar que los totales sean correctos

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ subtotal + shipping = total
✅ sum(orderItems.subtotal) = subtotal
```

---

### GRUPO 4: Actualizar Estado (PUT /api/admin/orders/{id}/status)

#### TEST-UPDATE-01: Cambiar estado a "processing"
**Objetivo:** Actualizar pedido pendiente a procesando

```bash
PUT /api/admin/orders/3/status
Authorization: Bearer {token}
Body: { "status": "processing", "note": "Pedido en preparación" }

Resultado esperado:
✅ Status: 200 OK
✅ Response.id = 3
✅ Response.status = "processing"
✅ Response.message existe
✅ Response.updatedAt es fecha reciente
```

#### TEST-UPDATE-02: Verificar actualización en base de datos
**Objetivo:** Confirmar que el cambio se guardó

```bash
# Primero actualizar
PUT /api/admin/orders/3/status
Body: { "status": "processing" }

# Luego verificar
GET /api/admin/orders/3
Authorization: Bearer {token}

Resultado esperado:
✅ Response.status = "processing"
✅ statusHistory incluye nueva entrada
✅ statusHistory[0].status = "processing"
✅ statusHistory[0].note = "Pedido en preparación"
```

#### TEST-UPDATE-03: Cambiar estado a "delivered"
**Objetivo:** Marcar pedido como entregado

```bash
PUT /api/admin/orders/2/status
Authorization: Bearer {token}
Body: { "status": "delivered", "note": "Entregado con éxito" }

Resultado esperado:
✅ Status: 200 OK
✅ Response.status = "delivered"
```

#### TEST-UPDATE-04: Cambiar estado a "cancelled"
**Objetivo:** Cancelar un pedido

```bash
PUT /api/admin/orders/3/status
Authorization: Bearer {token}
Body: { "status": "cancelled", "note": "Cancelado por el cliente" }

Resultado esperado:
✅ Status: 200 OK
✅ Response.status = "cancelled"
```

#### TEST-UPDATE-05: Estado inválido
**Objetivo:** Rechazar estados no permitidos

```bash
PUT /api/admin/orders/1/status
Authorization: Bearer {token}
Body: { "status": "invalid_status" }

Resultado esperado:
❌ Status: 400 Bad Request
✅ Response.error = "Estado inválido"
✅ Response.validStatuses incluye: pending, processing, delivered, cancelled
```

#### TEST-UPDATE-06: Pedido no existente
**Objetivo:** Manejar IDs que no existen

```bash
PUT /api/admin/orders/99999/status
Authorization: Bearer {token}
Body: { "status": "processing" }

Resultado esperado:
❌ Status: 404 Not Found
✅ Response.error = "Pedido no encontrado"
✅ Response.orderId = 99999
```

#### TEST-UPDATE-07: Actualizar sin nota
**Objetivo:** Verificar que la nota es opcional

```bash
PUT /api/admin/orders/1/status
Authorization: Bearer {token}
Body: { "status": "processing" }

Resultado esperado:
✅ Status: 200 OK
✅ Response.status = "processing"
```

#### TEST-UPDATE-08: Nota muy larga
**Objetivo:** Validar longitud máxima de nota

```bash
PUT /api/admin/orders/1/status
Authorization: Bearer {token}
Body: { 
  "status": "processing",
  "note": "a".repeat(501) // 501 caracteres
}

Resultado esperado:
❌ Status: 400 Bad Request
✅ Response.error menciona longitud máxima
```

#### TEST-UPDATE-09: Verificar log de actividad
**Objetivo:** Confirmar que se registra en ActivityLogs

```bash
# Actualizar estado
PUT /api/admin/orders/1/status
Body: { "status": "processing" }

# Verificar actividad
GET /api/admin/activity/recent
Authorization: Bearer {token}

Resultado esperado:
✅ Status: 200 OK
✅ Lista incluye entrada: "Pedido #1 actualizado a processing"
```

#### TEST-UPDATE-10: Múltiples actualizaciones
**Objetivo:** Verificar historial con varios cambios

```bash
# Primera actualización
PUT /api/admin/orders/1/status
Body: { "status": "processing" }

# Segunda actualización
PUT /api/admin/orders/1/status
Body: { "status": "delivered" }

# Verificar historial
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ statusHistory.length >= 2
✅ statusHistory[0].status = "delivered" (más reciente)
✅ statusHistory[1].status = "processing"
```

---

### GRUPO 5: Rendimiento

#### TEST-PERF-01: Tiempo de respuesta - Lista
**Objetivo:** Verificar que la lista carga rápidamente

```bash
GET /api/admin/orders?page=1&limit=20
Authorization: Bearer {token}

Resultado esperado:
✅ Tiempo de respuesta < 500ms
```

#### TEST-PERF-02: Tiempo de respuesta - Detalles
**Objetivo:** Verificar carga rápida de detalles

```bash
GET /api/admin/orders/1
Authorization: Bearer {token}

Resultado esperado:
✅ Tiempo de respuesta < 300ms
```

#### TEST-PERF-03: Carga con muchos registros
**Objetivo:** Verificar rendimiento con 100 items

```bash
GET /api/admin/orders?page=1&limit=100
Authorization: Bearer {token}

Resultado esperado:
✅ Tiempo de respuesta < 1000ms
✅ No hay errores de timeout
```

---

## 📊 Resultados Esperados

### Resumen de Cobertura

| Categoría | Tests | Esperados |
|-----------|-------|-----------|
| Autenticación | 4 | ✅ 4 exitosos |
| Listar Pedidos | 14 | ✅ 14 exitosos |
| Detalles | 7 | ✅ 7 exitosos |
| Actualizar Estado | 10 | ✅ 10 exitosos |
| Rendimiento | 3 | ✅ 3 exitosos |
| **TOTAL** | **38** | **✅ 38 exitosos** |

---

## 🔍 Validaciones Adicionales

### SQL: Verificar datos en BD

```sql
-- Verificar que se crearon órdenes
SELECT COUNT(*) FROM Orders; -- Debe ser >= 5

-- Verificar estados
SELECT Status, COUNT(*) FROM Orders GROUP BY Status;

-- Verificar items de pedidos
SELECT o.Id, COUNT(oi.Id) as ItemCount
FROM Orders o
LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
GROUP BY o.Id;

-- Verificar historial
SELECT OrderId, COUNT(*) as HistoryCount
FROM OrderStatusHistory
GROUP BY OrderId;
```

---

## 📝 Registro de Pruebas

### Template para documentar resultados:

```
TEST ID: TEST-LIST-01
Fecha: [Fecha de ejecución]
Ejecutado por: [Nombre]
Resultado: ✅ PASS / ❌ FAIL
Tiempo de respuesta: [ms]
Observaciones: [Notas adicionales]
```

---

## ✅ Checklist Final

Antes de considerar las pruebas completas:

- [ ] Todos los tests de autenticación pasan
- [ ] Todos los tests de listado pasan
- [ ] Todos los tests de detalles pasan
- [ ] Todos los tests de actualización pasan
- [ ] Todos los tests de rendimiento pasan
- [ ] Sin errores en logs del backend
- [ ] Sin errores en logs del frontend
- [ ] Datos en BD son consistentes
- [ ] Historial de estados se registra correctamente
- [ ] Logs de actividad se crean apropiadamente

---

## 🆘 Troubleshooting

### Si algún test falla:

1. **Verificar logs del backend** (Visual Studio → Output)
2. **Verificar datos en BD** (usar queries SQL arriba)
3. **Verificar token JWT** (no expirado, rol correcto)
4. **Verificar URL del backend** (https://localhost:5006)
5. **Reiniciar backend** si es necesario
6. **Limpiar y re-insertar datos de prueba** si están corruptos

---

**¡Buena suerte con las pruebas!** 🚀

Para cualquier problema, consulta `ORDER-MANAGEMENT-API-READY.md`
