# 📦 GESTIÓN DE PEDIDOS - API BACKEND IMPLEMENTADA

**Fecha de Implementación:** 16 de Noviembre 2025  
**Estado:** ✅ **COMPLETAMENTE IMPLEMENTADO Y LISTO PARA INTEGRACIÓN**

---

## 🎉 RESUMEN DE IMPLEMENTACIÓN

Todos los endpoints requeridos para la gestión de pedidos han sido implementados y están listos para su uso:

✅ **GET /api/admin/orders** - Listado con filtros y paginación  
✅ **GET /api/admin/orders/{id}** - Detalles completos del pedido  
✅ **PUT /api/admin/orders/{id}/status** - Actualización de estado

---

## 🔌 ENDPOINTS DISPONIBLES

### 1. **Listar Pedidos con Filtros y Paginación**

#### `GET /api/admin/orders`

**URL Completa:**
```
GET https://localhost:5006/api/admin/orders?page=1&limit=10&status=pending&search=maria
```

**Headers Requeridos:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Query Parameters:**

| Parámetro | Tipo | Requerido | Default | Descripción |
|-----------|------|-----------|---------|-------------|
| `page` | int | No | 1 | Número de página |
| `limit` | int | No | 10 | Items por página (máx: 100) |
| `status` | string | No | null | Filtrar por estado: `pending`, `processing`, `delivered`, `cancelled`, `all` |
| `search` | string | No | null | Buscar por nombre, email o ID de pedido |
| `sortBy` | string | No | `createdAt` | Campo para ordenar |
| `sortOrder` | string | No | `desc` | Orden: `asc` o `desc` |

**Ejemplos de Uso:**

```typescript
// Todos los pedidos (primera página)
GET /api/admin/orders

// Pedidos pendientes
GET /api/admin/orders?status=pending

// Buscar por cliente
GET /api/admin/orders?search=maria

// Página específica con límite
GET /api/admin/orders?page=2&limit=20

// Combinación de filtros
GET /api/admin/orders?page=1&limit=10&status=processing&search=juan
```

**Respuesta Exitosa (200 OK):**

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
    },
    {
      "id": 1233,
      "customerName": "Carlos Rodríguez",
      "customerEmail": "carlos@email.com",
      "items": 2,
      "amount": 890.50,
      "status": "processing",
      "createdAt": "2025-11-16T09:15:00Z",
      "updatedAt": "2025-11-16T11:45:00Z"
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

**Autorización:** `Admin`, `Employee`

---

### 2. **Obtener Detalles Completos de un Pedido**

#### `GET /api/admin/orders/{id}`

**URL Completa:**
```
GET https://localhost:5006/api/admin/orders/1234
```

**Headers Requeridos:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Respuesta Exitosa (200 OK):**

```json
{
  "id": 1234,
  "customerName": "María González",
  "customerEmail": "maria@email.com",
  "items": 3,
  "amount": 1250.00,
  "status": "pending",
  "createdAt": "2025-11-16T10:30:00Z",
  "updatedAt": "2025-11-16T10:30:00Z",
  "customer": {
    "id": 567,
    "name": "María González",
    "email": "maria@email.com",
    "phone": "+34 612 345 678"
  },
  "shippingAddress": {
    "street": "Calle Principal 123",
    "city": "Madrid",
    "state": "Madrid",
    "zipCode": "28001",
    "country": "España"
  },
  "orderItems": [
    {
      "productId": 1,
      "name": "Camisa Casual Bosko",
      "quantity": 2,
      "price": 49.99,
      "subtotal": 99.98,
      "imageUrl": "https://images.unsplash.com/photo-1596755094514-f87e34085b2c"
    },
    {
      "productId": 5,
      "name": "Pantalón Slim Fit Negro",
      "quantity": 1,
      "price": 69.99,
      "subtotal": 69.99,
      "imageUrl": "https://images.unsplash.com/photo-1473966968600-fa801b869a1a"
    }
  ],
  "subtotal": 1200.00,
  "shipping": 50.00,
  "total": 1250.00,
  "paymentMethod": "credit_card",
  "statusHistory": [
    {
      "status": "pending",
      "timestamp": "2025-11-16T10:30:00Z",
      "note": "Pedido recibido"
    }
  ]
}
```

**Respuesta Error (404 Not Found):**

```json
{
  "error": "Pedido no encontrado",
  "orderId": 1234
}
```

**Autorización:** `Admin`, `Employee`

---

### 3. **Actualizar Estado del Pedido**

#### `PUT /api/admin/orders/{id}/status`

**URL Completa:**
```
PUT https://localhost:5006/api/admin/orders/1234/status
```

**Headers Requeridos:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Body (JSON):**

```json
{
  "status": "processing",
  "note": "Pedido en preparación en almacén"
}
```

**Campos del Body:**

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `status` | string | Sí | Nuevo estado: `pending`, `processing`, `delivered`, `cancelled` |
| `note` | string | No | Nota opcional sobre el cambio (máx. 500 caracteres) |

**Respuesta Exitosa (200 OK):**

```json
{
  "id": 1234,
  "status": "processing",
  "updatedAt": "2025-11-16T12:00:00Z",
  "message": "Estado del pedido actualizado exitosamente"
}
```

**Respuesta Error (400 Bad Request) - Estado inválido:**

```json
{
  "error": "Estado inválido",
  "validStatuses": ["pending", "processing", "delivered", "cancelled"]
}
```

**Respuesta Error (404 Not Found):**

```json
{
  "error": "Pedido no encontrado",
  "orderId": 1234
}
```

**Autorización:** `Admin`, `Employee`

---

## 🔐 AUTENTICACIÓN Y SEGURIDAD

### Headers Requeridos

Todos los endpoints requieren autenticación mediante JWT:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Roles Permitidos

- ✅ **Admin**: Acceso completo a todos los endpoints
- ✅ **Employee**: Acceso completo a todos los endpoints
- ❌ **Customer**: Sin acceso

### Validaciones Implementadas

✅ Token JWT válido y no expirado  
✅ Usuario con rol Admin o Employee  
✅ Estados válidos para actualización  
✅ Longitud máxima de notas (500 caracteres)  
✅ Límite máximo de paginación (100 items)  
✅ Validación de IDs numéricos  

---

## 📊 CARACTERÍSTICAS IMPLEMENTADAS

### Búsqueda Inteligente

El parámetro `search` busca en:
- ✅ Nombre del cliente (`customerName`)
- ✅ Email del cliente (`customerEmail`)
- ✅ ID del pedido (numérico convertido a string)

**Ejemplo:**
```
GET /api/admin/orders?search=maria
// Encuentra: "María González", "maria@email.com", pedidos de Maria, etc.
```

### Filtrado por Estado

Estados soportados:
- `pending` - Pendiente
- `processing` - En proceso
- `delivered` - Entregado
- `cancelled` - Cancelado
- `all` - Todos (sin filtro)

### Paginación

- **Default:** 10 items por página
- **Máximo:** 100 items por página
- **Primera página:** `page=1`
- Respuesta incluye total de items y páginas

### Historial de Estados

Cada cambio de estado se registra automáticamente con:
- ✅ Estado anterior y nuevo
- ✅ Timestamp exacto
- ✅ Nota opcional del administrador
- ✅ Ordenado del más reciente al más antiguo

### Logs de Actividad

Cada actualización de estado genera un registro de actividad:
```
"Pedido #1234 actualizado a processing"
```

---

## 🗄️ ESTRUCTURA DE BASE DE DATOS

### Tabla: Orders

```sql
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerEmail NVARCHAR(255) NOT NULL,
    ShippingAddress NVARCHAR(500),
    Subtotal DECIMAL(18, 2) NOT NULL,
    Shipping DECIMAL(18, 2) NOT NULL,
    Total DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'pending',
    PaymentMethod NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Users(Id)
);

-- Índices para optimización
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt DESC);
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
```

### Tabla: OrderItems

```sql
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Subtotal DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
```

### Tabla: OrderStatusHistory

```sql
CREATE TABLE OrderStatusHistory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Note NVARCHAR(500),
    Timestamp DATETIME NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);

CREATE INDEX IX_OrderStatusHistory_OrderId ON OrderStatusHistory(OrderId);
CREATE INDEX IX_OrderStatusHistory_Timestamp ON OrderStatusHistory(Timestamp DESC);
```

---

## 🧪 DATOS DE PRUEBA

Los siguientes pedidos están disponibles en la base de datos para testing:

| ID | Cliente | Items | Total | Estado | Fecha |
|----|---------|-------|-------|--------|-------|
| 1 | Cliente Test | 3 | 284.97€ | delivered | Hace 5 días |
| 2 | Cliente Test | 1 | 161.98€ | processing | Hace 2 días |
| 3 | Cliente Test | 3 | 229.96€ | pending | Hace 3 horas |
| 4 | Cliente Test | 1 | 204.99€ | delivered | Hace 10 días |
| 5 | Cliente Test | 1 | 131.99€ | cancelled | Hace 8 días |

### Obtener Datos de Prueba

```bash
# Listar todos los pedidos
GET /api/admin/orders

# Ver detalles del primer pedido
GET /api/admin/orders/1

# Filtrar por estado pendiente
GET /api/admin/orders?status=pending
```

---

## 🚀 INTEGRACIÓN CON FRONTEND

### Paso 1: Crear el Servicio Angular

Crea el archivo: `src/app/services/order-admin.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config/api.config';

export interface OrdersListResponse {
  orders: Order[];
  pagination: {
    total: number;
    page: number;
    pages: number;
    limit: number;
  };
}

export interface Order {
  id: number;
  customerName: string;
  customerEmail: string;
  items: number;
  amount: number;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface OrderDetail extends Order {
  customer: {
    id: number;
    name: string;
    email: string;
    phone?: string;
  };
  shippingAddress: {
    street: string;
    city: string;
    state: string;
    zipCode: string;
    country: string;
  };
  orderItems: Array<{
    productId: number;
    name: string;
    quantity: number;
    price: number;
    subtotal: number;
    imageUrl?: string;
  }>;
  subtotal: number;
  shipping: number;
  total: number;
  paymentMethod: string;
  statusHistory: Array<{
    status: string;
    timestamp: string;
    note?: string;
  }>;
}

export interface UpdateStatusRequest {
  status: string;
  note?: string;
}

@Injectable({ providedIn: 'root' })
export class OrderAdminService {
  private apiUrl = `${API_URL}/api/admin/orders`;

  constructor(private http: HttpClient) {}

  /**
   * Obtiene lista paginada de pedidos con filtros
   */
  getOrders(
    page: number = 1,
    limit: number = 10,
    status?: string,
    search?: string
  ): Observable<OrdersListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());

    if (status && status !== 'all') {
      params = params.set('status', status);
    }

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<OrdersListResponse>(this.apiUrl, { params });
  }

  /**
   * Obtiene detalles completos de un pedido
   */
  getOrderDetails(id: number): Observable<OrderDetail> {
    return this.http.get<OrderDetail>(`${this.apiUrl}/${id}`);
  }

  /**
   * Actualiza el estado de un pedido
   */
  updateOrderStatus(
    id: number,
    status: string,
    note?: string
  ): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, { status, note });
  }
}
```

### Paso 2: Actualizar el Componente

Modifica: `admin/pages/orders/order-management.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { OrderAdminService, Order, OrderDetail } from '../../../services/order-admin.service';

@Component({
  selector: 'app-order-management',
  templateUrl: './order-management.component.html',
  styleUrls: ['./order-management.component.css']
})
export class OrderManagementComponent implements OnInit {
  orders: Order[] = [];
  isLoading = false;
  currentPage = 1;
  totalPages = 1;
  itemsPerPage = 10;
  statusFilter = 'all';
  searchQuery = '';

  // Modals
  showDetailsModal = false;
  showStatusModal = false;
  selectedOrder: OrderDetail | null = null;
  newStatus = '';
  statusNote = '';

  constructor(private orderService: OrderAdminService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    
    this.orderService.getOrders(
      this.currentPage,
      this.itemsPerPage,
      this.statusFilter,
      this.searchQuery
    ).subscribe({
      next: (response) => {
        this.orders = response.orders;
        this.totalPages = response.pagination.pages;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error cargando pedidos:', error);
        this.isLoading = false;
        // Mostrar mensaje de error al usuario
      }
    });
  }

  viewOrderDetails(orderId: number): void {
    this.orderService.getOrderDetails(orderId).subscribe({
      next: (order) => {
        this.selectedOrder = order;
        this.showDetailsModal = true;
      },
      error: (error) => {
        console.error('Error cargando detalles del pedido:', error);
        // Mostrar mensaje de error al usuario
      }
    });
  }

  openStatusModal(order: Order): void {
    this.orderService.getOrderDetails(order.id).subscribe({
      next: (orderDetail) => {
        this.selectedOrder = orderDetail;
        this.newStatus = orderDetail.status;
        this.statusNote = '';
        this.showStatusModal = true;
      },
      error: (error) => {
        console.error('Error cargando pedido:', error);
      }
    });
  }

  updateOrderStatus(): void {
    if (!this.selectedOrder) return;

    this.orderService.updateOrderStatus(
      this.selectedOrder.id,
      this.newStatus,
      this.statusNote
    ).subscribe({
      next: (response) => {
        console.log('Estado actualizado:', response);
        this.showStatusModal = false;
        this.loadOrders(); // Recargar lista
        // Mostrar mensaje de éxito al usuario
      },
      error: (error) => {
        console.error('Error actualizando estado:', error);
        // Mostrar mensaje de error al usuario
      }
    });
  }

  applyFilters(): void {
    this.currentPage = 1; // Reset to first page
    this.loadOrders();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadOrders();
  }

  onSearchChange(query: string): void {
    this.searchQuery = query;
    this.applyFilters();
  }

  onStatusFilterChange(status: string): void {
    this.statusFilter = status;
    this.applyFilters();
  }
}
```

### Paso 3: Configurar el API URL

Asegúrate de tener configurado el API_URL en `src/app/config/api.config.ts`:

```typescript
export const API_URL = 'https://localhost:5006';
```

### Paso 4: Verificar Interceptor de Autenticación

El interceptor debe agregar automáticamente el token JWT:

```typescript
// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem('auth_token');
    
    if (token) {
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      return next.handle(cloned);
    }
    
    return next.handle(req);
  }
}
```

---

## 🧪 TESTING

### Usando Postman o Thunder Client

#### 1. Login y Obtener Token

```
POST https://localhost:5006/api/auth/login
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

Copia el `token` de la respuesta.

#### 2. Listar Pedidos

```
GET https://localhost:5006/api/admin/orders?page=1&limit=10
Authorization: Bearer {tu_token_aqui}
```

#### 3. Ver Detalles de un Pedido

```
GET https://localhost:5006/api/admin/orders/1
Authorization: Bearer {tu_token_aqui}
```

#### 4. Actualizar Estado

```
PUT https://localhost:5006/api/admin/orders/3/status
Authorization: Bearer {tu_token_aqui}
Content-Type: application/json

{
  "status": "processing",
  "note": "Pedido en preparación"
}
```

### Usando cURL

```bash
# Login
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin123!"}'

# Listar pedidos (reemplaza {TOKEN} con el token obtenido)
curl -X GET "https://localhost:5006/api/admin/orders?page=1&limit=10" \
  -H "Authorization: Bearer {TOKEN}"

# Ver detalles
curl -X GET "https://localhost:5006/api/admin/orders/1" \
  -H "Authorization: Bearer {TOKEN}"

# Actualizar estado
curl -X PUT "https://localhost:5006/api/admin/orders/3/status" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"status":"processing","note":"Pedido en preparación"}'
```

---

## ⚡ RENDIMIENTO Y OPTIMIZACIONES

### Índices de Base de Datos Implementados

✅ `IX_Orders_Status` - Para filtrado por estado  
✅ `IX_Orders_CreatedAt` - Para ordenamiento por fecha  
✅ `IX_Orders_CustomerId` - Para búsquedas por cliente  
✅ `IX_OrderItems_OrderId` - Para joins eficientes  
✅ `IX_OrderStatusHistory_OrderId` - Para historial  

### Consultas Optimizadas

- ✅ Uso de `Include()` para evitar N+1 queries
- ✅ Proyección con `Select()` para minimizar datos transferidos
- ✅ Paginación eficiente con `Skip()` y `Take()`
- ✅ Búsqueda con índices en campos apropiados

### Recomendaciones

- Implementar caché para pedidos frecuentemente consultados
- Considerar paginación con cursor para grandes volúmenes
- Agregar rate limiting (ya incluido en documentación)

---

## 📋 PRÓXIMOS PASOS

### Para el Equipo Frontend:

1. ✅ **Implementar el servicio Angular** según el código proporcionado
2. ✅ **Actualizar el componente** para usar el servicio real
3. ✅ **Probar con datos reales** de la BD
4. ✅ **Manejar errores** y mostrar mensajes apropiados
5. ✅ **Agregar notificaciones** de éxito/error (toast notifications)

### Mejoras Futuras (Opcional):

- [ ] Exportar pedidos a CSV/Excel
- [ ] Filtros avanzados (rango de fechas, monto)
- [ ] Notificaciones en tiempo real con SignalR
- [ ] Impresión de etiquetas de envío
- [ ] Integración con sistemas de mensajería (email, SMS)

---

## 🆘 SOPORTE Y CONTACTO

Si encuentras algún problema durante la integración:

1. **Verifica** que el token JWT esté siendo enviado correctamente
2. **Revisa** los logs del backend en Visual Studio (Output window)
3. **Comprueba** que la BD tenga datos de prueba
4. **Valida** que el usuario tenga rol Admin o Employee

### Errores Comunes:

**401 Unauthorized:**
- Token JWT expirado o inválido
- Usuario sin rol apropiado

**404 Not Found:**
- ID de pedido no existe
- URL del endpoint incorrecta

**400 Bad Request:**
- Estado inválido en actualización
- Formato de datos incorrecto

---

## ✅ CHECKLIST DE INTEGRACIÓN

### Backend (Completado)
- [x] Endpoints implementados
- [x] DTOs configurados
- [x] Validaciones implementadas
- [x] Autorización configurada
- [x] Base de datos configurada
- [x] Datos de prueba insertados
- [x] Documentación completa

### Frontend (Pendiente)
- [ ] Servicio Angular creado
- [ ] Componente actualizado
- [ ] Interceptor configurado
- [ ] Manejo de errores implementado
- [ ] Testing completado
- [ ] Integración verificada

---

## 📚 DOCUMENTACIÓN RELACIONADA

- [AUTHENTICATION-SUMMARY.txt](./AUTHENTICATION-SUMMARY.txt) - Sistema de autenticación
- [ADMIN-PANEL-BACKEND-IMPLEMENTATION.md](./ADMIN-PANEL-BACKEND-IMPLEMENTATION.md) - Panel de administración
- [TEST-DATA-CATALOG.md](./TEST-DATA-CATALOG.md) - Datos de prueba disponibles
- [API-EXAMPLES-AUTHENTICATION.md](./API-EXAMPLES-AUTHENTICATION.md) - Ejemplos de autenticación

---

**¡El sistema de gestión de pedidos está completamente implementado y listo para usar!** 🎉

Si necesitas ayuda con la integración en el frontend, no dudes en contactar al equipo de backend.
