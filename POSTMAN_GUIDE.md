# ?? POSTMAN - Guía de Pruebas

Colección de ejemplos de requests para probar todos los endpoints de BOSKOBACK.

## ?? Variables de Entorno

Crea estas variables en Postman:

```
baseUrl: https://localhost:7xxx (ajusta el puerto)
adminToken: (se llenará tras login)
customerToken: (se llenará tras login)
employeeToken: (se llenará tras login)
```

---

## ?? 1. AUTENTICACIÓN

### 1.1 Registro de Usuario
```http
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
  "name": "Nuevo Cliente",
  "email": "nuevocliente@example.com",
  "password": "Password123"
}
```

**Test Script:**
```javascript
pm.test("Status 200", () => pm.response.to.have.status(200));
pm.test("Returns token", () => {
    const json = pm.response.json();
    pm.expect(json.token).to.exist;
    pm.environment.set("customerToken", json.token);
});
```

### 1.2 Login - Admin
```http
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "admin@bosko.com",
  "password": "Admin123"
}
```

**Test Script:**
```javascript
pm.test("Login successful", () => {
    const json = pm.response.json();
    pm.expect(json.token).to.exist;
    pm.expect(json.user.role).to.equal("Admin");
    pm.environment.set("adminToken", json.token);
});
```

### 1.3 Login - Customer
```http
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "customer@bosko.com",
  "password": "Password123"
}
```

**Test Script:**
```javascript
pm.environment.set("customerToken", pm.response.json().token);
```

### 1.4 Login - Employee
```http
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "employee@bosko.com",
  "password": "Password123"
}
```

**Test Script:**
```javascript
pm.environment.set("employeeToken", pm.response.json().token);
```

### 1.5 Forgot Password
```http
POST {{baseUrl}}/api/auth/forgot-password
Content-Type: application/json

{
  "email": "customer@bosko.com"
}
```

### 1.6 Reset Password
```http
POST {{baseUrl}}/api/auth/reset-password
Content-Type: application/json

{
  "email": "customer@bosko.com",
  "token": "GUID-FROM-CONSOLE",
  "newPassword": "NewPassword123"
}
```

### 1.7 Change Password
```http
POST {{baseUrl}}/api/auth/change-password
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "oldPassword": "Password123",
  "newPassword": "NewPassword456"
}
```

---

## ?? 2. PRODUCTOS

### 2.1 Listar Todos los Productos (público)
```http
GET {{baseUrl}}/api/products
```

### 2.2 Buscar Productos
```http
GET {{baseUrl}}/api/products?search=zapato
```

### 2.3 Filtrar por Categoría
```http
GET {{baseUrl}}/api/products?categoryId=1
```

### 2.4 Filtrar por Categoría y Búsqueda
```http
GET {{baseUrl}}/api/products?categoryId=2&search=verano
```

### 2.5 Obtener Producto por ID
```http
GET {{baseUrl}}/api/products/1
```

### 2.6 Crear Producto (Admin)
```http
POST {{baseUrl}}/api/products
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Producto Nuevo",
  "description": "Descripción detallada del producto",
  "price": 99.99,
  "imageUrl": "https://images.unsplash.com/photo-123456?w=400",
  "categoryId": 1
}
```

### 2.7 Actualizar Producto (Admin)
```http
PUT {{baseUrl}}/api/products/11
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Producto Actualizado",
  "description": "Nueva descripción",
  "price": 109.99,
  "imageUrl": "https://images.unsplash.com/photo-654321?w=400",
  "categoryId": 1
}
```

### 2.8 Eliminar Producto (Admin)
```http
DELETE {{baseUrl}}/api/products/11
Authorization: Bearer {{adminToken}}
```

### 2.9 Intentar Crear Producto como Customer (debe fallar)
```http
POST {{baseUrl}}/api/products
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "name": "Producto No Autorizado",
  "description": "Esto debe fallar",
  "price": 50.00,
  "imageUrl": "https://example.com/image.jpg",
  "categoryId": 1
}
```

**Expected:** 403 Forbidden

---

## ??? 3. CATEGORÍAS

### 3.1 Listar Categorías (público)
```http
GET {{baseUrl}}/api/categories
```

### 3.2 Obtener Categoría por ID
```http
GET {{baseUrl}}/api/categories/1
```

### 3.3 Crear Categoría (Admin)
```http
POST {{baseUrl}}/api/categories
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Nueva Categoría",
  "description": "Descripción de la categoría",
  "imageUrl": "https://images.unsplash.com/photo-789?w=400"
}
```

### 3.4 Actualizar Categoría (Admin)
```http
PUT {{baseUrl}}/api/categories/7
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Categoría Actualizada",
  "description": "Nueva descripción",
  "imageUrl": "https://images.unsplash.com/photo-999?w=400"
}
```

### 3.5 Eliminar Categoría (Admin)
```http
DELETE {{baseUrl}}/api/categories/7
Authorization: Bearer {{adminToken}}
```

---

## ?? 4. PEDIDOS

### 4.1 Crear Pedido (Customer)
```http
POST {{baseUrl}}/api/orders
Authorization: Bearer {{customerToken}}
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

**Test Script:**
```javascript
pm.test("Order created", () => {
    pm.response.to.have.status(201);
    const json = pm.response.json();
    pm.environment.set("lastOrderId", json.orderId);
});
```

### 4.2 Ver Mis Pedidos (Customer)
```http
GET {{baseUrl}}/api/orders
Authorization: Bearer {{customerToken}}
```

### 4.3 Ver Detalle de Mi Pedido (Customer)
```http
GET {{baseUrl}}/api/orders/{{lastOrderId}}
Authorization: Bearer {{customerToken}}
```

### 4.4 Ver Todos los Pedidos (Admin)
```http
GET {{baseUrl}}/api/orders
Authorization: Bearer {{adminToken}}
```

### 4.5 Filtrar Pedidos por Estado (Admin)
```http
GET {{baseUrl}}/api/orders?status=Pending
Authorization: Bearer {{adminToken}}
```

### 4.6 Ver Todos los Pedidos (Employee)
```http
GET {{baseUrl}}/api/orders
Authorization: Bearer {{employeeToken}}
```

### 4.7 Actualizar Estado de Pedido (Employee)
```http
PUT {{baseUrl}}/api/orders/{{lastOrderId}}
Authorization: Bearer {{employeeToken}}
Content-Type: application/json

{
  "status": "Shipped"
}
```

### 4.8 Intentar Ver Pedido de Otro Usuario (debe fallar)
```http
GET {{baseUrl}}/api/orders/1
Authorization: Bearer {{customerToken}}
```

**Expected:** 403 Forbidden (si el pedido no pertenece al customer)

---

## ?? 5. USUARIOS (Admin)

### 5.1 Listar Usuarios (Admin)
```http
GET {{baseUrl}}/api/users
Authorization: Bearer {{adminToken}}
```

### 5.2 Crear Usuario - Employee (Admin)
```http
POST {{baseUrl}}/api/users
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Nuevo Empleado",
  "email": "nuevoemployee@bosko.com",
  "password": "Password123",
  "roleName": "Employee"
}
```

### 5.3 Crear Usuario - Customer (Admin)
```http
POST {{baseUrl}}/api/users
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Nuevo Cliente Admin",
  "email": "clienteadmin@bosko.com",
  "password": "Password123",
  "roleName": "Customer"
}
```

### 5.4 Actualizar Usuario (Admin)
```http
PUT {{baseUrl}}/api/users/5
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Nombre Actualizado",
  "email": "emailactualizado@bosko.com",
  "roleName": "Employee"
}
```

### 5.5 Eliminar Usuario (Admin)
```http
DELETE {{baseUrl}}/api/users/5
Authorization: Bearer {{adminToken}}
```

### 5.6 Intentar Listar Usuarios como Customer (debe fallar)
```http
GET {{baseUrl}}/api/users
Authorization: Bearer {{customerToken}}
```

**Expected:** 403 Forbidden

---

## ? 6. RESEÑAS

### 6.1 Ver Reseñas de un Producto (público)
```http
GET {{baseUrl}}/api/products/1/reviews
```

### 6.2 Crear Reseña (Customer)
```http
POST {{baseUrl}}/api/products/1/reviews
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "rating": 5,
  "comment": "Excelente producto, muy recomendado. La calidad es superior."
}
```

### 6.3 Actualizar Reseña (Customer - autor)
```http
PUT {{baseUrl}}/api/reviews/1
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "rating": 4,
  "comment": "Comentario actualizado tras más uso."
}
```

### 6.4 Eliminar Reseña (Customer - autor)
```http
DELETE {{baseUrl}}/api/reviews/5
Authorization: Bearer {{customerToken}}
```

### 6.5 Eliminar Reseña (Admin - moderación)
```http
DELETE {{baseUrl}}/api/reviews/5
Authorization: Bearer {{adminToken}}
```

### 6.6 Intentar Reseñar Dos Veces el Mismo Producto (debe fallar)
```http
POST {{baseUrl}}/api/products/1/reviews
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "rating": 5,
  "comment": "Otra reseña del mismo producto"
}
```

**Expected:** 400 Bad Request (Ya has reseñado este producto)

---

## ?? 7. WISHLIST

### 7.1 Ver Mi Wishlist
```http
GET {{baseUrl}}/api/wishlist
Authorization: Bearer {{customerToken}}
```

### 7.2 Agregar a Wishlist
```http
POST {{baseUrl}}/api/wishlist/2
Authorization: Bearer {{customerToken}}
```

### 7.3 Agregar Otro Producto a Wishlist
```http
POST {{baseUrl}}/api/wishlist/4
Authorization: Bearer {{customerToken}}
```

### 7.4 Intentar Agregar Duplicado (debe fallar)
```http
POST {{baseUrl}}/api/wishlist/2
Authorization: Bearer {{customerToken}}
```

**Expected:** 409 Conflict

### 7.5 Eliminar de Wishlist
```http
DELETE {{baseUrl}}/api/wishlist/2
Authorization: Bearer {{customerToken}}
```

---

## ?? 8. DIRECCIONES

### 8.1 Ver Mis Direcciones
```http
GET {{baseUrl}}/api/addresses
Authorization: Bearer {{customerToken}}
```

### 8.2 Crear Dirección
```http
POST {{baseUrl}}/api/addresses
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "street": "Calle Nueva 999",
  "city": "Sevilla",
  "state": "Sevilla",
  "postalCode": "41001",
  "country": "España"
}
```

**Test Script:**
```javascript
pm.test("Address created", () => {
    const json = pm.response.json();
    pm.environment.set("lastAddressId", json.id);
});
```

### 8.3 Actualizar Dirección
```http
PUT {{baseUrl}}/api/addresses/{{lastAddressId}}
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "street": "Calle Actualizada 888",
  "city": "Sevilla",
  "state": "Sevilla",
  "postalCode": "41002",
  "country": "España"
}
```

### 8.4 Eliminar Dirección
```http
DELETE {{baseUrl}}/api/addresses/{{lastAddressId}}
Authorization: Bearer {{customerToken}}
```

---

## ?? 9. FLUJOS DE PRUEBA COMPLETOS

### Flujo 1: Usuario Nuevo
1. Registro ? obtener token
2. Ver productos
3. Agregar a wishlist
4. Crear dirección
5. Crear pedido
6. Ver mis pedidos
7. Crear reseña

### Flujo 2: Admin
1. Login como admin
2. Crear categoría
3. Crear producto en esa categoría
4. Ver todos los pedidos
5. Actualizar estado de un pedido
6. Crear un usuario Employee
7. Ver lista de usuarios

### Flujo 3: Employee
1. Login como employee
2. Ver todos los pedidos pendientes
3. Actualizar estado de pedido
4. Intentar crear producto (debe fallar - 403)
5. Ver detalle de cualquier pedido

### Flujo 4: Autorización
1. Login como customer
2. Intentar crear producto (403)
3. Intentar ver lista de usuarios (403)
4. Intentar ver pedido de otro usuario (403)
5. Ver solo sus propios pedidos (200)

---

## ?? Tests Automáticos Sugeridos

### Collection Pre-request Script
```javascript
// Verificar que variables necesarias existen
if (!pm.environment.get("baseUrl")) {
    console.error("baseUrl not set");
}
```

### Collection Tests
```javascript
// Verificar que no hay errores 500
pm.test("No server errors", () => {
    pm.expect(pm.response.code).to.be.below(500);
});
```

---

## ?? Checklist de Pruebas

- [ ] Todos los endpoints de autenticación funcionan
- [ ] Login retorna token válido con claim de rol
- [ ] Admin puede crear/editar/eliminar productos
- [ ] Customer NO puede crear productos (403)
- [ ] Customer ve solo sus pedidos
- [ ] Admin/Employee ven todos los pedidos
- [ ] Pedidos recalculan precios correctamente
- [ ] Reseñas permiten solo una por usuario/producto
- [ ] Wishlist no permite duplicados
- [ ] Direcciones solo accesibles por propietario
- [ ] Reset password funciona con token válido
- [ ] Tokens expirados son rechazados

---

## ?? Tips para Postman

1. **Usar Variables de Entorno**: Facilita cambiar entre dev/prod
2. **Tests Automáticos**: Valida respuestas automáticamente
3. **Pre-request Scripts**: Prepara datos dinámicos
4. **Collections**: Organiza requests por funcionalidad
5. **Runner**: Ejecuta toda la colección automáticamente
6. **Monitor**: Programa ejecuciones periódicas
