# 🔒 11 — Security

## Topik Security Backend

Security mencakup banyak aspek. Berikut yang paling penting untuk Backend Developer:

---

## 1. HTTPS

Semua komunikasi harus dienkripsi. HTTP = plain text, HTTPS = encrypted.

```csharp
// Paksa redirect ke HTTPS
app.UseHttpsRedirection();
app.UseHsts(); // HTTP Strict Transport Security
```

---

## 2. SQL Injection Prevention

```csharp
// ❌ RENTAN SQL Injection
var sql = $"SELECT * FROM Products WHERE Name = '{userInput}'";

// ✅ AMAN: EF Core pakai parameterized query otomatis
var products = await _context.Products
    .Where(p => p.Name == userInput)
    .ToListAsync();

// ✅ AMAN: Raw SQL dengan parameter
var products = await _context.Products
    .FromSqlInterpolated($"SELECT * FROM Products WHERE Name = {userInput}")
    .ToListAsync();
```

---

## 3. CORS (Cross-Origin Resource Sharing)

```csharp
builder.Services.AddCors(options =>
{
    // Development: izinkan semua
    options.AddPolicy("Development", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    // Production: spesifik domain
    options.AddPolicy("Production", policy =>
        policy
            .WithOrigins("https://yourdomain.com", "https://app.yourdomain.com")
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials());
});
```

---

## 4. Rate Limiting

```bash
dotnet add package AspNetCoreRateLimit
```

```csharp
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new() { Endpoint = "*", Period = "1m", Limit = 60 }, // 60 req/menit
        new() { Endpoint = "POST:/api/auth/login", Period = "5m", Limit = 5 } // 5 login/5 menit
    };
});
```

---

## 5. Secret Management

```bash
# JANGAN simpan secret di appsettings.json yang di-commit ke git!

# Development: gunakan dotnet user-secrets
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "your-secret-key"

# Production: gunakan environment variable atau Azure Key Vault
```

---

## 6. XSS Prevention

```csharp
// Input sanitization
var sanitizedInput = System.Web.HttpUtility.HtmlEncode(userInput);

// Content Security Policy header
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});
```

---

## OWASP Top 10 (Wajib Tahu!)

| # | Kerentanan | Pencegahan |
|---|-----------|-----------|
| A01 | Broken Access Control | Validasi authorization di setiap endpoint |
| A02 | Cryptographic Failures | HTTPS, bcrypt untuk password |
| A03 | Injection | Parameterized query, ORM |
| A04 | Insecure Design | Threat modeling |
| A05 | Security Misconfiguration | Review setiap config |
| A06 | Vulnerable Components | Update dependencies |
| A07 | Auth Failures | JWT + Refresh Token |
| A08 | Software Integrity | Code signing |
| A09 | Logging Failures | Comprehensive logging |
| A10 | SSRF | Validate URLs |

---

## 🎤 Tips Interview

**Q: "Apa OWASP Top 10?"**
```
Daftar 10 kerentanan web yang paling kritis dari OWASP (Open Web Application Security Project).
Penting untuk diketahui setiap developer.
Nomor 1: Broken Access Control (tidak validasi hak akses)
```
