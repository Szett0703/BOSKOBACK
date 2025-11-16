using DBTest_BACK.Data;
using DBTest_BACK.Services;
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

// Controllers y API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con configuración de seguridad JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Bosko E-Commerce API", 
        Version = "v1",
        Description = "API para gestión de pedidos, productos y usuarios"
    });
    
    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
});

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();

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
        .WithExposedHeaders("Content-Disposition"); // Para descargas de archivos
    });
});

// ============================================
// LOGGING MEJORADO
// ============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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
Console.WriteLine("" );

// Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bosko API v1");
        c.RoutePrefix = "swagger";
    });
    Console.WriteLine("✅ Swagger UI habilitado en: https://localhost:5006/swagger");
}

// HTTPS Redirection
app.UseHttpsRedirection();

// Routing (DEBE IR ANTES DE CORS Y AUTH)
app.UseRouting();

// CORS (DEBE IR DESPUÉS DE ROUTING Y ANTES DE AUTH)
app.UseCors("AllowFrontend");
Console.WriteLine("✅ CORS configurado para: http://localhost:4200, http://localhost:4300");

// Authentication y Authorization (EN ESTE ORDEN)
app.UseAuthentication();
app.UseAuthorization();

// Middleware de logging personalizado
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var path = context.Request.Path;
    var origin = context.Request.Headers["Origin"].ToString();
    
    Console.WriteLine($"📨 {method} {path} - Origin: {origin}");
    
    await next();
    
    var statusCode = context.Response.StatusCode;
    var statusEmoji = statusCode < 300 ? "✅" : statusCode < 400 ? "⚠️" : "❌";
    Console.WriteLine($"{statusEmoji} {method} {path} → {statusCode}");
});

// Map Controllers
app.MapControllers();

// Endpoint de health check
app.MapGet("/health", () => new 
{ 
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    urls = new[] { "https://localhost:5006", "http://localhost:5005" }
})
.WithName("HealthCheck")
.AllowAnonymous();

// Endpoint raíz
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
Console.WriteLine("   GET    /api/admin/orders");
Console.WriteLine("   GET    /api/products");
Console.WriteLine("   GET    /health");
Console.WriteLine("");

// ============================================
// RUN
// ============================================
app.Run();
