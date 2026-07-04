# ⚡ CheatSheet — Authentication (JWT)

---

## 🚀 Install Packages

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 7.0.3
dotnet add package BCrypt.Net-Next --version 4.0.3
```

---

## ⚙️ appsettings.json — Konfigurasi JWT

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32Characters!",
    "Issuer": "YourAppName",
    "Audience": "YourAppClient",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

---

## ⚙️ Program.cs — Setup JWT Authentication

```csharp
// 1. Bind JwtSettings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// 2. Setup Authentication + JWT Bearer
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
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ClockSkew                = TimeSpan.Zero,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});

builder.Services.AddAuthorization();

// Di middleware pipeline — URUTAN PENTING!
app.UseAuthentication(); // Sebelum
app.UseAuthorization();  // Sesudah
```

---

## 🔑 Generate JWT Access Token

```csharp
public string GenerateAccessToken(User user)
{
    var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role,               user.Role),
        new Claim("userId",                      user.Id.ToString()),
    };

    var token = new JwtSecurityToken(
        issuer:             issuer,
        audience:           audience,
        claims:             claims,
        expires:            DateTime.UtcNow.AddMinutes(60),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

---

## 🔄 Generate Refresh Token

```csharp
public string GenerateRefreshToken()
{
    var randomBytes = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);
    return Convert.ToBase64String(randomBytes);
}
```

---

## 🔒 Hash Password (bcrypt)

```csharp
// Hash
var hash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 11);

// Verify
var isValid = BCrypt.Net.BCrypt.Verify(plainTextPassword, storedHash);
```

---

## 🛡️ Protected Endpoint

```csharp
// Butuh JWT valid
[HttpGet("profile")]
[Authorize]
public IActionResult GetProfile()
{
    // Ambil claims dari JWT
    var userId    = User.FindFirst("userId")?.Value;
    var email     = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
    var role      = User.FindFirst(ClaimTypes.Role)?.Value;
    var firstName = User.FindFirst("firstName")?.Value;

    return Ok(new { userId, email, role });
}

// Butuh JWT + Role Admin
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public IActionResult DeleteForAdmin(int id) { ... }
```

---

## 📤 Request dengan JWT

```http
GET /api/auth/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## 🔄 Refresh Token Flow

```csharp
// Endpoint refresh
[HttpPost("refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
{
    // 1. Cari di database
    var token = await _db.RefreshTokens
        .Include(rt => rt.User)
        .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

    // 2. Validasi
    if (token == null || !token.IsActive)
        return Unauthorized("Refresh token tidak valid");

    // 3. Revoke token lama
    token.IsRevoked = true;

    // 4. Buat token baru
    var newAccess  = GenerateAccessToken(token.User);
    var newRefresh = GenerateRefreshToken();

    // 5. Simpan refresh token baru
    _db.RefreshTokens.Add(new RefreshToken
    {
        Token     = newRefresh,
        UserId    = token.UserId,
        ExpiresAt = DateTime.UtcNow.AddDays(7)
    });

    await _db.SaveChangesAsync();

    return Ok(new { accessToken = newAccess, refreshToken = newRefresh });
}
```

---

## 🚪 Cara Logout

```csharp
// Revoke refresh token di database
[HttpPost("logout")]
public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
{
    var token = await _db.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

    if (token != null)
    {
        token.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    return Ok("Logout berhasil");
}
```

---

## 📋 Swagger + JWT Button

```csharp
builder.Services.AddSwaggerGen(options =>
{
    var jwtScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In   = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Id   = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});
```

---

## 🎯 Ambil UserId dari Token di Controller

```csharp
// Cara 1: via ClaimTypes
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

// Cara 2: via custom claim
var userId = User.FindFirst("userId")?.Value;

// Cara 3: via JwtRegisteredClaimNames
var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

// Convert ke int
int.TryParse(userId, out var userIdInt);
```
