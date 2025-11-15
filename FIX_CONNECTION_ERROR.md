# ?? SOLUCIÓN AL ERROR DE CONEXIÓN

## ? Error Actual
```
GET http://localhost:5000/api/categories net::ERR_CONNECTION_REFUSED
```

## ? Solución Implementada

He cambiado la configuración del backend para que corra en el puerto **5000** (HTTP) y **5001** (HTTPS), que es lo que espera tu frontend Angular.

---

## ?? Pasos para Arrancar el Backend

### 1?? Aplicar Migraciones (Solo la primera vez)
```bash
cd C:\Users\santi.SZETT\Desktop\Dev\BOSKO\BOSKOBACK\BOSKOBACK
dotnet ef database update
```

### 2?? Poblar Datos de Prueba (Solo la primera vez)
```bash
sqlcmd -S LOCALHOST\SQLEXPRESS -d BOSKO -i SeedData.sql
```

### 3?? Iniciar el Backend
```bash
dotnet run
```

**El backend ahora estará en:**
- ? HTTP: `http://localhost:5000`
- ? HTTPS: `https://localhost:5001`
- ? Swagger: `http://localhost:5000/swagger`

---

## ?? Verificar que el Backend Está Corriendo

### Opción 1: Abrir Swagger
```
http://localhost:5000/swagger
```
Deberías ver la interfaz de Swagger con todos los endpoints.

### Opción 2: Probar el Endpoint de Categorías
```bash
curl http://localhost:5000/api/categories
```

O abre en el navegador:
```
http://localhost:5000/api/categories
```

---

## ?? Configuración de CORS

El backend está configurado para aceptar peticiones desde:
```
http://localhost:4200
```

Si tu frontend Angular corre en otro puerto, necesitarás actualizar el CORS en `Program.cs`.

---

## ?? Checklist de Inicio

- [ ] **Backend corriendo**: `dotnet run` ejecutado
- [ ] **Puerto correcto**: Backend en `http://localhost:5000`
- [ ] **Base de datos**: Migraciones aplicadas
- [ ] **Datos de prueba**: SeedData.sql ejecutado
- [ ] **Swagger funciona**: `http://localhost:5000/swagger` abre
- [ ] **Frontend conecta**: Angular puede hacer GET a `/api/categories`

---

## ?? Troubleshooting

### Error: "Cannot connect to database"
```bash
# Verificar que SQL Server está corriendo
# Verificar connection string en appsettings.json
```

### Error: "Port already in use"
```bash
# Detener cualquier proceso en el puerto 5000
# Windows:
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Error: "CORS policy error"
```
# Verificar que el frontend corre en http://localhost:4200
# Si usa otro puerto, actualizar Program.cs línea 52
```

---

## ?? Próximos Pasos

Una vez el backend esté corriendo en `http://localhost:5000`:

1. ? Refrescar tu aplicación Angular
2. ? El error `ERR_CONNECTION_REFUSED` debe desaparecer
3. ? Las categorías deberían cargarse correctamente

---

## ?? Comandos Útiles

### Ver puertos en uso
```bash
netstat -ano | findstr :5000
```

### Detener proceso en puerto
```bash
# Windows
taskkill /PID <PID> /F

# O reiniciar el proyecto en VS
```

### Verificar que API responde
```bash
curl http://localhost:5000/api/categories
curl http://localhost:5000/api/products
```

---

## ? Resumen

**Cambio realizado:**
- Puerto del backend cambiado de `5271` ? `5000`
- Ahora coincide con lo que espera tu frontend Angular

**Para arrancar:**
```bash
dotnet run
```

**Verificar:**
```
http://localhost:5000/swagger
http://localhost:5000/api/categories
```

---

*Archivo actualizado: Properties/launchSettings.json*
*Puerto HTTP: 5000*
*Puerto HTTPS: 5001*
