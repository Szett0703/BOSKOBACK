# ?? GUÍA DE DESPLIEGUE Y PRÓXIMOS PASOS

## ? Estado Actual

El backend está completamente implementado con todas las funcionalidades solicitadas:
- ? Sistema de roles y permisos
- ? Autenticación JWT completa
- ? CRUD de productos y categorías
- ? Gestión de pedidos
- ? Administración de usuarios
- ? Reseñas de productos
- ? Wishlist
- ? Direcciones de envío
- ? Documentación completa
- ? Datos de prueba preparados

## ?? Próximos Pasos Inmediatos

### 1. Aplicar Migraciones
```bash
cd C:\Users\santi.SZETT\Desktop\Dev\BOSKO\BOSKOBACK\BOSKOBACK
dotnet ef database update
```

Esto creará:
- Todas las tablas necesarias
- Roles (Admin, Employee, Customer) con seed automático

### 2. Poblar con Datos de Prueba
Ejecuta el script SQL en SQL Server:

**Opción A - SQL Server Management Studio:**
1. Abre SSMS
2. Conecta a tu servidor: `LOCALHOST\SQLEXPRESS`
3. Abre el archivo `SeedData.sql`
4. Asegúrate que la base `BOSKO` está seleccionada
5. Ejecuta (F5)

**Opción B - Línea de comandos:**
```bash
sqlcmd -S LOCALHOST\SQLEXPRESS -d BOSKO -i SeedData.sql
```

Esto creará:
- Usuario Admin (admin@bosko.com / Admin123)
- Usuarios de prueba
- 6 categorías con productos
- Pedidos de ejemplo
- Reseñas, direcciones y wishlist

### 3. Ejecutar la API
```bash
dotnet run
```

La API estará disponible en:
- HTTPS: https://localhost:7xxx
- Swagger: https://localhost:7xxx/swagger

### 4. Probar Funcionalidades

**Prueba Rápida con Swagger:**
1. Abre Swagger en el navegador
2. Prueba `POST /api/auth/login` con:
   ```json
   {
     "email": "admin@bosko.com",
     "password": "Admin123"
   }
   ```
3. Copia el token del response
4. Click en "Authorize" (candado verde arriba)
5. Ingresa: `Bearer {tu-token}`
6. Prueba endpoints protegidos

**Prueba Completa con Postman:**
- Importa la colección usando `POSTMAN_GUIDE.md` como referencia
- Sigue los flujos de prueba documentados

---

## ?? Configuración para Producción

### 1. Actualizar appsettings.json (Producción)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_PROD_SERVER;Database=BOSKO_PROD;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "GENERATE_A_STRONG_RANDOM_KEY_32_CHARS_MINIMUM",
    "Issuer": "BoskoAPI",
    "Audience": "BoskoApp",
    "ExpireMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 2. Variables de Entorno (Recomendado)

En producción, usa variables de entorno en lugar de appsettings:

```bash
# Azure App Service
ConnectionStrings__DefaultConnection=...
Jwt__Key=...

# Docker
docker run -e "ConnectionStrings__DefaultConnection=..." \
           -e "Jwt__Key=..." \
           boskoback:latest
```

### 3. CORS para Frontend en Producción

En `Program.cs`, actualiza:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient",
        policy => policy.WithOrigins(
            "https://tu-dominio-frontend.com",
            "https://www.tu-dominio-frontend.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});
```

### 4. Seguridad Adicional

- [ ] Usar HTTPS en producción (certificado SSL)
- [ ] Configurar rate limiting para prevenir abuso
- [ ] Implementar logging estructurado (Serilog)
- [ ] Configurar health checks
- [ ] Implementar Application Insights o similar
- [ ] Revisar y fortalecer políticas de CORS
- [ ] Implementar refresh tokens para JWT
- [ ] Configurar backup automático de base de datos

---

## ?? Opciones de Despliegue

### Opción 1: Azure App Service (Recomendado)

**Paso 1 - Publicar:**
```bash
dotnet publish -c Release -o ./publish
```

**Paso 2 - Crear App Service:**
```bash
az webapp create --name boskoback \
  --resource-group bosko-rg \
  --plan bosko-plan
```

**Paso 3 - Configurar Connection String:**
```bash
az webapp config connection-string set \
  --name boskoback \
  --resource-group bosko-rg \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=..."
```

**Paso 4 - Deploy:**
```bash
az webapp deployment source config-zip \
  --name boskoback \
  --resource-group bosko-rg \
  --src publish.zip
```

### Opción 2: Docker

**Crear Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BOSKOBACK.csproj", "./"]
RUN dotnet restore "BOSKOBACK.csproj"
COPY . .
RUN dotnet build "BOSKOBACK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BOSKOBACK.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BOSKOBACK.dll"]
```

**Build y Run:**
```bash
docker build -t boskoback:latest .
docker run -d -p 8080:80 \
  -e "ConnectionStrings__DefaultConnection=..." \
  -e "Jwt__Key=..." \
  boskoback:latest
```

### Opción 3: IIS (Windows Server)

1. Instalar IIS con ASP.NET Core Module
2. Publicar proyecto: `dotnet publish -c Release`
3. Copiar archivos publicados a `C:\inetpub\wwwroot\boskoback`
4. Crear Application Pool (.NET CLR: No Managed Code)
5. Configurar Connection String en web.config
6. Configurar bindings (HTTPS recomendado)

---

## ??? Base de Datos en Producción

### SQL Azure (Recomendado para Azure App Service)

**Crear base de datos:**
```bash
az sql server create --name bosko-sql-server \
  --resource-group bosko-rg \
  --location eastus \
  --admin-user boskoAdmin \
  --admin-password "YourSecurePassword123!"

az sql db create --resource-group bosko-rg \
  --server bosko-sql-server \
  --name BOSKO_PROD \
  --service-objective S0
```

**Aplicar migraciones:**
```bash
dotnet ef database update --connection "Server=bosko-sql-server.database.windows.net;Database=BOSKO_PROD;..."
```

### SQL Server On-Premise

1. Crear base de datos BOSKO_PROD
2. Crear usuario con permisos adecuados
3. Ejecutar migraciones desde máquina con acceso
4. Ejecutar SeedData.sql (ajustado para producción)

---

## ?? Usuario Admin Inicial en Producción

**IMPORTANTE:** No uses las contraseñas de prueba en producción.

**Opción 1 - Crear vía API tras despliegue:**
```http
POST https://tu-api-produccion.com/api/auth/register
{
  "name": "Admin Principal",
  "email": "admin@tu-dominio.com",
  "password": "ContraseñaSegura123!@#"
}
```

Luego, actualiza manualmente el RoleId a 1 (Admin) en la base de datos:
```sql
UPDATE Users SET RoleId = 1 WHERE Email = 'admin@tu-dominio.com';
```

**Opción 2 - Script SQL directo:**
```sql
-- Generar hash BCrypt de tu contraseña segura primero
-- (usa el método descrito en PASSWORDS.md)
INSERT INTO Users (Name, Email, PasswordHash, Provider, RoleId)
VALUES ('Admin', 'admin@tu-dominio.com', 'HASH_AQUI', 'Local', 1);
```

---

## ?? Monitoreo y Mantenimiento

### Logs
- Implementar logging estructurado
- Almacenar logs en servicio centralizado
- Configurar alertas para errores críticos

### Métricas
- Response times
- Error rates
- Autenticaciones fallidas
- Uso de recursos

### Backups
- Base de datos: diario
- Código: control de versiones (Git)
- Configuración: documentada y versionada

---

## ?? Testing Antes de Producción

- [ ] Ejecutar todos los tests de Postman
- [ ] Verificar autenticación y autorización
- [ ] Probar flujos completos de usuario
- [ ] Validar manejo de errores
- [ ] Revisar seguridad (SQL injection, XSS)
- [ ] Test de carga (si es crítico)
- [ ] Verificar CORS con frontend real
- [ ] Probar recuperación de contraseña

---

## ?? Checklist de Pre-Despliegue

### Código
- [ ] Todas las migraciones aplicadas
- [ ] Compilación sin warnings
- [ ] Configuración de producción lista
- [ ] Secrets no en código (usar variables entorno)

### Base de Datos
- [ ] Backup de desarrollo creado
- [ ] Base de datos de producción creada
- [ ] Migraciones aplicadas
- [ ] Usuario admin inicial creado

### Configuración
- [ ] Connection string de producción configurada
- [ ] JWT Key fuerte generada
- [ ] CORS configurado para dominio real
- [ ] HTTPS habilitado
- [ ] Logs configurados

### Documentación
- [ ] README actualizado con URLs de producción
- [ ] Credenciales documentadas de forma segura
- [ ] Procedimientos de backup documentados
- [ ] Contactos de soporte definidos

---

## ?? Troubleshooting Post-Despliegue

### API no responde
1. Verificar que el servicio está corriendo
2. Revisar logs del servidor
3. Verificar firewall y reglas de red
4. Comprobar health check endpoint

### Errores de base de datos
1. Verificar connection string
2. Comprobar credenciales
3. Verificar que migraciones están aplicadas
4. Revisar logs de SQL

### Problemas de autenticación
1. Verificar JWT Key en configuración
2. Comprobar Issuer/Audience
3. Revisar expiración de tokens
4. Validar roles en claims

### CORS errors
1. Verificar origen en configuración
2. Comprobar que frontend usa HTTPS si API usa HTTPS
3. Revisar headers permitidos

---

## ?? Soporte y Mantenimiento

### Actualizaciones Futuras
- Mantener .NET actualizado
- Actualizar paquetes NuGet regularmente
- Revisar y actualizar dependencias con vulnerabilidades

### Escalabilidad
- Considerar múltiples instancias (load balancer)
- Implementar caché distribuido (Redis)
- Optimizar queries de base de datos
- Implementar paginación en listados grandes

---

## ? Estado: LISTO PARA DESPLIEGUE

El backend está completamente implementado, documentado y listo para:
1. Aplicar migraciones
2. Poblar datos de prueba
3. Realizar testing exhaustivo
4. Desplegar a producción

**Última actualización:** $(Get-Date)
**Versión:** 1.0.0
**Tecnología:** ASP.NET Core 8.0 + EF Core + SQL Server
