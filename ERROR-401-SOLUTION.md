# 🔐 SOLUCIÓN ERROR 401 - Unauthorized

**Fecha:** 16 de Noviembre 2025  
**Error:** 401 Unauthorized en `GET /api/admin/orders`

---

## ✅ BUENAS NOTICIAS

El **error 500 está resuelto**. El endpoint funciona correctamente.

El error **401 significa que falta autenticación** o el token JWT es inválido.

---

## 🔴 PROBLEMA ACTUAL

```
Error: response status is 401
Response headers:
  www-authenticate: Bearer
```

Esto indica que:
- ❌ No se está enviando el token JWT
- ❌ El token es inválido o expiró
- ❌ El token no se está enviando en el header correcto

---

## ✅ SOLUCIÓN 1: VERIFICAR TOKEN EN FRONTEND

### Paso 1: Verificar que tienes un token válido

```typescript
// En tu código TypeScript/Angular
console.log('Token:', localStorage.getItem('auth_token'));
// O también puede estar como:
console.log('Token:', localStorage.getItem('token'));
```

**Si es `null`:** Necesitas hacer login primero.

---

## ✅ SOLUCIÓN 2: HACER LOGIN PARA OBTENER TOKEN

### Opción A: Desde tu aplicación Angular

```typescript
// Servicio de autenticación
login(email: string, password: string) {
  return this.http.post(`${API_URL}/api/auth/login`, {
    email: email,
    password: password
  }).subscribe({
    next: (response: any) => {
      // Guardar token
      localStorage.setItem('auth_token', response.token);
      console.log('✅ Login exitoso, token guardado');
    },
    error: (err) => {
      console.error('❌ Error en login:', err);
    }
  });
}
```

### Opción B: Desde Postman/Thunder Client

```bash
POST https://localhost:5006/api/auth/login
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

**Respuesta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Admin User",
    "email": "admin@test.com",
    "role": "Admin"
  }
}
```

**Copia el token** y úsalo en las siguientes peticiones.

---

## ✅ SOLUCIÓN 3: VERIFICAR INTERCEPTOR DE ANGULAR

El interceptor debe agregar automáticamente el token a todas las peticiones:

### Verifica que existe: `auth.interceptor.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Obtener token de localStorage
    const token = localStorage.getItem('auth_token') || localStorage.getItem('token');
    
    console.log('🔑 Token en interceptor:', token ? 'Presente' : 'FALTA');
    
    if (token) {
      // Clonar request y agregar header Authorization
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      
      console.log('✅ Header agregado:', cloned.headers.get('Authorization'));
      return next.handle(cloned);
    }
    
    console.warn('⚠️ No hay token, request sin autenticación');
    return next.handle(req);
  }
}
```

### Registrar el interceptor en `app.module.ts`

```typescript
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

@NgModule({
  // ...
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ]
})
export class AppModule { }
```

---

## ✅ SOLUCIÓN 4: PROBAR MANUALMENTE CON TOKEN

### Paso 1: Obtener token (si no lo tienes)

```bash
# En tu navegador o Postman
POST https://localhost:5006/api/auth/login
Body: {
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

### Paso 2: Guardar token en localStorage

```javascript
// En consola del navegador (F12)
localStorage.setItem('auth_token', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...');
console.log('Token guardado:', localStorage.getItem('auth_token'));
```

### Paso 3: Recargar página y probar

```bash
# Recarga la página (F5)
# Intenta de nuevo acceder a la gestión de pedidos
```

---

## 🧪 TESTING CON CURL

```bash
# 1. Login y guardar token
TOKEN=$(curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin123!"}' \
  -k -s | jq -r '.token')

echo "Token obtenido: $TOKEN"

# 2. Usar token en request de pedidos
curl -X GET "https://localhost:5006/api/admin/orders?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN" \
  -k

# Debe retornar 200 con JSON de pedidos ✅
```

---

## 🔍 DEBUGGING

### 1. Verificar en DevTools (F12)

```javascript
// En la consola:
console.log('Token:', localStorage.getItem('auth_token'));
console.log('Token alt:', localStorage.getItem('token'));

// Si ambos son null:
console.log('❌ NO HAY TOKEN - Necesitas hacer login');

// Si existe:
console.log('✅ Token presente');
```

### 2. Verificar en Network Tab

1. Abre DevTools (F12)
2. Ve a la pestaña **Network**
3. Haz la petición a `/api/admin/orders`
4. Click en la petición
5. Ve a la pestaña **Headers**
6. Busca en **Request Headers**:

**Debe aparecer:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Si NO aparece:**
- ❌ El interceptor no está funcionando
- ❌ No hay token en localStorage
- ❌ El interceptor no está registrado

---

## 🔧 SOLUCIONES RÁPIDAS

### Solución A: Login desde la UI
```typescript
// 1. Ve a la página de login de tu aplicación
// 2. Ingresa:
//    Email: admin@test.com
//    Password: Admin123!
// 3. Después de login exitoso, ve a gestión de pedidos
```

### Solución B: Agregar token manualmente
```javascript
// En consola del navegador (F12):
localStorage.setItem('auth_token', 'TU_TOKEN_AQUI');
location.reload(); // Recargar página
```

### Solución C: Verificar credenciales
```sql
-- En SQL Server, verifica usuarios:
SELECT Id, Name, Email, Role, IsActive 
FROM Users 
WHERE Role = 'Admin';

-- Debe haber al menos un admin activo
```

---

## 📋 CHECKLIST DE AUTENTICACIÓN

- [ ] ✅ Usuario Admin existe en BD
- [ ] ✅ Password es correcto (Admin123!)
- [ ] ✅ Hice login en la aplicación
- [ ] ✅ Token está en localStorage
- [ ] ✅ Interceptor está configurado
- [ ] ✅ Interceptor está registrado en app.module
- [ ] ✅ Header Authorization se envía en requests
- [ ] ✅ Token no está expirado

---

## 🎯 TESTING COMPLETO

### Test 1: Login
```bash
POST https://localhost:5006/api/auth/login
Body: {"email":"admin@test.com","password":"Admin123!"}

ESPERADO:
✅ Status: 200 OK
✅ Response con token
```

### Test 2: Orders con token
```bash
GET https://localhost:5006/api/admin/orders?page=1&limit=10
Headers: Authorization: Bearer {token}

ESPERADO:
✅ Status: 200 OK
✅ Response con pedidos
```

### Test 3: Orders sin token
```bash
GET https://localhost:5006/api/admin/orders?page=1&limit=10
(Sin header Authorization)

ESPERADO:
❌ Status: 401 Unauthorized
```

---

## 🔐 INFORMACIÓN DEL TOKEN JWT

El token JWT contiene:
- **ID del usuario**
- **Rol** (Admin, Employee, Customer)
- **Fecha de expiración**

### Validar token en jwt.io

1. Ve a https://jwt.io
2. Pega tu token
3. Verifica:
   - ✅ Rol es "Admin" o "Employee"
   - ✅ Fecha de expiración (`exp`) es futura
   - ✅ Token está bien formado

---

## ✅ RESUMEN

**Error anterior:** 500 (Error del servidor) → ✅ RESUELTO  
**Error actual:** 401 (No autorizado) → Necesitas autenticarte

**Solución rápida:**
1. Haz login en tu aplicación
2. El token se guardará automáticamente
3. Intenta de nuevo acceder a pedidos

**Si no funciona:**
- Verifica que el interceptor esté configurado
- Verifica que el token esté en localStorage
- Verifica que el token sea válido (jwt.io)

---

## 📞 SI NECESITAS MÁS AYUDA

Envíame:
1. ✅ Resultado de `console.log(localStorage.getItem('auth_token'))`
2. ✅ Screenshot del Network tab (Headers)
3. ✅ Código del interceptor (si lo tienes)
4. ✅ Confirma que hiciste login

---

**¡El backend funciona! Solo necesitas autenticarte.** 🔐✅
