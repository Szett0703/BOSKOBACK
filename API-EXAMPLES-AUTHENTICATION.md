# ?? EJEMPLOS DE RESPUESTAS JSON - AUTENTICACIÓN BOSKO

## ?? 1. POST /api/auth/login - LOGIN EXITOSO

**Request:**
```json
{
  "email": "admin@bosko.com",
  "password": "Bosko123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6IkFkbWluIEJvc2tvIiwiZW1haWwiOiJhZG1pbkBib3Nrby5jb20iLCJyb2xlIjoiQWRtaW4iLCJwcm92aWRlciI6IkxvY2FsIiwicGhvbmUiOiIrMzQgNjAwIDAwMCAwMDEiLCJqdGkiOiI3ZjE2MTg4Ny0yYzg1LTRkOTEtYmI0ZC0zNzE5ZGJhNjg1ZWEiLCJleHAiOjE3MzIxNzYwMDAsImlzcyI6IkJvc2tvQVBJIiwiYXVkIjoiQm9za29Gcm9udGVuZCJ9.signature",
  "user": {
    "id": 1,
    "name": "Admin Bosko",
    "email": "admin@bosko.com",
    "role": "Admin",
    "provider": "Local",
    "phone": "+34 600 000 001",
    "isActive": true,
    "createdAt": "2024-11-16T10:00:00Z"
  }
}
```

---

## ?? 2. POST /api/auth/login - LOGIN FALLIDO

**Request:**
```json
{
  "email": "admin@bosko.com",
  "password": "wrongpassword"
}
```

**Response (401 Unauthorized):**
```json
{
  "message": "Email o contraseña incorrectos"
}
```

---

## ?? 3. POST /api/auth/register - REGISTRO EXITOSO

**Request:**
```json
{
  "name": "Nuevo Usuario",
  "email": "nuevo@test.com",
  "password": "Password123!",
  "phone": "+34 600 111 222"
}
```

**Response (201 Created):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 5,
    "name": "Nuevo Usuario",
    "email": "nuevo@test.com",
    "role": "Customer",
    "provider": "Local",
    "phone": "+34 600 111 222",
    "isActive": true,
    "createdAt": "2024-11-16T11:30:00Z"
  }
}
```

---

## ?? 4. POST /api/auth/register - EMAIL YA EXISTE

**Request:**
```json
{
  "name": "Test",
  "email": "admin@bosko.com",
  "password": "Test123!"
}
```

**Response (400 Bad Request):**
```json
{
  "message": "El email ya está registrado"
}
```

---

## ?? 5. POST /api/auth/google-login - GOOGLE LOGIN EXITOSO

**Request:**
```json
{
  "token": "google_id_token_from_frontend_here..."
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 4,
    "name": "Usuario Google",
    "email": "usuario@gmail.com",
    "role": "Customer",
    "provider": "Google",
    "phone": null,
    "isActive": true,
    "createdAt": "2024-11-16T12:00:00Z"
  }
}
```

---

## ?? 6. POST /api/auth/forgot-password - ENVÍO EXITOSO

**Request:**
```json
{
  "email": "admin@bosko.com"
}
```

**Response (200 OK) - Modo DEBUG:**
```json
{
  "message": "Si el email existe, recibirás instrucciones para restablecer tu contraseña",
  "token": "ABC123XYZ456DEF789GHI012JKL345MNO678PQR==",
  "expiresAt": "2024-11-16T14:00:00Z"
}
```

**Response (200 OK) - Modo PRODUCCIÓN:**
```json
{
  "message": "Si el email existe, recibirás instrucciones para restablecer tu contraseña"
}
```

---

## ?? 7. POST /api/auth/reset-password - RESET EXITOSO

**Request:**
```json
{
  "email": "admin@bosko.com",
  "token": "ABC123XYZ456DEF789GHI012JKL345MNO678PQR==",
  "newPassword": "NuevaPassword123!"
}
```

**Response (200 OK):**
```json
{
  "message": "Contraseña actualizada exitosamente"
}
```

---

## ?? 8. POST /api/auth/reset-password - TOKEN INVÁLIDO

**Request:**
```json
{
  "email": "admin@bosko.com",
  "token": "invalid_token",
  "newPassword": "NuevaPassword123!"
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Token inválido o expirado"
}
```

---

## ?? 9. POST /api/auth/init-users - INICIALIZACIÓN

**Request:** (Sin body)

**Response (200 OK):**
```json
{
  "message": "Passwords inicializados para 3 usuarios",
  "password": "Bosko123!",
  "users": [
    {
      "email": "admin@bosko.com",
      "role": "Admin"
    },
    {
      "email": "employee@bosko.com",
      "role": "Employee"
    },
    {
      "email": "customer@bosko.com",
      "role": "Customer"
    }
  ]
}
```

---

## ?? 10. POST /api/products - CON AUTORIZACIÓN ADMIN

**Request:**
```http
POST /api/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Producto de Prueba",
  "description": "Creado por Admin",
  "price": 99.99,
  "stock": 10,
  "categoryId": 1,
  "image": "https://example.com/image.jpg"
}
```

**Response (201 Created):**
```json
{
  "id": 16,
  "name": "Producto de Prueba",
  "description": "Creado por Admin",
  "price": 99.99,
  "stock": 10,
  "image": "https://example.com/image.jpg",
  "categoryId": 1,
  "categoryName": "Camisas",
  "createdAt": "2024-11-16T13:00:00Z"
}
```

---

## ?? 11. POST /api/products - SIN TOKEN (401)

**Request:**
```http
POST /api/products
Content-Type: application/json

{
  "name": "Producto de Prueba",
  "price": 99.99,
  "stock": 10
}
```

**Response (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "00-abc123..."
}
```

---

## ?? 12. POST /api/products - CON TOKEN CUSTOMER (403)

**Request:**
```http
POST /api/products
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "name": "Producto de Prueba",
  "price": 99.99,
  "stock": 10
}
```

**Response (403 Forbidden):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-xyz789..."
}
```

---

## ? 13. GET /api/products - PÚBLICO (Sin Token)

**Request:**
```http
GET /api/products
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Camisa Oxford Azul",
    "description": "Camisa clásica de algodón 100%",
    "price": 49.99,
    "stock": 25,
    "image": "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=500",
    "categoryId": 1,
    "categoryName": "Camisas",
    "createdAt": "2024-11-16T10:00:00Z"
  },
  {
    "id": 2,
    "name": "Camisa Lino Blanca",
    "description": "Camisa de lino premium, ideal para verano",
    "price": 59.99,
    "stock": 30,
    "image": "https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf?w=500",
    "categoryId": 1,
    "categoryName": "Camisas",
    "createdAt": "2024-11-16T10:01:00Z"
  }
]
```

---

## ?? 14. DECODIFICACIÓN DEL JWT (jwt.io)

**Token:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6IkFkbWluIEJvc2tvIiwiZW1haWwiOiJhZG1pbkBib3Nrby5jb20iLCJyb2xlIjoiQWRtaW4iLCJwcm92aWRlciI6IkxvY2FsIiwicGhvbmUiOiIrMzQgNjAwIDAwMCAwMDEiLCJqdGkiOiI3ZjE2MTg4Ny0yYzg1LTRkOTEtYmI0ZC0zNzE5ZGJhNjg1ZWEiLCJleHAiOjE3MzIxNzYwMDAsImlzcyI6IkJvc2tvQVBJIiwiYXVkIjoiQm9za29Gcm9udGVuZCJ9.signature
```

**Header:**
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload:**
```json
{
  "sub": "1",
  "name": "Admin Bosko",
  "email": "admin@bosko.com",
  "role": "Admin",
  "provider": "Local",
  "phone": "+34 600 000 001",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin",
  "jti": "7f161887-2c85-4d91-bb4d-3719dba685ea",
  "exp": 1732176000,
  "iss": "BoskoAPI",
  "aud": "BoskoFrontend"
}
```

---

## ?? 15. VALIDACIÓN DE MODELO - BAD REQUEST

**Request:**
```json
{
  "email": "invalid-email",
  "password": ""
}
```

**Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "Email inválido"
    ],
    "Password": [
      "La contraseña es requerida"
    ]
  }
}
```

---

## ?? 16. ESTADO DE USUARIO INACTIVO

**Request:**
```json
{
  "email": "inactive@bosko.com",
  "password": "Bosko123!"
}
```

**Response (401 Unauthorized):**
```json
{
  "message": "Usuario inactivo. Contacte al administrador"
}
```

---

## ?? RESUMEN DE STATUS CODES

| Endpoint | Success | Auth Error | Permission Error | Validation Error |
|----------|---------|------------|------------------|------------------|
| POST /auth/login | 200 | 401 | - | 400 |
| POST /auth/register | 201 | - | - | 400 |
| POST /auth/google-login | 200 | 401 | - | 400 |
| POST /auth/forgot-password | 200 | - | - | 400 |
| POST /auth/reset-password | 200 | - | - | 400 |
| GET /products | 200 | - | - | - |
| POST /products | 201 | 401 | 403 | 400 |
| PUT /products/{id} | 204 | 401 | 403 | 400 |
| DELETE /products/{id} | 204 | 401 | 403 | - |

---

## ?? MAPEO DE CLAIMS PARA FRONTEND

El frontend Angular puede leer cualquiera de estos claims:

**Para User ID:**
- `sub` ? (implementado)
- `userId` (compatible)
- `nameid` (compatible)
- `id` (compatible)

**Para Nombre:**
- `name` ? (implementado)
- `unique_name` (compatible)
- `given_name` (compatible)

**Para Email:**
- `email` ? (implementado)

**Para Rol:**
- `role` ? (implementado)
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role` ? (también incluido)

**Para Provider:**
- `provider` ? (implementado)

---

**Backend Team | Bosko E-Commerce**  
**Documentación de API de Autenticación**
