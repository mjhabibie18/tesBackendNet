// ============================================================
// Program.cs — Entry Point dengan Setup Authorization
// ============================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TesBackendNet.Authorization.Configurations;
using TesBackendNet.Authorization.Data;
using TesBackendNet.Authorization.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Configuration ─────────────────────────────────────────────
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// ── Dependency Injection ──────────────────────────────────────
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Authentication (Siapa Anda?) ──────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidateAudience         = true,
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});

// ── Authorization (Bolehkah Anda Akses?) ──────────────────────
// Mendaftarkan service Otorisasi ke dalam aplikasi.
builder.Services.AddAuthorization(options => 
{
    // ── Policy-Based Authorization (Advanced RBAC) ───────────
    // Selain menggunakan [Authorize(Roles = "Admin")], kita juga bisa
    // membuat "Policy" khusus yang lebih fleksibel.
    
    // Contoh Policy 1: Hanya boleh diakses oleh Admin atau Manager
    options.AddPolicy("RequireElevatedRights", policy =>
        policy.RequireRole("Admin", "Manager"));

    // Contoh Policy 2: Policy berbasis claim khusus (bukan hanya Role)
    options.AddPolicy("RequireAdminWithEmailDomain", policy =>
    {
        policy.RequireRole("Admin");
        // Misalnya: Harus Admin DAN emailnya harus berakhiran @example.com
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => 
                c.Type == "email" && c.Value.EndsWith("@example.com")));
    });
    
    // Cara pakenya di controller:
    // [Authorize(Policy = "RequireElevatedRights")]
});

// ── Controllers + Swagger ─────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "TesBackendNet - Authorization API (RBAC)",
        Version     = "v1",
        Description = "Implementasi Role-Based Access Control dan Policy-Based Authorization."
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name         = "Authorization",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = JwtBearerDefaults.AuthenticationScheme,
        Description  = "Masukkan token JWT untuk mencoba RBAC. Contoh: Bearer eyJ...",
        Reference    = new OpenApiReference
        {
            Id   = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Auto Migration ────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Untuk testing cepat tanpa EF Migrations folder manual (Opsional)
    // db.Database.Migrate(); 
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// ── Middleware Pipeline ───────────────────────────────────────
// URUTAN PENTING! Authentication HARUS sebelum Authorization
app.UseAuthentication(); // 1. Cek JWT, Extract Claims, Set User Context
app.UseAuthorization();  // 2. Cek Role/Policy berdasarkan attribut di Controller

app.MapControllers();
app.Run();
