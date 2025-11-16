# 🚀 GUÍA DE INSTALACIÓN COMPLETA - BOSKO E-COMMERCE

**Fecha:** 16 de Noviembre 2025  
**Versión:** 1.0 - Sistema Completo  
**Tiempo estimado:** 15-20 minutos

---

## 📋 ÍNDICE DE PASOS

1. [Prerequisitos](#prerequisitos)
2. [Configuración de Base de Datos](#configuración-de-base-de-datos)
3. [Configuración del Backend](#configuración-del-backend)
4. [Verificación y Testing](#verificación-y-testing)
5. [Troubleshooting](#troubleshooting)

---

## ✅ PREREQUISITOS

Antes de comenzar, asegúrate de tener:

```
□ SQL Server instalado y corriendo
□ SQL Server Management Studio (SSMS)
□ Visual Studio 2022 o VS Code
□ .NET 8 SDK instalado
□ Git (opcional)
```

**Verificar instalaciones:**

```bash
# Verificar .NET 8
dotnet --version
# Debe mostrar: 8.0.x

# Verificar SQL Server
# Abrir SSMS y conectar a localhost
```

---

## 🗄️ CONFIGURACIÓN DE BASE DE DATOS

### **PASO 1: Crear Base de Datos y Tablas Principales**

1. Abre **SQL Server Management Studio (SSMS)**
2. Conéctate a `localhost`
3. Ejecuta los siguientes scripts **EN ORDEN**:

#### **1.1. Setup Inicial**

```sql
-- Archivo: Database/BoskoDB-Setup.sql
-- Este crea la base de datos, Products y Categories
```

**Cómo ejecutar:**
- Abre el archivo en SSMS
- Presiona **F5** o click en **Execute**
- Verifica que veas: "✅ Base de datos BoskoDB creada"

#### **1.2. Setup de Autenticación**

```sql
-- Archivo: Database/Users-Authentication-Setup.sql
-- Este crea la tabla Users y PasswordResetTokens
```

**Cómo ejecutar:**
- Abre el archivo en SSMS
- Presiona **F5** o click en **Execute**
- Verifica que veas: "✅ Tabla Users creada exitosamente"

#### **1.3. Setup del Admin Panel**

```sql
-- Archivo: Database/Admin-Panel-Setup.sql
-- Este crea las tablas Orders, OrderItems, ActivityLogs, Notifications
```

**Cómo ejecutar:**
- Abre el archivo en SSMS
- Presiona **F5** o click en **Execute**
- Verifica que veas: "✅ ADMIN PANEL - INSTALACIÓN COMPLETADA"

### **PASO 2: Verificar Tablas Creadas**

```sql
-- Verificar que todas las tablas existen
SELECT name FROM sys.tables ORDER BY name;

-- Debe mostrar:
-- ActivityLogs
-- Categories
-- Notifications
-- OrderItems
-- Orders
-- OrderStatusHistory
-- PasswordResetTokens
-- Products
-- Productos (deprecated - ignorar)
-- Users
```

### **PASO 3: Insertar Datos de Prueba**

```sql
-- Archivo: Database/Complete-Test-Data.sql
-- Este inserta categorías, productos, pedidos, actividades
```

**Cómo ejecutar:**
- Abre el archivo en SSMS
- Presiona **F5** o click en **Execute**
- Verifica que veas: "✅ DATOS DE PRUEBA INSERTADOS EXITOSAMENTE"

**Verificación rápida:**

```sql
-- Verificar datos insertados
SELECT 'Categories' AS Tabla, COUNT(*) AS Total FROM Categories
UNION ALL SELECT 'Products', COUNT(*) FROM Products
UNION ALL SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL SELECT 'Users', COUNT(*) FROM Users;

-- Debe mostrar:
-- Categories: 5
-- Products: 20
-- Orders: 5
-- Users: 1-3 (dependiendo de usuarios creados)
```

---

## 💻 CONFIGURACIÓN DEL BACKEND

### **PASO 4: Configurar Connection String**

1. Abre el archivo `appsettings.json`
2. Verifica que el connection string sea correcto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BoskoDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Si usas autenticación SQL Server:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BoskoDB;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True;"
  }
}
```

### **PASO 5: Restaurar Paquetes NuGet**

```bash
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK\
dotnet restore
```

### **PASO 6: Compilar el Proyecto**

```bash
dotnet build
```

**Debe mostrar:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### **PASO 7: Inicializar Passwords de Usuarios**

**Opción A: Usar el endpoint (RECOMENDADO)**

```bash
# 1. Ejecutar el proyecto
dotnet run

# 2. Abrir navegador
https://localhost:5006/swagger

# 3. Ejecutar endpoint
POST /api/auth/init-users

# Debe retornar:
{
  "message": "Passwords inicializados para 3 usuarios",
  "password": "Bosko123!"
}
```

**Opción B: Cambiar rol de tu usuario actual**

```sql
-- Si ya tienes un usuario con el que puedes hacer login
UPDATE Users 
SET Role = 'Admin'
WHERE Email = 'santiago.c0399@gmail.com';

-- Verificar
SELECT Name, Email, Role FROM Users WHERE Email = 'santiago.c0399@gmail.com';
```

---

## 🧪 VERIFICACIÓN Y TESTING

### **PASO 8: Probar el Backend**

#### **8.1. Iniciar el Servidor**

```bash
dotnet run
```

**Debe mostrar:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5006
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

#### **8.2. Abrir Swagger**

```
https://localhost:5006/swagger
```

**Debe mostrar la interfaz de Swagger con:**
- ✅ Auth endpoints
- ✅ Products endpoints
- ✅ Categories endpoints
- ✅ Admin endpoints

#### **8.3. Hacer Login**

```
POST /api/auth/login

Body:
{
  "email": "santiago.c0399@gmail.com",
  "password": "TU_PASSWORD_ACTUAL"
}

// O si creaste usuarios de prueba:
{
  "email": "admin@bosko.com",
  "password": "Bosko123!"
}
```

**Debe retornar:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Admin Bosko",
    "email": "admin@bosko.com",
    "role": "Admin",
    ...
  }
}
```

#### **8.4. Autorizar en Swagger**

1. Copia el token del login
2. Click en el botón **"Authorize"** (candado verde arriba a la derecha)
3. Pega el token así: `Bearer tu_token_aqui`
4. Click en **"Authorize"**
5. Click en **"Close"**

#### **8.5. Probar Endpoints**

**Dashboard Stats:**
```
GET /api/admin/dashboard/stats
```

**Debe retornar:**
```json
{
  "sales": {
    "total": 1012.91,
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
    "total": 3,
    "active": 3
  },
  "products": {
    "total": 20,
    "inStock": 20,
    "outOfStock": 0
  }
}
```

**Productos:**
```
GET /api/products
```

**Debe retornar 20 productos con categorías**

**Pedidos Recientes:**
```
GET /api/admin/orders/recent?limit=5
```

**Debe retornar 5 pedidos ordenados por fecha**

---

## ✅ CHECKLIST FINAL

### **Base de Datos:**
```
□ Base de datos BoskoDB creada
□ Tabla Users existe y tiene usuarios
□ Tabla Products existe y tiene 20 productos
□ Tabla Categories existe y tiene 5 categorías
□ Tabla Orders existe y tiene 5 pedidos
□ Tabla OrderItems existe y tiene items
□ Tabla ActivityLogs existe y tiene actividades
□ Tabla Notifications existe y tiene notificaciones
```

### **Backend:**
```
□ Proyecto compila sin errores
□ Connection string configurado correctamente
□ Proyecto corre sin errores (dotnet run)
□ Swagger accesible en https://localhost:5006/swagger
□ Login funciona y retorna token
□ Token autorizado en Swagger
□ Endpoints de dashboard retornan datos
□ Endpoints de productos retornan 20 productos
□ Endpoints de admin requieren autorización
```

### **Datos de Prueba:**
```
□ 5 categorías insertadas
□ 20 productos insertados (4 por categoría)
□ 5 pedidos insertados con diferentes estados
□ 10 actividades del sistema
□ 5 notificaciones para admin
□ Todos los productos tienen imágenes
□ Todos los pedidos tienen historial
```

---

## 🐛 TROUBLESHOOTING

### **Error: "Cannot connect to SQL Server"**

**Solución:**
```sql
-- Verificar que SQL Server está corriendo
-- En Services (services.msc) buscar "SQL Server"
-- Debe estar "Running"

-- O ejecutar en CMD:
net start MSSQLSERVER
```

### **Error: "Login failed for user"**

**Solución:**
```json
// Cambiar connection string a Windows Authentication
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BoskoDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### **Error: "Database BoskoDB does not exist"**

**Solución:**
```sql
-- Ejecutar nuevamente:
Database/BoskoDB-Setup.sql
```

### **Error: "Table 'Orders' doesn't exist"**

**Solución:**
```sql
-- Ejecutar:
Database/Admin-Panel-Setup.sql
```

### **Error: 401 Unauthorized en endpoints de admin**

**Solución:**
```
1. Hacer login para obtener token
2. Copiar el token
3. En Swagger click "Authorize"
4. Pegar: Bearer {tu_token}
5. Click "Authorize" y luego "Close"
```

### **Error: "No products returned"**

**Solución:**
```sql
-- Ejecutar:
Database/Complete-Test-Data.sql

-- Verificar que se insertaron:
SELECT COUNT(*) FROM Products;
-- Debe retornar 20
```

### **Error: "Dashboard stats returns zeros"**

**Solución:**
```sql
-- Verificar que hay pedidos:
SELECT COUNT(*) FROM Orders;

-- Si retorna 0, ejecutar:
Database/Complete-Test-Data.sql
```

---

## 🎯 PRÓXIMOS PASOS

Una vez que todo esté funcionando:

### **1. Para Desarrollo:**
```
✅ Backend funcionando: https://localhost:5006
✅ Swagger disponible: https://localhost:5006/swagger
✅ Datos de prueba cargados
✅ Login funcionando con Admin
```

### **2. Para Frontend:**
```
✅ Integrar con endpoints del backend
✅ Usar token JWT para autenticación
✅ Probar dashboard con datos reales
✅ Probar CRUD de productos
✅ Probar gestión de pedidos
```

### **3. Para Testing:**
```
✅ Usar credenciales de prueba:
   Admin: admin@bosko.com / Bosko123!
   Employee: employee@bosko.com / Bosko123!
   Customer: customer@bosko.com / Bosko123!

✅ Probar todos los endpoints en Swagger
✅ Verificar permisos por rol
✅ Probar filtros y búsquedas
✅ Probar paginación
```

---

## 📚 DOCUMENTACIÓN RELACIONADA

- **Guía de Proyecto:** `BOSKO-PROJECT-GUIDELINES.md`
- **Referencia Rápida:** `QUICK-REFERENCE.md`
- **API Authentication:** `API-EXAMPLES-AUTHENTICATION.md`
- **Testing Guide:** `TESTING-GUIDE.md`
- **Estado del Proyecto:** `PROJECT-STATUS-REPORT.md`
- **Admin Panel:** `ADMIN-PANEL-BACKEND-IMPLEMENTATION.md`
- **Catálogo de Datos:** `TEST-DATA-CATALOG.md`

---

## 🎉 SISTEMA COMPLETAMENTE FUNCIONAL

Si seguiste todos los pasos, ahora tienes:

✅ **Base de datos completa** con todas las tablas  
✅ **20 productos de ejemplo** en 5 categorías  
✅ **5 pedidos de prueba** con diferentes estados  
✅ **Backend funcionando** con 13 endpoints de admin  
✅ **Autenticación completa** con JWT  
✅ **Dashboard con datos reales**  
✅ **Swagger para testing fácil**  

**¡El sistema Bosko E-Commerce está listo para desarrollo y demos!** 🚀

---

## 💡 TIPS FINALES

1. **Mantén Swagger abierto** para testing rápido
2. **Usa Postman** para guardar colecciones de requests
3. **Revisa los logs** de la consola si algo falla
4. **Documenta cambios** en el CHANGELOG
5. **Haz backups** de la BD antes de cambios grandes

---

**Fecha de última actualización:** 16 de Noviembre 2025  
**Versión del documento:** 1.0  
**Mantenido por:** Backend Team

---

## 📞 SOPORTE

**¿Problemas con la instalación?**
1. Revisa la sección de Troubleshooting
2. Verifica el CHECKLIST FINAL
3. Consulta la documentación relacionada

**¿Todo funcionó correctamente?**
¡Excelente! Ahora puedes empezar a desarrollar features nuevas o integrar con el frontend.

---

**¡HAPPY CODING!** 💻✨
