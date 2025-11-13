# Bosko Backend API - ASP.NET Core 8.0

Backend API completo para la aplicación de e-commerce Bosko, construido con ASP.NET Core 8.0, Entity Framework Core y SQL Server.

## ?? Características

- ? Autenticación JWT (JSON Web Tokens)
- ? Registro y Login de usuarios tradicional
- ? Autenticación con Google OAuth
- ? Recuperación de contraseña
- ? Gestión de productos y categorías
- ? Sistema de pedidos (orders) completo
- ? CORS configurado para Angular frontend
- ? Documentación Swagger/OpenAPI
- ? Base de datos con datos iniciales (seed data)

## ?? Prerrequisitos

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, o Full)
- Visual Studio 2022 o VS Code (opcional)

## ?? Configuración

### 1. Cadena de Conexión

La cadena de conexión en `appsettings.json` está configurada para SQL Server Express:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=LOCALHOST\\SQLEXPRESS;Database=BOSKO;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
```

**Ajusta el servidor según tu entorno:**
- SQL Server Express: `LOCALHOST\\SQLEXPRESS`
- SQL Server LocalDB: `(localdb)\\mssqllocaldb`
- SQL Server Full: `localhost` o tu servidor

### 2. JWT Secret Key

En producción, cambia la clave secreta en `appsettings.json`:

```json
"Jwt": {
  "Key": "TuClaveSecretaSuperSeguraYLargaAqui",
  "Issuer": "BoskoAPI",
  "Audience": "BoskoAPIUsers",
  "ExpireMinutes": 1440
}
```

### 3. Migración y Base de Datos

La base de datos ya ha sido creada con el comando:

```bash
dotnet ef database update
```

Si necesitas recrear la base de datos:

```bash
# Eliminar la base de datos
dotnet ef database drop

# Actualizar/Crear base de datos
dotnet ef database update
```

## ?? Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7xxx`
- HTTP: `http://localhost:5xxx`

(Los puertos pueden variar, revisa la consola)

## ?? Documentación API (Swagger)

Una vez ejecutada la aplicación, accede a Swagger en:

```
https://localhost:7xxx/swagger
```

## ?? Endpoints de Autenticación

### Registro de Usuario
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "password": "Password123"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan@example.com",
  "password": "Password123"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Juan Pérez",
    "email": "juan@example.com"
  }
}
```

### Login con Google
```http
POST /api/auth/google-login
Content-Type: application/json

{
  "idToken": "TOKEN_DE_GOOGLE_AQUI"
}
```

### Recuperar Contraseña
```http
POST /api/auth/forgot-password
Content-Type: application/json

{
  "email": "juan@example.com"
}
```

### Resetear Contraseña
```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "juan@example.com",
  "token": "GUID_TOKEN",
  "newPassword": "NewPassword123"
}
```

## ??? Endpoints de Productos

### Listar Productos
```http
GET /api/products
GET /api/products?categoryId=1
```

### Obtener Producto por ID
```http
GET /api/products/1
```

## ?? Endpoints de Categorías

### Listar Categorías
```http
GET /api/categories
```

### Obtener Categoría por ID
```http
GET /api/categories/1
```

## ?? Endpoints de Pedidos (Requiere Autenticación)

### Crear Pedido
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

### Obtener Mis Pedidos
```http
GET /api/orders
Authorization: Bearer {JWT_TOKEN}
```

### Obtener Pedido por ID
```http
GET /api/orders/1
Authorization: Bearer {JWT_TOKEN}
```

## ?? Autenticación JWT

Para endpoints protegidos, incluye el token JWT en el header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

En Swagger, usa el botón "Authorize" e ingresa: `Bearer {tu_token}`

## ?? Datos Iniciales

La base de datos incluye:

### Categorías (6):
1. Men - Fashion for Men
2. Women - Fashion for Women
3. Kids - Fashion for Kids
4. Accessories - Fashion Accessories
5. Shoes - Footwear Collection
6. Sports - Sports & Active Wear

### Productos (8):
1. Classic Winter Jacket - $129.99
2. Elegant Summer Dress - $89.99
3. Designer Sneakers - $149.99
4. Luxury Watch - $299.99
5. Kids Casual T-Shirt - $24.99
6. Sports Running Shoes - $119.99
7. Leather Handbag - $179.99
8. Denim Jeans - $79.99

## ?? CORS

El CORS está configurado para permitir requests desde:
```
http://localhost:4200
```

Para producción, actualiza en `Program.cs`:
```csharp
policy => policy.WithOrigins("https://tu-dominio.com")
```

## ?? Estructura del Proyecto

```
BOSKOBACK/
??? Controllers/
?   ??? AuthController.cs       # Autenticación y autorización
?   ??? ProductsController.cs   # Gestión de productos
?   ??? CategoriesController.cs # Gestión de categorías
?   ??? OrdersController.cs     # Gestión de pedidos
??? Data/
?   ??? BoskoDbContext.cs       # Contexto de Entity Framework
??? DTOs/
?   ??? AuthDTOs.cs             # DTOs de autenticación
?   ??? ProductDto.cs           # DTOs de productos
?   ??? CategoryDto.cs          # DTOs de categorías
?   ??? OrderDTOs.cs            # DTOs de pedidos
??? Models/
?   ??? User.cs                 # Modelo de usuario
?   ??? Product.cs              # Modelo de producto
?   ??? Category.cs             # Modelo de categoría
?   ??? Order.cs                # Modelo de pedido
?   ??? OrderItem.cs            # Modelo de item de pedido
??? Migrations/                 # Migraciones de EF Core
??? Program.cs                  # Configuración principal
??? appsettings.json           # Configuración de la aplicación
```

## ??? Tecnologías Utilizadas

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- JWT Bearer Authentication
- BCrypt.Net (para hash de contraseñas)
- Google.Apis.Auth (para autenticación con Google)
- Swagger/Swashbuckle (documentación API)

## ?? Notas de Seguridad

1. **JWT Secret**: Cambiar la clave secreta en producción y almacenarla en variables de entorno
2. **CORS**: Restringir orígenes permitidos en producción
3. **HTTPS**: Usar siempre HTTPS en producción
4. **Contraseñas**: Las contraseñas se hashean con BCrypt antes de almacenarlas
5. **Tokens de Reset**: Los tokens de recuperación expiran en 1 hora

## ?? Troubleshooting

### Error de Conexión a SQL Server

Si tienes problemas conectando a SQL Server:

1. Verifica que SQL Server esté corriendo
2. Ajusta la cadena de conexión en `appsettings.json`
3. Para SQL Server Express: usa `LOCALHOST\\SQLEXPRESS`
4. Para LocalDB: usa `(localdb)\\mssqllocaldb`

### Error de CORS

Si el frontend no puede conectarse:

1. Verifica que el origen esté permitido en `Program.cs`
2. Asegúrate que el frontend corra en `http://localhost:4200`

### Error de JWT Inválido

1. Verifica que el token no haya expirado (24 horas por defecto)
2. Asegúrate de incluir "Bearer " antes del token
3. Verifica que la clave secreta sea la misma

## ?? Recuperación de Contraseña

?? **Nota**: El envío de emails está simulado. Los enlaces de recuperación se imprimen en la consola.

Para implementar envío real de emails:

1. Instala un paquete SMTP (ej: `MailKit`)
2. Configura credenciales SMTP en `appsettings.json`
3. Implementa el envío en `AuthController.ForgotPassword`

## ?? Próximos Pasos

Para producción considera:

1. Implementar refresh tokens
2. Agregar rate limiting
3. Implementar logging (Serilog, NLog)
4. Agregar validaciones adicionales
5. Implementar paginación en listas
6. Agregar caché (Redis)
7. Implementar envío real de emails
8. Agregar pruebas unitarias e integración

## ?? Licencia

Este proyecto fue creado para la aplicación Bosko.

## ????? Autor

Desarrollado como backend para la aplicación de e-commerce Bosko.
