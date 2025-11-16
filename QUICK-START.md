# 🚀 INICIO RÁPIDO - BACKEND REPARADO

## ⚡ COMANDOS RÁPIDOS (5 minutos)

### 1️⃣ CONFIAR EN CERTIFICADOS HTTPS (Solo primera vez)
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```
**Click "Sí" cuando aparezca el popup de Windows**

---

### 2️⃣ INICIAR BACKEND
```bash
# Navegar al directorio del proyecto
cd C:\Users\santi.SZETT\Desktop\Dev\DBTestBack\DBTest-BACK

# Ejecutar
dotnet run
```

---

### 3️⃣ VERIFICAR QUE FUNCIONA

#### A) En el navegador:
- Abre: `https://localhost:5006/swagger`
- Debe cargar Swagger UI ✅

#### B) Test rápido con cURL:
```bash
curl https://localhost:5006/health -k
```

**Respuesta esperada:**
```json
{
  "status": "healthy",
  "timestamp": "2025-11-16T...",
  "environment": "Development"
}
```

---

## 📊 SALIDA ESPERADA AL INICIAR

```
============================================
🚀 BOSKO E-COMMERCE API
============================================
Environment: Development
HTTPS: https://localhost:5006
HTTP:  http://localhost:5005
Swagger: https://localhost:5006/swagger
============================================

✅ HTTP Server listening on: http://localhost:5005
✅ HTTPS Server listening on: https://localhost:5006
✅ Swagger UI habilitado en: https://localhost:5006/swagger
✅ CORS configurado para: http://localhost:4200, http://localhost:4300

============================================
✅ API LISTA - Esperando requests...
============================================
```

---

## 🔗 URLs IMPORTANTES

| Descripción | URL |
|------------|-----|
| Swagger UI | https://localhost:5006/swagger |
| Health Check | https://localhost:5006/health |
| API Root | https://localhost:5006/ |
| Login | https://localhost:5006/api/auth/login |
| Orders | https://localhost:5006/api/admin/orders |

---

## 🧪 TEST RÁPIDO DE ENDPOINTS

### 1. Health Check (sin auth)
```bash
curl https://localhost:5006/health -k
```

### 2. Login
```bash
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@test.com\",\"password\":\"Admin123!\"}" \
  -k
```

### 3. Orders (con token)
```bash
TOKEN="tu_token_aqui"
curl -X GET "https://localhost:5006/api/admin/orders?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN" \
  -k
```

---

## 🔧 CONECTAR CON ANGULAR

### En tu servicio Angular:
```typescript
// environment.ts
export const environment = {
  apiUrl: 'https://localhost:5006/api'
};

// order-admin.service.ts
import { environment } from '../../environments/environment';

private apiUrl = `${environment.apiUrl}/admin/orders`;
```

### Probar conexión:
```typescript
// En consola del navegador (F12)
fetch('https://localhost:5006/health')
  .then(res => res.json())
  .then(data => console.log('✅ Conectado:', data));
```

---

## 🆘 PROBLEMAS COMUNES

### Error: "Certificate not trusted"
```bash
dotnet dev-certs https --trust
```

### Error: "Port already in use"
```powershell
# Como Admin en PowerShell
netstat -ano | findstr :5006
taskkill /PID <PID> /F
```

### Error: "Connection refused"
```bash
# Verificar que el backend esté corriendo
dotnet run

# Ver logs en consola
```

---

## ✅ CHECKLIST

- [ ] Certificados HTTPS confiables
- [ ] Backend iniciado con `dotnet run`
- [ ] Swagger abre en https://localhost:5006/swagger
- [ ] Health check responde
- [ ] Login funciona
- [ ] Frontend conectado

---

## 📞 AYUDA ADICIONAL

**Documentación completa:** `BACKEND-REPAIR-COMPLETE-REPORT.md`

**Tiempo total:** ~5 minutos

**Estado:** ✅ Backend 100% funcional

---

**¡Listo para conectar con tu frontend Angular!** 🚀
