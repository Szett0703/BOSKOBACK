# 🔧 COMANDOS POWERSHELL - WINDOWS

## ⚡ SETUP INICIAL (Ejecutar UNA VEZ)

### 1. Confiar en Certificados HTTPS
```powershell
# Limpiar certificados antiguos
dotnet dev-certs https --clean

# Generar y confiar en nuevos certificados
dotnet dev-certs https --trust

# Verificar
dotnet dev-certs https --check --trust
```

**Nota:** Aparecerá un popup de Windows, hacer click en "Sí"

---

## 🚀 INICIAR BACKEND

### Método 1: Simple
```powershell
dotnet run
```

### Método 2: Con profile específico
```powershell
dotnet run --launch-profile https
```

### Método 3: Con rebuild
```powershell
dotnet clean
dotnet build
dotnet run
```

---

## 🧪 TESTING CON POWERSHELL

### Test 1: Health Check
```powershell
Invoke-WebRequest -Uri "https://localhost:5006/health" -Method GET -SkipCertificateCheck | Select-Object -ExpandProperty Content
```

### Test 2: Login
```powershell
$body = @{
    email = "admin@test.com"
    password = "Admin123!"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "https://localhost:5006/api/auth/login" `
    -Method POST `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck

$response.Content | ConvertFrom-Json
```

### Test 3: Get Orders (con token)
```powershell
# Guardar token de login anterior
$token = "eyJhbGci..." # Reemplazar con tu token

$headers = @{
    "Authorization" = "Bearer $token"
}

Invoke-WebRequest -Uri "https://localhost:5006/api/admin/orders?page=1&limit=10" `
    -Method GET `
    -Headers $headers `
    -SkipCertificateCheck | Select-Object -ExpandProperty Content
```

---

## 🔍 DIAGNÓSTICO

### Ver qué está usando el puerto 5006
```powershell
# Ver procesos en puerto 5006
netstat -ano | findstr :5006

# Obtener información del proceso
Get-Process -Id <PID>
```

### Matar proceso en puerto 5006
```powershell
# Como Administrador
$port = 5006
$processId = (Get-NetTCPConnection -LocalPort $port).OwningProcess
Stop-Process -Id $processId -Force
```

### Ver todos los puertos en uso
```powershell
netstat -ano | findstr LISTENING
```

---

## 🔐 CERTIFICADOS

### Listar certificados de desarrollo
```powershell
# PowerShell
Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object {$_.Subject -like "*localhost*"}
```

### Eliminar todos los certificados localhost
```powershell
# Como Administrador
Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object {$_.Subject -like "*localhost*"} | Remove-Item
```

### Regenerar certificados
```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

---

## 📝 LOGS Y DEBUGGING

### Ver logs en tiempo real
```powershell
# Ejecutar con logging verbose
dotnet run --verbosity detailed
```

### Guardar logs en archivo
```powershell
dotnet run 2>&1 | Tee-Object -FilePath "logs.txt"
```

### Ver últimas líneas de log
```powershell
Get-Content logs.txt -Tail 50
```

---

## 🔄 RESTAURAR Y REBUILD

### Restaurar packages
```powershell
dotnet restore
```

### Clean completo
```powershell
dotnet clean
Remove-Item -Recurse -Force .\bin
Remove-Item -Recurse -Force .\obj
```

### Build sin ejecutar
```powershell
dotnet build
```

### Verificar compilación
```powershell
dotnet build --no-incremental
```

---

## 🗄️ BASE DE DATOS

### Crear migración
```powershell
dotnet ef migrations add NombreMigracion
```

### Aplicar migraciones
```powershell
dotnet ef database update
```

### Eliminar última migración
```powershell
dotnet ef migrations remove
```

### Ver todas las migraciones
```powershell
dotnet ef migrations list
```

### Resetear base de datos
```powershell
dotnet ef database drop
dotnet ef database update
```

---

## 🔧 UTILIDADES

### Ver versión de .NET
```powershell
dotnet --version
```

### Ver info completa
```powershell
dotnet --info
```

### Listar SDKs instalados
```powershell
dotnet --list-sdks
```

### Listar runtimes instalados
```powershell
dotnet --list-runtimes
```

### Ver variables de entorno
```powershell
Get-ChildItem Env: | Where-Object {$_.Name -like "*ASPNET*"}
```

---

## 🌐 FIREWALL (Como Administrador)

### Agregar regla para puerto 5006
```powershell
New-NetFirewallRule -DisplayName "ASP.NET Core HTTPS (5006)" `
    -Direction Inbound `
    -LocalPort 5006 `
    -Protocol TCP `
    -Action Allow
```

### Agregar regla para puerto 5005
```powershell
New-NetFirewallRule -DisplayName "ASP.NET Core HTTP (5005)" `
    -Direction Inbound `
    -LocalPort 5005 `
    -Protocol TCP `
    -Action Allow
```

### Ver reglas existentes
```powershell
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*ASP.NET*"}
```

---

## 🧹 LIMPIEZA

### Limpiar todo
```powershell
# Detener procesos
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue

# Limpiar archivos temporales
dotnet clean
Remove-Item -Recurse -Force .\bin -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .\obj -ErrorAction SilentlyContinue

# Restaurar
dotnet restore
```

---

## 📦 PUBLICACIÓN

### Publicar para Windows
```powershell
dotnet publish -c Release -o .\publish
```

### Publicar con runtime específico
```powershell
dotnet publish -c Release -r win-x64 --self-contained -o .\publish
```

### Ejecutar publicación
```powershell
.\publish\DBTest-BACK.exe
```

---

## 🎯 SCRIPT COMPLETO DE INICIO

Crea un archivo `start-backend.ps1`:

```powershell
# start-backend.ps1
Write-Host "🚀 Iniciando Bosko Backend..." -ForegroundColor Cyan

# 1. Verificar certificados
Write-Host "📜 Verificando certificados HTTPS..." -ForegroundColor Yellow
dotnet dev-certs https --check --trust
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️ Configurando certificados..." -ForegroundColor Yellow
    dotnet dev-certs https --trust
}

# 2. Restaurar packages
Write-Host "📦 Restaurando packages..." -ForegroundColor Yellow
dotnet restore

# 3. Build
Write-Host "🔨 Compilando proyecto..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error en compilación" -ForegroundColor Red
    exit 1
}

# 4. Ejecutar
Write-Host "✅ Iniciando servidor..." -ForegroundColor Green
Write-Host "📝 Presiona Ctrl+C para detener" -ForegroundColor Yellow
Write-Host ""
dotnet run
```

**Usar el script:**
```powershell
.\start-backend.ps1
```

---

## 🆘 TROUBLESHOOTING

### Error: "Access denied"
```powershell
# Ejecutar PowerShell como Administrador
# Click derecho en PowerShell → "Ejecutar como administrador"
```

### Error: "Execution policy"
```powershell
# Permitir ejecución de scripts (como Admin)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Error: "dotnet command not found"
```powershell
# Verificar PATH
$env:Path -split ';' | Where-Object {$_ -like "*dotnet*"}

# Reinstalar .NET SDK si es necesario
# https://dotnet.microsoft.com/download
```

### Puerto ocupado
```powershell
# Ver qué lo usa
netstat -ano | findstr :5006

# Matarlo (como Admin)
taskkill /PID <PID> /F
```

---

## ✅ CHECKLIST DE COMANDOS

- [ ] `dotnet --version` → Verificar .NET instalado
- [ ] `dotnet dev-certs https --trust` → Certificados HTTPS
- [ ] `dotnet restore` → Restaurar packages
- [ ] `dotnet build` → Compilar
- [ ] `dotnet run` → Ejecutar
- [ ] Abrir `https://localhost:5006/swagger`
- [ ] Probar endpoints en Swagger

---

**Tiempo estimado:** ~5 minutos  
**Requisitos:** PowerShell 5.1+ y .NET 8 SDK

**¡Listo para producción local!** 🚀
