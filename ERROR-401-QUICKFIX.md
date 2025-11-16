# 🚨 ERROR 401 - SOLUCIÓN RÁPIDA

## ✅ EL BACKEND FUNCIONA

El error cambió de **500** (servidor roto) a **401** (sin autenticación).

Esto significa: **¡EL ENDPOINT FUNCIONA!** Solo necesitas **autenticarte**.

---

## ⚡ SOLUCIÓN EN 3 PASOS (2 minutos)

### 1️⃣ OBTENER TOKEN (30 segundos)

**Opción A: Desde tu aplicación Angular**
- Ve a la página de login
- Usuario: `admin@test.com`
- Password: `Admin123!`
- Haz click en "Login"
- El token se guarda automáticamente

**Opción B: Con Postman/Thunder Client**
```bash
POST https://localhost:5006/api/auth/login
Body: {
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

Copia el `token` de la respuesta.

---

### 2️⃣ GUARDAR TOKEN (30 segundos)

```javascript
// Abre la consola del navegador (F12)
localStorage.setItem('auth_token', 'PEGA_TU_TOKEN_AQUI');

// Verifica:
console.log('Token guardado:', localStorage.getItem('auth_token'));
```

---

### 3️⃣ RECARGAR Y PROBAR (30 segundos)

```bash
# Recarga la página
F5

# Ve a gestión de pedidos
# Deberías ver la lista ✅
```

---

## 🔍 VERIFICACIÓN RÁPIDA

### ¿Tienes token?
```javascript
// En consola (F12):
localStorage.getItem('auth_token')

// Si es null:
console.log('❌ NO HAY TOKEN - Haz login primero');

// Si aparece un string largo:
console.log('✅ TOKEN PRESENTE');
```

### ¿Se envía el header?
1. F12 → Network
2. Haz la petición a `/api/admin/orders`
3. Click en la petición → Headers
4. Busca: `Authorization: Bearer ...`

**Si NO aparece:** El interceptor no funciona.

---

## 🔧 SI NO FUNCIONA EL INTERCEPTOR

### Crea el archivo: `auth.interceptor.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem('auth_token');
    
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
    
    return next.handle(req);
  }
}
```

### Registra en `app.module.ts`

```typescript
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

providers: [
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
  }
]
```

---

## 🧪 TESTING MANUAL

### Con cURL:
```bash
# 1. Login
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin123!"}' \
  -k

# 2. Copia el token y usa:
curl -X GET "https://localhost:5006/api/admin/orders?page=1&limit=10" \
  -H "Authorization: Bearer {TU_TOKEN}" \
  -k

# Debe retornar 200 con JSON ✅
```

---

## ✅ CHECKLIST

- [ ] Hice login (admin@test.com / Admin123!)
- [ ] Token está en localStorage
- [ ] Interceptor configurado
- [ ] Header Authorization se envía
- [ ] ¡Funciona! 🎉

---

## 📞 AYUDA ADICIONAL

**Lee:** `ERROR-401-SOLUTION.md` (documentación completa)

**Ejecuta:** `Database/Verify-Auth.sql` (verificar usuarios en BD)

---

**Tiempo total: ~2 minutos** ⏱️

**TL;DR: Haz login, el token se guarda, recarga la página, funciona.** ✅
