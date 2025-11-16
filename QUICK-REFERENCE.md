# 🎯 BOSKO QUICK REFERENCE - CHEAT SHEET

**Guía rápida para desarrollo diario**

---

## 🚀 COMANDOS FRECUENTES

### Entity Framework Core
```bash
# Crear migración
dotnet ef migrations add NombreDescriptivo

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef migrations remove

# Ver estado de migraciones
dotnet ef migrations list

# Generar script SQL
dotnet ef migrations script
```

### Ejecutar Proyecto
```bash
# Modo desarrollo
dotnet run

# Modo watch (auto-reload)
dotnet watch run

# Compilar
dotnet build

# Limpiar
dotnet clean

# Restaurar paquetes
dotnet restore
```

### Testing (cuando esté implementado)
```bash
# Ejecutar todos los tests
dotnet test

# Con coverage
dotnet test /p:CollectCoverage=true

# Tests específicos
dotnet test --filter "FullyQualifiedName~ProductService"
```

---

## 📝 TEMPLATES DE CÓDIGO

### Controller Básico
```csharp
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(AppDbContext context, ILogger<ItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ItemDto>>> GetAll()
    {
        try
        {
            var items = await _context.Items
                .Select(i => new ItemDto { /* ... */ })
                .ToListAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items");
            return StatusCode(500, new { message = "Error interno" });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ItemDto>> GetById(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
            return NotFound(new { message = "Item no encontrado" });
            
        return Ok(new ItemDto { /* ... */ });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ItemDto>> Create(ItemCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var item = new Item { /* ... */ };
        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), 
            new { id = item.Id }, 
            new ItemDto { /* ... */ });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, ItemUpdateDto dto)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
            return NotFound();

        // Actualizar propiedades
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
            return NotFound();

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

### Model con Validaciones
```csharp
public class Item
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Relación con otra entidad
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
```

### DTO Simple
```csharp
// Response DTO
public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

// Create DTO
public class ItemCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(200, ErrorMessage = "Máximo 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "Precio inválido")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock no puede ser negativo")]
    public int Stock { get; set; }

    public int CategoryId { get; set; }
}

// Update DTO
public class ItemUpdateDto
{
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [Range(0.01, 999999.99)]
    public decimal? Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }
}
```

### Service Básico
```csharp
public interface IItemService
{
    Task<List<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ItemDto> CreateAsync(ItemCreateDto dto);
    Task<bool> UpdateAsync(int id, ItemUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

public class ItemService : IItemService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ItemService> _logger;

    public ItemService(AppDbContext context, ILogger<ItemService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ItemDto>> GetAllAsync()
    {
        return await _context.Items
            .Include(i => i.Category)
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                Stock = i.Stock,
                CategoryName = i.Category!.Name
            })
            .ToListAsync();
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _context.Items
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
            return null;

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Stock = item.Stock,
            CategoryName = item.Category!.Name
        };
    }

    public async Task<ItemDto> CreateAsync(ItemCreateDto dto)
    {
        var item = new Item
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Stock = item.Stock
        };
    }

    // Implementar otros métodos...
}
```

---

## 🔐 SEGURIDAD SNIPPETS

### Hashear Password
```csharp
var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
```

### Verificar Password
```csharp
bool isValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);
```

### Generar JWT
```csharp
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
    new Claim(JwtRegisteredClaimNames.Name, userName),
    new Claim(JwtRegisteredClaimNames.Email, email),
    new Claim("role", role)
};

var token = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
    signingCredentials: credentials
);

return new JwtSecurityTokenHandler().WriteToken(token);
```

### Obtener Usuario del Token
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var email = User.FindFirst(ClaimTypes.Email)?.Value;
var role = User.FindFirst("role")?.Value;
```

---

## 🗄️ SQL QUERIES ÚTILES

### Ver Usuarios
```sql
SELECT Id, Name, Email, Role, Provider, IsActive 
FROM Users 
ORDER BY Role;
```

### Cambiar Rol de Usuario
```sql
UPDATE Users 
SET Role = 'Admin', UpdatedAt = GETUTCDATE()
WHERE Email = 'usuario@email.com';
```

### Ver Productos con Categoría
```sql
SELECT p.Id, p.Name, p.Price, p.Stock, c.Name as CategoryName
FROM Products p
LEFT JOIN Categories c ON p.CategoryId = c.Id
ORDER BY p.CreatedAt DESC;
```

### Resetear Password de Usuario
```sql
-- Primero registra un usuario nuevo con el password deseado
-- Luego copia el PasswordHash generado
UPDATE Users 
SET PasswordHash = 'HASH_AQUI'
WHERE Email = 'usuario@email.com';
```

### Ver Estadísticas
```sql
-- Productos por categoría
SELECT c.Name, COUNT(p.Id) as ProductCount
FROM Categories c
LEFT JOIN Products p ON c.Id = p.CategoryId
GROUP BY c.Name;

-- Usuarios por rol
SELECT Role, COUNT(*) as Count
FROM Users
GROUP BY Role;

-- Stock bajo
SELECT Name, Stock
FROM Products
WHERE Stock < 10
ORDER BY Stock;
```

---

## 🎨 CONVENCIONES RÁPIDAS

### Naming
```
Controllers:     PascalCase + "Controller"     → ProductsController
Models:          PascalCase singular           → Product
DTOs:            PascalCase + suffix           → ProductCreateDto
Services:        PascalCase + "Service"        → ProductService
Interfaces:      "I" + PascalCase              → IProductService
Private fields:  _camelCase                    → _context
Parameters:      camelCase                     → productId
Local vars:      camelCase                     → totalPrice
```

### Archivos
```
Controllers/     Plural                        → ProductsController.cs
Models/          Singular                      → Product.cs
DTOs/            Singular                      → ProductDto.cs
Services/        Singular                      → ProductService.cs
```

### Rutas API
```
GET    /api/products           → Obtener todos
GET    /api/products/{id}      → Obtener uno
POST   /api/products           → Crear
PUT    /api/products/{id}      → Actualizar
DELETE /api/products/{id}      → Eliminar
GET    /api/products/search    → Búsqueda custom
```

---

## 🔧 TROUBLESHOOTING RÁPIDO

### Error: "The ConnectionString property has not been initialized"
```
✅ Verificar appsettings.json tiene ConnectionStrings
✅ Verificar SQL Server está corriendo
✅ Verificar nombre de base de datos
```

### Error: "Cannot insert explicit value for identity column"
```
✅ No asignar manualmente el Id
✅ EF Core lo maneja automáticamente
```

### Error: 401 Unauthorized en endpoint protegido
```
✅ Incluir header: Authorization: Bearer TOKEN
✅ Verificar token no expiró
✅ Verificar [Authorize] está en el endpoint
```

### Error: 403 Forbidden
```
✅ Verificar usuario tiene el rol correcto
✅ Verificar [Authorize(Roles = "...")] coincide con claim
```

### Error: "A possible object cycle was detected"
```
✅ Usar DTOs en lugar de retornar entidades directamente
✅ O configurar JSON options para ignorar ciclos
```

---

## 📊 DATOS DE PRUEBA

### Usuarios por Defecto
```
Admin:
  Email: admin@bosko.com
  Password: Bosko123!
  
Employee:
  Email: employee@bosko.com
  Password: Bosko123!
  
Customer:
  Email: customer@bosko.com
  Password: Bosko123!
```

### Categorías de Ejemplo
```
1. Camisetas
2. Pantalones
3. Zapatos
4. Accesorios
```

### Productos de Ejemplo
```
Camiseta Básica - 29.99€ - Stock: 50
Jeans Clásicos - 59.99€ - Stock: 30
Zapatillas Sport - 89.99€ - Stock: 25
```

---

## 🚦 STATUS CODES

```
200 OK              → GET exitoso
201 Created         → POST exitoso (incluir Location header)
204 No Content      → PUT/DELETE exitoso
400 Bad Request     → Validación falló
401 Unauthorized    → No autenticado (sin token o token inválido)
403 Forbidden       → Autenticado pero sin permisos
404 Not Found       → Recurso no existe
500 Internal Error  → Error en el servidor
```

---

## 🔍 DEBUGGING TIPS

### Ver Query SQL Generado por EF
```csharp
var query = _context.Products
    .Include(p => p.Category)
    .Where(p => p.Price > 50);
    
var sql = query.ToQueryString();
_logger.LogInformation(sql);
```

### Log de Request/Response
```csharp
_logger.LogInformation($"Processing request: {HttpContext.Request.Method} {HttpContext.Request.Path}");
```

### Verificar Claims del Usuario
```csharp
[HttpGet("whoami")]
[Authorize]
public IActionResult WhoAmI()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value });
    return Ok(claims);
}
```

---

## 📚 RECURSOS ÚTILES

### URLs Locales
```
Backend:       https://localhost:5006
Swagger:       https://localhost:5006/swagger
Frontend:      http://localhost:4200
```

### Documentación
```
Proyecto:      README.md
API Ejemplos:  API-EXAMPLES-AUTHENTICATION.md
Testing:       TESTING-GUIDE.md
Guidelines:    BOSKO-PROJECT-GUIDELINES.md
```

### Paquetes NuGet Instalados
```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.AspNetCore.Authentication.JwtBearer
BCrypt.Net-Next
Swashbuckle.AspNetCore
```

---

## ⚡ GIT WORKFLOW RÁPIDO

```bash
# 1. Crear rama feature
git checkout -b feature/add-orders

# 2. Hacer cambios y commit
git add .
git commit -m "feat: add orders module"

# 3. Push y crear PR
git push origin feature/add-orders

# 4. Después de merge, actualizar main
git checkout main
git pull origin main

# 5. Limpiar rama local
git branch -d feature/add-orders
```

---

## 🎯 CHECKLIST ANTES DE COMMIT

```
□ Código compila sin errores
□ No hay warnings críticos
□ Tests pasan (cuando estén implementados)
□ Comentarios innecesarios removidos
□ Console.WriteLine/Debug removidos
□ Sigue naming conventions
□ DTOs para requests/responses
□ Validaciones implementadas
□ Try-catch con logging
□ [Authorize] en endpoints sensibles
```

---

## 🔑 VARIABLES DE ENTORNO IMPORTANTES

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BoskoDB;..."
  },
  "JwtSettings": {
    "SecretKey": "MinLength32Chars...",
    "Issuer": "BoskoAPI",
    "Audience": "BoskoFrontend",
    "ExpirationMinutes": 1440
  },
  "AllowedHosts": "*"
}
```

---

**💡 TIP:** Guarda este archivo en favoritos para referencia rápida durante el desarrollo diario.

---

**Última actualización:** 16 de Noviembre 2025  
**Versión:** 1.0
