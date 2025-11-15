# Generación de Hashes BCrypt para Contraseñas

Este documento explica cómo se generaron los hashes de contraseñas para el script SeedData.sql.

## Contraseñas de Prueba

### Admin
- **Email**: admin@bosko.com
- **Contraseña**: Admin123
- **Hash BCrypt**: $2a$11$xvqZr7Z8LQXGxKzN4YhPa.YqE5bZB8HFqL5o4VHxFLvJxYqXqK7oi

### Otros Usuarios
- **Email**: employee@bosko.com, customer@bosko.com, pedro@example.com
- **Contraseña**: Password123
- **Hash BCrypt**: $2a$11$Qy4VZ8L5v0qL8Y9Q0gqXN.E6H4v7k8Lz0VYqXqK7oiWqXqK7oiWqX

## Cómo Generar Nuevos Hashes

Si necesitas cambiar las contraseñas o crear usuarios adicionales, puedes generar nuevos hashes de las siguientes formas:

### Opción 1: Usar la API de Registro
La forma más sencilla es usar el endpoint de registro que ya genera los hashes automáticamente:

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Nuevo Usuario",
    "email": "nuevo@example.com",
    "password": "MiPassword123"
  }'
```

Luego puedes consultar la base de datos para obtener el hash generado:
```sql
SELECT PasswordHash FROM Users WHERE Email = 'nuevo@example.com';
```

### Opción 2: Programa en C# (Consola)
Crea un pequeño programa de consola:

```csharp
using BCrypt.Net;

class Program
{
    static void Main(string[] args)
    {
        string password = "Admin123";
        string hash = BCrypt.HashPassword(password);
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
    }
}
```

Instala el paquete:
```bash
dotnet add package BCrypt.Net-Next
```

Ejecuta:
```bash
dotnet run
```

### Opción 3: Herramienta Online (solo para desarrollo)
?? **NO usar para contraseñas de producción**

Puedes usar herramientas online como:
- https://bcrypt-generator.com/
- https://www.browserling.com/tools/bcrypt

Configuración:
- Rounds: 11 (recomendado para balance seguridad/performance)

## Seguridad

### Para Desarrollo
- Los hashes en SeedData.sql son para entorno de desarrollo/pruebas únicamente
- Las contraseñas son conocidas públicamente (Admin123, Password123)

### Para Producción
1. **Nunca** uses contraseñas predecibles como "Admin123"
2. **Nunca** commits hashes de contraseñas reales al control de versiones
3. El primer admin debe crearse manualmente con contraseña segura
4. Usa variables de entorno o Azure Key Vault para secretos
5. Implementa políticas de contraseñas fuertes:
   - Mínimo 8 caracteres
   - Combinación de mayúsculas, minúsculas, números y símbolos
   - Validación en el backend

## Verificación de Contraseñas

BCrypt incluye el salt en el hash, por lo que la verificación es sencilla:

```csharp
bool isValid = BCrypt.Verify("Admin123", "$2a$11$xvqZr7Z8...");
```

La API ya implementa esta verificación en el endpoint de login.

## Cambio de Contraseñas

Los usuarios pueden cambiar sus contraseñas usando:
- `/api/auth/change-password` (estando autenticados)
- `/api/auth/forgot-password` + `/api/auth/reset-password` (si olvidaron)

Los administradores pueden crear usuarios con contraseñas temporales usando `/api/users`.
