# 🚀 INICIO RÁPIDO - Backend Bosko E-Commerce

## ⚡ COMANDOS RÁPIDOS

### 1. Compilar
```bash
dotnet build
```

### 2. Ejecutar
```bash
dotnet run
```

### 3. Abrir Swagger
```
https://localhost:5006/swagger
```

---

## ✅ VERIFICACIÓN RÁPIDA (30 segundos)

### ¿El servidor está corriendo?
Verás este mensaje en la consola:
```
============================================
🚀 BOSKO E-COMMERCE API
============================================
Environment: Development
HTTPS: https://localhost:5006
HTTP:  http://localhost:5005
Swagger: https://localhost:5006/swagger
============================================
✅ Swagger UI habilitado en: https://localhost:5006/swagger
✅ CORS configurado para: http://localhost:4200, http://localhost:4300
============================================
✅ API LISTA - Esperando requests...
============================================
```

### ¿Swagger está funcionando?
1. Abre: `https://localhost:5006/swagger`
2. Deberías ver la interfaz de Swagger con todos los endpoints
3. **NO debería aparecer el error "Failed to load API definition"**

---

## 🧪 PRUEBAS RÁPIDAS

### 1. Health Check
```bash
curl https://localhost:5006/health -k
```

**Respuesta esperada:**
```json
{
  "status": "healthy",
  "timestamp": "2025-11-16T...",
  "environment": "Development",
  "urls": ["https://localhost:5006", "http://localhost:5005"]
}
```

### 2. Login de Prueba
```bash
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@test.com\",\"password\":\"Admin123!\"}" \
  -k
```

**Respuesta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Admin User",
    "email": "admin@test.com",
    "role": "Admin"
  }
}
```

---

## 🔧 SOLUCIÓN DE PROBLEMAS

### Problema: "Port already in use"
```powershell
# PowerShell (Admin)
netstat -ano | findstr :5006
taskkill /PID <PID> /F
dotnet run
```

### Problema: "Certificate error"
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
dotnet run
```

### Problema: "Connection string not found"
Verifica que `appsettings.json` tenga:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BoskoDB;..."
  }
}
```

---

## 📝 ENDPOINTS PRINCIPALES

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrarse
- `POST /api/auth/forgot-password` - Recuperar contraseña
- `POST /api/auth/reset-password` - Resetear contraseña

### Admin - Pedidos
- `GET /api/admin/orders` - Listar pedidos
- `GET /api/admin/orders/{id}` - Detalle de pedido
- `PATCH /api/admin/orders/{id}/status` - Cambiar estado

### Admin - Productos
- `GET /api/admin/products` - Listar productos
- `POST /api/admin/products` - Crear producto
- `PUT /api/admin/products/{id}` - Actualizar producto
- `DELETE /api/admin/products/{id}` - Eliminar producto

### Admin - Categorías
- `GET /api/admin/categories` - Listar categorías
- `POST /api/admin/categories` - Crear categoría
- `PUT /api/admin/categories/{id}` - Actualizar categoría
- `DELETE /api/admin/categories/{id}` - Eliminar categoría

### Admin - Usuarios
- `GET /api/admin/users` - Listar usuarios
- `PUT /api/admin/users/{id}` - Actualizar usuario
- `DELETE /api/admin/users/{id}` - Eliminar usuario

### Público
- `GET /api/products` - Listar productos (público)
- `GET /api/categories` - Listar categorías (público)
- `GET /health` - Health check

---

## 🔐 AUTENTICACIÓN CON JWT

### Obtener Token:
```bash
POST /api/auth/login
{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

### Usar Token en Requests:
```bash
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### En Swagger:
1. Click en el botón "Authorize" (candado verde)
2. Ingresa: `Bearer <tu-token-aqui>`
3. Click "Authorize"
4. Ahora puedes probar endpoints protegidos

---

## ✅ CHECKLIST DE INICIO

- [ ] .NET 8 SDK instalado
- [ ] SQL Server corriendo
- [ ] Base de datos BoskoDB creada
- [ ] `dotnet dev-certs https --trust` ejecutado
- [ ] Connection string configurado en appsettings.json
- [ ] `dotnet build` exitoso
- [ ] `dotnet run` ejecutándose
- [ ] Swagger abre en https://localhost:5006/swagger
- [ ] Health check funciona
- [ ] ¡Listo! 🎉

---

## 🚀 MODO PRODUCCIÓN

### Configurar para Producción:
1. Cambiar `ASPNETCORE_ENVIRONMENT` a `Production`
2. Actualizar connection string de producción
3. Configurar secretos seguros
4. Habilitar HTTPS obligatorio
5. Configurar rate limiting
6. Configurar logging a archivo o servicio

---

## 📚 DOCUMENTACIÓN COMPLETA

- `SWAGGER-FINAL-FIX.md` - Solución del error 500
- `MODEL-DATABASE-SYNC-FIX.md` - Sincronización de modelos
- `COMPLETE-VERIFICATION-REPORT.md` - Reporte completo
- `BACKEND-COMPLETE-DOCUMENTATION.md` - Documentación general

---

**¡Tu backend está listo para usar!** 🎉

Ejecuta `dotnet run` y empieza a desarrollar tu frontend Angular.
