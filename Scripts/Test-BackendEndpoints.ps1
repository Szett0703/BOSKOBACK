# 🔍 VERIFICADOR DE ENDPOINTS - BOSKO BACKEND
# Este script verifica que todos los endpoints del backend están funcionando

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "🔍 VERIFICADOR DE ENDPOINTS BACKEND" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "https://localhost:5006"
$allPassed = $true

# Función para verificar endpoint
function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Endpoint,
        [string]$Description,
        [bool]$RequiresAuth = $false
    )
    
    try {
        $url = "$baseUrl$Endpoint"
        Write-Host "Testing: $Method $Endpoint" -NoNewline
        
        $headers = @{}
        if ($RequiresAuth) {
            # Por ahora solo verificamos que responda, aunque sea con 401
            # En producción deberías agregar el token real
        }
        
        $response = Invoke-WebRequest -Uri $url -Method $Method -Headers $headers -SkipCertificateCheck -ErrorAction SilentlyContinue
        
        if ($response.StatusCode -eq 200) {
            Write-Host " ✅ OK (200)" -ForegroundColor Green
            return $true
        } else {
            Write-Host " ⚠️  $($response.StatusCode)" -ForegroundColor Yellow
            return $false
        }
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        
        # 401 es aceptable para endpoints protegidos
        if ($RequiresAuth -and $statusCode -eq 401) {
            Write-Host " ✅ OK (401 - Auth required)" -ForegroundColor Green
            return $true
        }
        # 404 es un error
        elseif ($statusCode -eq 404) {
            Write-Host " ❌ NOT FOUND (404)" -ForegroundColor Red
            $script:allPassed = $false
            return $false
        }
        else {
            Write-Host " ⚠️  Error: $statusCode" -ForegroundColor Yellow
            return $false
        }
    }
}

Write-Host "Verificando backend en: $baseUrl" -ForegroundColor Yellow
Write-Host ""

# Health Check
Write-Host "=== HEALTH CHECK ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/health" -Description "Health Check"
Write-Host ""

# Authentication Endpoints
Write-Host "=== AUTHENTICATION ===" -ForegroundColor Cyan
Test-Endpoint -Method "POST" -Endpoint "/api/auth/login" -Description "Login"
Test-Endpoint -Method "POST" -Endpoint "/api/auth/register" -Description "Register"
Test-Endpoint -Method "POST" -Endpoint "/api/auth/init-users" -Description "Init Users"
Write-Host ""

# Public Endpoints
Write-Host "=== PUBLIC ENDPOINTS ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/api/products" -Description "Get Products"
Test-Endpoint -Method "GET" -Endpoint "/api/categories" -Description "Get Categories"
Write-Host ""

# Admin Dashboard
Write-Host "=== ADMIN DASHBOARD ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/api/admin/dashboard/stats" -Description "Dashboard Stats" -RequiresAuth $true
Test-Endpoint -Method "GET" -Endpoint "/api/admin/dashboard/sales-chart" -Description "Sales Chart" -RequiresAuth $true
Test-Endpoint -Method "GET" -Endpoint "/api/admin/dashboard/orders-status" -Description "Orders Status" -RequiresAuth $true
Write-Host ""

# Admin Products
Write-Host "=== ADMIN PRODUCTS ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/api/admin/products" -Description "List Products" -RequiresAuth $true
Write-Host ""

# Admin Categories
Write-Host "=== ADMIN CATEGORIES ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/api/admin/categories" -Description "List Categories" -RequiresAuth $true
Test-Endpoint -Method "GET" -Endpoint "/api/admin/categories/simple" -Description "Simple Categories" -RequiresAuth $true
Write-Host ""

# Admin Users
Write-Host "=== ADMIN USERS ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/api/admin/users" -Description "List Users" -RequiresAuth $true
Write-Host ""

# Admin Orders
Write-Host "=== ADMIN ORDERS ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Endpoint "/api/admin/orders" -Description "List Orders" -RequiresAuth $true
Test-Endpoint -Method "GET" -Endpoint "/api/admin/orders/recent" -Description "Recent Orders" -RequiresAuth $true
Write-Host ""

# Results
Write-Host "============================================" -ForegroundColor Cyan
if ($allPassed) {
    Write-Host "✅ TODOS LOS ENDPOINTS ESTÁN DISPONIBLES" -ForegroundColor Green
} else {
    Write-Host "❌ ALGUNOS ENDPOINTS NO ESTÁN DISPONIBLES" -ForegroundColor Red
    Write-Host ""
    Write-Host "Revisa:" -ForegroundColor Yellow
    Write-Host "1. Que el backend esté corriendo (dotnet run)" -ForegroundColor Yellow
    Write-Host "2. Que la URL sea: https://localhost:5006" -ForegroundColor Yellow
    Write-Host "3. Que los controladores estén registrados" -ForegroundColor Yellow
}
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Información adicional
Write-Host "📝 NOTAS:" -ForegroundColor Yellow
Write-Host "- Los endpoints con 401 son normales (requieren autenticación)" -ForegroundColor Gray
Write-Host "- Los endpoints con 404 indican que la ruta no existe" -ForegroundColor Gray
Write-Host "- Para probar con autenticación, usa Swagger:" -ForegroundColor Gray
Write-Host "  https://localhost:5006/swagger" -ForegroundColor Cyan
Write-Host ""
