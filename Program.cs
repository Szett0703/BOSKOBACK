using DBTest_BACK.Data;
using DBTest_BACK.Services;
using DBTest_BACK.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACIÓN DE KESTREL Y PUERTOS
// ============================================
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5005, listenOptions =>
    {
        Console.WriteLine("✅ HTTP Server listening on: http://localhost:5005");
    });
    
    serverOptions.ListenLocalhost(5006, listenOptions =>
    {
        listenOptions.UseHttps();
        Console.WriteLine("✅ HTTPS Server listening on: https://localhost:5006");
    });
});

// Configuración adicional de URLs
builder.WebHost.UseUrls("https://localhost:5006", "http://localhost:5005");

// ============================================
// SERVICIOS
// ============================================

// Controllers y API con configuración completa de JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Manejar referencias circulares - CRÍTICO para Swagger
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // Ignorar propiedades nulas
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        // Configuración adicional para evitar problemas
        options.JsonSerializerOptions.MaxDepth = 32;
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger con configuración ROBUSTA para evitar errores 500
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Bosko E-Commerce API", 
        Version = "v1",
        Description = "API para gestión de pedidos, productos y usuarios",
        Contact = new OpenApiContact
        {
            Name = "Bosko Team",
            Email = "support@bosko.com"
        }
    });
    
    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // ⚠️ CONFIGURACIÓN CRÍTICA PARA EVITAR ERROR 500 ⚠️
    
    // 1. Usar nombres completos para evitar conflictos
    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    
    // 2. Ignorar propiedades virtuales que causan ciclos
    c.SchemaFilter<IgnoreVirtualPropertiesSchemaFilter>();
    
    // 3. Configuración segura de referencias
    c.UseAllOfToExtendReferenceSchemas();
    c.UseOneOfForPolymorphism();
    c.UseAllOfForInheritance();
    
    // 4. Opciones de serialización seguras
    c.DescribeAllParametersInCamelCase();
});

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Registrar servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<ICategoryAdminService, CategoryAdminService>();
builder.Services.AddScoped<IProductAdminService, ProductAdminService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// ============================================
// AUTENTICACIÓN JWT
// ============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey no está configurada en appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var role = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            Console.WriteLine($"✅ JWT Token validated - User: {userId}, Role: {role}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"⚠️ JWT Challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// ============================================
// AUTORIZACIÓN
// ============================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrEmployee", policy => policy.RequireRole("Admin", "Employee"));
    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
});

// ============================================
// CORS - PERMITIR FRONTEND ANGULAR
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "http://localhost:4300",
            "https://localhost:4200",
            "https://localhost:4300"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("Content-Disposition");
    });
});

// ============================================
// LOGGING MEJORADO
// ============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ============================================
// BUILD APP
// ============================================
var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE (ORDEN CRÍTICO)
// ============================================

Console.WriteLine("");
Console.WriteLine("============================================");
Console.WriteLine("🚀 BOSKO E-COMMERCE API");
Console.WriteLine("============================================");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"HTTPS: https://localhost:5006");
Console.WriteLine($"HTTP:  http://localhost:5005");
Console.WriteLine($"Swagger: https://localhost:5006/swagger");
Console.WriteLine("============================================");
Console.WriteLine("");

// Configuración específica para Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bosko API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
    Console.WriteLine("✅ Swagger UI habilitado en: https://localhost:5006/swagger");
}
else
{
    app.UseHsts();
}

// HTTPS Redirection
app.UseHttpsRedirection();

// Routing
app.UseRouting();

// CORS
app.UseCors("AllowFrontend");
Console.WriteLine("✅ CORS configurado para: http://localhost:4200, http://localhost:4300");

// Authentication y Authorization
app.UseAuthentication();
app.UseAuthorization();

// Middleware de logging
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var path = context.Request.Path;
    
    Console.WriteLine($"📨 {method} {path}");
    
    await next();
    
    var statusCode = context.Response.StatusCode;
    var statusEmoji = statusCode < 300 ? "✅" : statusCode < 400 ? "⚠️" : "❌";
    Console.WriteLine($"{statusEmoji} {method} {path} → {statusCode}");
});

// Map Controllers
app.MapControllers();

// Health check
app.MapGet("/health", () => new 
{ 
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    urls = new[] { "https://localhost:5006", "http://localhost:5005" }
})
.WithName("HealthCheck")
.AllowAnonymous();

// Root endpoint
app.MapGet("/", () => new
{
    message = "Bosko E-Commerce API",
    version = "1.0",
    swagger = "/swagger",
    health = "/health",
    endpoints = new
    {
        auth = "/api/auth",
        admin = "/api/admin",
        products = "/api/products",
        categories = "/api/categories"
    }
})
.WithName("Root")
.AllowAnonymous();

Console.WriteLine("");
Console.WriteLine("============================================");
Console.WriteLine("✅ API LISTA - Esperando requests...");
Console.WriteLine("============================================");
Console.WriteLine("");
Console.WriteLine("📝 Endpoints principales:");
Console.WriteLine("   POST   /api/auth/login");
Console.WriteLine("   POST   /api/auth/register");
Console.WriteLine("   GET    /api/admin/orders");
Console.WriteLine("   GET    /api/products");
Console.WriteLine("   GET    /api/categories");
Console.WriteLine("   GET    /health");
Console.WriteLine("");

// ============================================
// RUN
// ============================================
app.Run();
