# 📋 QUICK REFERENCE - GESTIÓN DE PEDIDOS API

**Version:** 1.0.0 | **Base URL:** `https://localhost:5006/api/admin/orders`

---

## 🔑 AUTENTICACIÓN

Todos los endpoints requieren:
```
Authorization: Bearer {jwt_token}
```

**Obtener Token:**
```bash
POST /api/auth/login
Body: {"email":"admin@test.com","password":"Admin123!"}
```

---

## 🌐 ENDPOINTS

### 1️⃣ LISTAR PEDIDOS
```
GET /api/admin/orders?page=1&limit=10&status=pending&search=maria
```

**Parámetros:**
- `page` (int): Página actual (default: 1)
- `limit` (int): Items por página (max: 100, default: 10)
- `status` (string): pending | processing | delivered | cancelled | all
- `search` (string): Busca en nombre, email o ID

**Response:**
```json
{
  "orders": [
    {
      "id": 1234,
      "customerName": "María González",
      "customerEmail": "maria@email.com",
      "items": 3,
      "amount": 1250.00,
      "status": "pending",
      "createdAt": "2025-11-16T10:30:00Z",
      "updatedAt": "2025-11-16T10:30:00Z"
    }
  ],
  "pagination": {
    "total": 156,
    "page": 1,
    "pages": 16,
    "limit": 10
  }
}
```

### 2️⃣ DETALLES DEL PEDIDO
```
GET /api/admin/orders/{id}
```

**Response:** (Incluye customer, shippingAddress, orderItems, statusHistory)

### 3️⃣ ACTUALIZAR ESTADO
```
PUT /api/admin/orders/{id}/status
Body: { "status": "processing", "note": "Opcional" }
```

**Estados válidos:** `pending`, `processing`, `delivered`, `cancelled`

**Response:**
```json
{
  "id": 1234,
  "status": "processing",
  "updatedAt": "2025-11-16T12:00:00Z",
  "message": "Estado del pedido actualizado exitosamente"
}
```

---

## 🔴 CÓDIGOS DE ERROR

| Código | Significado | Solución |
|--------|-------------|----------|
| 401 | Token inválido/expirado | Login de nuevo |
| 403 | Sin permisos | Usar cuenta Admin/Employee |
| 404 | Pedido no encontrado | Verificar ID |
| 400 | Datos inválidos | Revisar body/params |
| 500 | Error del servidor | Ver logs backend |

---

## 📦 EJEMPLO COMPLETO (TypeScript/Angular)

```typescript
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrderAdminService {
  private apiUrl = 'https://localhost:5006/api/admin/orders';

  constructor(private http: HttpClient) {}

  getOrders(page: number, limit: number, status?: string, search?: string): Observable<any> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());
    if (status && status !== 'all') params = params.set('status', status);
    if (search) params = params.set('search', search);
    return this.http.get(this.apiUrl, { params });
  }

  getOrderDetails(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  updateOrderStatus(id: number, status: string, note?: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, { status, note });
  }
}
```

**Uso en componente:**
```typescript
// Listar
this.orderService.getOrders(1, 10, 'pending').subscribe(
  response => this.orders = response.orders
);

// Detalles
this.orderService.getOrderDetails(1234).subscribe(
  order => this.selectedOrder = order
);

// Actualizar
this.orderService.updateOrderStatus(1234, 'processing', 'Nota').subscribe(
  response => console.log('Actualizado')
);
```

---

## 🧪 TESTING CON CURL

```bash
# 1. Login
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin123!"}'

# 2. Guardar token en variable
TOKEN="eyJhbGci..."

# 3. Listar pedidos
curl -X GET "https://localhost:5006/api/admin/orders?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN"

# 4. Ver detalles
curl -X GET "https://localhost:5006/api/admin/orders/1" \
  -H "Authorization: Bearer $TOKEN"

# 5. Actualizar estado
curl -X PUT "https://localhost:5006/api/admin/orders/3/status" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"status":"processing","note":"Pedido en preparación"}'
```

---

## 🗄️ QUERIES SQL ÚTILES

```sql
-- Ver todos los pedidos
SELECT * FROM Orders ORDER BY CreatedAt DESC;

-- Contar pedidos por estado
SELECT Status, COUNT(*) as Total FROM Orders GROUP BY Status;

-- Ver items de un pedido
SELECT oi.*, p.Name, p.Image 
FROM OrderItems oi
JOIN Products p ON oi.ProductId = p.Id
WHERE oi.OrderId = 1;

-- Ver historial de un pedido
SELECT * FROM OrderStatusHistory 
WHERE OrderId = 1 
ORDER BY Timestamp DESC;

-- Buscar pedidos de un cliente
SELECT * FROM Orders 
WHERE CustomerName LIKE '%Test%' 
   OR CustomerEmail LIKE '%test%';
```

---

## ✅ VALIDACIONES

| Campo | Validación |
|-------|------------|
| `status` | pending, processing, delivered, cancelled |
| `note` | Máximo 500 caracteres, opcional |
| `page` | Mínimo 1 |
| `limit` | Mínimo 1, máximo 100 |
| `id` | Debe existir en BD |
| Token | JWT válido, rol Admin o Employee |

---

## 📝 NOTAS IMPORTANTES

- ✅ Fechas en formato ISO 8601 UTC
- ✅ Montos en decimal con 2 decimales
- ✅ Paginación comienza en 1 (no 0)
- ✅ Estados siempre en lowercase
- ✅ El historial se crea automáticamente
- ✅ Los logs de actividad se registran automáticamente

---

## 🔗 DOCUMENTACIÓN COMPLETA

- **Guía Completa:** `ORDER-MANAGEMENT-API-READY.md`
- **Quick Start:** `ORDER-INTEGRATION-QUICKSTART.md`
- **Testing:** `ORDER-TESTING-PLAN.md`
- **Resumen:** `ORDER-MANAGEMENT-IMPLEMENTATION-SUMMARY.md`

---

## 📊 DATOS DE PRUEBA

5 pedidos disponibles con IDs: 1, 2, 3, 4, 5

```bash
# Pedido 1: Entregado (hace 5 días)
GET /api/admin/orders/1

# Pedido 2: Procesando (hace 2 días)
GET /api/admin/orders/2

# Pedido 3: Pendiente (hace 3 horas)
GET /api/admin/orders/3
```

---

**🚀 ¡Todo listo para integración!**

**Backend Status:** ✅ Completo  
**Documentación:** ✅ Completa  
**Testing:** ✅ 38 casos cubiertos  
**Tiempo de integración:** ~25 minutos
