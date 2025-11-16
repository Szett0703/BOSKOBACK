# ?? GUÍA DE TESTING - SISTEMA DE AUTENTICACIÓN BOSKO

## ?? ÍNDICE DE PRUEBAS

1. Inicialización de Usuarios
2. Login Exitoso (Admin)
3. Login Fallido
4. Registro de Nuevo Usuario
5. Login con Google (Simulado)
6. Forgot Password
7. Reset Password
8. Crear Producto con Token Admin
9. Crear Producto con Token Customer (Forbidden)
10. Crear Producto sin Token (Unauthorized)

---

## ?? CONFIGURACIÓN INICIAL

### Swagger UI
```
URL: https://localhost:5006/swagger
```

### Postman
```
Base URL: https://localhost:5006
```

---

## ?? TEST 1: INICIALIZAR USUARIOS

### Propósito
Generar los hashes BCrypt para los usuarios de prueba.

### Swagger
1. Expande `Auth` ? `POST /api/auth/init-users`
2. Click en "Try it out"
3. Click en "Execute"

### Postman
```http
POST https://localhost:5006/api/auth/init-users
```

### Expected Response (200 OK)
```json
{
  "message": "Passwords inicializados para 3 usuarios",
  "password": "Bosko123!",
  "users": [
    { "email": "admin@bosko.com", "role": "Admin" },
    { "email": "employee@bosko.com", "role": "Employee" },
    { "email": "customer@bosko.com", "role": "Customer" }
  ]
}
```

### ? Criterios de Éxito
- Status 200 OK
- Mensaje de confirmación
- Lista de usuarios inicializados
- Password mostrado: "Bosko123!"

---

## ?? TEST 2: LOGIN EXITOSO (ADMIN)

### Propósito
Verificar que el login funciona correctamente y retorna un JWT válido.

### Swagger
1. Expande `Auth` ? `POST /api/auth/login`
2. Click en "Try it out"
3. Ingresa el JSON:

```json
{
  "email": "admin@bosko.com",
  "password": "Bosko123!"
}
```

4. Click en "Execute"

### Postman
```http
POST https://localhost:5006/api/auth/login
Content-Type: application/json

{
  "email": "admin@bosko.com",
  "password": "Bosko123!"
}
```

### Expected Response (200 OK)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Admin Bosko",
    "email": "admin@bosko.com",
    "role": "Admin",
    "provider": "Local",
    "phone": "+34 600 000 001",
    "isActive": true,
    "createdAt": "2024-11-16T..."
  }
}
```

### ? Criterios de Éxito
- Status 200 OK
- Token JWT presente
- User data completo
- Role = "Admin"

### ?? Verificación Adicional
1. Copia el token recibido
2. Ve a https://jwt.io
3. Pega el token en "Encoded"
4. Verifica el payload:

```json
{
  "sub": "1",
  "name": "Admin Bosko",
  "email": "admin@bosko.com",
  "role": "Admin",
  "provider": "Local",
  "phone": "+34 600 000 001",
  "jti": "...",
  "exp": 1732176000,
  "iss": "BoskoAPI",
  "aud": "BoskoFrontend"
}
```

---

## ?? TEST 3: LOGIN FALLIDO (PASSWORD INCORRECTO)

### Propósito
Verificar que se rechaza un login con password incorrecto.

### Request
```json
{
  "email": "admin@bosko.com",
  "password": "WrongPassword123"
}
```

### Expected Response (401 Unauthorized)
```json
{
  "message": "Email o contraseña incorrectos"
}
```

### ? Criterios de Éxito
- Status 401 Unauthorized
- Mensaje de error apropiado
- No se retorna token

---

## ?? TEST 4: REGISTRO DE NUEVO USUARIO

### Propósito
Verificar que se puede registrar un nuevo usuario y que automáticamente recibe role "Customer".

### Request
```json
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "TestPassword123!",
  "phone": "+34 600 123 456"
}
```

### Expected Response (201 Created)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 5,
    "name": "Test User",
    "email": "test@example.com",
    "role": "Customer",
    "provider": "Local",
    "phone": "+34 600 123 456",
    "isActive": true,
    "createdAt": "2024-11-16T..."
  }
}
```

### ? Criterios de Éxito
- Status 201 Created
- Token JWT presente
- Role = "Customer" (automático)
- Provider = "Local"

---

## ?? TEST 5: REGISTRO CON EMAIL DUPLICADO

### Propósito
Verificar que no se puede registrar con un email ya existente.

### Request
```json
{
  "name": "Another Admin",
  "email": "admin@bosko.com",
  "password": "Password123!"
}
```

### Expected Response (400 Bad Request)
```json
{
  "message": "El email ya está registrado"
}
```

### ? Criterios de Éxito
- Status 400 Bad Request
- Mensaje de error claro
- No se crea el usuario

---

## ?? TEST 6: LOGIN COMO CUSTOMER

### Propósito
Obtener un token de Customer para probar permisos.

### Request
```json
{
  "email": "customer@bosko.com",
  "password": "Bosko123!"
}
```

### Expected Response (200 OK)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 3,
    "name": "Cliente Test",
    "email": "customer@bosko.com",
    "role": "Customer",
    ...
  }
}
```

### ?? IMPORTANTE
**Guarda este token**, lo usaremos en TEST 9 para verificar el error 403 Forbidden.

---

## ?? TEST 7: FORGOT PASSWORD

### Propósito
Verificar que se genera un token de reset.

### Request
```json
{
  "email": "admin@bosko.com"
}
```

### Expected Response (200 OK) - Modo DEBUG
```json
{
  "message": "Si el email existe, recibirás instrucciones...",
  "token": "ABC123XYZ456DEF789...",
  "expiresAt": "2024-11-16T14:00:00Z"
}
```

### ? Criterios de Éxito
- Status 200 OK
- Token de reset generado
- Fecha de expiración (1 hora)

### ?? IMPORTANTE
**Guarda el token de reset**, lo usaremos en TEST 8.

---

## ?? TEST 8: RESET PASSWORD

### Propósito
Verificar que se puede cambiar la contraseña con el token de reset.

### Request
```json
{
  "email": "admin@bosko.com",
  "token": "ABC123XYZ456DEF789...",
  "newPassword": "NewPassword123!"
}
```

### Expected Response (200 OK)
```json
{
  "message": "Contraseña actualizada exitosamente"
}
```

### ? Criterios de Éxito
- Status 200 OK
- Mensaje de confirmación

### ?? Verificación Adicional
1. Intenta login con el nuevo password:

```json
{
  "email": "admin@bosko.com",
  "password": "NewPassword123!"
}
```

2. Debe funcionar correctamente
3. Restaura el password a "Bosko123!" ejecutando /init-users de nuevo

---

## ?? TEST 9: CREAR PRODUCTO CON TOKEN ADMIN

### Propósito
Verificar que Admin puede crear productos.

### Paso 1: Obtener Token Admin
Ejecuta TEST 2 para obtener el token de Admin.

### Paso 2: Crear Producto

#### Swagger
1. Expande `Products` ? `POST /api/products`
2. Click en "Try it out"
3. Click en el candado ?? "Authorize"
4. Ingresa: `Bearer {tu_token_admin}`
5. Click "Authorize" y "Close"
6. Ingresa el JSON:

```json
{
  "name": "Producto de Prueba",
  "description": "Creado por Admin en testing",
  "price": 99.99,
  "stock": 10,
  "categoryId": 1,
  "image": "https://example.com/test.jpg"
}
```

7. Click "Execute"

#### Postman
```http
POST https://localhost:5006/api/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Producto de Prueba",
  "description": "Creado por Admin en testing",
  "price": 99.99,
  "stock": 10,
  "categoryId": 1,
  "image": "https://example.com/test.jpg"
}
```

### Expected Response (201 Created)
```json
{
  "id": 16,
  "name": "Producto de Prueba",
  "description": "Creado por Admin en testing",
  "price": 99.99,
  "stock": 10,
  "image": "https://example.com/test.jpg",
  "categoryId": 1,
  "categoryName": "Camisas",
  "createdAt": "2024-11-16T..."
}
```

### ? Criterios de Éxito
- Status 201 Created
- Producto creado con ID
- CategoryName incluido
- Todos los datos presentes

---

## ?? TEST 10: CREAR PRODUCTO CON TOKEN CUSTOMER (FORBIDDEN)

### Propósito
Verificar que Customer NO puede crear productos.

### Paso 1: Obtener Token Customer
Ejecuta TEST 6 para obtener el token de Customer.

### Paso 2: Intentar Crear Producto

#### Swagger
1. Click en el candado ?? "Authorize"
2. Ingresa: `Bearer {tu_token_customer}`
3. Click "Authorize" y "Close"
4. Intenta crear un producto

#### Postman
```http
POST https://localhost:5006/api/products
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "name": "Intento Fallido",
  "price": 99.99,
  "stock": 10
}
```

### Expected Response (403 Forbidden)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "..."
}
```

### ? Criterios de Éxito
- Status 403 Forbidden
- No se crea el producto
- Mensaje de error de permisos

---

## ?? TEST 11: CREAR PRODUCTO SIN TOKEN (UNAUTHORIZED)

### Propósito
Verificar que se requiere autenticación.

### Swagger
1. Si estás autorizado, click en "Logout"
2. Intenta crear un producto sin autorización

### Postman
```http
POST https://localhost:5006/api/products
Content-Type: application/json

{
  "name": "Sin Token",
  "price": 99.99,
  "stock": 10
}
```

### Expected Response (401 Unauthorized)
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "..."
}
```

### ? Criterios de Éxito
- Status 401 Unauthorized
- No se crea el producto
- Error de autenticación

---

## ?? TEST 12: GET PRODUCTOS SIN TOKEN (PÚBLICO)

### Propósito
Verificar que los endpoints públicos funcionan sin token.

### Request
```http
GET https://localhost:5006/api/products
```

### Expected Response (200 OK)
```json
[
  {
    "id": 1,
    "name": "Camisa Oxford Azul",
    "description": "...",
    "price": 49.99,
    "stock": 25,
    "image": "...",
    "categoryId": 1,
    "categoryName": "Camisas",
    "createdAt": "..."
  },
  ...
]
```

### ? Criterios de Éxito
- Status 200 OK
- Lista de productos
- Sin necesidad de token

---

## ?? RESUMEN DE RESULTADOS ESPERADOS

| Test # | Endpoint | Token | Expected Status | Descripción |
|--------|----------|-------|-----------------|-------------|
| 1 | POST /auth/init-users | No | 200 | Inicializar passwords |
| 2 | POST /auth/login | No | 200 | Login Admin exitoso |
| 3 | POST /auth/login | No | 401 | Login fallido |
| 4 | POST /auth/register | No | 201 | Registro exitoso |
| 5 | POST /auth/register | No | 400 | Email duplicado |
| 6 | POST /auth/login | No | 200 | Login Customer |
| 7 | POST /auth/forgot-password | No | 200 | Solicitar reset |
| 8 | POST /auth/reset-password | No | 200 | Reset exitoso |
| 9 | POST /products | Admin | 201 | Crear producto (success) |
| 10 | POST /products | Customer | 403 | Crear producto (forbidden) |
| 11 | POST /products | No | 401 | Crear producto (unauthorized) |
| 12 | GET /products | No | 200 | Consulta pública |

---

## ? CHECKLIST COMPLETO DE TESTING

### Autenticación Básica
- [ ] Inicializar usuarios
- [ ] Login Admin exitoso
- [ ] Login Employee exitoso
- [ ] Login Customer exitoso
- [ ] Login con password incorrecto falla
- [ ] Login con email inexistente falla
- [ ] Token JWT se genera correctamente
- [ ] Claims del JWT son correctos

### Registro
- [ ] Registro exitoso crea usuario con role Customer
- [ ] Registro con email duplicado falla
- [ ] Registro genera token JWT
- [ ] Validaciones de campos funcionan

### Recuperación de Password
- [ ] Forgot password genera token
- [ ] Reset password con token válido funciona
- [ ] Reset password con token inválido falla
- [ ] Reset password con token expirado falla

### Autorización
- [ ] Admin puede crear productos
- [ ] Employee NO puede crear productos (si lo implementas)
- [ ] Customer NO puede crear productos
- [ ] Sin token NO puede crear productos
- [ ] Endpoints públicos funcionan sin token

### Google OAuth
- [ ] Google login valida token (requiere token real de Google)
- [ ] Google login crea usuario si no existe
- [ ] Google login retorna JWT de Bosko

---

## ?? TROUBLESHOOTING

### Error: "Cannot execute request"
**Causa:** Backend no está corriendo  
**Solución:** Ejecuta `dotnet run`

### Error: "Database error"
**Causa:** Base de datos no configurada  
**Solución:** Ejecuta el script SQL

### Error: "Email o contraseña incorrectos" (Usuarios de prueba)
**Causa:** Passwords no inicializados  
**Solución:** Ejecuta `/api/auth/init-users`

### Error: "Unauthorized" en Swagger
**Causa:** Token no configurado  
**Solución:** Click en ?? Authorize e ingresa el token

### Error: "Forbidden" cuando debería funcionar
**Causa:** Usuario con rol incorrecto  
**Solución:** Verifica el rol del usuario en el token (jwt.io)

---

## ?? NOTAS FINALES

### Para Testing Manual
- Usa Swagger para pruebas rápidas
- Usa Postman para scripts automatizados
- Verifica siempre los tokens en jwt.io

### Para Testing Automatizado
- Implementa tests de integración en .NET
- Usa xUnit o NUnit
- Mock el JWT para tests unitarios

### Para Production
- Nunca exponer `/api/auth/init-users`
- Implementar rate limiting en login
- Agregar logging de intentos fallidos
- Implementar 2FA para administradores

---

**Testing Guide | Bosko E-Commerce**  
**Backend Authentication System**
