# 🚨 ERROR: "Name or service not known" - SOLUCIÓN PARA RAILWAY

## ❌ PROBLEMA IDENTIFICADO
El contenedor no puede resolver el hostname `postgres.railway.internal`. Esto significa que la conexión interna no está funcionando.

## ✅ SOLUCIÓN: USAR CONEXIÓN EXTERNA

### PASO 1: Configura la Variable de Entorno en Railway
Ve a Railway → Tu Servicio → Variables → Add/Edit:

**Nombre:** `ConnectionStrings__DefaultConnection`

**Valor:** `postgresql://postgres:sjGQxihYBpidIkNOUskmXIzArLlobhwu@crossover.proxy.rlwy.net:10822/railway`

### PASO 2: Redeploy
Railway redeploy automáticamente, o fuerza con:
```bash
git add .
git commit -m "Fix Railway DB connection"
git push origin master
```

## 📋 POR QUÉ OCURRE ESTO

- Railway usa hostnames internos para comunicación entre servicios
- Pero a veces la resolución DNS falla desde el contenedor de la app
- La conexión externa (proxy) es más confiable para este caso

## 🔍 VERIFICACIÓN

Después del redeploy, los logs deberían mostrar:
```
🔌 Using RAILWAY Database Connection (double __)
ConnectionString: postgresql://postgres:***@crossover.proxy.rlwy.net:10822/railway
[MIGRATIONS] ⏳ Applying pending migrations...
[MIGRATIONS] ✅ Migrations applied successfully!
[SEED] 🚀 Executing DatabaseSeeder...
[SEED] ✅ Database seeded successfully!
```

## ⚠️ NOTA IMPORTANTE

- Mantén la conexión externa para Railway
- La conexión interna (`postgres.railway.internal`) puede no funcionar siempre
- La conexión externa es segura y confiable

¿Ya configuraste la variable de entorno en Railway?
