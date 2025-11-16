# 📨 MENSAJE PARA EL EQUIPO DE FRONTEND

**De:** Backend Team  
**Para:** Frontend Angular Team  
**Asunto:** ✅ Backend Admin Panel Completado - Endpoints Disponibles  
**Fecha:** 16 de Noviembre 2025

---

## 🎉 BACKEND COMPLETADO Y LISTO PARA INTEGRACIÓN

Hola equipo de frontend! 👋

Les informo que el backend del **Panel de Administración** está **100% completado, testeado y listo para integrar**.

---

## 🚀 QUÉ SE HA DESARROLLADO

He completado los 3 módulos solicitados:

### ✅ **MÓDULO 1: Gestión de Productos**
- Crear, editar, eliminar productos
- Listar con paginación y filtros avanzados
- Búsqueda por nombre/descripción
- Filtro por categoría, stock, precio
- Ordenamiento personalizado
- Validaciones completas

### ✅ **MÓDULO 2: Gestión de Categorías**
- Crear, editar, eliminar categorías
- Listar con contador de productos
- Validación de categorías duplicadas
- Protección contra eliminación con productos
- Vista simple y completa

### ✅ **MÓDULO 3: Gestión de Usuarios**
- Listar con paginación y filtros
- Editar información de usuarios
- Cambiar roles (Admin, Employee, Customer)
- Activar/desactivar usuarios
- Eliminar usuarios
- Protección del último admin
- Estadísticas de pedidos y gastos

---

## 🔗 BASE URL DEL BACKEND

```
HTTPS: https://localhost:5006
HTTP:  http://localhost:5005
Swagger: https://localhost:5006/swagger
```

---

## 🔐 AUTENTICACIÓN

Todos los endpoints admin requieren **JWT Bearer Token**.

### Obtener Token:

```typescript
// 1. Login
POST https://localhost:5006/api/auth/login
Content-Type: application/json

{
  "email": "admin@bosko.com",
  "password": "Admin123!"
}

// Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Admin User",
    "email": "admin@bosko.com",
    "role": "Admin"
  }
}

// 2. Guardar token
localStorage.setItem('auth_token', token);

// 3. Usar en todas las peticiones
Authorization: Bearer {token}
```

---

## 📦 ENDPOINTS DISPONIBLES

### 🛍️ **PRODUCTOS (Admin)**

**Base:** `/api/admin/products`  
**Rol Requerido:** Admin o Employee

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/admin/products` | Listar productos con filtros y paginación |
| GET | `/api/admin/products/{id}` | Obtener producto por ID |
| GET | `/api/admin/products/by-category/{categoryId}` | Productos de una categoría |
| POST | `/api/admin/products` | Crear producto |
| PUT | `/api/admin/products/{id}` | Actualizar producto |
| DELETE | `/api/admin/products/{id}` | Eliminar producto (Solo Admin) |

**Filtros disponibles en GET:**
- `page` (default: 1)
- `pageSize` (default: 10)
- `search` (busca en nombre y descripción)
- `categoryId` (filtrar por categoría)
- `inStock` (true/false)
- `minPrice`, `maxPrice`
- `sortBy` (Name, Price, Stock, CreatedAt)
- `sortDescending` (true/false)

**Ejemplo de petición:**
```typescript
// Angular Service
getProducts(filters: ProductFilter) {
  return this.http.get<ApiResponse<PagedResponse<Product>>>(
    'https://localhost:5006/api/admin/products',
    { params: filters }
  );
}

// Uso en componente
this.productService.getProducts({
  page: 1,
  pageSize: 10,
  search: 'camiseta',
  categoryId: 1,
  sortBy: 'price'
}).subscribe(response => {
  if (response.success) {
    this.products = response.data.items;
    this.totalPages = response.data.totalPages;
  }
});
```

---

### 📁 **CATEGORÍAS (Admin)**

**Base:** `/api/admin/categories`  
**Rol Requerido:** Admin o Employee

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/admin/categories` | Listar todas las categorías con contador |
| GET | `/api/admin/categories/simple` | Lista simple (ID y nombre) |
| GET | `/api/admin/categories/{id}` | Obtener categoría por ID |
| POST | `/api/admin/categories` | Crear categoría |
| PUT | `/api/admin/categories/{id}` | Actualizar categoría |
| DELETE | `/api/admin/categories/{id}` | Eliminar categoría (Solo Admin) |

**Ejemplo de petición:**
```typescript
// Angular Service
createCategory(category: CategoryCreate) {
  return this.http.post<ApiResponse<Category>>(
    'https://localhost:5006/api/admin/categories',
    category
  );
}

// Uso en componente
this.categoryService.createCategory({
  name: 'Camisetas',
  description: 'Camisetas de algodón',
  image: 'https://example.com/image.jpg'
}).subscribe(response => {
  if (response.success) {
    console.log('Categoría creada:', response.data);
    this.loadCategories();
  }
});
```

---

### 👥 **USUARIOS (Admin)**

**Base:** `/api/admin/users`  
**Rol Requerido:** Solo Admin

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/admin/users` | Listar usuarios con filtros y paginación |
| GET | `/api/admin/users/{id}` | Obtener usuario con estadísticas |
| PUT | `/api/admin/users/{id}` | Actualizar usuario |
| PATCH | `/api/admin/users/{id}/role` | Cambiar rol |
| PATCH | `/api/admin/users/{id}/toggle-status` | Activar/Desactivar |
| DELETE | `/api/admin/users/{id}` | Eliminar usuario |

**Filtros disponibles en GET:**
- `page` (default: 1)
- `pageSize` (default: 20)
- `search` (busca en nombre y email)
- `role` (Admin, Employee, Customer)
- `isActive` (true/false)
- `sortBy` (Name, Email, CreatedAt)
- `sortDescending` (true/false)

**Ejemplo de petición:**
```typescript
// Angular Service
getUsers(filters: UserFilter) {
  return this.http.get<ApiResponse<PagedResponse<User>>>(
    'https://localhost:5006/api/admin/users',
    { params: filters }
  );
}

changeUserRole(userId: number, role: string) {
  return this.http.patch<ApiResponse<boolean>>(
    `https://localhost:5006/api/admin/users/${userId}/role`,
    { role }
  );
}

// Uso en componente
this.userService.getUsers({
  page: 1,
  pageSize: 20,
  role: 'Customer',
  isActive: true
}).subscribe(response => {
  if (response.success) {
    this.users = response.data.items;
  }
});
```

---

## 📋 FORMATO DE RESPUESTAS

Todas las respuestas siguen este formato estándar:

```typescript
interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors?: string[];
}

interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}
```

**Ejemplo de respuesta exitosa:**
```json
{
  "success": true,
  "message": "Producto creado exitosamente",
  "data": {
    "id": 1,
    "name": "Camiseta Básica",
    "price": 19.99,
    "stock": 150
  }
}
```

**Ejemplo de respuesta de error:**
```json
{
  "success": false,
  "message": "Error de validación",
  "data": null,
  "errors": [
    "El nombre es requerido",
    "El precio debe ser mayor a 0"
  ]
}
```

---

## 🎨 INTERFACES TYPESCRIPT RECOMENDADAS

```typescript
// interfaces/api-response.interface.ts
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors?: string[];
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// interfaces/product.interface.ts
export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  stock: number;
  image?: string;
  categoryId?: number;
  categoryName?: string;
  createdAt: Date;
}

export interface ProductCreate {
  name: string;
  description?: string;
  price: number;
  stock: number;
  image?: string;
  categoryId?: number;
}

export interface ProductFilter {
  page?: number;
  pageSize?: number;
  search?: string;
  categoryId?: number;
  inStock?: boolean;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: 'Name' | 'Price' | 'Stock' | 'CreatedAt';
  sortDescending?: boolean;
}

// interfaces/category.interface.ts
export interface Category {
  id: number;
  name: string;
  description: string;
  image?: string;
  productCount: number;
  createdAt: Date;
}

export interface CategoryCreate {
  name: string;
  description: string;
  image?: string;
}

// interfaces/user.interface.ts
export interface User {
  id: number;
  name: string;
  email: string;
  phone?: string;
  role: 'Admin' | 'Employee' | 'Customer';
  provider: 'Local' | 'Google';
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  totalOrders?: number;
  totalSpent?: number;
}

export interface UserUpdate {
  name: string;
  email: string;
  phone?: string;
  role: string;
  isActive: boolean;
}

export interface UserFilter {
  page?: number;
  pageSize?: number;
  search?: string;
  role?: string;
  isActive?: boolean;
  sortBy?: 'Name' | 'Email' | 'CreatedAt';
  sortDescending?: boolean;
}
```

---

## 🛠️ SERVICIOS ANGULAR RECOMENDADOS

### ProductAdminService

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductAdminService {
  private apiUrl = `${environment.apiUrl}/admin/products`;

  constructor(private http: HttpClient) {}

  getProducts(filters: ProductFilter): Observable<ApiResponse<PagedResponse<Product>>> {
    return this.http.get<ApiResponse<PagedResponse<Product>>>(this.apiUrl, { params: filters as any });
  }

  getProduct(id: number): Observable<ApiResponse<Product>> {
    return this.http.get<ApiResponse<Product>>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: ProductCreate): Observable<ApiResponse<Product>> {
    return this.http.post<ApiResponse<Product>>(this.apiUrl, product);
  }

  updateProduct(id: number, product: ProductCreate): Observable<ApiResponse<Product>> {
    return this.http.put<ApiResponse<Product>>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }

  getProductsByCategory(categoryId: number): Observable<ApiResponse<Product[]>> {
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/by-category/${categoryId}`);
  }
}
```

### CategoryAdminService

```typescript
@Injectable({ providedIn: 'root' })
export class CategoryAdminService {
  private apiUrl = `${environment.apiUrl}/admin/categories`;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>(this.apiUrl);
  }

  getCategoriesSimple(): Observable<ApiResponse<{id: number, name: string, productCount: number}[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/simple`);
  }

  getCategory(id: number): Observable<ApiResponse<Category>> {
    return this.http.get<ApiResponse<Category>>(`${this.apiUrl}/${id}`);
  }

  createCategory(category: CategoryCreate): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>(this.apiUrl, category);
  }

  updateCategory(id: number, category: CategoryCreate): Observable<ApiResponse<Category>> {
    return this.http.put<ApiResponse<Category>>(`${this.apiUrl}/${id}`, category);
  }

  deleteCategory(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
```

### UserAdminService

```typescript
@Injectable({ providedIn: 'root' })
export class UserAdminService {
  private apiUrl = `${environment.apiUrl}/admin/users`;

  constructor(private http: HttpClient) {}

  getUsers(filters: UserFilter): Observable<ApiResponse<PagedResponse<User>>> {
    return this.http.get<ApiResponse<PagedResponse<User>>>(this.apiUrl, { params: filters as any });
  }

  getUser(id: number): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/${id}`);
  }

  updateUser(id: number, user: UserUpdate): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/${id}`, user);
  }

  changeUserRole(id: number, role: string): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/${id}/role`, { role });
  }

  toggleUserStatus(id: number): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/${id}/toggle-status`, {});
  }

  deleteUser(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
```

---

## 🔒 INTERCEPTOR HTTP (Auth)

```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('auth_token');

    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(req);
  }
}

// Registrar en app.module.ts:
providers: [
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
  }
]
```

---

## ⚙️ CONFIGURACIÓN ENVIRONMENT

```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5006/api'
};

// environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.bosko.com/api' // Cambiar en producción
};
```

---

## 🧪 TESTING

Pueden probar todos los endpoints en **Swagger**:

1. Abrir: `https://localhost:5006/swagger`
2. Click en **"Authorize"** (🔓)
3. Pegar: `Bearer {token}`
4. Click **"Authorize"**
5. Probar endpoints

---

## 📝 NOTAS IMPORTANTES

### ✅ Validaciones Implementadas
- Todos los campos tienen validación
- Los errores se devuelven en el array `errors`
- Mensajes descriptivos en español

### ✅ Seguridad
- JWT Bearer requerido en todos los endpoints admin
- Roles verificados en cada endpoint
- Protección del último administrador
- CORS configurado para localhost:4200 y 4300

### ✅ Activity Logs
- Todas las acciones se registran automáticamente
- Visible en el dashboard de admin
- Incluye: creaciones, actualizaciones, eliminaciones, cambios de rol

### ✅ Manejo de Errores
- Códigos HTTP correctos (200, 400, 401, 404)
- Mensajes descriptivos
- Respuestas estandarizadas

---

## 🚀 PRÓXIMOS PASOS

1. **Crear los servicios** con las interfaces proporcionadas
2. **Configurar el interceptor** para JWT
3. **Actualizar environment** con la URL del backend
4. **Probar conexión** con `/api/health`
5. **Implementar componentes** del panel admin
6. **Probar CRUD completo** de cada módulo

---

## 📞 SOPORTE

Si tienen dudas o problemas:
1. Revisen la documentación en: `BACKEND-COMPLETE-DOCUMENTATION.md`
2. Prueben en Swagger primero
3. Verifiquen que el token JWT está siendo enviado
4. Revisen los logs del backend en consola

---

## ✅ CHECKLIST DE INTEGRACIÓN

- [ ] ⏳ Crear interfaces TypeScript
- [ ] ⏳ Crear servicios Angular
- [ ] ⏳ Configurar interceptor HTTP
- [ ] ⏳ Actualizar environment.ts
- [ ] ⏳ Probar login y obtener token
- [ ] ⏳ Probar GET de productos
- [ ] ⏳ Probar CREATE de producto
- [ ] ⏳ Probar UPDATE de producto
- [ ] ⏳ Probar DELETE de producto
- [ ] ⏳ Implementar paginación
- [ ] ⏳ Implementar filtros
- [ ] ⏳ Repetir para categorías
- [ ] ⏳ Repetir para usuarios

---

**¡El backend está 100% listo para ser integrado!** 🎉

Si necesitan más información o ejemplos, no duden en contactarme.

**Backend Team** 🚀
