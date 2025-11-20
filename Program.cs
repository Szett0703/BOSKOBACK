using DBTest_BACK.Data;
using DBTest_BACK.Services;
using DBTest_BACK.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 🚀 RAILWAY PORT CONFIGURATION
// ============================================

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

Console.WriteLine($"🚀 Server will listen on: http://0.0.0.0:{port}");

// ============================================
// 🔌 CONNECTION STRING (LOCAL + RAILWAY)
// ============================================

// Railway sometimes exposes env vars with _ or with __
// So we check BOTH to avoid issues.
var envConn1 = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var envConn2 = Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection");

string connectionString;

if (!string.IsNullOrEmpty(envConn1))
{
    connectionString = envConn1;
    Console.WriteLine("🔌 Using RAILWAY Database Connection (double __)");
}
else if (!string.IsNullOrEmpty(envConn2))
{
    connectionString = envConn2;
    Console.WriteLine("🔌 Using RAILWAY Database Connection (single _)");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("🔌 Using LOCAL Database Connection");
}

Console.WriteLine($"ConnectionString: {connectionString}");

// ============================================
// VALIDATE CONNECTION STRING FOR POSTGRESQL
// ============================================

if (!string.IsNullOrEmpty(connectionString) &&
    (connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
     connectionString.Contains("Integrated Security", StringComparison.OrdinalIgnoreCase) ||
     connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)))
{
    Console.WriteLine("[ERROR] La connection string detectada parece ser de SQL Server. Asegúrate de usar una cadena de conexión PostgreSQL o establece ConnectionStrings__DefaultConnection.");
    throw new InvalidOperationException("Connection string no válida para Npgsql. Use PostgreSQL connection string.");
}

// ============================================
// 🐘 POSTGRESQL CONFIG
// ============================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ============================================
// JSON + CONTROLLERS
// ============================================

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bosko E-Commerce API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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

    c.CustomSchemaIds(t => t.FullName);
    c.SchemaFilter<IgnoreVirtualPropertiesSchemaFilter>();
});

// ============================================
// 🛡️ JWT AUTHENTICATION
// ============================================

var jwt = builder.Configuration.GetSection("JwtSettings");
var key = jwt["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.Zero
    };
});

// ============================================
// 🚀 REGISTER SERVICES
// ============================================

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
// 🌐 CORS
// ============================================

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("AllowFrontend", p =>
    {
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(origin =>
             origin.StartsWith("http://localhost") ||
             origin.StartsWith("https://localhost") ||
             origin.Contains("netlify.app")
         );
    });
});

// ============================================
// 🌍 BUILD APP
// ============================================

var app = builder.Build();

// ============================================
// 📌 APPLY EF CORE MIGRATIONS + RUN SEEDER
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("[MIGRATIONS] ⏳ Applying pending migrations...");
        await db.Database.MigrateAsync();
        Console.WriteLine("[MIGRATIONS] ✅ Migrations applied successfully!");

        Console.WriteLine("[SEED] 🚀 Executing DatabaseSeeder...");
        await DBTest_BACK.Seeding.DatabaseSeeder.SeedAsync(db);
        Console.WriteLine("[SEED] ✅ Database seeded successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] ❌ Error during migrations or seeding: {ex.Message}");
        Console.WriteLine(ex.ToString());
    }
}

// ============================================
// 🌐 SWAGGER
// ============================================

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================
// MIDDLEWARE
// ============================================

// HTTPS redirection DISABLED for Railway (Railway handles HTTPS externally)
// app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health status
app.MapGet("/health", () => new { status = "healthy", environment = app.Environment.EnvironmentName })
    .AllowAnonymous();

Console.WriteLine($"✅ Bosko API running on port {port}");

app.Run();
