# BOSKOBACK - Backend completo con Roles, Permisos y Gestión Integral

Backend ASP.NET Core 8 + EF Core con funcionalidades completas de e-commerce:
- Autenticación JWT (registro, login, recuperación de contraseña, Google OAuth)
- Autorización basada en roles (Admin, Employee, Customer)
- CRUD completo de productos y categorías (solo Admin)
- Gestión de pedidos con permisos por rol
- Gestión de usuarios (solo Admin)
- Sistema de reseñas de productos
- Lista de deseos (Wishlist)
- Gestión de direcciones de envío

## ?? INICIO RÁPIDO

### URLs del Backend
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `http://localhost:5000/swagger`

### Arrancar el Backend
```bash
# 1. Aplicar migraciones (primera vez)
dotnet ef database update

# 2. Poblar datos de prueba (primera vez)
sqlcmd -S LOCALHOST\SQLEXPRESS -d BOSKO -i SeedData.sql

# 3. Iniciar API
dotnet run
```

### Verificar que Funciona
```bash
# Abrir Swagger
http://localhost:5000/swagger

# O probar endpoint
curl http://localhost:5000/api/categories
```

## ?? Requisitos

- .NET 8 SDK
- SQL Server (Express, LocalDB o Full)
- Conexión configurada en `appsettings.json`

## ??? Modelo de Datos

### Entidades principales:
- **Role**: roles del sistema (Admin, Employee, Customer)
- **User**: usuarios con relación a rol
- **Product**: productos del catálogo
- **Category**: categorías de productos
- **Order**: pedidos de clientes
- **OrderItem**: items de cada pedido
- **Address**: direcciones de envío de usuarios
- **Review**: reseñas de productos
- **WishlistItem**: lista de deseos de usuarios

### Relaciones:
- User ? Role (many-to-one)
- User ? Orders (one-to-many, NO CASCADE para preservar histórico)
- User ? Addresses (one-to-many, CASCADE)
- User ? Reviews (one-to-many, CASCADE)
- User ? WishlistItems (one-to-many, CASCADE)
- Product ? Category (many-to-one, RESTRICT para evitar borrados accidentales)
- Product ? Reviews (one-to-many, CASCADE)
- Product ? WishlistItems (one-to-many, CASCADE)
- Order ? OrderItems (one-to-many, CASCADE)
- OrderItem ? Product (many-to-one, RESTRICT para preservar histórico)

## ?? Configuración Inicial

### 1. Cadena de Conexión

Actualiza `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=LOCALHOST\\SQLEXPRESS;Database=BOSKO;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. JWT Configuration

En `appsettings.json`:

```json
"Jwt": {
  "Key": "TuClaveSecretaSuperSeguraYLargaDe32CaracteresOMas",
  "Issuer": "BoskoAPI",
  "Audience": "BoskoApp",
  "ExpireMinutes": 1440
}
```

### 3. Aplicar Migraciones

```bash
# Crear/actualizar base de datos
dotnet ef database update
```

Esto creará:
- Todas las tablas necesarias
- Roles por defecto: Admin (1), Employee (2), Customer (3)

### 4. Poblar con Datos de Prueba

Ejecuta el script `SeedData.sql` en SQL Server Management Studio o con sqlcmd:

```bash
sqlcmd -S LOCALHOST\SQLEXPRESS -d BOSKO -i SeedData.sql
```

Este script crea:
- Usuario Admin (admin@bosko.com / Admin123)
- Usuarios de prueba (employee@bosko.com, customer@bosko.com / Password123)
- 6 categorías
- 10 productos
- Pedidos de ejemplo
- Reseñas
- Direcciones
- Items en wishlist

### 5. Ejecutar la API

```bash
dotnet run
```

Accede a Swagger: `http://localhost:5000/swagger`

## ?? CORS y Frontend

El backend está configurado para aceptar peticiones desde:
```
http://localhost:4200
```

Si tu frontend Angular corre en otro puerto, actualiza en `Program.cs`:
```csharp
policy => policy.WithOrigins("http://localhost:XXXX")
```

## ?? Autenticación y Autorización

### Roles del Sistema

1. **Admin**: acceso completo
   - Gestión de productos y categorías (CRUD)
   - Gestión de usuarios (CRUD)
   - Ver y gestionar todos los pedidos
   - Moderar reseñas

2. **Employee**: gestión operativa
   - Ver todos los pedidos
   - Actualizar estado de pedidos
   - NO puede crear/editar productos ni gestionar usuarios

3. **Customer**: usuario final
   - Registro y login
   - Ver productos y categorías
   - Crear pedidos
   - Gestionar sus direcciones
   - Crear reseñas
   - Gestionar su wishlist
   - Ver solo sus propios pedidos

### Endpoints de Autenticación

#### Registro
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "password": "Password123"
}
```
? Retorna token JWT automáticamente. Asigna rol Customer por defecto.

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@bosko.com",
  "password": "Admin123"
}
```
? Retorna token JWT con claim de rol incluido.

#### Login con Google
```http
POST /api/auth/google-login
Content-Type: application/json

{
  "idToken": "GOOGLE_ID_TOKEN"
}
```

#### Recuperar Contraseña
```http
POST /api/auth/forgot-password
Content-Type: application/json

{
  "email": "usuario@example.com"
}
```
? Genera token de reset válido por 1 hora. (En dev se imprime en consola)

```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "usuario@example.com",
  "token": "GUID-TOKEN",
  "newPassword": "NuevaPassword123"
}
```

#### Cambiar Contraseña (autenticado)
```http
POST /api/auth/change-password
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "oldPassword": "Password123",
  "newPassword": "NuevaPassword456"
}
```

## ?? Endpoints de Productos

### Listar productos (público)
```http
GET /api/products
GET /api/products?categoryId=1
GET /api/products?search=zapato
GET /api/products?categoryId=2&search=verano
```

### Obtener producto (público)
```http
GET /api/products/{id}
```

### Crear producto (Admin)
```http
POST /api/products
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "name": "Producto Nuevo",
  "description": "Descripción detallada",
  "price": 99.99,
  "imageUrl": "https://example.com/image.jpg",
  "categoryId": 1
}
```

### Actualizar producto (Admin)
```http
PUT /api/products/{id}
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "name": "Producto Actualizado",
  "description": "Nueva descripción",
  "price": 109.99,
  "imageUrl": "https://example.com/new-image.jpg",
  "categoryId": 1
}
```

### Eliminar producto (Admin)
```http
DELETE /api/products/{id}
Authorization: Bearer {JWT_TOKEN}
```
?? No permite borrar si tiene pedidos asociados.

## ??? Endpoints de Categorías

### Listar categorías (público)
```http
GET /api/categories
```

### Crear categoría (Admin)
```http
POST /api/categories
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "name": "Nueva Categoría",
  "description": "Descripción",
  "imageUrl": "https://example.com/cat.jpg"
}
```

### Actualizar categoría (Admin)
```http
PUT /api/categories/{id}
```

### Eliminar categoría (Admin)
```http
DELETE /api/categories/{id}
```
?? No permite borrar si tiene productos asociados.

## ?? Endpoints de Pedidos

### Crear pedido (checkout)
```http
POST /api/orders
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

[
  {
    "productId": 1,
    "quantity": 2
  },
  {
    "productId": 3,
    "quantity": 1
  }
]
```
? Recalcula precios desde DB. Aplica 10% de impuesto. Status inicial: "Pending".

### Listar pedidos
```http
GET /api/orders
GET /api/orders?status=Pending
Authorization: Bearer {JWT_TOKEN}
```
- **Customer**: ve solo sus pedidos
- **Admin/Employee**: ven todos los pedidos

### Ver detalle de pedido
```http
GET /api/orders/{id}
Authorization: Bearer {JWT_TOKEN}
```
- **Customer**: solo si es el propietario
- **Admin/Employee**: cualquier pedido

### Actualizar estado de pedido (Admin/Employee)
```http
PUT /api/orders/{id}
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "status": "Shipped"
}
```
Estados comunes: Pending, Processing, Shipped, Completed, Cancelled

## ?? Endpoints de Usuarios (Admin)

### Listar usuarios
```http
GET /api/users
Authorization: Bearer {JWT_TOKEN}
```

### Crear usuario
```http
POST /api/users
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "name": "Nuevo Empleado",
  "email": "empleado@bosko.com",
  "password": "Password123",
  "roleName": "Employee"
}
```
? Si no se proporciona password, se genera uno aleatorio.

### Actualizar usuario
```http
PUT /api/users/{id}
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "name": "Nombre Actualizado",
  "email": "nuevo@email.com",
  "roleName": "Admin"
}
```

### Eliminar usuario
```http
DELETE /api/users/{id}
Authorization: Bearer {JWT_TOKEN}
```
?? No permite borrar si tiene pedidos (preserva histórico).

## ? Endpoints de Reseñas

### Ver reseñas de un producto (público)
```http
GET /api/products/{id}/reviews
```

### Crear reseña (Customer)
```http
POST /api/products/{id}/reviews
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "rating": 5,
  "comment": "Excelente producto, muy recomendado"
}
```
?? Un usuario solo puede reseñar un producto una vez.

### Actualizar reseña
```http
PUT /api/reviews/{id}
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "rating": 4,
  "comment": "Comentario actualizado"
}
```
? Solo el autor o Admin.

### Eliminar reseña
```http
DELETE /api/reviews/{id}
Authorization: Bearer {JWT_TOKEN}
```
? Solo el autor o Admin (moderación).

## ?? Endpoints de Wishlist

### Ver mi wishlist
```http
GET /api/wishlist
Authorization: Bearer {JWT_TOKEN}
```

### Agregar a wishlist
```http
POST /api/wishlist/{productId}
Authorization: Bearer {JWT_TOKEN}
```

### Eliminar de wishlist
```http
DELETE /api/wishlist/{productId}
Authorization: Bearer {JWT_TOKEN}
```

## ?? Endpoints de Direcciones

### Listar mis direcciones
```http
GET /api/addresses
Authorization: Bearer {JWT_TOKEN}
```

### Crear dirección
```http
POST /api/addresses
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json

{
  "street": "Calle Principal 123",
  "city": "Madrid",
  "state": "Madrid",
  "postalCode": "28001",
  "country": "España"
}
```

### Actualizar dirección
```http
PUT /api/addresses/{id}
Authorization: Bearer {JWT_TOKEN}
```

### Eliminar dirección
```http
DELETE /api/addresses/{id}
Authorization: Bearer {JWT_TOKEN}
```
? Solo el propietario.

## ?? Pruebas con Postman

### 1. Login como Admin
```json
POST /api/auth/login
{
  "email": "admin@bosko.com",
  "password": "Admin123"
}
```
Copia el token recibido.

### 2. Autorización en Postman
En cada request protegido:
- Tab "Authorization"
- Type: "Bearer Token"
- Token: [pega el JWT]

### 3. Probar flujos por rol

**Como Admin:**
- Crear producto ?
- Crear categoría ?
- Ver todos los pedidos ?
- Gestionar usuarios ?

**Como Customer (customer@bosko.com / Password123):**
- Crear producto ? (403 Forbidden)
- Ver solo sus pedidos ?
- Crear reseña ?
- Gestionar wishlist ?

**Como Employee (employee@bosko.com / Password123):**
- Ver todos los pedidos ?
- Actualizar estado de pedido ?
- Crear producto ? (403)

## ?? Seguridad

- Contraseñas hasheadas con BCrypt (salt automático)
- JWT con claims de rol para autorización
- Tokens de reset de contraseña expiran en 1 hora
- Validación de propiedad en endpoints sensibles
- Prevent cascade delete en relaciones con histórico

## ?? Troubleshooting

### Error de conexión a SQL Server
- Verifica que SQL Server esté corriendo
- Ajusta el nombre del servidor en la cadena de conexión
- Para LocalDB: `(localdb)\\mssqllocaldb`

### 403 Forbidden
- Verifica que el token sea válido y no haya expirado
- Asegúrate de incluir "Bearer " antes del token
- Verifica que tu rol tenga permisos para ese endpoint

### Frontend no conecta (ERR_CONNECTION_REFUSED)
- ? Verifica que el backend esté corriendo: `dotnet run`
- ? Verifica que el puerto sea 5000: `http://localhost:5000/swagger`
- ? Verifica CORS en `Program.cs` para tu puerto de frontend

### Migraciones pendientes
```bash
dotnet ef database update
```

## ?? Swagger

Accede a `/swagger` en desarrollo: `http://localhost:5000/swagger`

Para usar endpoints protegidos:
1. Haz login y copia el token
2. Click en "Authorize" (candado)
3. Ingresa: `Bearer {tu-token}`

## ?? Próximas Mejoras

- [ ] Envío real de emails (SMTP/SendGrid)
- [ ] Paginación en listados grandes
- [ ] Rate limiting
- [ ] Logging estructurado (Serilog)
- [ ] Tests unitarios e integración
- [ ] Caché con Redis
- [ ] Refresh tokens
- [ ] Gestión de inventario (stock)

## ?? Documentación Adicional

- `QUICK_START.md` - Guía de inicio rápido
- `POSTMAN_GUIDE.md` - Testing exhaustivo
- `DEPLOYMENT_GUIDE.md` - Despliegue a producción
- `FIX_CONNECTION_ERROR.md` - Solución a errores de conexión

## ?? Licencia

Proyecto desarrollado para Bosko E-Commerce.
