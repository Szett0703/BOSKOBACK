# 🔴 PROBLEMA ENCONTRADO - ACCIÓN INMEDIATA

## ❌ CAUSA DEL ERROR 500

Tu base de datos tiene **estructura incorrecta**.

**El backend busca:**
- `Orders.CustomerId` → Tu BD tiene `Orders.UserId` ❌
- `Orders.Total` → Tu BD tiene `Orders.TotalAmount` ❌
- `Orders.CustomerName` → Tu BD NO tiene esta columna ❌
- `OrderItems.Price` → Tu BD tiene `OrderItems.UnitPrice` ❌

**Por eso da error 500** cuando intenta leer pedidos.

---

## ✅ SOLUCIÓN (5 minutos)

### Ejecuta este script en SSMS:
```
Database/FIX-DATABASE-SCHEMA.sql
```

**El script hará:**
1. Respaldar datos existentes
2. Eliminar tablas incorrectas
3. Crear tablas correctas
4. Migrar datos
5. Insertar datos de prueba

---

## 📝 PASOS

```bash
# 1. Abrir SSMS
# 2. Conectar a localhost
# 3. Abrir: Database/FIX-DATABASE-SCHEMA.sql
# 4. Ejecutar (F5)
# 5. Esperar mensaje: "CORRECCIÓN COMPLETADA"
# 6. Reiniciar backend: dotnet run
# 7. Probar: https://localhost:5006/swagger
# 8. GET /api/admin/orders → Debe funcionar ✅
```

---

## ⏱️ TIEMPO: 5 minutos

## 📊 RESULTADO: Error 500 desaparecerá

---

**Lee más:** `DATABASE-SCHEMA-PROBLEM.md`

**Ejecuta:** `Database/FIX-DATABASE-SCHEMA.sql`
