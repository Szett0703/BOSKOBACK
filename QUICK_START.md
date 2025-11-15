# ?? INICIO RÁPIDO - BOSKOBACK

## ? 5 Pasos para Empezar

### 1?? Aplicar Migraciones
```bash
dotnet ef database update
```
? Crea todas las tablas y roles

### 2?? Poblar Datos de Prueba
```bash
sqlcmd -S LOCALHOST\SQLEXPRESS -d BOSKO -i SeedData.sql
```
? Crea usuarios, productos, categorías, pedidos, etc.

### 3?? Ejecutar API
```bash
dotnet run
```
? Inicia el servidor en https://localhost:7xxx

### 4?? Abrir Swagger
```
https://localhost:7xxx/swagger
```
? Interfaz interactiva para probar endpoints

### 5?? Hacer Login
```http
POST /api/auth/login
{
  "email": "admin@bosko.com",
  "password": "Admin123"
}
```
? Copia el token para usar endpoints protegidos

---

## ?? Credenciales de Prueba

```
Admin (Todo el acceso):
  ?? admin@bosko.com
  ?? Admin123

Employee (Gestión de pedidos):
  ?? employee@bosko.com
  ?? Password123

Customer (Compras normales):
  ?? customer@bosko.com
  ?? Password123
```

---

## ?? Pruebas Rápidas en Swagger

### 1. Login como Admin
```
POST /api/auth/login
{
  "email": "admin@bosko.com",
  "password": "Admin123"
}
```
? Copia el `token` del response

### 2. Autorizar en Swagger
- Click en el botón **"Authorize"** ?? (arriba a la derecha)
- Ingresa: `Bearer {tu-token-aquí}`
- Click **"Authorize"**
- Click **"Close"**

### 3. Probar Endpoints Protegidos

**Ver todos los usuarios (Admin):**
```
GET /api/users
```

**Crear un producto (Admin):**
```
POST /api/products
{
  "name": "Producto Nuevo",
  "description": "Descripción",
  "price": 99.99,
  "imageUrl": "https://example.com/img.jpg",
  "categoryId": 1
}
```

**Ver productos (público):**
```
GET /api/products
```

**Crear un pedido:**
```
POST /api/orders
[
  {
    "productId": 1,
    "quantity": 2
  }
]
```

---

## ?? Flujo Completo de Prueba

### Flujo Customer (Compra Normal)

1. **Login como Customer**
   ```
   POST /api/auth/login
   Email: customer@bosko.com
   Password: Password123
   ```

2. **Ver productos disponibles**
   ```
   GET /api/products
   ```

3. **Agregar a wishlist**
   ```
   POST /api/wishlist/1
   ```

4. **Ver mi wishlist**
   ```
   GET /api/wishlist
   ```

5. **Crear dirección de envío**
   ```
   POST /api/addresses
   {
     "street": "Calle Principal 123",
     "city": "Madrid",
     "state": "Madrid",
     "postalCode": "28001",
     "country": "España"
   }
   ```

6. **Crear pedido**
   ```
   POST /api/orders
   [
     { "productId": 1, "quantity": 2 },
     { "productId": 3, "quantity": 1 }
   ]
   ```

7. **Ver mis pedidos**
   ```
   GET /api/orders
   ```

8. **Crear reseña de producto**
   ```
   POST /api/products/1/reviews
   {
     "rating": 5,
     "comment": "Excelente producto"
   }
   ```

### Flujo Admin (Gestión)

1. **Login como Admin**
   ```
   POST /api/auth/login
   Email: admin@bosko.com
   Password: Admin123
   ```

2. **Ver todos los usuarios**
   ```
   GET /api/users
   ```

3. **Crear nueva categoría**
   ```
   POST /api/categories
   {
     "name": "Electrónica",
     "description": "Productos electrónicos",
     "imageUrl": "https://example.com/electronics.jpg"
   }
   ```

4. **Crear producto en esa categoría**
   ```
   POST /api/products
   {
     "name": "Smartphone",
     "description": "Último modelo",
     "price": 599.99,
     "imageUrl": "https://example.com/phone.jpg",
     "categoryId": 7
   }
   ```

5. **Ver todos los pedidos**
   ```
   GET /api/orders
   ```

6. **Actualizar estado de pedido**
   ```
   PUT /api/orders/1
   {
     "status": "Shipped"
   }
   ```

7. **Crear nuevo empleado**
   ```
   POST /api/users
   {
     "name": "Nuevo Empleado",
     "email": "empleado2@bosko.com",
     "password": "Password123",
     "roleName": "Employee"
   }
   ```

---

## ? Pruebas de Seguridad (Deben Fallar)

### Customer intenta crear producto
```
POST /api/products
Authorization: Bearer {customer-token}
```
**Esperado**: ? 403 Forbidden

### Customer intenta ver usuarios
```
GET /api/users
Authorization: Bearer {customer-token}
```
**Esperado**: ? 403 Forbidden

### Customer intenta ver pedido de otro
```
GET /api/orders/1
Authorization: Bearer {customer-token}
```
**Esperado**: ? 403 Forbidden (si no es su pedido)

### Sin token intenta crear pedido
```
POST /api/orders
```
**Esperado**: ? 401 Unauthorized

---

## ?? Datos Disponibles Tras Seed

- ? **6 Categorías**: Hombre, Mujer, Niños, Accesorios, Calzado, Deportes
- ? **10 Productos** con precios e imágenes
- ? **4 Usuarios** (1 Admin, 1 Employee, 2 Customers)
- ? **3 Pedidos** de ejemplo
- ? **4 Reseñas** de productos
- ? **3 Direcciones** de envío
- ? **4 Items** en wishlist

---

## ?? Troubleshooting Rápido

### Error: "No se puede conectar a la base de datos"
```bash
# Verificar connection string en appsettings.json
# Asegurarse que SQL Server está corriendo
```

### Error: "Cannot create database"
```bash
# Verificar permisos del usuario
# Intentar crear la base manualmente:
CREATE DATABASE BOSKO;
```

### Error: "Tabla no existe"
```bash
# Aplicar migraciones:
dotnet ef database update
```

### Error: "No hay datos"
```bash
# Ejecutar script de seed:
sqlcmd -S LOCALHOST\SQLEXPRESS -d BOSKO -i SeedData.sql
```

### Error: 403 Forbidden
```
# Verificar que el token sea válido
# Verificar que el usuario tenga el rol necesario
# Verificar que se incluya "Bearer " antes del token
```

---

## ?? Siguientes Pasos

Después de probar localmente:

1. ? **Testing exhaustivo**: Usa `POSTMAN_GUIDE.md`
2. ? **Revisar logs**: Verifica que no haya warnings
3. ? **Validar seguridad**: Prueba casos de acceso no autorizado
4. ? **Preparar producción**: Sigue `DEPLOYMENT_GUIDE.md`

---

## ?? Documentación Completa

- **Uso general**: `README.md`
- **Testing**: `POSTMAN_GUIDE.md`
- **Despliegue**: `DEPLOYMENT_GUIDE.md`
- **Resumen ejecutivo**: `EXECUTIVE_SUMMARY.md`
- **Checklist**: `IMPLEMENTATION_CHECKLIST.md`

---

## ? ¡Listo!

Tu backend está funcionando. Ahora puedes:
- ? Probar todos los endpoints
- ? Integrar con el frontend
- ? Desplegar a producción

**¿Problemas?** Consulta la documentación o los archivos de troubleshooting.

---

*Proyecto BOSKOBACK v1.0.0*
*ASP.NET Core 8.0*
