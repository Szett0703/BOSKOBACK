# 🚀 CONFIGURACIÓN DE RAILWAY PARA BOSKO E-COMMERCE

## ❌ PROBLEMA ACTUAL
La aplicación está usando la conexión local (`localhost:5432`) porque Railway no tiene configurada la variable de entorno `ConnectionStrings__DefaultConnection`.

## ✅ SOLUCIÓN: CONFIGURAR VARIABLE DE ENTORNO EN RAILWAY

### PASO 1: Obtener la URL de PostgreSQL de Railway
1. Ve a tu proyecto en Railway
2. Ve a la sección "Database" → PostgreSQL
3. Copia la "Connection URL" (debe verse así):
   ```
   postgresql://postgres:tu_password@containers-us-west-1.railway.app:5432/railway
   ```

### PASO 2: Configurar Variable de Entorno
1. En Railway, ve a tu servicio (no la DB, sino el servicio de la app)
2. Ve a "Variables" → "Add Variable"
3. Nombre: `ConnectionStrings__DefaultConnection`
4. Valor: pega la URL completa de PostgreSQL
5. Haz clic en "Add"

### PASO 3: Redeploy Automático
Después de agregar la variable, Railway redeploy automáticamente, o puedes forzar un redeploy:
```bash
git commit --allow-empty -m "Trigger Railway redeploy"
git push origin main
```

### PASO 4: Verificar Logs
En los nuevos logs deberías ver:
```
🔌 Using RAILWAY Database Connection (double __)
[MIGRATIONS] ⏳ Applying pending migrations...
[MIGRATIONS] ✅ Migrations applied successfully!
[SEED] 🚀 Executing DatabaseSeeder...
[SEED] ✅ Database seeded successfully!
```

## 🔍 VERIFICACIÓN ADICIONAL

### Si aún no funciona:
1. Verifica que la variable esté en el servicio de la APP, no en la DB
2. Asegúrate de que la URL de PostgreSQL sea correcta
3. Revisa que no haya espacios extra en la variable

### Para probar localmente:
Si quieres probar con Railway DB desde local:
```bash
# En tu terminal local
export ConnectionStrings__DefaultConnection="postgresql://tu_url_de_railway"
dotnet run
```

## 📋 EJEMPLO DE CONFIGURACIÓN

Variable en Railway:
```
ConnectionStrings__DefaultConnection=postgresql://postgres:abcd1234@containers-us-west-1.railway.app:5432/railway
```

## ⚠️ NOTAS IMPORTANTES

- La variable debe estar en el **servicio de la aplicación**, no en la base de datos
- Railway redeploy automáticamente cuando cambias variables
- Si usas Railway CLI: `railway variables set ConnectionStrings__DefaultConnection="tu_url"`

¿Necesitas ayuda con algún paso específico?
