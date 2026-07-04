# ⚙️ 24 — Environment & Configuration

## Environments di ASP.NET Core

ASP.NET Core mendukung multiple environment: Development, Staging, Production.

---

## Environment Configuration

```bash
# Set via environment variable
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_ENVIRONMENT=Staging
ASPNETCORE_ENVIRONMENT=Production

# Set via launchSettings.json (development only)
{
  "profiles": {
    "Development": {
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## appsettings Hierarchy

```
appsettings.json                    ← Base (semua environment)
appsettings.Development.json        ← Override untuk Development
appsettings.Staging.json            ← Override untuk Staging
appsettings.Production.json         ← Override untuk Production (jangan commit!)
Environment Variables               ← Override tertinggi
```

---

## Baca Konfigurasi

```csharp
// Simple key
var value = builder.Configuration["MySection:MyKey"];

// GetValue dengan default
var maxRetry = builder.Configuration.GetValue<int>("MaxRetry", 3);

// GetConnectionString (shortcut untuk ConnectionStrings section)
var connStr = builder.Configuration.GetConnectionString("Default");

// Strongly typed (Options Pattern)
builder.Services.Configure<MyOptions>(
    builder.Configuration.GetSection("MySection"));
```

---

## Environment Variables Override

```
# appsettings.json key: "ConnectionStrings:DefaultConnection"
# Environment variable: ConnectionStrings__DefaultConnection (ganti : dengan __)

$env:ConnectionStrings__DefaultConnection = "Server=..."
$env:Jwt__SecretKey = "your-secret-key"
```

---

## User Secrets (Development)

```bash
# Inisialisasi
dotnet user-secrets init

# Set secret
dotnet user-secrets set "Jwt:SecretKey" "super-secret-key"
dotnet user-secrets set "ConnectionStrings:Default" "Server=..."

# List semua secrets
dotnet user-secrets list

# Hapus secret
dotnet user-secrets remove "Jwt:SecretKey"
```

---

## ✅ Best Practice

1. **Jangan commit secrets** ke git (gunakan .gitignore)
2. **User Secrets untuk development**: aman, tidak di-commit
3. **Environment Variables untuk production**: Docker, Kubernetes, cloud
4. **Azure Key Vault / AWS Secrets Manager** untuk production secrets
5. **Validate konfigurasi** saat startup (fail fast)

```csharp
// Validate saat startup
var jwtSecret = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("Jwt:SecretKey harus diisi minimal 32 karakter!");
```
