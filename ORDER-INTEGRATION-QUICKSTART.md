# 🚀 GUÍA RÁPIDA DE INTEGRACIÓN - GESTIÓN DE PEDIDOS

## ⚡ Inicio Rápido (5 minutos)

### 1. Crear el Servicio (2 min)

Crea el archivo `src/app/services/order-admin.service.ts`:

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_URL = 'https://localhost:5006';

@Injectable({ providedIn: 'root' })
export class OrderAdminService {
  private apiUrl = `${API_URL}/api/admin/orders`;

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

### 2. Actualizar el Componente (2 min)

En `order-management.component.ts`, reemplaza las funciones simuladas:

```typescript
import { OrderAdminService } from '../../../services/order-admin.service';

export class OrderManagementComponent implements OnInit {
  constructor(private orderService: OrderAdminService) {}

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
      error: (err) => {
        console.error('Error:', err);
        this.isLoading = false;
      }
    });
  }

  viewOrderDetails(orderId: number): void {
    this.orderService.getOrderDetails(orderId).subscribe({
      next: (order) => {
        this.selectedOrder = order;
        this.showDetailsModal = true;
      },
      error: (err) => console.error('Error:', err)
    });
  }

  updateOrderStatus(): void {
    if (!this.selectedOrder) return;
    
    this.orderService.updateOrderStatus(
      this.selectedOrder.id,
      this.newStatus,
      this.statusNote
    ).subscribe({
      next: () => {
        this.showStatusModal = false;
        this.loadOrders();
        alert('Estado actualizado exitosamente');
      },
      error: (err) => {
        console.error('Error:', err);
        alert('Error al actualizar el estado');
      }
    });
  }
}
```

### 3. Verificar (1 min)

1. ✅ Asegúrate que el backend esté corriendo en `https://localhost:5006`
2. ✅ Verifica que tengas un token JWT válido
3. ✅ Abre la página de gestión de pedidos
4. ✅ Deberías ver los pedidos reales de la base de datos

---

## 🔑 URLs de los Endpoints

```
GET    /api/admin/orders              → Lista de pedidos
GET    /api/admin/orders/{id}         → Detalles del pedido
PUT    /api/admin/orders/{id}/status  → Actualizar estado
```

---

## 📝 Formato de Respuestas

### Lista de Pedidos

```typescript
{
  orders: Array<{
    id: number;
    customerName: string;
    customerEmail: string;
    items: number;          // Cantidad de items en el pedido
    amount: number;         // Total del pedido
    status: string;         // 'pending' | 'processing' | 'delivered' | 'cancelled'
    createdAt: string;      // ISO 8601 date
    updatedAt: string;      // ISO 8601 date
  }>,
  pagination: {
    total: number;          // Total de pedidos
    page: number;           // Página actual
    pages: number;          // Total de páginas
    limit: number;          // Items por página
  }
}
```

### Detalles del Pedido

```typescript
{
  // Campos básicos (igual que en lista)
  id, customerName, customerEmail, items, amount, status, createdAt, updatedAt,
  
  // Información adicional
  customer: {
    id: number;
    name: string;
    email: string;
    phone?: string;
  },
  shippingAddress: {
    street: string;
    city: string;
    state: string;
    zipCode: string;
    country: string;
  },
  orderItems: Array<{
    productId: number;
    name: string;
    quantity: number;
    price: number;
    subtotal: number;
    imageUrl?: string;
  }>,
  subtotal: number,
  shipping: number,
  total: number,
  paymentMethod: string,
  statusHistory: Array<{
    status: string;
    timestamp: string;
    note?: string;
  }>
}
```

---

## 🎯 Ejemplos de Uso

### Filtrar por estado pendiente

```typescript
this.orderService.getOrders(1, 10, 'pending').subscribe(response => {
  console.log(response.orders);
});
```

### Buscar por cliente

```typescript
this.orderService.getOrders(1, 10, undefined, 'maria').subscribe(response => {
  console.log(response.orders);
});
```

### Actualizar estado con nota

```typescript
this.orderService.updateOrderStatus(
  1234,
  'processing',
  'Pedido en preparación en almacén'
).subscribe(response => {
  console.log('Estado actualizado:', response);
});
```

---

## 🔧 Troubleshooting

### Error 401 (Unauthorized)
```typescript
// Verifica que el token JWT esté siendo enviado
// Debe estar en localStorage como 'auth_token' o 'token'
const token = localStorage.getItem('auth_token');
console.log('Token:', token);
```

### Error 404 (Not Found)
```typescript
// Verifica que el ID del pedido exista
// Verifica que la URL del backend sea correcta
console.log('API URL:', this.apiUrl);
```

### No se ven datos
```typescript
// 1. Verifica que el backend esté corriendo
// 2. Verifica que haya datos en la BD (ejecuta Insert-All-Data-Final.sql)
// 3. Verifica los logs del navegador (F12 → Console)
// 4. Verifica los logs del backend (Output window en Visual Studio)
```

---

## ✅ Checklist

- [ ] Servicio creado en `services/order-admin.service.ts`
- [ ] Componente actualizado para usar el servicio
- [ ] Backend corriendo en `https://localhost:5006`
- [ ] Base de datos tiene datos de prueba
- [ ] Token JWT válido en localStorage
- [ ] Página carga sin errores
- [ ] Se ven los pedidos reales
- [ ] Filtros funcionan correctamente
- [ ] Modal de detalles muestra información completa
- [ ] Actualización de estado funciona

---

## 📞 ¿Necesitas Ayuda?

1. Revisa la documentación completa en `ORDER-MANAGEMENT-API-READY.md`
2. Verifica los logs en consola del navegador (F12)
3. Verifica los logs del backend (Visual Studio → Output)
4. Prueba los endpoints directamente con Postman/Thunder Client

---

**¡Listo! Tu sistema de gestión de pedidos debería estar funcionando.** 🎉
