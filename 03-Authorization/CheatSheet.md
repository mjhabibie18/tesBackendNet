# Cheat Sheet: Authorization & RBAC

## 1. Setup Role dalam JWT
Agar ASP.NET Core mengerti Role dari JWT, pastikan claim *Role* dimasukkan dengan benar saat generate token:
```csharp
// Di TokenService.cs
var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(ClaimTypes.Role, user.Role) // ← INI SANGAT PENTING!
};
```

## 2. Proteksi Endpoint (Method Level)
Gunakan attribute `[Authorize]` di atas action method:
```csharp
// Hanya untuk user yang sudah login (Token Valid)
[Authorize]
public IActionResult GetProfile() { ... }

// Hanya untuk Admin
[Authorize(Roles = "Admin")]
public IActionResult GetAdminData() { ... }

// Untuk Admin ATAU Manager (OR logic)
[Authorize(Roles = "Admin,Manager")]
public IActionResult GetManagerData() { ... }
```

## 3. Proteksi Seluruh Controller (Class Level)
Taruh di atas class Controller. Semua method di dalamnya otomatis terproteksi:
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpGet] // Otomatis perlu role "Admin"
    public IActionResult Index() { ... }
    
    [HttpGet("public")]
    [AllowAnonymous] // Override class level, ini bisa diakses umum
    public IActionResult PublicInfo() { ... }
}
```

## 4. Policy-Based Authorization (Advanced)
Jika butuh logika kompleks (contoh: "Admin" DAN "Manager" dengan syarat tambahan), buat policy di `Program.cs`:
```csharp
// 1. Setup di Program.cs
builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("RequireManagerOrAdmin", policy =>
        policy.RequireRole("Admin", "Manager"));
});

// 2. Gunakan di Controller
[Authorize(Policy = "RequireManagerOrAdmin")]
public IActionResult GetDashboard() { ... }
```

## 5. Global Authorization (Semua API Terkunci Default)
Jika Anda ingin SEMUA endpoint terkunci secara default (secure by default), pasang filter secara global di `Program.cs`:
```csharp
builder.Services.AddControllers(options =>
{
    // Setiap endpoint WAJIB authorize kecuali diberi [AllowAnonymous]
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
```
