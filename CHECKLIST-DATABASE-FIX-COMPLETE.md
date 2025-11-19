# ✅ CHECKLIST - CORRECCIÓN DE BASE DE DATOS

## 📋 Lista de Verificación Completa

Marca cada paso cuando lo completes:

---

## 🔧 FASE 1: PREPARACIÓN (2 minutos)

- [ ] **1.1** Abrir SQL Server Management Studio o Azure Data Studio
- [ ] **1.2** Conectarse al servidor `localhost` (o tu servidor)
- [ ] **1.3** Verificar que la base de datos `BoskoDB` existe
- [ ] **1.4** Abrir archivo `Database/FIX-ORDERS-TABLES-MISSING-COLUMNS.sql`
- [ ] **1.5** Leer el contenido del script (opcional pero recomendado)

---

## ⚙️ FASE 2: EJECUCIÓN DEL SCRIPT (1 minuto)

- [ ] **2.1** Seleccionar base de datos `BoskoDB` en el dropdown o ejecutar `USE BoskoDB`
- [ ] **2.2** Copiar TODO el contenido del archivo SQL
- [ ] **2.3** Pegar en una nueva ventana de Query
- [ ] **2.4** Ejecutar el script completo (F5 o botón Execute)
- [ ] **2.5** Esperar a que termine (toma ~10 segundos)

---

## ✔️ FASE 3: VERIFICACIÓN DE RESULTADOS (1 minuto)

### **En el panel "Messages" debe aparecer:**

- [ ] **3.1** Mensaje: "🔧 INICIANDO CORRECCIÓN DE BASE DE DATOS"
- [ ] **3.2** PASO 1 completado: "✅ Columna OrderNumber agregada"
- [ ] **3.3** PASO 1 completado: "✅ Columna Tax agregada"
- [ ] **3.4** PASO 1 completado: "✅ Columna TrackingNumber agregada"
- [ ] **3.5** PASO 1 completado: "✅ Columna Notes agregada"
- [ ] **3.6** PASO 2 completado: "✅ Columna ProductImage agregada"
- [ ] **3.7** PASO 3 completado: "✅ Tabla ShippingAddresses creada exitosamente"
- [ ] **3.8** PASO 4 completado: "✅ Índice único IX_Orders_OrderNumber_Unique creado"
- [ ] **3.9** PASO 5 completado: "✅ Restricción FK_OrderItems_ProductId actualizada"
- [ ] **3.10** PASO 6 completado: Ver cantidad de usuarios y productos
- [ ] **3.11** PASO 7 completado: "✅ Tabla Orders tiene todas las columnas requeridas"
- [ ] **3.12** PASO 7 completado: "✅ Tabla OrderItems tiene todas las columnas requeridas"
- [ ] **3.13** PASO 7 completado: "✅ Tabla ShippingAddresses existe"
- [ ] **3.14** Mensaje final: "✅ CORRECCIÓN COMPLETADA EXITOSAMENTE"

---

## 🔍 FASE 4: VERIFICACIÓN MANUAL (2 minutos)

### **Ejecutar estos comandos SQL:**

```sql
-- Verificar columnas en Orders
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Orders'
ORDER BY ORDINAL_POSITION;
```

- [ ] **4.1** Aparece columna `OrderNumber`
- [ ] **4.2** Aparece columna `Tax`
- [ ] **4.3** Aparece columna `TrackingNumber`
- [ ] **4.4** Aparece columna `Notes`

```sql
-- Verificar columnas en OrderItems
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'OrderItems'
ORDER BY ORDINAL_POSITION;
```

- [ ] **4.5** Aparece columna `ProductImage`

```sql
-- Verificar tabla ShippingAddresses
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'ShippingAddresses';
```

- [ ] **4.6** Retorna 1 fila (la tabla existe)

```sql
-- Verificar Foreign Keys
SELECT 
    fk.name AS FK_Name,
    tp.name AS Parent_Table,
    tr.name AS Referenced_Table
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
WHERE tp.name = 'ShippingAddresses';
```

- [ ] **4.7** Aparece FK: `FK_ShippingAddresses_Orders`

---

## 🚀 FASE 5: REINICIAR BACKEND (30 segundos)

- [ ] **5.1** Detener el backend (Ctrl+C en terminal o Shift+F5 en VS)
- [ ] **5.2** Reiniciar el backend (`dotnet run` o F5 en VS)
- [ ] **5.3** Esperar mensaje: "✅ API LISTA - Esperando requests..."
- [ ] **5.4** Verificar que no hay errores de Entity Framework en los logs

---

## 🧪 FASE 6: TESTING EN SWAGGER (3 minutos)

### **6.1 Verificar Swagger UI:**

- [ ] **6.1.1** Abrir navegador: `https://localhost:5006/swagger`
- [ ] **6.1.2** Swagger carga sin errores
- [ ] **6.1.3** Buscar endpoint: `POST /api/orders`
- [ ] **6.1.4** Endpoint aparece en la lista

### **6.2 Obtener Token JWT:**

- [ ] **6.2.1** Buscar endpoint: `POST /api/auth/login`
- [ ] **6.2.2** Click en "Try it out"
- [ ] **6.2.3** Pegar JSON:
```json
{
  "email": "admin@bosko.com",
  "password": "Bosko123!"
}
```
- [ ] **6.2.4** Click en "Execute"
- [ ] **6.2.5** Response 200 OK
- [ ] **6.2.6** Copiar el `token` de la respuesta

### **6.3 Autorizar en Swagger:**

- [ ] **6.3.1** Click en botón "Authorize" (arriba a la derecha)
- [ ] **6.3.2** Pegar: `Bearer {tu-token-aqui}`
- [ ] **6.3.3** Click en "Authorize"
- [ ] **6.3.4** Click en "Close"

### **6.4 Probar POST /api/orders:**

- [ ] **6.4.1** Buscar endpoint: `POST /api/orders`
- [ ] **6.4.2** Click en "Try it out"
- [ ] **6.4.3** Pegar este JSON:

```json
{
  "customerId": 1,
  "items": [
    {
      "productId": 1,
      "productName": "Producto Test",
      "productImage": "https://via.placeholder.com/150",
      "quantity": 1,
      "unitPrice": 99.99
    }
  ],
  "shippingAddress": {
    "fullName": "Test User",
    "phone": "555-1234",
    "street": "Test Street 123",
    "city": "Test City",
    "state": "Test State",
    "postalCode": "12345",
    "country": "México"
  },
  "paymentMethod": "credit_card",
  "notes": "Test order from Swagger"
}
```

- [ ] **6.4.4** Click en "Execute"
- [ ] **6.4.5** Response code es **201 Created** (NO 400)
- [ ] **6.4.6** Response body contiene:
  - `"success": true`
  - `"message": "Pedido creado exitosamente"`
  - `"data"` con el pedido completo
  - `"orderNumber"` con formato `ORD-...`

---

## 🌐 FASE 7: TESTING EN ANGULAR (2 minutos)

- [ ] **7.1** Abrir la aplicación Angular en navegador
- [ ] **7.2** Hacer login con usuario válido
- [ ] **7.3** Agregar productos al carrito
- [ ] **7.4** Ir al carrito
- [ ] **7.5** Click en "Finalizar Compra" o "Checkout"
- [ ] **7.6** Llenar formulario de dirección de envío
- [ ] **7.7** Click en "Confirmar Pedido"
- [ ] **7.8** Pedido se crea SIN error 400
- [ ] **7.9** Aparece mensaje de éxito
- [ ] **7.10** Redirige a página de confirmación o "Mis Pedidos"
- [ ] **7.11** El pedido aparece en la lista

---

## 📊 FASE 8: VERIFICACIÓN DE DATOS (1 minuto)

### **Ejecutar en SQL Server:**

```sql
USE BoskoDB;

-- Ver último pedido creado
SELECT TOP 1 * FROM Orders ORDER BY CreatedAt DESC;
```

- [ ] **8.1** Aparece el pedido de prueba
- [ ] **8.2** Campo `OrderNumber` tiene valor (ej: "ORD-20251119...")
- [ ] **8.3** Campo `Tax` tiene valor (16% del subtotal)
- [ ] **8.4** Campo `Notes` tiene el texto ingresado

```sql
-- Ver items del último pedido
SELECT oi.* FROM OrderItems oi
INNER JOIN Orders o ON oi.OrderId = o.Id
ORDER BY o.CreatedAt DESC;
```

- [ ] **8.5** Aparecen los items del pedido
- [ ] **8.6** Campo `ProductImage` tiene valor (URL de la imagen)

```sql
-- Ver dirección del último pedido
SELECT sa.* FROM ShippingAddresses sa
INNER JOIN Orders o ON sa.OrderId = o.Id
ORDER BY o.CreatedAt DESC;
```

- [ ] **8.7** Aparece la dirección de envío
- [ ] **8.8** Todos los campos tienen datos correctos

---

## 🎯 FASE 9: VERIFICACIÓN DE FUNCIONALIDADES (3 minutos)

### **9.1 Crear múltiples pedidos:**

- [ ] **9.1.1** Crear 2-3 pedidos de prueba
- [ ] **9.1.2** Cada pedido tiene `OrderNumber` único
- [ ] **9.1.3** Todos los pedidos se guardan correctamente

### **9.2 Verificar endpoints GET:**

```
GET /api/orders/my-orders
```
- [ ] **9.2.1** Retorna lista de pedidos del usuario autenticado
- [ ] **9.2.2** Response 200 OK

```
GET /api/orders/{id}
```
- [ ] **9.2.3** Retorna detalles completos del pedido
- [ ] **9.2.4** Incluye `items` array
- [ ] **9.2.5** Incluye `shippingAddress` object
- [ ] **9.2.6** Response 200 OK

### **9.3 Verificar endpoint UPDATE:**

```
PUT /api/orders/{id}
```
- [ ] **9.3.1** Actualizar dirección de envío de un pedido pendiente
- [ ] **9.3.2** Response 200 OK
- [ ] **9.3.3** Dirección se actualiza correctamente

### **9.4 Verificar endpoint CANCEL:**

```
POST /api/orders/{id}/cancel
```
- [ ] **9.4.1** Cancelar un pedido pendiente
- [ ] **9.4.2** Response 200 OK
- [ ] **9.4.3** Estado cambia a "cancelled"
- [ ] **9.4.4** Stock de productos se restaura

### **9.5 Verificar Admin endpoints:**

```
GET /api/orders (Admin/Employee)
```
- [ ] **9.5.1** Admin puede ver todos los pedidos
- [ ] **9.5.2** Paginación funciona correctamente
- [ ] **9.5.3** Filtros funcionan (status, search, dates)

```
PUT /api/orders/{id}/status (Admin/Employee)
```
- [ ] **9.5.4** Admin puede cambiar estado de pedido
- [ ] **9.5.5** Response 200 OK
- [ ] **9.5.6** Historial de estados se registra

```
GET /api/orders/stats (Admin/Employee)
```
- [ ] **9.5.7** Retorna estadísticas de pedidos
- [ ] **9.5.8** Cálculos son correctos

---

## 📈 FASE 10: VERIFICACIÓN DE PERFORMANCE (1 minuto)

- [ ] **10.1** Crear un pedido toma menos de 2 segundos
- [ ] **10.2** Listar pedidos toma menos de 1 segundo
- [ ] **10.3** No hay errores en logs del backend
- [ ] **10.4** No hay warnings de Entity Framework
- [ ] **10.5** Frontend responde rápidamente

---

## 📝 FASE 11: DOCUMENTACIÓN (30 segundos)

- [ ] **11.1** Revisar archivo `ORDERS-SYSTEM-COMPLETE-SUMMARY.md`
- [ ] **11.2** Guardar este checklist completado
- [ ] **11.3** Anotar cualquier problema encontrado

---

## ✅ RESUMEN FINAL

### **Total de checks:** 105
### **Tiempo estimado:** 15-20 minutos

### **Status por Fase:**

| Fase | Descripción | Status |
|------|-------------|--------|
| 1 | Preparación | ⬜ |
| 2 | Ejecución del Script | ⬜ |
| 3 | Verificación de Resultados | ⬜ |
| 4 | Verificación Manual | ⬜ |
| 5 | Reiniciar Backend | ⬜ |
| 6 | Testing en Swagger | ⬜ |
| 7 | Testing en Angular | ⬜ |
| 8 | Verificación de Datos | ⬜ |
| 9 | Verificación de Funcionalidades | ⬜ |
| 10 | Verificación de Performance | ⬜ |
| 11 | Documentación | ⬜ |

---

## 🚨 SI ALGO FALLA

### **Durante la ejecución del script:**

1. Copiar el mensaje de error completo
2. Identificar en qué PASO falló (1-7)
3. Revisar archivo `CRITICAL-DATABASE-FIX-REQUIRED.md`
4. Buscar el error en la sección "SI HAY ERRORES"
5. Ejecutar la solución sugerida

### **Durante testing:**

1. Verificar que el backend está corriendo
2. Verificar logs en Visual Studio → Output → Debug
3. Verificar que customerId y productId existen en BD
4. Revisar archivo `ORDERS-TROUBLESHOOTING-GUIDE.md`

---

## 🎉 ÉXITO

### **Cuando TODOS los checks estén ✅:**

```
┌────────────────────────────────────────────┐
│                                            │
│   ✅ SISTEMA DE PEDIDOS COMPLETAMENTE      │
│      FUNCIONAL Y VERIFICADO                │
│                                            │
│   - Base de datos actualizada              │
│   - Backend funcionando                    │
│   - Todos los endpoints probados           │
│   - Angular integrado                      │
│   - Performance óptimo                     │
│                                            │
│   🎯 READY FOR PRODUCTION                  │
│                                            │
└────────────────────────────────────────────┘
```

---

**Fecha de completación:** _______________  
**Completado por:** _______________  
**Tiempo total:** _______________  
**Problemas encontrados:** _______________

---

**Última Actualización:** 19 de Noviembre 2025  
**Versión:** 1.0  
**Status:** ✅ Checklist Completo
