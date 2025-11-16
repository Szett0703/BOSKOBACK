# ?? GUÍA DE DESARROLLO Y MEJORES PRÁCTICAS - BOSKO E-COMMERCE

**Proyecto:** Bosko E-Commerce Backend  
**Framework:** .NET 8  
**Fecha:** 16 de Noviembre 2025  
**Estado:** Proyecto en Desarrollo Activo

---

## ?? TABLA DE CONTENIDO

1. [Arquitectura del Proyecto](#arquitectura-del-proyecto)
2. [Convenciones de Código](#convenciones-de-código)
3. [Estructura de Carpetas](#estructura-de-carpetas)
4. [Seguridad](#seguridad)
5. [Base de Datos](#base-de-datos)
6. [API REST](#api-rest)
7. [Autenticación y Autorización](#autenticación-y-autorización)
8. [Validaciones](#validaciones)
9. [Manejo de Errores](#manejo-de-errores)
10. [Testing](#testing)
11. [Documentación](#documentación)
12. [Performance](#performance)
13. [Code Smells a Evitar](#code-smells-a-evitar)
14. [Checklist de Pull Request](#checklist-de-pull-request)

---

## 1. ARQUITECTURA DEL PROYECTO

### ??? Patrón Arquitectónico Actual

**Patrón:** Clean Architecture simplificada con separación de responsabilidades

```
?? DBTest-BACK/
??? ?? Controllers/          ? Endpoints HTTP (capa de presentación)
??? ?? Models/               ? Entidades de dominio
??? ?? DTOs/                 ? Data Transfer Objects (contratos de API)
??? ?? Services/             ? Lógica de negocio
??? ?? Data/                 ? Contexto de base de datos
??? ?? Database/             ? Scripts SQL
??? ?? Migrations/           ? Migraciones de EF Core
??? ?? Program.cs            ? Configuración de la aplicación
```

### ? REGLAS DE ARQUITECTURA

#### ?? HACER:
```csharp
// ? Controllers delgados, solo coordinación
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateDto dto)
{
    var product = await _productService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}

// ? Lógica en Services
public class ProductService
{
    public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
    {
        // Validaciones de negocio
        // Transformaciones
        // Persistencia
    }
}
```

#### ? EVITAR:
```csharp
// ? Lógica de negocio en Controllers
[HttpPost]
public async Task<ActionResult> CreateProduct(ProductCreateDto dto)
{
    // ? NO hacer validaciones complejas aquí
    if (dto.Price < 0 || dto.Stock < 0 || dto.Name.Length > 200)
    {
        return BadRequest();
    }
    
    // ? NO hacer transformaciones aquí
    var product = new Product { /* ... */ };
    
    // ? NO acceder directamente al DbContext
    _context.Products.Add(product);
    await _context.SaveChangesAsync();
}
```

### ?? Capas y Responsabilidades

| Capa | Responsabilidad | Ejemplo |
|------|----------------|---------|
| **Controllers** | Recibir requests, validar DTOs, devolver responses | `ProductsController.cs` |
| **Services** | Lógica de negocio, orquestación | `AuthService.cs` |
| **Models** | Entidades de dominio, reglas de negocio | `Product.cs`, `User.cs` |
| **DTOs** | Contratos de API, validaciones de entrada | `ProductCreateDto.cs` |
| **Data** | Acceso a datos, configuración EF Core | `AppDbContext.cs` |

---

## 2. CONVENCIONES DE CÓDIGO

### ?? Naming Conventions

#### ? Archivos y Clases

```csharp
// ? CORRECTO
ProductsController.cs       // Controllers en plural
AuthController.cs           // Auth es singular (no tiene plural natural)
ProductService.cs           // Services en singular
ProductDto.cs              // DTOs en singular
ProductCreateDto.cs        // DTOs con sufijo descriptivo
IProductService.cs         // Interfaces con prefijo I
```

#### ? Variables y Propiedades

```csharp
// ? PascalCase para propiedades públicas
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// ? camelCase para variables locales y parámetros
public async Task<Product> GetProductAsync(int productId)
{
    var product = await _context.Products.FindAsync(productId);
    var productDto = MapToDto(product);
    return productDto;
}

// ? _camelCase para campos privados
private readonly AppDbContext _context;
private readonly ILogger<ProductsController> _logger;
```

#### ? Métodos

```csharp
// ? Verbos descriptivos
public async Task<Product> GetProductByIdAsync(int id)
public async Task<List<Product>> GetAllProductsAsync()
public async Task<Product> CreateProductAsync(ProductCreateDto dto)
public async Task UpdateProductAsync(int id, ProductUpdateDto dto)
public async Task DeleteProductAsync(int id)
public bool IsProductAvailable(Product product)
public decimal CalculateDiscount(Product product)
```

### ?? Comentarios y Documentación

```csharp
// ? XML Documentation para APIs públicas
/// <summary>
/// Obtiene un producto por su ID.
/// </summary>
/// <param name="id">El ID del producto.</param>
/// <returns>El producto encontrado o NotFound.</returns>
[HttpGet("{id}")]
public async Task<ActionResult<ProductDto>> GetProduct(int id)
{
    // ...
}

// ? Comentarios para lógica compleja
// Calculamos el descuento basado en el stock y la temporada
decimal discount = 0;
if (product.Stock > 100 && DateTime.Now.Month == 12)
{
    discount = 0.15m; // 15% de descuento en diciembre
}

// ? Evitar comentarios obvios
// ? Get the product
var product = await _context.Products.FindAsync(id);
```

---

## 3. ESTRUCTURA DE CARPETAS

### ?? Organización Actual vs Recomendada

#### ? ESTRUCTURA ACTUAL (BUENA)
```
DBTest-BACK/
??? Controllers/
??? Models/
??? DTOs/
??? Services/
??? Data/
??? Database/
??? Migrations/
```

#### ?? ESTRUCTURA RECOMENDADA PARA CRECIMIENTO
```
DBTest-BACK/
??? ?? Controllers/
?   ??? AuthController.cs
?   ??? ProductsController.cs
?   ??? CategoriesController.cs
?   ??? OrdersController.cs (futuro)
?
??? ?? Models/
?   ??? Entities/              # Entidades de dominio
?   ?   ??? User.cs
?   ?   ??? Product.cs
?   ?   ??? Category.cs
?   ?   ??? Order.cs (futuro)
?   ??? Enums/                 # Enumeraciones
?       ??? OrderStatus.cs
?
??? ?? DTOs/
?   ??? Auth/
?   ?   ??? LoginDto.cs
?   ?   ??? RegisterDto.cs
?   ?   ??? AuthResponseDto.cs
?   ??? Products/
?   ?   ??? ProductDto.cs
?   ?   ??? ProductCreateDto.cs
?   ?   ??? ProductUpdateDto.cs
?   ??? Common/
?       ??? PaginationDto.cs
?       ??? ApiResponseDto.cs
?
??? ?? Services/
?   ??? Interfaces/
?   ?   ??? IAuthService.cs
?   ?   ??? IProductService.cs
?   ?   ??? IEmailService.cs
?   ??? Implementations/
?       ??? AuthService.cs
?       ??? ProductService.cs
?       ??? EmailService.cs
?
??? ?? Data/
?   ??? AppDbContext.cs
?   ??? Repositories/          # Patrón Repository (futuro)
?   ?   ??? IProductRepository.cs
?   ?   ??? ProductRepository.cs
?   ??? Configurations/        # Configuraciones de EF Core
?       ??? ProductConfiguration.cs
?
??? ?? Middleware/             # Custom middleware
?   ??? ExceptionHandlingMiddleware.cs
?   ??? RequestLoggingMiddleware.cs
?
??? ?? Filters/                # Action Filters
?   ??? ValidateModelStateFilter.cs
?
??? ?? Extensions/             # Extension methods
?   ??? ServiceExtensions.cs
?   ??? ClaimsPrincipalExtensions.cs
?
??? ?? Validators/             # FluentValidation (futuro)
?   ??? ProductValidator.cs
?
??? ?? Helpers/                # Utilidades
?   ??? JwtHelper.cs
?   ??? PasswordHelper.cs
?
??? ?? Database/
?   ??? Scripts/
?       ??? BoskoDB-Setup.sql
?       ??? Users-Authentication-Setup.sql
?
??? ?? Migrations/
?
??? ?? Tests/                  # Tests unitarios (futuro)
?   ??? Controllers/
?   ??? Services/
?   ??? Helpers/
?
??? ?? Program.cs
```

---

## 4. SEGURIDAD

### ?? Reglas de Seguridad Críticas

#### ? PASSWORDS

```csharp
// ? SIEMPRE hashear passwords con BCrypt
public async Task<User> CreateUserAsync(RegisterDto dto)
{
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new User
    {
        Email = dto.Email,
        PasswordHash = passwordHash // ?
    };
}

// ? NUNCA guardar passwords en texto plano
var user = new User
{
    Email = dto.Email,
    Password = dto.Password // ? NUNCA
};
```

#### ? JWT TOKENS

```csharp
// ? Secret key largo y seguro
"JwtSettings": {
  "SecretKey": "BoskoECommerce_SuperSecretKey_2024_MinLength32Characters_ForSecurity!",
  "Issuer": "BoskoAPI",
  "Audience": "BoskoFrontend",
  "ExpirationMinutes": 1440
}

// ? Secret key débil
"SecretKey": "123456" // ? NUNCA
```

#### ? SQL INJECTION

```csharp
// ? Usar parámetros de EF Core (protege automáticamente)
var product = await _context.Products
    .FirstOrDefaultAsync(p => p.Id == id);

// ? Si usas SQL raw, SIEMPRE con parámetros
var product = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Id = {0}", id)
    .FirstOrDefaultAsync();

// ? NUNCA concatenar strings en SQL
var query = $"SELECT * FROM Products WHERE Name = '{name}'"; // ? VULNERABLE
```

#### ? AUTENTICACIÓN Y AUTORIZACIÓN

```csharp
// ? Proteger endpoints sensibles
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProduct(int id)
{
    // Solo Admin puede eliminar
}

// ? Permitir acceso público explícitamente
[AllowAnonymous]
[HttpGet]
public async Task<ActionResult<List<ProductDto>>> GetProducts()
{
    // Público
}

// ? Endpoint sensible sin protección
[HttpDelete("{id}")] // ? Falta [Authorize]
public async Task<IActionResult> DeleteProduct(int id)
```

#### ? INFORMACIÓN SENSIBLE

```csharp
// ? NUNCA exponer información sensible en logs
_logger.LogInformation($"Login attempt for email: {email}");

// ? NO loguear passwords o tokens
_logger.LogInformation($"Login: {email} with password: {password}"); // ?
_logger.LogInformation($"JWT token: {token}"); // ?

// ? Ocultar información sensible en respuestas de error
catch (Exception ex)
{
    _logger.LogError(ex, "Error creating product");
    return StatusCode(500, new { message = "Error interno del servidor" });
}

// ? NO exponer detalles técnicos al cliente
catch (Exception ex)
{
    return StatusCode(500, ex.Message); // ? Expone información sensible
}
```

### ?? Checklist de Seguridad

```
? Passwords hasheados con BCrypt
? JWT con secret key fuerte (>32 chars)
? HTTPS obligatorio en producción
? CORS configurado correctamente
? [Authorize] en endpoints protegidos
? Validación de inputs
? Sanitización de datos
? Rate limiting (futuro)
? Logging sin información sensible
? Connection strings en secrets (no en código)
```

---

## 5. BASE DE DATOS

### ??? Convenciones de Base de Datos

#### ? NOMBRES DE TABLAS

```sql
-- ? PascalCase, singular
CREATE TABLE User (...)
CREATE TABLE Product (...)
CREATE TABLE Category (...)
CREATE TABLE OrderItem (...)

-- ? Evitar
CREATE TABLE users (...)      -- ? minúsculas
CREATE TABLE Products (...)    -- ? plural confuso con entity
```

#### ? NOMBRES DE COLUMNAS

```sql
-- ? PascalCase
Id INT PRIMARY KEY
Name NVARCHAR(100)
CreatedAt DATETIME2
UpdatedAt DATETIME2

-- ? Evitar
id INT                 -- ? minúsculas
product_name NVARCHAR -- ? snake_case
```

#### ? CONSTRAINTS Y ÍNDICES

```sql
-- ? Nombres descriptivos
CONSTRAINT PK_Users_Id PRIMARY KEY (Id)
CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Employee', 'Customer'))
CONSTRAINT UQ_Users_Email UNIQUE (Email)

-- ? Índices con prefijo IX_
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Users_Email ON Users(Email);
```

#### ? TIPOS DE DATOS

```sql
-- ? Usar tipos apropiados
Id              INT IDENTITY(1,1)        -- IDs auto-incrementales
Name            NVARCHAR(100)            -- Texto corto
Description     NVARCHAR(1000)           -- Texto largo
Price           DECIMAL(10,2)            -- Precios con 2 decimales
Stock           INT                      -- Cantidades enteras
IsActive        BIT                      -- Booleanos
CreatedAt       DATETIME2                -- Fechas con precisión
Email           NVARCHAR(255)            -- Emails estándar
```

#### ? MIGRACIONES DE EF CORE

```bash
# ? Nombres descriptivos
dotnet ef migrations add AddUserAuthentication
dotnet ef migrations add AddProductCategories
dotnet ef migrations add AddOrderTracking

# ? Nombres genéricos
dotnet ef migrations add Update1  # ?
dotnet ef migrations add New      # ?
```

### ?? Configuración de Entidades

```csharp
// ? Configuración en OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ? Configurar relaciones
    modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Products)
        .HasForeignKey(p => p.CategoryId)
        .OnDelete(DeleteBehavior.SetNull);

    // ? Configurar índices
    modelBuilder.Entity<Product>()
        .HasIndex(p => p.CategoryId)
        .HasDatabaseName("IX_Products_CategoryId");

    // ? Configurar constraints
    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
}
```

---

## 6. API REST

### ?? Convenciones de Endpoints

#### ? RUTAS

```csharp
// ? Recursos en plural, acciones en verbos HTTP
[Route("api/[controller]")]
[HttpGet]                           // GET /api/products
[HttpGet("{id}")]                   // GET /api/products/5
[HttpPost]                          // POST /api/products
[HttpPut("{id}")]                   // PUT /api/products/5
[HttpDelete("{id}")]                // DELETE /api/products/5

// ? Sub-recursos
[HttpGet("{id}/reviews")]          // GET /api/products/5/reviews
[HttpGet("categories/{categoryId}")]// GET /api/products/categories/2

// ? Evitar verbos en rutas
[HttpGet("GetProduct/{id}")]       // ? Redundante
[HttpPost("CreateProduct")]         // ? Usar POST /products
```

#### ? HTTP STATUS CODES

```csharp
// ? GET exitoso
return Ok(product);                 // 200 OK

// ? POST exitoso
return CreatedAtAction(nameof(GetProduct), 
    new { id = product.Id }, product);  // 201 Created

// ? PUT exitoso
return NoContent();                 // 204 No Content

// ? DELETE exitoso
return NoContent();                 // 204 No Content

// ? Not Found
return NotFound(new { message = "Producto no encontrado" }); // 404

// ? Bad Request
return BadRequest(ModelState);      // 400

// ? Unauthorized
return Unauthorized(new { message = "..." }); // 401

// ? Forbidden
// El middleware de autorización retorna 403 automáticamente

// ? Internal Server Error
return StatusCode(500, new { message = "Error interno" }); // 500
```

#### ? PAGINACIÓN

```csharp
// ? Query parameters para paginación
[HttpGet]
public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? search = null)
{
    var products = await _productService.GetPagedAsync(page, pageSize, search);
    return Ok(products);
}

// ? Response con metadata de paginación
public class PagedResult<T>
{
    public List<T> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
```

#### ? FILTRADO Y BÚSQUEDA

```csharp
// ? Query parameters para filtros
[HttpGet]
public async Task<ActionResult<List<ProductDto>>> GetProducts(
    [FromQuery] int? categoryId = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null,
    [FromQuery] string? search = null)
{
    var query = _context.Products.AsQueryable();
    
    if (categoryId.HasValue)
        query = query.Where(p => p.CategoryId == categoryId);
        
    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice);
        
    // ...
}
```

---

## 7. AUTENTICACIÓN Y AUTORIZACIÓN

### ?? JWT Best Practices

#### ? CONFIGURACIÓN

```csharp
// ? En appsettings.json
{
  "JwtSettings": {
    "SecretKey": "MinLength32CharactersForSecurity!",
    "Issuer": "BoskoAPI",
    "Audience": "BoskoFrontend",
    "ExpirationMinutes": 1440  // 24 horas
  }
}

// ? En Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
```

#### ? CLAIMS

```csharp
// ? Claims estándar y custom
var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(JwtRegisteredClaimNames.Name, user.Name),
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim("role", user.Role),                    // Custom claim
    new Claim("provider", user.Provider),            // Custom claim
    new Claim(ClaimTypes.Role, user.Role),           // .NET standard
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};
```

#### ? AUTORIZACIÓN POR ROLES

```csharp
// ? En Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
        
    options.AddPolicy("AdminOrEmployee", policy => 
        policy.RequireRole("Admin", "Employee"));
});

// ? En Controllers
[Authorize(Roles = "Admin")]
[HttpPost]
public async Task<ActionResult> CreateProduct(...)

[Authorize(Roles = "Admin,Employee")]
[HttpGet("stats")]
public async Task<ActionResult> GetStats(...)

[Authorize]  // Cualquier usuario autenticado
[HttpGet("profile")]
public async Task<ActionResult> GetProfile(...)
```

---

## 8. VALIDACIONES

### ? VALIDACIONES EN DTOS

```csharp
// ? Data Annotations en DTOs
public class ProductCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(200, ErrorMessage = "Máximo 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "Precio debe estar entre 0.01 y 999999.99")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock no puede ser negativo")]
    public int Stock { get; set; }

    [Url(ErrorMessage = "URL inválida")]
    public string? Image { get; set; }
}
```

#### ? VALIDACIÓN MANUAL

```csharp
// ? En Controllers
[HttpPost]
public async Task<ActionResult> CreateProduct(ProductCreateDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // Validaciones adicionales de negocio
    if (await _context.Products.AnyAsync(p => p.Name == dto.Name))
    {
        return BadRequest(new { message = "Producto con ese nombre ya existe" });
    }
    
    // ...
}
```

#### ?? FUTURO: FluentValidation

```csharp
// ?? Recomendado para proyectos grandes
public class ProductValidator : AbstractValidator<ProductCreateDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("Máximo 200 caracteres");
            
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Precio debe ser mayor a 0");
            
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock no puede ser negativo");
    }
}
```

---

## 9. MANEJO DE ERRORES

### ?? Global Exception Handling

#### ? MIDDLEWARE RECOMENDADO (FUTURO)

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status404NotFound);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (UnauthorizedException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            message = ex.Message,
            statusCode = statusCode
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
}
```

#### ? EN CONTROLLERS (ACTUAL)

```csharp
// ? Try-catch con logging
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateDto dto)
{
    try
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning(ex, "Validation error creating product");
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating product");
        return StatusCode(500, new { message = "Error interno del servidor" });
    }
}
```

---

## 10. TESTING

### ?? Estructura de Tests (FUTURO)

```
Tests/
??? UnitTests/
?   ??? Services/
?   ?   ??? ProductServiceTests.cs
?   ?   ??? AuthServiceTests.cs
?   ??? Helpers/
?       ??? JwtHelperTests.cs
?
??? IntegrationTests/
?   ??? Controllers/
?   ?   ??? ProductsControllerTests.cs
?   ?   ??? AuthControllerTests.cs
?   ??? Database/
?       ??? AppDbContextTests.cs
?
??? E2ETests/
    ??? AuthenticationFlowTests.cs
```

#### ? UNIT TEST EJEMPLO

```csharp
public class ProductServiceTests
{
    [Fact]
    public async Task CreateProduct_ValidDto_ReturnsProduct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
            
        using var context = new AppDbContext(options);
        var service = new ProductService(context);
        
        var dto = new ProductCreateDto
        {
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10
        };
        
        // Act
        var result = await service.CreateAsync(dto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(99.99m, result.Price);
    }
}
```

---

## 11. DOCUMENTACIÓN

### ?? Swagger / OpenAPI

#### ? CONFIGURACIÓN

```csharp
// ? En Program.cs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bosko E-Commerce API",
        Version = "v1",
        Description = "API REST para el sistema de e-commerce Bosko",
        Contact = new OpenApiContact
        {
            Name = "Bosko Team",
            Email = "support@bosko.com"
        }
    });
    
    // Incluir XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```

#### ? XML COMMENTS

```csharp
/// <summary>
/// Obtiene todos los productos con filtros opcionales.
/// </summary>
/// <param name="categoryId">ID de categoría para filtrar (opcional).</param>
/// <param name="minPrice">Precio mínimo (opcional).</param>
/// <param name="maxPrice">Precio máximo (opcional).</param>
/// <returns>Lista de productos que cumplen los criterios.</returns>
/// <response code="200">Retorna la lista de productos.</response>
/// <response code="400">Si los parámetros son inválidos.</response>
[HttpGet]
[ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<List<ProductDto>>> GetProducts(
    [FromQuery] int? categoryId = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
{
    // ...
}
```

---

## 12. PERFORMANCE

### ? Optimizaciones de Base de Datos

#### ? ÍNDICES

```csharp
// ? Crear índices para consultas frecuentes
modelBuilder.Entity<Product>()
    .HasIndex(p => p.CategoryId);

modelBuilder.Entity<Product>()
    .HasIndex(p => p.Name);

modelBuilder.Entity<User>()
    .HasIndex(u => u.Email)
    .IsUnique();
```

#### ? EAGER LOADING vs LAZY LOADING

```csharp
// ? Usar .Include() para evitar N+1 queries
var products = await _context.Products
    .Include(p => p.Category)
    .ToListAsync();

// ? Lazy loading causa N+1 queries
var products = await _context.Products.ToListAsync();
foreach (var product in products)
{
    var category = product.Category; // ? Query por cada producto
}
```

#### ? PAGINACIÓN

```csharp
// ? Siempre paginar resultados grandes
var products = await _context.Products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// ? No traer todos los registros
var products = await _context.Products.ToListAsync(); // ? Si hay 10,000 productos
```

#### ? PROYECCIONES

```csharp
// ? Seleccionar solo campos necesarios
var products = await _context.Products
    .Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    })
    .ToListAsync();

// ? Traer entidades completas si no son necesarias
var products = await _context.Products.ToListAsync(); // ? Trae todos los campos
```

### ?? Caching (FUTURO)

```csharp
// ?? Memory Cache para datos frecuentes
public class ProductService
{
    private readonly IMemoryCache _cache;
    
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        const string cacheKey = "all_categories";
        
        if (!_cache.TryGetValue(cacheKey, out List<CategoryDto> categories))
        {
            categories = await _context.Categories
                .Select(c => new CategoryDto { ... })
                .ToListAsync();
                
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                
            _cache.Set(cacheKey, categories, cacheOptions);
        }
        
        return categories;
    }
}
```

---

## 13. CODE SMELLS A EVITAR

### ?? Anti-patrones Comunes

#### ? DIOS-CONTROLLER

```csharp
// ? Controller con demasiadas responsabilidades
public class ProductsController
{
    public async Task<ActionResult> GetProducts() { }
    public async Task<ActionResult> CreateProduct() { }
    public async Task<ActionResult> UpdateStock() { }
    public async Task<ActionResult> CalculateDiscount() { }
    public async Task<ActionResult> GenerateReport() { }
    public async Task<ActionResult> SendEmail() { }
    public async Task<ActionResult> ProcessPayment() { }
    // ? Demasiadas responsabilidades
}

// ? Separar en múltiples controllers/services
ProductsController
StockController
ReportsController
EmailService
PaymentService
```

#### ? MAGIC NUMBERS

```csharp
// ? Números mágicos
if (user.Role == 1) { }
if (order.Status == 3) { }
if (product.Stock < 5) { }

// ? Usar constantes o enums
public enum UserRole { Admin = 1, Employee = 2, Customer = 3 }
if (user.Role == UserRole.Admin) { }

public const int LowStockThreshold = 5;
if (product.Stock < LowStockThreshold) { }
```

#### ? DEMASIADOS PARÁMETROS

```csharp
// ? Método con muchos parámetros
public async Task<Product> CreateProduct(
    string name, 
    string description, 
    decimal price, 
    int stock, 
    int categoryId, 
    string image, 
    bool isActive,
    string brand,
    string sku)
{
    // ...
}

// ? Usar DTO
public async Task<Product> CreateProduct(ProductCreateDto dto)
{
    // ...
}
```

#### ? ANEMIC MODELS

```csharp
// ? Modelo sin comportamiento
public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
}

// ? Modelo con comportamiento
public class Order
{
    public int Id { get; set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    
    public void CalculateTotal(List<OrderItem> items)
    {
        Total = items.Sum(i => i.Price * i.Quantity);
    }
    
    public void MarkAsCompleted()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be completed");
            
        Status = OrderStatus.Completed;
    }
}
```

---

## 14. CHECKLIST DE PULL REQUEST

### ? Antes de Hacer Commit

```
CODE QUALITY:
? Código sigue las convenciones del proyecto
? No hay código comentado innecesario
? No hay console.log o print statements
? Variables tienen nombres descriptivos
? Métodos tienen responsabilidad única
? No hay duplicación de código

FUNCIONALIDAD:
? La funcionalidad cumple los requisitos
? Se probó en local y funciona
? No rompe funcionalidad existente
? Maneja casos edge correctamente

SEGURIDAD:
? No hay información sensible en el código
? Inputs están validados
? No hay vulnerabilidades SQL injection
? Passwords hasheados
? Endpoints protegidos con [Authorize]

BASE DE DATOS:
? Migraciones creadas si hay cambios en modelos
? Scripts SQL documentados
? Índices creados para consultas frecuentes

TESTING:
? Tests unitarios pasan (cuando aplique)
? Tests de integración pasan (cuando aplique)
? Probado manualmente

DOCUMENTACIÓN:
? README actualizado si es necesario
? Comentarios XML en APIs públicas
? Swagger/OpenAPI actualizado
? CHANGELOG actualizado
```

---

## ?? MÉTRICAS DE CALIDAD

### KPIs del Proyecto

```
? Code Coverage: >80% (objetivo futuro)
? Build Time: <30 segundos
? API Response Time: <200ms (promedio)
? Zero Critical Security Issues
? Technical Debt: <10% del código total
```

---

## ?? PROCESO DE DESARROLLO

### Git Workflow

```bash
# 1. Crear rama feature
git checkout -b feature/add-orders-module

# 2. Hacer cambios y commits descriptivos
git add .
git commit -m "feat: add Order entity and DTOs"
git commit -m "feat: implement OrdersController with CRUD"
git commit -m "test: add unit tests for OrderService"

# 3. Hacer push y crear PR
git push origin feature/add-orders-module

# 4. Code review
# 5. Merge a main después de aprobación
```

### Commit Messages

```
? BUENOS:
feat: add user authentication with JWT
fix: resolve null reference in ProductsController
refactor: extract email logic to EmailService
docs: update API documentation
test: add unit tests for AuthService

? MALOS:
update
fix bug
changes
wip
asdf
```

---

## ?? ROADMAP DE MEJORAS

### Prioridad Alta (Próximos 3 meses)

1. **Testing**
   - Implementar xUnit
   - Unit tests para Services
   - Integration tests para Controllers
   - Test Coverage >70%

2. **Logging**
   - Implementar Serilog
   - Structured logging
   - Log aggregation (Seq/ELK)

3. **Error Handling**
   - Global exception middleware
   - Custom exceptions
   - Detailed error responses

4. **Validation**
   - FluentValidation
   - Custom validators
   - Business rule validation

### Prioridad Media (6 meses)

1. **Performance**
   - Response caching
   - Memory cache
   - Database query optimization
   - Connection pooling

2. **Security**
   - Rate limiting
   - API key authentication
   - CORS refinement
   - Security headers

3. **Monitoring**
   - Application Insights
   - Health checks
   - Performance metrics
   - Error tracking

### Prioridad Baja (12 meses)

1. **Architecture**
   - CQRS pattern
   - MediatR implementation
   - Domain-driven design
   - Clean architecture completa

2. **Infrastructure**
   - Docker containers
   - CI/CD pipeline
   - Azure deployment
   - Load balancing

---

## ?? RECURSOS Y REFERENCIAS

### Documentación Oficial

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)

### Libros Recomendados

- Clean Code - Robert C. Martin
- Clean Architecture - Robert C. Martin
- Domain-Driven Design - Eric Evans
- Refactoring - Martin Fowler

### Herramientas

- Visual Studio 2022
- SQL Server Management Studio
- Postman / Swagger
- Git / GitHub

---

## ? CONCLUSIÓN

Esta guía debe ser la **referencia principal** para todo el desarrollo del proyecto Bosko E-Commerce.

**Reglas de oro:**

1. **Simplicidad sobre complejidad**
2. **Seguridad primero**
3. **Performance importa**
4. **Testing no es opcional**
5. **Documentar decisiones importantes**
6. **Code review obligatorio**
7. **Refactorizar constantemente**
8. **Aprender de los errores**

---

**Última actualización:** 16 de Noviembre 2025  
**Versión:** 1.0  
**Mantenido por:** Equipo de Desarrollo Bosko

---

## ?? CHANGELOG

```
v1.0 (2025-11-16)
- Versión inicial de la guía
- Basado en el estado actual del proyecto
- Arquitectura, convenciones y mejores prácticas definidas
```
