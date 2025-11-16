# ⚡ INICIO ULTRA RÁPIDO - 3 COMANDOS

## 🚀 PARA EMPEZAR AHORA (30 segundos)

```bash
# 1. Confiar en certificados (solo primera vez)
dotnet dev-certs https --trust

# 2. Iniciar backend
dotnet run

# 3. Abrir Swagger
start https://localhost:5006/swagger
```

---

## ✅ SI VES ESTO, ESTÁ FUNCIONANDO:

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
✅ Swagger UI habilitado
✅ CORS configurado
✅ API LISTA - Esperando requests...
============================================
```

---

## 📋 URLs IMPORTANTES

| Qué | URL |
|-----|-----|
| Swagger | https://localhost:5006/swagger |
| Health Check | https://localhost:5006/health |
| Login | https://localhost:5006/api/auth/login |
| Orders | https://localhost:5006/api/admin/orders |

---

## 🧪 TEST RÁPIDO (30 segundos)

```bash
# Test 1: Health check
curl https://localhost:5006/health -k

# Test 2: Login
curl -X POST https://localhost:5006/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@test.com\",\"password\":\"Admin123!\"}" \
  -k

# Si ambos funcionan → ✅ Backend OK
```

---

## 🔗 CONECTAR ANGULAR

```typescript
// environment.ts
export const environment = {
  apiUrl: 'https://localhost:5006/api'
};
```

---

## 🆘 SI HAY PROBLEMAS

### Problema: "Certificate error"
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
dotnet run
```

### Problema: "Port already in use"
```powershell
# PowerShell (Admin)
netstat -ano | findstr :5006
taskkill /PID <PID> /F
dotnet run
```

### Problema: "dotnet not found"
Instalar .NET 8 SDK: https://dotnet.microsoft.com/download

---

## ✅ CHECKLIST

- [ ] .NET 8 SDK instalado
- [ ] `dotnet dev-certs https --trust` ejecutado
- [ ] `dotnet run` ejecutado
- [ ] Swagger abre en https://localhost:5006/swagger
- [ ] Health check funciona
- [ ] ¡Listo! 🎉

---

## 📚 MÁS INFO

- **Guía Completa:** `BACKEND-FINAL-SUMMARY.md`
- **Comandos PowerShell:** `POWERSHELL-COMMANDS.md`
- **Quick Start:** `QUICK-START.md`

---

**Tiempo total:** ~2 minutos  
**Estado:** ✅ Backend funcional  
**Próximo paso:** Conectar con Angular

**¡Tu backend está listo!** 🚀
