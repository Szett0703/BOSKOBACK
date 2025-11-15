# ?? ESTRUCTURA DEL PROYECTO BOSKOBACK

## ?? Estructura de Carpetas

```
BOSKOBACK/
??? Controllers/
?   ??? AuthController.cs              ? Autenticación completa
?   ??? ProductsController.cs          ? CRUD + Reseñas
?   ??? CategoriesController.cs        ? CRUD
?   ??? OrdersController.cs            ? Gestión de pedidos
?   ??? UsersController.cs             ? Administración (Admin)
?   ??? WishlistController.cs          ? Lista de deseos
?   ??? AddressesController.cs         ? Direcciones
?   ??? ReviewsController.cs           ? Gestión de reseñas
?
??? Models/
?   ??? User.cs                        ? Con RoleId y navegaciones
?   ??? Role.cs                        ? Admin, Employee, Customer
?   ??? Product.cs                     ? Con navegaciones
?   ??? Category.cs                    ? Categorías
?   ??? Order.cs                       ? Pedidos
?   ??? OrderItem.cs                   ? Items de pedido
?   ??? Address.cs                     ? Direcciones de envío
?   ??? Review.cs                      ? Reseñas de productos
?   ??? WishlistItem.cs                ? Lista de deseos
?
??? DTOs/
?   ??? AuthDTOs.cs                    ? Register, Login, Google, etc.
?   ??? ProductDto.cs                  ? Productos
?   ??? CategoryDto.cs                 ? Categorías
?   ??? OrderDTOs.cs                   ? Orders y OrderItems
?   ??? ChangePasswordDto.cs           ? Cambio de contraseña
?   ??? AddressDto.cs                  ? Direcciones
?   ??? ReviewDto.cs                   ? Reseñas
?   ??? UserDto.cs                     ? Usuarios (Admin)
?   ??? UpdateOrderStatusDto.cs        ? Actualizar estado
?
??? Data/
?   ??? BoskoDbContext.cs              ? DbContext completo
?
??? Migrations/
?   ??? 20251113141040_InitialCreate.cs
?   ??? 20251113141040_InitialCreate.Designer.cs
?   ??? 20251113161933_AddRolesAndUserManagementFeatures.cs
?   ??? 20251113161933_AddRolesAndUserManagementFeatures.Designer.cs
?   ??? BoskoDbContextModelSnapshot.cs
?
??? Properties/
?   ??? launchSettings.json
?
??? obj/
??? bin/
?
??? Program.cs                         ? Configuración principal
??? appsettings.json                   ? Configuración
??? appsettings.Development.json
??? BOSKOBACK.csproj                   ? Proyecto
?
??? Documentación/
    ??? README.md                      ? Guía principal
    ??? EXECUTIVE_SUMMARY.md           ? Resumen ejecutivo
    ??? IMPLEMENTATION_CHECKLIST.md    ? Checklist completo
    ??? POSTMAN_GUIDE.md               ? Guía de pruebas
    ??? DEPLOYMENT_GUIDE.md            ? Guía de despliegue
    ??? PASSWORDS.md                   ? Hashes y seguridad
    ??? SeedData.sql                   ? Datos de prueba
    ??? PROJECT_STRUCTURE.md           ? Este archivo
```

---

## ?? Estadísticas del Proyecto

### Archivos de Código
- **Controllers**: 8 archivos
- **Models**: 9 archivos
- **DTOs**: 8 archivos
- **Data**: 1 archivo (DbContext)
- **Migraciones**: 5 archivos

**Total Código**: 31 archivos

### Documentación
- **README.md**: Guía completa de uso
- **EXECUTIVE_SUMMARY.md**: Resumen para stakeholders
- **IMPLEMENTATION_CHECKLIST.md**: Checklist técnico detallado
- **POSTMAN_GUIDE.md**: Guía de testing con ejemplos
- **DEPLOYMENT_GUIDE.md**: Guía de despliegue paso a paso
- **PASSWORDS.md**: Seguridad y hashes
- **SeedData.sql**: Script de datos de prueba

**Total Documentación**: 7 archivos

---

## ?? Archivos Clave

### Configuración
- **Program.cs**: Configuración de servicios, middleware, JWT, CORS
- **appsettings.json**: Connection strings, JWT settings

### Base de Datos
- **BoskoDbContext.cs**: Configuración EF Core y relaciones
- **Migrations/**: Historial de cambios en esquema
- **SeedData.sql**: Datos iniciales para desarrollo

### Seguridad
- **AuthController.cs**: Login, registro, recuperación de contraseña
- **JWT Configuration**: En Program.cs
- **Role-Based Authorization**: Atributos en controllers

---

## ?? Descripción de Controllers

### AuthController (6 endpoints)
Gestiona toda la autenticación y seguridad:
- `POST /register` - Registro de nuevos usuarios
- `POST /login` - Login tradicional
- `POST /google-login` - Login con Google OAuth
- `POST /forgot-password` - Solicitud de reset
- `POST /reset-password` - Reset con token
- `POST /change-password` - Cambio autenticado

### ProductsController (8 endpoints)
Gestión de productos y reseñas:
- `GET /products` - Listar con filtros
- `GET /products/{id}` - Detalle
- `POST /products` - Crear (Admin)
- `PUT /products/{id}` - Actualizar (Admin)
- `DELETE /products/{id}` - Eliminar (Admin)
- `GET /products/{id}/reviews` - Ver reseñas
- `POST /products/{id}/reviews` - Crear reseña

### CategoriesController (5 endpoints)
Gestión de categorías:
- `GET /categories` - Listar
- `GET /categories/{id}` - Detalle
- `POST /categories` - Crear (Admin)
- `PUT /categories/{id}` - Actualizar (Admin)
- `DELETE /categories/{id}` - Eliminar (Admin)

### OrdersController (4 endpoints)
Gestión de pedidos:
- `POST /orders` - Crear pedido (checkout)
- `GET /orders` - Listar (según rol)
- `GET /orders/{id}` - Ver detalle
- `PUT /orders/{id}` - Actualizar estado (Admin/Employee)

### UsersController (4 endpoints)
Administración de usuarios (Admin):
- `GET /users` - Listar usuarios
- `POST /users` - Crear usuario
- `PUT /users/{id}` - Actualizar usuario
- `DELETE /users/{id}` - Eliminar usuario

### WishlistController (3 endpoints)
Lista de deseos:
- `GET /wishlist` - Ver mi wishlist
- `POST /wishlist/{productId}` - Agregar
- `DELETE /wishlist/{productId}` - Eliminar

### AddressesController (4 endpoints)
Direcciones de envío:
- `GET /addresses` - Mis direcciones
- `POST /addresses` - Crear dirección
- `PUT /addresses/{id}` - Actualizar
- `DELETE /addresses/{id}` - Eliminar

### ReviewsController (2 endpoints)
Gestión de reseñas:
- `PUT /reviews/{id}` - Actualizar reseña
- `DELETE /reviews/{id}` - Eliminar reseña

---

## ??? Descripción de Modelos

### User
Usuario del sistema con rol asignado.
**Relaciones**: Role, Orders, Addresses, Reviews, WishlistItems

### Role
Roles del sistema (Admin, Employee, Customer).
**Relaciones**: Users

### Product
Productos del catálogo.
**Relaciones**: Category, Reviews, WishlistItems

### Category
Categorías de productos.
**Relaciones**: Products

### Order
Pedidos de clientes.
**Relaciones**: User, OrderItems

### OrderItem
Items individuales de un pedido.
**Relaciones**: Order, Product

### Address
Direcciones de envío de usuarios.
**Relaciones**: User

### Review
Reseñas de productos.
**Relaciones**: User, Product

### WishlistItem
Items en lista de deseos.
**Relaciones**: User, Product

---

## ?? Flujo de Datos

### Registro y Login
```
Cliente ? AuthController ? BoskoDbContext ? SQL Server
                         ?
                    JWT Token
                         ?
                    Cliente
```

### Crear Pedido
```
Cliente + JWT ? OrdersController ? Validar Productos
                                 ?
                            Calcular Total
                                 ?
                         Crear Order + Items
                                 ?
                         BoskoDbContext ? SQL Server
```

### Administrar Productos (Admin)
```
Admin + JWT ? ProductsController ? Validar Rol
                                  ?
                         CRUD Operations
                                  ?
                         BoskoDbContext ? SQL Server
```

---

## ?? Configuración de Seguridad

### JWT
- **Issuer**: BoskoAPI
- **Audience**: BoskoApp
- **Expiration**: 1440 minutos (24 horas)
- **Claims**: NameIdentifier, Email, Name, Role

### BCrypt
- **Rounds**: 11 (balance seguridad/performance)
- **Salt**: Automático e incluido en hash

### Authorization
- **[Authorize]**: Requiere login
- **[Authorize(Roles="Admin")]**: Solo Admin
- **[Authorize(Roles="Admin,Employee")]**: Admin o Employee

---

## ?? Dependencias (NuGet)

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.x" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.x" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.x" />
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.x" />
  <PackageReference Include="BCrypt.Net-Next" Version="4.0.x" />
  <PackageReference Include="Google.Apis.Auth" Version="1.x" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.x" />
</ItemGroup>
```

---

## ?? Características Técnicas

### Arquitectura
- ? API RESTful
- ? Repository Pattern (implícito con DbContext)
- ? DTO Pattern
- ? Dependency Injection
- ? Async/Await

### Patrones de Diseño
- ? Repository (EF Core)
- ? Data Transfer Object
- ? Dependency Injection
- ? Builder Pattern (JWT)

### Buenas Prácticas
- ? Separación de responsabilidades
- ? Validaciones en backend
- ? Manejo de errores consistente
- ? Nomenclatura clara
- ? Código documentado

---

## ?? Guías de Uso

1. **Para Developers**: Ver `README.md`
2. **Para Testing**: Ver `POSTMAN_GUIDE.md`
3. **Para Deployment**: Ver `DEPLOYMENT_GUIDE.md`
4. **Para Management**: Ver `EXECUTIVE_SUMMARY.md`
5. **Para Checklist**: Ver `IMPLEMENTATION_CHECKLIST.md`

---

## ? Estado del Proyecto

**Estado**: ? COMPLETADO
**Fecha**: Noviembre 2024
**Versión**: 1.0.0
**Framework**: ASP.NET Core 8.0

---

*Este proyecto está listo para testing y despliegue.*
