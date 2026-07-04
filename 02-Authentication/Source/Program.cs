// ============================================================
// Program.cs — Entry Point dengan JWT Authentication Setup
// ============================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TesBackendNet.Authentication.Configurations;
using TesBackendNet.Authentication.Data;
using TesBackendNet.Authentication.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── JWT Settings (Options Pattern) ───────────────────────────
// Bind "Jwt" section dari appsettings.json ke JwtSettings class
// Setelah ini, IOptions<JwtSettings> bisa di-inject ke service manapun
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// ── Dependency Injection ──────────────────────────────────────
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Authentication ────────────────────────────────────────────
// AddAuthentication(): daftarkan middleware authentication
// Parameter: default authentication scheme
//
// Mengapa AddAuthentication SEBELUM AddAuthorization?
//   - Pipeline: Auth harus sudah verify identity SEBELUM check authorization
//   - Jika terbalik, authorization tidak tahu siapa usernya
//
// JwtBearerDefaults.AuthenticationScheme = "Bearer"
// Artinya: default scheme untuk authenticate request adalah JWT Bearer
builder.Services.AddAuthentication(options =>
{
    // DefaultAuthenticateScheme: scheme untuk verify request
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // DefaultChallengeScheme: scheme untuk challenge (return 401 jika tidak auth)
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // ── JWT Bearer Options ────────────────────────────────────
    // Konfigurasi bagaimana JWT divalidasi

    // TokenValidationParameters: kriteria validasi JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validasi issuer (penerbit token)
        ValidateIssuer           = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],

        // Validasi audience (target token)
        ValidateAudience         = true,
        ValidAudience            = builder.Configuration["Jwt:Audience"],

        // Validasi expiry (waktu kadaluarsa)
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero, // Tidak ada toleransi waktu

        // Validasi signing key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };

    // Events untuk logging / custom behavior
    options.Events = new JwtBearerEvents
    {
        // Dipanggil saat token gagal divalidasi
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                // Tambahkan header khusus untuk memberi tahu client token expire
                context.Response.Headers["Token-Expired"] = "true";
            }
            return Task.CompletedTask;
        }
    };
});

// ── Authorization ─────────────────────────────────────────────
// AddAuthorization(): daftarkan middleware authorization
// Authorization menggunakan hasil Authentication untuk check permissions
builder.Services.AddAuthorization();

// ── Controllers + Swagger ─────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger dengan JWT Support ────────────────────────────────
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "TesBackendNet - Authentication API",
        Version     = "v1",
        Description = "Demo JWT Authentication: Register, Login, Refresh Token, Logout"
    });

    // Definisi security untuk Swagger UI
    // Ini menambahkan tombol "Authorize" di Swagger
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name         = "Authorization",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = JwtBearerDefaults.AuthenticationScheme,
        Description  = "Masukkan JWT token. Contoh: Bearer eyJ...",
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
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// URUTAN PENTING!
// Authentication HARUS sebelum Authorization
app.UseAuthentication(); // ← Verify JWT, set HttpContext.User
app.UseAuthorization();  // ← Check [Authorize] attribute

app.MapControllers();
app.Run();
