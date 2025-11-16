# 🔧 GUÍA COMPLETA DE TROUBLESHOOTING

**Fecha:** 16 de Noviembre 2025  
**Para:** Bosko E-Commerce Backend  
**Framework:** .NET 8

---

## 🎯 DIAGNÓSTICO RÁPIDO

### Síntomas que estás experimentando:

1. **ERR_CONNECTION_REFUSED** en `https://localhost:5006/api/categories`
2. **500 Internal Server Error** en `https://localhost:5006/api/admin/orders`

---

## 🔍 DIAGNÓSTICO POR SÍNTOMA

### SÍNTOMA 1: ERR_CONNECTION_REFUSED

**¿Qué significa?**
El navegador no puede conectarse con el servidor. El backend NO está corriendo o NO está escuchando en el puerto correcto.

**Verificación inmediata:**
```bash
# Test 1: ¿El backend está corriendo?
netstat -ano | findstr :5006

# Si NO aparece nada → Backend NO está corriendo
# Si aparece algo → Backend está corriendo
```

**Causa raíz:** ❌ **Backend no está corriendo**

**Solución:**
```bash
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK
dotnet run
```

**Verificación:**
- ¿Ves la salida con `✅ HTTPS Server listening on: https://localhost:5006`?
- SÍ → Backend corriendo ✅
- NO → Ver sección "Backend no inicia"

---

### SÍNTOMA 2: 500 Internal Server Error

**¿Qué significa?**
El backend está corriendo PERO hay un error en el código o en la base de datos.

**Verificación inmediata:**
```bash
# Ver logs del backend
# Los logs aparecen en la consola donde ejecutaste dotnet run

# Busca líneas con ❌ o ERROR
```

**Causas posibles:**
1. ❌ **Tabla Orders no existe**
2. ❌ **Tabla Orders está vacía**
3. ❌ **Error de conexión a SQL Server**
4. ❌ **NullReferenceException en el código**

**Solución paso a paso:**

#### Paso 1: Verificar que SQL Server está corriendo
```bash
# Abrir Services (services.msc)
# Buscar: SQL Server (MSSQLSERVER) o SQL Server (SQLEXPRESS)
# Estado debe ser: Running

# O ejecutar:
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Si funciona → SQL Server OK ✅
# Si falla → SQL Server NO está corriendo ❌
```

#### Paso 2: Verificar que la base de datos existe
```sql
-- Abrir SSMS (SQL Server Management Studio)
-- Conectarse a: localhost

-- Ejecutar:
SELECT name FROM sys.databases WHERE name = 'BoskoDB';

-- Si devuelve "BoskoDB" → Base de datos existe ✅
-- Si no devuelve nada → Base de datos NO existe ❌
```

#### Paso 3: Verificar que las tablas existen
```sql
USE BoskoDB;
GO

-- Ejecutar script de verificación:
-- Database/COMPLETE-DATABASE-VERIFICATION.sql

-- O manualmente:
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Deben aparecer:
-- ✅ Categories
-- ✅ Orders
-- ✅ OrderItems
-- ✅ Products
-- ✅ Users
```

#### Paso 4: Verificar que las tablas tienen datos
```sql
USE BoskoDB;
GO

SELECT COUNT(*) AS CategoriesCount FROM Categories;
SELECT COUNT(*) AS ProductsCount FROM Products;
SELECT COUNT(*) AS UsersCount FROM Users;
SELECT COUNT(*) AS OrdersCount FROM Orders;

-- Si algún COUNT es 0 → Tabla vacía → Insertar datos
```

---

## 📋 SOLUCIONES ESPECÍFICAS

### SOLUCIÓN A: Backend no está corriendo

**Síntomas:**
- ERR_CONNECTION_REFUSED
- No hay salida en consola
- `netstat -ano | findstr :5006` no devuelve nada

**Pasos:**
```bash
# 1. Navegar al proyecto
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK

# 2. Verificar que compila
dotnet build

# 3. Si compila OK, ejecutar
dotnet run

# 4. Esperar a ver:
# "✅ HTTPS Server listening on: https://localhost:5006"

# 5. Si aparece error, leer el error y seguir a SOLUCIÓN B
```

---

### SOLUCIÓN B: Error de compilación

**Síntomas:**
- `dotnet build` falla
- Errores en Program.cs
- Errores de paquetes NuGet

**Pasos:**
```bash
# 1. Limpiar proyecto
dotnet clean

# 2. Restaurar paquetes
dotnet restore

# 3. Build de nuevo
dotnet build

# 4. Si persiste el error, ver el mensaje específico
# y buscar en sección "Errores Comunes"
```

---

### SOLUCIÓN C: Base de datos no existe

**Síntomas:**
- 500 Internal Server Error
- Logs muestran: "Cannot open database BoskoDB"
- SSMS no muestra BoskoDB

**Pasos:**
```bash
# 1. Verificar ConnectionString en appsettings.json
# Debe ser:
"Server=localhost;Database=BoskoDB;Integrated Security=true;TrustServerCertificate=True;"

# 2. Crear base de datos con migraciones
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK

# 3. Aplicar migraciones
dotnet ef database update

# 4. Verificar que se creó
sqlcmd -S localhost -E -Q "SELECT name FROM sys.databases WHERE name = 'BoskoDB'"

# 5. Si se creó OK, continuar con SOLUCIÓN D
```

---

### SOLUCIÓN D: Tablas no existen

**Síntomas:**
- Base de datos existe PERO está vacía
- Migraciones no se han aplicado
- Error: "Invalid object name 'Categories'"

**Pasos:**
```bash
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK

# 1. Ver migraciones disponibles
dotnet ef migrations list

# 2. Aplicar todas las migraciones
dotnet ef database update

# 3. Verificar que se crearon las tablas
```

```sql
-- En SSMS:
USE BoskoDB;
GO

SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
```

---

### SOLUCIÓN E: Tablas vacías

**Síntomas:**
- Tablas existen PERO no tienen datos
- GET /api/categories devuelve array vacío []
- GET /api/admin/orders da error 500 por falta de datos

**Pasos:**
```sql
-- 1. Ejecutar en SSMS:
-- Database/Complete-Data-Insert-Clean.sql

-- Esto insertará:
-- ✅ 5 Categorías
-- ✅ 20 Productos
-- ✅ 5 Pedidos
-- ✅ Items de pedidos
-- ✅ Usuarios (Admin y Customer)

-- 2. Verificar:
USE BoskoDB;
GO

SELECT COUNT(*) FROM Categories; -- Debe devolver 5
SELECT COUNT(*) FROM Products;   -- Debe devolver 20
SELECT COUNT(*) FROM Orders;     -- Debe devolver 5
SELECT COUNT(*) FROM Users;      -- Debe devolver al menos 2
```

---

### SOLUCIÓN F: Error de autenticación (401)

**Síntomas:**
- GET /api/admin/orders devuelve 401 Unauthorized
- Swagger muestra candado cerrado 🔒
- Frontend devuelve 401

**Pasos:**
```bash
# 1. Hacer login para obtener token
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@test.com\",\"password\":\"Admin123!\"}" \
  -k

# 2. Copiar el token de la respuesta

# 3. Usar el token en requests:
curl -X GET "https://localhost:5006/api/admin/orders?page=1&limit=10" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -k

# 4. En Swagger:
# - Click en "Authorize" button (🔓)
# - Pegar: Bearer TU_TOKEN_AQUI
# - Click "Authorize"
# - Ahora puedes hacer requests autenticados
```

---

### SOLUCIÓN G: Certificado HTTPS no confiable

**Síntomas:**
- Navegador muestra "Your connection is not private"
- NET::ERR_CERT_AUTHORITY_INVALID
- Swagger no carga

**Pasos:**
```bash
# 1. Limpiar certificados antiguos
dotnet dev-certs https --clean

# 2. Generar y confiar en nuevos certificados
dotnet dev-certs https --trust

# 3. Aparecerá un popup de Windows
# Click en "Sí" para confiar

# 4. Verificar
dotnet dev-certs https --check --trust

# 5. Reiniciar navegador

# 6. Abrir https://localhost:5006/swagger
# Debe cargar sin advertencias
```

---

### SOLUCIÓN H: Puerto ya en uso

**Síntomas:**
- Error: "Address already in use"
- Backend no inicia
- Otro proceso está usando el puerto 5006

**Pasos:**
```bash
# 1. Ver qué está usando el puerto
netstat -ano | findstr :5006

# 2. Ver el PID (última columna)
# Ejemplo: 0.0.0.0:5006  0.0.0.0:0  LISTENING  12345

# 3. Matar el proceso (como Administrador)
taskkill /PID 12345 /F

# 4. Intentar de nuevo
dotnet run
```

---

## 🔍 DIAGNÓSTICO CON LOGS

### Cómo leer los logs del backend:

```bash
# Al ejecutar dotnet run, verás logs como:

✅ HTTP Server listening on: http://localhost:5005
✅ HTTPS Server listening on: https://localhost:5006
✅ Swagger UI habilitado

# Estos son buenos ✅

# Si ves:
❌ JWT Authentication failed
❌ Error connecting to database
❌ NullReferenceException

# Estos indican problemas ❌
```

### Logs comunes y su significado:

```
📨 GET /api/categories - Origin: http://localhost:4200
✅ GET /api/categories → 200
```
**Significado:** Request exitoso ✅

```
📨 GET /api/admin/orders - Origin: http://localhost:4200
❌ GET /api/admin/orders → 500
```
**Significado:** Error en el servidor ❌
**Acción:** Ver el log de excepción que aparece después

```
❌ JWT Authentication failed: The token is expired
```
**Significado:** Token JWT expirado
**Acción:** Hacer login de nuevo para obtener nuevo token

```
❌ Error connecting to database: Cannot open database
```
**Significado:** No puede conectarse a SQL Server
**Acción:** Ver SOLUCIÓN C

---

## 📝 CHECKLIST DE VERIFICACIÓN COMPLETA

### 1. Entorno
- [ ] .NET 8 SDK instalado (`dotnet --version`)
- [ ] SQL Server corriendo (services.msc)
- [ ] Visual Studio o VS Code instalado

### 2. Base de Datos
- [ ] Base de datos BoskoDB existe
- [ ] Todas las tablas existen (Categories, Products, Orders, Users, etc.)
- [ ] Tablas tienen datos
- [ ] Usuario Admin existe

### 3. Backend
- [ ] Proyecto compila sin errores (`dotnet build`)
- [ ] Backend está corriendo (`dotnet run`)
- [ ] Puerto 5006 está libre
- [ ] Logs no muestran errores

### 4. Certificados
- [ ] Certificados HTTPS confiables (`dotnet dev-certs https --trust`)
- [ ] Navegador acepta certificado
- [ ] Swagger carga correctamente

### 5. Conexión
- [ ] ConnectionString correcto en appsettings.json
- [ ] SQL Server acepta conexiones
- [ ] CORS configurado correctamente

### 6. Endpoints
- [ ] GET /health funciona (200 OK)
- [ ] GET /api/categories funciona (200 OK)
- [ ] POST /api/auth/login funciona (200 OK + token)
- [ ] GET /api/admin/orders funciona con token (200 OK)

---

## 🔧 COMANDOS DE DIAGNÓSTICO

### Verificar entorno:
```bash
# Versión de .NET
dotnet --version

# Info completa
dotnet --info

# SQL Server corriendo
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

### Verificar proyecto:
```bash
# Compilar
dotnet build

# Ver errores detallados
dotnet build --verbosity detailed

# Listar migraciones
dotnet ef migrations list

# Aplicar migraciones
dotnet ef database update
```

### Verificar puertos:
```bash
# Puerto 5006
netstat -ano | findstr :5006

# Puerto 5005
netstat -ano | findstr :5005

# Todos los puertos en uso
netstat -ano | findstr LISTENING
```

### Verificar base de datos:
```sql
-- En SSMS o sqlcmd:

-- Listar bases de datos
SELECT name FROM sys.databases;

-- Usar Bosko DB
USE BoskoDB;

-- Listar tablas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Contar registros
SELECT 
    'Categories' AS Tabla, COUNT(*) AS Total FROM Categories UNION ALL
SELECT 'Products', COUNT(*) FROM Products UNION ALL
SELECT 'Users', COUNT(*) FROM Users UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders;
```

---

## 🎯 FLUJO DE DIAGNÓSTICO COMPLETO

```
1. ¿Backend compila?
   NO → dotnet build → Ver errores → Corregir → Volver a 1
   SÍ → Continuar a 2

2. ¿Backend inicia?
   NO → Ver logs → Solucionar error → Volver a 2
   SÍ → Continuar a 3

3. ¿SQL Server corriendo?
   NO → Iniciar SQL Server → Volver a 3
   SÍ → Continuar a 4

4. ¿Base de datos existe?
   NO → dotnet ef database update → Volver a 4
   SÍ → Continuar a 5

5. ¿Tablas existen?
   NO → dotnet ef database update → Volver a 5
   SÍ → Continuar a 6

6. ¿Tablas tienen datos?
   NO → Ejecutar Complete-Data-Insert-Clean.sql → Volver a 6
   SÍ → Continuar a 7

7. ¿Certificados HTTPS OK?
   NO → dotnet dev-certs https --trust → Volver a 7
   SÍ → Continuar a 8

8. ¿Swagger carga?
   NO → Ver logs del navegador → Solucionar → Volver a 8
   SÍ → Continuar a 9

9. ¿GET /api/categories funciona?
   NO → Ver SOLUCIÓN específica → Volver a 9
   SÍ → Continuar a 10

10. ¿POST /api/auth/login funciona?
    NO → Verificar usuarios en BD → Volver a 10
    SÍ → Continuar a 11

11. ¿GET /api/admin/orders funciona con token?
    NO → Ver logs → SOLUCIÓN específica → Volver a 11
    SÍ → ✅ TODO FUNCIONA

12. ✅ Backend 100% funcional
    → Conectar con frontend Angular
    → ¡Listo!
```

---

## 📞 SIGUIENTE ACCIÓN RECOMENDADA

**PASO 1:** Ejecuta el script de verificación
```sql
-- En SSMS:
-- Archivo: Database/COMPLETE-DATABASE-VERIFICATION.sql

-- Esto te dirá exactamente qué falta
```

**PASO 2:** Sigue las recomendaciones del script

**PASO 3:** Inicia el backend
```bash
dotnet run
```

**PASO 4:** Prueba los endpoints en Swagger
```
https://localhost:5006/swagger
```

---

## ✅ CONFIRMACIÓN FINAL

Una vez que todos los checks estén en ✅, tu backend estará 100% funcional.

**Tiempo estimado de troubleshooting:** 10-15 minutos

**Resultado esperado:** Backend corriendo sin errores, todos los endpoints funcionando

---

**¡Tu backend va a funcionar perfectamente!** 🚀
