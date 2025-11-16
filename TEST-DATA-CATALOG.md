# 📊 CATÁLOGO COMPLETO DE DATOS DE PRUEBA - BOSKO E-COMMERCE

**Fecha:** 16 de Noviembre 2025  
**Versión:** 1.0  
**Propósito:** Documentación completa de todos los datos de prueba del sistema

---

## 🎯 RESUMEN EJECUTIVO

Este documento detalla **TODOS los datos de prueba** disponibles en el sistema Bosko E-Commerce después de ejecutar el script `Complete-Test-Data.sql`. Incluye usuarios, productos, pedidos, y toda la información necesaria para testing y demos.

---

## 👥 USUARIOS DE PRUEBA

### **1. Administrador**
```
Nombre: Admin Bosko (o Santiago - tu usuario actual)
Email: santiago.c0399@gmail.com (o admin@bosko.com)
Password: [Tu password actual]
Rol: Admin
Permisos:
  ✅ Dashboard completo
  ✅ Gestión de pedidos
  ✅ Gestión de productos
  ✅ Gestión de categorías
  ✅ Gestión de usuarios
  ✅ Ver estadísticas
  ✅ Cambiar roles
```

### **2. Empleado** (Si existe)
```
Nombre: Empleado Test
Email: employee@bosko.com
Password: Bosko123! (si ejecutaste init-users)
Rol: Employee
Permisos:
  ✅ Dashboard (solo lectura)
  ✅ Ver pedidos
  ✅ Actualizar estado de pedidos
  ✅ Ver productos (lectura)
  ❌ Crear/editar productos
  ❌ Gestión de usuarios
```

### **3. Cliente** (Para pruebas)
```
Nombre: Cliente Test
Email: customer@bosko.com
Password: Bosko123! (si ejecutaste init-users)
Rol: Customer
Permisos:
  ✅ Ver productos
  ✅ Crear pedidos (frontend)
  ❌ Acceso al admin panel
```

---

## 🏷️ CATEGORÍAS (5 Total)

### **ID 1: Camisas**
```
Nombre: Camisas
Descripción: Camisas casuales y formales para hombre
Productos: 4 productos
Estado: Activa
```

### **ID 2: Pantalones**
```
Nombre: Pantalones
Descripción: Pantalones de todo tipo
Productos: 4 productos
Estado: Activa
```

### **ID 3: Chaquetas**
```
Nombre: Chaquetas
Descripción: Chaquetas, blazers y abrigos
Productos: 4 productos
Estado: Activa
```

### **ID 4: Calzado**
```
Nombre: Calzado
Descripción: Zapatos, zapatillas y botas
Productos: 4 productos
Estado: Activa
```

### **ID 5: Accesorios**
```
Nombre: Accesorios
Descripción: Complementos y accesorios
Productos: 4 productos
Estado: Activa
```

---

## 👕 PRODUCTOS (20 Total - 4 por Categoría)

### **CATEGORÍA: CAMISAS**

#### **Producto #1: Camisa Casual Bosko**
```
Precio: €49.99
Stock: 150 unidades
Descripción: Camisa de algodón premium con corte moderno. Perfecta para uso diario.
Categoría: Camisas
Imagen: https://images.unsplash.com/photo-1596755094514-f87e34085b2c
Estado: Disponible
```

#### **Producto #2: Camisa Formal Blanca**
```
Precio: €59.99
Stock: 120 unidades
Descripción: Camisa formal de algodón egipcio. Ideal para eventos especiales.
Categoría: Camisas
Imagen: https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf
Estado: Disponible
```

#### **Producto #3: Polo Bosko Premium**
```
Precio: €39.99
Stock: 200 unidades
Descripción: Polo de algodón pima con logo bordado. Estilo deportivo elegante.
Categoría: Camisas
Imagen: https://images.unsplash.com/photo-1586790170083-2f9ceadc732d
Estado: Disponible
```

#### **Producto #4: Camisa de Lino Verano**
```
Precio: €54.99
Stock: 90 unidades
Descripción: Camisa ligera de lino natural. Perfecta para el verano.
Categoría: Camisas
Imagen: https://images.unsplash.com/photo-1620012253295-c15cc3e65df4
Estado: Disponible
```

---

### **CATEGORÍA: PANTALONES**

#### **Producto #5: Pantalón Slim Fit Negro**
```
Precio: €69.99
Stock: 130 unidades
Descripción: Pantalón entallado de corte moderno. Tela elástica y cómoda.
Categoría: Pantalones
Imagen: https://images.unsplash.com/photo-1473966968600-fa801b869a1a
Estado: Disponible
```

#### **Producto #6: Jeans Clásicos Bosko**
```
Precio: €79.99
Stock: 110 unidades
Descripción: Jeans de mezclilla premium con lavado especial. Durabilidad garantizada.
Categoría: Pantalones
Imagen: https://images.unsplash.com/photo-1542272604-787c3835535d
Estado: Disponible
```

#### **Producto #7: Chino Beige Elegante**
```
Precio: €64.99
Stock: 95 unidades
Descripción: Pantalón chino de algodón. Versátil para cualquier ocasión.
Categoría: Pantalones
Imagen: https://images.unsplash.com/photo-1624378439575-d8705ad7ae80
Estado: Disponible
```

#### **Producto #8: Jogger Deportivo**
```
Precio: €49.99
Stock: 160 unidades
Descripción: Pantalón jogger cómodo para actividades deportivas o casual.
Categoría: Pantalones
Imagen: https://images.unsplash.com/photo-1611312449408-fcece27cdbb7
Estado: Disponible
```

---

### **CATEGORÍA: CHAQUETAS**

#### **Producto #9: Chaqueta de Cuero Premium**
```
Precio: €189.99
Stock: 45 unidades
Descripción: Chaqueta de cuero genuino con forro interno. Estilo atemporal.
Categoría: Chaquetas
Imagen: https://images.unsplash.com/photo-1551028719-00167b16eac5
Estado: Disponible
```

#### **Producto #10: Blazer Formal Azul**
```
Precio: €149.99
Stock: 60 unidades
Descripción: Blazer de corte italiano. Elegancia para eventos formales.
Categoría: Chaquetas
Imagen: https://images.unsplash.com/photo-1507679799987-c73779587ccf
Estado: Disponible
```

#### **Producto #11: Bomber Jacket Moderna**
```
Precio: €99.99
Stock: 85 unidades
Descripción: Chaqueta bomber de nylon resistente. Estilo urbano moderno.
Categoría: Chaquetas
Imagen: https://images.unsplash.com/photo-1591047139829-d91aecb6caea
Estado: Disponible
```

#### **Producto #12: Abrigo de Invierno**
```
Precio: €169.99
Stock: 50 unidades
Descripción: Abrigo largo con aislamiento térmico. Perfecto para el frío.
Categoría: Chaquetas
Imagen: https://images.unsplash.com/photo-1539533018447-63fcce2678e3
Estado: Disponible
```

---

### **CATEGORÍA: CALZADO**

#### **Producto #13: Zapatillas Deportivas Bosko**
```
Precio: €89.99
Stock: 200 unidades
Descripción: Zapatillas de alto rendimiento con tecnología de amortiguación.
Categoría: Calzado
Imagen: https://images.unsplash.com/photo-1542291026-7eec264c27ff
Estado: Disponible
⭐ Producto más vendido
```

#### **Producto #14: Zapatos Formales Oxford**
```
Precio: €119.99
Stock: 75 unidades
Descripción: Zapatos de cuero para ocasiones formales. Fabricación artesanal.
Categoría: Calzado
Imagen: https://images.unsplash.com/photo-1614252235316-8c857d38b5f4
Estado: Disponible
```

#### **Producto #15: Botas de Cuero**
```
Precio: €139.99
Stock: 65 unidades
Descripción: Botas robustas de cuero genuino. Estilo casual elegante.
Categoría: Calzado
Imagen: https://images.unsplash.com/photo-1608256246200-53e635b5b65f
Estado: Disponible
```

#### **Producto #16: Sandalias de Verano**
```
Precio: €34.99
Stock: 180 unidades
Descripción: Sandalias cómodas para el verano. Material transpirable.
Categoría: Calzado
Imagen: https://images.unsplash.com/photo-1603487742131-4160ec999306
Estado: Disponible
```

---

### **CATEGORÍA: ACCESORIOS**

#### **Producto #17: Cinturón de Cuero Negro**
```
Precio: €29.99
Stock: 220 unidades
Descripción: Cinturón de cuero genuino con hebilla metálica. Elegancia clásica.
Categoría: Accesorios
Imagen: https://images.unsplash.com/photo-1624222247344-550fb60583f0
Estado: Disponible
```

#### **Producto #18: Cartera de Piel Bosko**
```
Precio: €44.99
Stock: 150 unidades
Descripción: Cartera compacta de piel con múltiples compartimentos.
Categoría: Accesorios
Imagen: https://images.unsplash.com/photo-1627123424574-724758594e93
Estado: Disponible
```

#### **Producto #19: Gafas de Sol Polarizadas**
```
Precio: €79.99
Stock: 100 unidades
Descripción: Gafas de sol con protección UV400 y lentes polarizados.
Categoría: Accesorios
Imagen: https://images.unsplash.com/photo-1572635196237-14b3f281503f
Estado: Disponible
```

#### **Producto #20: Gorra Bosko Signature**
```
Precio: €24.99
Stock: 250 unidades
Descripción: Gorra ajustable de algodón con logo bordado.
Categoría: Accesorios
Imagen: https://images.unsplash.com/photo-1588850561407-ed78c282e89b
Estado: Disponible
```

---

## 🛒 PEDIDOS DE EJEMPLO (5 Total)

### **Pedido #1: Entregado ✅**
```
Estado: delivered
Cliente: Cliente Test
Email: customer@bosko.com
Fecha: Hace 5 días
Dirección: Calle Mayor 123, Madrid, 28001, España
Método de Pago: Tarjeta de crédito

Items:
  • 2x Camisa Casual Bosko (€49.99 c/u) = €99.98
  • 1x Pantalón Slim Fit Negro (€69.99) = €69.99
  • 1x Zapatillas Deportivas Bosko (€89.99) = €89.99

Subtotal: €269.97
Envío: €15.00
TOTAL: €284.97

Historial:
  ⏱️ Hace 5 días: Pedido recibido
  📦 Hace 4 días: En preparación en almacén
  🚚 Hace 3 días: Enviado a transportista
  ✅ Hace 1 día: Entregado con éxito
```

### **Pedido #2: En Proceso 📦**
```
Estado: processing
Cliente: Cliente Test
Email: customer@bosko.com
Fecha: Hace 2 días
Dirección: Avenida Libertad 45, Barcelona, 08001, España
Método de Pago: PayPal

Items:
  • 1x Blazer Formal Azul (€149.99) = €149.99

Subtotal: €149.99
Envío: €12.00
TOTAL: €161.99

Historial:
  ⏱️ Hace 2 días: Pedido recibido
  📦 Hace 6 horas: Verificando stock
```

### **Pedido #3: Pendiente ⏳**
```
Estado: pending
Cliente: Cliente Test
Email: customer@bosko.com
Fecha: Hace 3 horas
Dirección: Plaza España 12, Valencia, 46001, España
Método de Pago: Tarjeta de crédito

Items:
  • 1x Jeans Clásicos Bosko (€79.99) = €79.99
  • 1x Bomber Jacket Moderna (€99.99) = €99.99
  • 1x Cinturón de Cuero Negro (€29.99) = €29.99

Subtotal: €214.96
Envío: €15.00
TOTAL: €229.96

Historial:
  ⏱️ Hace 3 horas: Pedido recibido, pendiente de procesamiento
```

### **Pedido #4: Entregado ✅**
```
Estado: delivered
Cliente: Cliente Test
Email: customer@bosko.com
Fecha: Hace 10 días
Dirección: Calle Mayor 123, Madrid, 28001, España
Método de Pago: Tarjeta de crédito

Items:
  • 1x Chaqueta de Cuero Premium (€189.99) = €189.99

Subtotal: €189.99
Envío: €15.00
TOTAL: €204.99

Historial:
  ⏱️ Hace 10 días: Pedido recibido
  📦 Hace 9 días: En preparación
  ✅ Hace 7 días: Entregado
```

### **Pedido #5: Cancelado ❌**
```
Estado: cancelled
Cliente: Cliente Test
Email: customer@bosko.com
Fecha: Hace 8 días
Dirección: Calle Mayor 123, Madrid, 28001, España
Método de Pago: Tarjeta de crédito

Items:
  • 1x Zapatos Formales Oxford (€119.99) = €119.99

Subtotal: €119.99
Envío: €12.00
TOTAL: €131.99

Historial:
  ⏱️ Hace 8 días: Pedido recibido
  ❌ Hace 7 días: Cancelado por el cliente
```

---

## 📊 ESTADÍSTICAS ESPERADAS

### **Dashboard Stats**
```json
{
  "sales": {
    "total": 1012.91,  // Suma de pedidos no cancelados
    "trend": 12.5
  },
  "orders": {
    "total": 5,
    "pending": 1,
    "processing": 1,
    "delivered": 2,
    "cancelled": 1
  },
  "customers": {
    "total": 3-5,  // Dependiendo de usuarios creados
    "active": 3-5
  },
  "products": {
    "total": 20,
    "inStock": 20,
    "outOfStock": 0
  }
}
```

### **Top 5 Productos Más Vendidos**
```
1. Camisa Casual Bosko - 2 ventas (€99.98)
2. Zapatillas Deportivas Bosko - 1 venta (€89.99)
3. Blazer Formal Azul - 1 venta (€149.99)
4. Jeans Clásicos Bosko - 1 venta (€79.99)
5. Chaqueta de Cuero Premium - 1 venta (€189.99)
```

---

## 📝 ACTIVIDAD DEL SISTEMA (10 Registros)

```
1. [Hace 3 horas] 📦 order: Nuevo pedido #3 recibido
2. [Hace 6 horas] 📦 order: Pedido #2 actualizado a procesando
3. [Hace 12 horas] 👕 product: Producto "Camisa Casual Bosko" actualizado
4. [Hace 18 horas] 👤 user: Nuevo cliente registrado en el sistema
5. [Hace 1 día] 👕 product: Nueva colección de otoño agregada
6. [Hace 1 día] 📦 order: Pedido #1 marcado como entregado
7. [Hace 2 días] 🏷️ category: Categoría "Accesorios" actualizada
8. [Hace 3 días] 👕 product: Producto "Zapatillas Deportivas Bosko" en oferta
9. [Hace 7 días] 📦 order: Pedido #5 cancelado por solicitud del cliente
10. [Hace 8 días] 👕 product: Stock repuesto para productos más vendidos
```

---

## 🔔 NOTIFICACIONES PARA ADMIN (5 Total)

```
1. [Hace 3 horas] 🔴 NO LEÍDA
   Título: Nuevo pedido recibido
   Mensaje: Pedido #3 de Cliente Test
   Tipo: order

2. [Hace 6 horas] 🔴 NO LEÍDA
   Título: Pedido listo para envío
   Mensaje: Pedido #2 empaquetado y listo
   Tipo: order

3. [Hace 24 horas] 🔴 NO LEÍDA
   Título: Stock bajo
   Mensaje: El producto "Sandalias de Verano" tiene stock bajo
   Tipo: product

4. [Hace 2 días] ✅ LEÍDA
   Título: Nuevo cliente registrado
   Mensaje: Se registró un nuevo cliente en la plataforma
   Tipo: user

5. [Hace 3 días] ✅ LEÍDA
   Título: Producto más vendido
   Mensaje: Las "Zapatillas Deportivas Bosko" son el producto más vendido esta semana
   Tipo: product
```

---

## 🧪 CASOS DE PRUEBA SUGERIDOS

### **1. Testing de Dashboard**
```
GET /api/admin/dashboard/stats
Verificar:
  ✅ Total de ventas es €1,012.91
  ✅ 5 pedidos en total
  ✅ 1 pedido pendiente
  ✅ 20 productos en stock
```

### **2. Testing de Productos**
```
GET /api/products
Verificar:
  ✅ Se retornan 20 productos
  ✅ Cada producto tiene categoría asignada
  ✅ Todos tienen precio > 0
  ✅ Todos tienen stock > 0
```

### **3. Testing de Pedidos**
```
GET /api/admin/orders/recent
Verificar:
  ✅ Se retornan los 5 pedidos más recientes
  ✅ Ordenados por fecha descendente
  ✅ Pedido #3 aparece primero (más reciente)
```

### **4. Testing de Top Products**
```
GET /api/admin/products/top-sellers?limit=5
Verificar:
  ✅ "Camisa Casual Bosko" aparece primero (2 ventas)
  ✅ Ordenados por cantidad de ventas descendente
  ✅ Incluye información de revenue
```

### **5. Testing de Filtros**
```
GET /api/products?categoryId=1
Verificar:
  ✅ Se retornan solo 4 productos (Camisas)
  ✅ Todos pertenecen a categoría "Camisas"

GET /api/admin/orders?status=pending
Verificar:
  ✅ Se retorna solo 1 pedido (Pedido #3)
  ✅ Estado es "pending"
```

---

## 📦 SCRIPTS DE EJECUCIÓN

### **Orden Recomendado de Ejecución:**

```sql
-- 1. Setup inicial de base de datos
Database/BoskoDB-Setup.sql

-- 2. Setup de autenticación
Database/Users-Authentication-Setup.sql

-- 3. Inicializar passwords (o usar endpoint)
Ejecutar: POST /api/auth/init-users

-- 4. Setup del Admin Panel (tablas de Orders, etc)
Database/Admin-Panel-Setup.sql

-- 5. Datos de prueba completos
Database/Complete-Test-Data.sql
```

### **Verificación Post-Instalación:**

```sql
-- Verificar todas las tablas
SELECT name FROM sys.tables 
ORDER BY name;

-- Debe mostrar:
-- ActivityLogs
-- Categories
-- Notifications
-- OrderItems
-- Orders
-- OrderStatusHistory
-- PasswordResetTokens
-- Products
-- Productos (deprecated)
-- Users

-- Contar registros
SELECT 
    'Users' AS Tabla, COUNT(*) AS Registros FROM Users
UNION ALL SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL SELECT 'Products', COUNT(*) FROM Products
UNION ALL SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL SELECT 'OrderItems', COUNT(*) FROM OrderItems
UNION ALL SELECT 'ActivityLogs', COUNT(*) FROM ActivityLogs
UNION ALL SELECT 'Notifications', COUNT(*) FROM Notifications;
```

---

## 🎯 ENDPOINTS DE TESTING

### **Dashboard y Estadísticas**
```
GET /api/admin/dashboard/stats
GET /api/admin/dashboard/sales-chart?months=6
GET /api/admin/dashboard/orders-status
GET /api/admin/orders/recent?limit=5
GET /api/admin/products/top-sellers?limit=5&period=month
GET /api/admin/activity/recent?limit=10
```

### **Productos**
```
GET /api/products
GET /api/products/{id}
GET /api/products?categoryId=1
POST /api/products (Admin only)
PUT /api/products/{id} (Admin only)
DELETE /api/products/{id} (Admin only)
```

### **Categorías**
```
GET /api/categories
GET /api/categories/{id}
POST /api/categories (Admin only)
PUT /api/categories/{id} (Admin only)
```

### **Pedidos**
```
GET /api/admin/orders?page=1&limit=20
GET /api/admin/orders/{id}
GET /api/admin/orders?status=pending
PUT /api/admin/orders/{id}/status
```

---

## 🎨 PERSONALIZACIÓN DE DATOS

### **Para Agregar Más Productos:**

```sql
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Image)
VALUES 
    ('Nuevo Producto', 'Descripción', 99.99, 100, 1, 'https://url-imagen.com');
```

### **Para Agregar Más Pedidos:**

```sql
-- Obtener IDs necesarios
DECLARE @CustomerId INT = (SELECT TOP 1 Id FROM Users WHERE Role = 'Customer');
DECLARE @ProductId INT = 1; -- ID del producto

-- Crear pedido
INSERT INTO Orders (CustomerId, CustomerName, CustomerEmail, ShippingAddress, 
                   Subtotal, Shipping, Total, Status, PaymentMethod)
VALUES (@CustomerId, 'Nombre Cliente', 'email@test.com', 'Dirección completa',
        100.00, 15.00, 115.00, 'pending', 'credit_card');

DECLARE @OrderId INT = SCOPE_IDENTITY();

-- Agregar items
INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, Price, Subtotal)
VALUES (@OrderId, @ProductId, 'Nombre Producto', 1, 100.00, 100.00);
```

---

## ✅ CHECKLIST DE VERIFICACIÓN

```
DATOS BÁSICOS:
□ 5 Categorías insertadas
□ 20 Productos insertados (4 por categoría)
□ Todos los productos tienen imagen
□ Todos los productos tienen stock > 0

PEDIDOS:
□ 5 Pedidos insertados
□ Estados variados (pending, processing, delivered, cancelled)
□ Cada pedido tiene items
□ Cada pedido tiene historial de estado

ADMIN PANEL:
□ 10 Actividades del sistema registradas
□ 5 Notificaciones para Admin
□ 3 notificaciones no leídas

FUNCIONALIDAD:
□ Dashboard muestra estadísticas correctas
□ Top products muestra ventas
□ Recent orders muestra pedidos ordenados
□ Filtros funcionan correctamente
```

---

## 🎉 CONCLUSIÓN

Con estos datos de prueba, el sistema Bosko E-Commerce está completamente poblado y listo para:

✅ **Demos y presentaciones**  
✅ **Testing de funcionalidades**  
✅ **Desarrollo de frontend**  
✅ **Validación de APIs**  
✅ **Training de usuarios**  

**¡Todos los datos son realistas y coherentes entre sí!**

---

**Última actualización:** 16 de Noviembre 2025  
**Mantenido por:** Backend Team  
**Versión de datos:** 1.0

---

## 📞 SOPORTE

**¿Necesitas más datos de prueba?**
- Modifica el script `Complete-Test-Data.sql`
- Ejecuta nuevamente
- O usa los endpoints de creación (POST)

**¿Los datos no se ven en el frontend?**
- Verifica que el backend esté corriendo
- Haz login como Admin
- Revisa la consola del navegador para errores
- Verifica CORS en el backend
