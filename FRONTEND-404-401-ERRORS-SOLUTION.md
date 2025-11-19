# 🔧 SOLUCIÓN DE ERRORES 404 Y 401 - ANGULAR + ASP.NET CORE

**Fecha:** 16 de Noviembre 2025  
**Problema:** Múltiples errores 404 (Not Found) y 401 (Unauthorized) en Angular

---

## 🔍 ANÁLISIS DE LOS ERRORES

### Errores 404 Detectados:

```
❌ GET https://localhost:5006/api/orders → 404
❌ GET https://localhost:5006/admin/products?page=1&pageSize=10&sortBy=CreatedAt → 404
❌ GET https://localhost:5006/admin/categories/simple → 404
❌ GET https://localhost:5006/admin/categories → 404
❌ GET https://localhost:5006/admin/users?page=1&pageSize=20&sortBy=CreatedAt → 404
```

### Errores 401 Detectados:

```
⚠️ POST https://localhost:5006/api/auth/login → 401 (Unauthorized)
```

---

## ✅ SOLUCIÓN: RUTAS CORRECTAS DEL BACKEND

### 📦 PRODUCTOS

| Tu Angular está usando | Ruta CORRECTA del Backend |
|------------------------|----------------------------|
| ❌ `/admin/products` | ✅ `/api/admin/products` |

**Controllers involucrados:**
- `AdminProductsController.cs` - Ruta: `[Route("api/admin/products")]`

**Endpoints disponibles:**
```
GET    /api/admin/products                  ✅ Lista paginada
GET    /api/admin/products/{id}             ✅ Detalles
POST   /api/admin/products                  ✅ Crear
PUT    /api/admin/products/{id}             ✅ Actualizar
DELETE /api/admin/products/{id}             ✅ Eliminar
GET    /api/admin/products/by-category/{id} ✅ Por categoría
```

---

### 📂 CATEGORÍAS

| Tu Angular está usando | Ruta CORRECTA del Backend |
|------------------------|----------------------------|
| ❌ `/admin/categories` | ✅ `/api/admin/categories` |
| ❌ `/admin/categories/simple` | ✅ `/api/admin/categories/simple` |

**Controllers involucrados:**
- `AdminCategoriesController.cs` - Ruta: `[Route("api/admin/categories")]`

**Endpoints disponibles:**
```
GET    /api/admin/categories        ✅ Todas las categorías
GET    /api/admin/categories/simple ✅ Lista simple (id, nombre)
GET    /api/admin/categories/{id}   ✅ Detalles
POST   /api/admin/categories        ✅ Crear
PUT    /api/admin/categories/{id}   ✅ Actualizar
DELETE /api/admin/categories/{id}   ✅ Eliminar
```

---

### 👥 USUARIOS

| Tu Angular está usando | Ruta CORRECTA del Backend |
|------------------------|----------------------------|
| ❌ `/admin/users` | ✅ `/api/admin/users` |

**Controllers involucrados:**
- `AdminUsersController.cs` - Ruta: `[Route("api/admin/users")]`

**Endpoints disponibles:**
```
GET    /api/admin/users                     ✅ Lista paginada
GET    /api/admin/users/{id}                ✅ Detalles
PUT    /api/admin/users/{id}                ✅ Actualizar
PATCH  /api/admin/users/{id}/role           ✅ Cambiar rol
PATCH  /api/admin/users/{id}/toggle-status  ✅ Activar/Desactivar
DELETE /api/admin/users/{id}                ✅ Eliminar
```

---

### 📦 ÓRDENES/PEDIDOS

| Tu Angular está usando | Ruta CORRECTA del Backend |
|------------------------|----------------------------|
| ❌ `/api/orders` | ✅ `/api/admin/orders` |

**Controllers involucrados:**
- `AdminController.cs` - Ruta: `[Route("api/admin")]`

**Endpoints disponibles:**
```
GET /api/admin/orders           ✅ Lista paginada
GET /api/admin/orders/{id}      ✅ Detalles
PUT /api/admin/orders/{id}/status ✅ Cambiar estado
GET /api/admin/orders/recent    ✅ Órdenes recientes
```

---

## 🔐 SOLUCIÓN AL ERROR 401 (Unauthorized)

### Problema: Login devuelve 401

**Causas posibles:**
1. Email o password incorrectos
2. Usuario no existe en la base de datos
3. Passwords no inicializados

### Solución:

#### 1. Verificar que ejecutaste el script de inicialización:

```bash
# En Swagger: POST /api/auth/init-users
```

Esto genera los hashes BCrypt para:
- admin@bosko.com
- employee@bosko.com
- customer@bosko.com

#### 2. Credenciales CORRECTAS:

```json
{
  "email": "admin@bosko.com",
  "password": "Bosko123!"
}
```

⚠️ **IMPORTANTE:** La contraseña es `Bosko123!` (con mayúscula B y signo de exclamación)

#### 3. Verificar en la base de datos:

```sql
-- Verificar que los usuarios existen
SELECT Id, Name, Email, Role, 
       CASE 
           WHEN PasswordHash IS NULL THEN '❌ SIN PASSWORD'
           WHEN LEN(PasswordHash) >= 60 THEN '✅ Password OK'
           ELSE '⚠️ Hash inválido'
       END AS PasswordStatus
FROM Users
WHERE Provider = 'Local';
```

---

## 🔧 CORRECCIONES NECESARIAS EN TU FRONTEND ANGULAR

### 1. Actualizar URLs en los servicios

**Archivo: `src/app/services/product.service.ts` (o similar)**

```typescript
// ❌ ANTES (INCORRECTO)
getProducts(): Observable<any> {
  return this.http.get(`${this.apiUrl}/admin/products`);
}

// ✅ DESPUÉS (CORRECTO)
getProducts(): Observable<any> {
  return this.http.get(`${this.apiUrl}/api/admin/products`);
}
```

### 2. Actualizar environment.ts

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5006'  // ✅ Sin /api al final
};

// Luego en los servicios:
// ${environment.apiUrl}/api/admin/products ✅
```

### 3. Verificar el Interceptor HTTP

Tu interceptor debe agregar el token JWT a TODAS las requests:

```typescript
// http.interceptor.ts
intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  const token = localStorage.getItem('token'); // o sessionStorage
  
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  return next.handle(req);
}
```

---

## 📋 CHECKLIST DE VERIFICACIÓN

### Backend:
- [ ] ✅ Backend corriendo en `https://localhost:5006`
- [ ] ✅ Swagger accesible en `https://localhost:5006/swagger`
- [ ] ✅ Endpoint `/api/auth/init-users` ejecutado
- [ ] ✅ Login funciona en Swagger con `admin@bosko.com` / `Bosko123!`
- [ ] ✅ Token JWT se genera correctamente
- [ ] ✅ Todos los controllers están registrados

### Frontend:
- [ ] ⏳ URLs actualizadas para incluir `/api/`
- [ ] ⏳ `environment.apiUrl` configurado correctamente
- [ ] ⏳ Interceptor agregando token en headers
- [ ] ⏳ Token se guarda en localStorage después del login
- [ ] ⏳ Guards verificando autenticación

---

## 🎯 MAPEO COMPLETO DE RUTAS

### Autenticación (Públicas)
```
POST /api/auth/login              ✅
POST /api/auth/register           ✅
POST /api/auth/google-login       ✅
POST /api/auth/forgot-password    ✅
POST /api/auth/reset-password     ✅
POST /api/auth/init-users         ✅
```

### Dashboard (Requiere Admin/Employee)
```
GET /api/admin/dashboard/stats           ✅
GET /api/admin/dashboard/sales-chart     ✅
GET /api/admin/dashboard/orders-status   ✅
GET /api/admin/orders/recent             ✅
GET /api/admin/products/top-sellers      ✅
GET /api/admin/activity/recent           ✅
GET /api/admin/notifications/unread-count ✅
```

### Productos Admin (Requiere Admin/Employee)
```
GET    /api/admin/products              ✅
POST   /api/admin/products              ✅ (Solo Admin)
GET    /api/admin/products/{id}         ✅
PUT    /api/admin/products/{id}         ✅ (Solo Admin)
DELETE /api/admin/products/{id}         ✅ (Solo Admin)
```

### Categorías Admin (Requiere Admin/Employee)
```
GET    /api/admin/categories            ✅
GET    /api/admin/categories/simple     ✅
POST   /api/admin/categories            ✅ (Solo Admin)
GET    /api/admin/categories/{id}       ✅
PUT    /api/admin/categories/{id}       ✅ (Solo Admin)
DELETE /api/admin/categories/{id}       ✅ (Solo Admin)
```

### Usuarios Admin (Requiere Admin)
```
GET    /api/admin/users                 ✅
GET    /api/admin/users/{id}            ✅
PUT    /api/admin/users/{id}            ✅
PATCH  /api/admin/users/{id}/role       ✅
PATCH  /api/admin/users/{id}/toggle-status ✅
DELETE /api/admin/users/{id}            ✅
```

### Órdenes Admin (Requiere Admin/Employee)
```
GET /api/admin/orders                   ✅
GET /api/admin/orders/{id}              ✅
PUT /api/admin/orders/{id}/status       ✅
```

### Productos Públicos (Sin autenticación)
```
GET /api/products                       ✅
GET /api/products/{id}                  ✅
GET /api/products?categoryId={id}       ✅
```

### Categorías Públicas (Sin autenticación)
```
GET /api/categories                     ✅
GET /api/categories/{id}                ✅
```

---

## 🔍 DEBUGGING EN DESARROLLO

### 1. Ver los logs del backend

En Visual Studio, ve a:
- **View** → **Output**
- Selecciona **"Debug"** en el dropdown
- Busca líneas que empiecen con:
  - `📨` (Request recibido)
  - `✅` (Success)
  - `❌` (Error)

### 2. Ver las requests en el navegador

1. Abre **Chrome DevTools** (F12)
2. Ve a la pestaña **Network**
3. Filtra por **XHR/Fetch**
4. Verifica:
   - ✅ URL completa (debe tener `/api/`)
   - ✅ Headers (debe tener `Authorization: Bearer ...`)
   - ✅ Status code (200 OK, no 404)

### 3. Probar directamente en Swagger

Antes de probar en Angular:
1. Abre `https://localhost:5006/swagger`
2. Ejecuta `POST /api/auth/login`
3. Copia el token
4. Click en **"Authorize"** (candado verde)
5. Pega: `Bearer {token}`
6. Prueba el endpoint que está fallando

---

## 🚀 SOLUCIÓN RÁPIDA (5 MINUTOS)

### Paso 1: Actualizar Base URL en Angular

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5006/api'  // ✅ Agregado /api
};
```

### Paso 2: Simplificar servicios

```typescript
// product.service.ts
getProducts(): Observable<any> {
  // Ahora solo: /admin/products
  return this.http.get(`${environment.apiUrl}/admin/products`);
}

// category.service.ts
getCategories(): Observable<any> {
  return this.http.get(`${environment.apiUrl}/admin/categories`);
}

// user.service.ts
getUsers(): Observable<any> {
  return this.http.get(`${environment.apiUrl}/admin/users`);
}

// order.service.ts
getOrders(): Observable<any> {
  return this.http.get(`${environment.apiUrl}/admin/orders`);
}
```

### Paso 3: Verificar login

```typescript
// auth.service.ts
login(credentials: any): Observable<any> {
  return this.http.post(`${environment.apiUrl}/auth/login`, credentials);
}
```

### Paso 4: Reiniciar Angular

```bash
ng serve --port 4200
```

---

## ✅ RESULTADO ESPERADO

Después de aplicar estos cambios:

```
✅ POST https://localhost:5006/api/auth/login → 200 OK
✅ GET  https://localhost:5006/api/admin/products → 200 OK
✅ GET  https://localhost:5006/api/admin/categories → 200 OK
✅ GET  https://localhost:5006/api/admin/users → 200 OK
✅ GET  https://localhost:5006/api/admin/orders → 200 OK
```

---

## 📝 RESUMEN

**El problema era:**
- ❌ Tu Angular llamaba a `/admin/products` (sin `/api/`)
- ✅ El backend espera `/api/admin/products`

**La solución:**
1. Agregar `/api` en la base URL del environment
2. O agregar `/api/` manualmente en cada servicio
3. Verificar que el token JWT se está enviando
4. Ejecutar `/api/auth/init-users` si el login falla

---

**¿Necesitas ayuda adicional?**  
Revisa:
- `SWAGGER-ERROR-FINAL-SOLUTION.md` - Solución de errores de Swagger
- `AUTHENTICATION-IMPLEMENTATION-COMPLETE.md` - Sistema de autenticación
- `BACKEND-COMPLETE-DOCUMENTATION.md` - Documentación completa

**¡Con estos cambios tu Angular debería conectarse perfectamente!** 🎉
