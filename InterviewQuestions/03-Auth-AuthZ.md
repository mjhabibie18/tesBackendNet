# 🔐 03 — Authentication & Authorization Interview Questions

---

## Soal 1: Jelaskan perbedaan Authentication dan Authorization!

**Tingkat**: Easy

### Solusi

```
Authentication = "Siapa kamu?"
  → Verifikasi identitas: login dengan email + password
  → Hasil: "Kamu adalah user John dengan ID 123"

Authorization = "Kamu boleh apa?"
  → Verifikasi hak akses: cek role dan permission
  → Hasil: "John adalah Admin, boleh hapus data"

PENTING: Authentication HARUS selesai sebelum Authorization.

Contoh salah:
  app.UseAuthorization();  // ❌ Auth sebelum AuthN!
  app.UseAuthentication();

Benar:
  app.UseAuthentication(); // ✅
  app.UseAuthorization();  // ✅
```

---

## Soal 2: Jelaskan struktur JWT!

**Tingkat**: Easy

### Solusi

```
JWT = Header.Payload.Signature
(semua bagian di-encode dengan Base64URL)

HEADER (algorithm + type):
{
  "alg": "HS256",
  "typ": "JWT"
}

PAYLOAD (claims = data user):
{
  "sub": "123",           ← user ID
  "email": "user@mail.com",
  "role": "Admin",
  "iat": 1704067200,       ← issued at (Unix timestamp)
  "exp": 1704070800        ← expiry (Unix timestamp)
}

SIGNATURE (verifikasi integritas):
HMACSHA256(
  base64(header) + "." + base64(payload),
  secretKey
)

PENTING: Payload bisa di-decode siapapun! (hanya Base64, bukan enkripsi)
JANGAN simpan data sensitif di payload (password, kartu kredit, dll)!
```

---

## Soal 3: Apa bedanya JWT dan Session?

**Tingkat**: Medium

### Solusi

| | JWT (Stateless) | Session (Stateful) |
|--|----------------|-------------------|
| **Storage** | Client (localStorage/cookie) | Server (memory/Redis) |
| **State** | Tidak ada di server | Ada di server |
| **Scalability** | ✅ Mudah horizontal scale | ❌ Perlu shared session storage |
| **Revocable** | ❌ Sulit (perlu blacklist) | ✅ Mudah (hapus dari storage) |
| **Size** | Lebih besar (data di token) | Kecil (hanya session ID) |
| **Microservices** | ✅ Cocok | ❌ Perlu shared session |

```
Gunakan JWT untuk: REST API, SPA, Mobile, Microservices
Gunakan Session untuk: Web tradisional dengan server-side rendering
```

---

## Soal 4: Bagaimana cara refresh JWT yang expire?

**Tingkat**: Medium

### Kode Lengkap

```csharp
// Flow Refresh Token:
// 1. Access Token expire
// 2. Client kirim Refresh Token ke POST /auth/refresh
// 3. Server validasi Refresh Token (ada di database? aktif?)
// 4. Server generate Access Token baru
// 5. Server rotate Refresh Token (revoke lama, buat baru)
// 6. Return token baru ke client

[HttpPost("refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
{
    // Cari refresh token di database
    var token = await _context.RefreshTokens
        .Include(rt => rt.User)
        .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

    // Validasi
    if (token == null || !token.IsActive)
        return Unauthorized("Refresh token tidak valid");

    // Revoke token lama (Refresh Token Rotation)
    token.IsRevoked = true;

    // Generate token baru
    var newAccess  = _tokenService.GenerateAccessToken(token.User);
    var newRefresh = _tokenService.GenerateRefreshToken();

    _context.RefreshTokens.Add(new RefreshToken
    {
        Token     = newRefresh,
        UserId    = token.UserId,
        ExpiresAt = DateTime.UtcNow.AddDays(7)
    });

    await _context.SaveChangesAsync();

    return Ok(new { accessToken = newAccess, refreshToken = newRefresh });
}
```

---

## Soal 5: Bagaimana cara secure password di database?

**Tingkat**: Medium

### Solusi

```csharp
// ❌ JANGAN pernah simpan plain text!
user.Password = "password123"; // BAHAYA!

// ❌ JANGAN pakai MD5 atau SHA1 (sudah di-crack)
user.Password = MD5.ComputeHash("password123"); // Tidak aman!

// ✅ Gunakan bcrypt (slow hash, dengan salt)
var hash = BCrypt.Net.BCrypt.HashPassword("password123", workFactor: 11);
// hash = "$2a$11$[22char salt][31char hash]"
// Setiap kali hash berbeda meskipun password sama! (salt unik per hash)

// Verifikasi
var isValid = BCrypt.Net.BCrypt.Verify("password123", storedHash); // true/false

// Mengapa bcrypt aman?
// 1. Slow by design: work factor 11 = ~300ms per hash
//    Brute force: 1 juta percobaan × 300ms = 300,000 detik ≈ 3.5 hari
// 2. Salt unik: tidak bisa pakai rainbow table
// 3. Adaptable: tingkatkan work factor seiring meningkatnya hardware
```

---

## Soal 6: Implementasi JWT Authentication dari nol!

**Tingkat**: Hard | **Topik**: Coding Test

### Deskripsi
Buat sistem authentication dengan Register, Login, dan protected endpoint.

### Kode Lengkap
Lihat implementasi lengkap di: [../02-Authentication/Source/](../02-Authentication/Source/)

### Ringkasan

```csharp
// 1. Program.cs — setup JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)),
            ValidateIssuer   = true,
            ValidIssuer      = config["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience    = config["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.Zero
        };
    });

// 2. Generate JWT
var token = new JwtSecurityToken(
    issuer:   issuer,
    audience: audience,
    claims:   new[] {
        new Claim("userId",       user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role)
    },
    expires:  DateTime.UtcNow.AddMinutes(60),
    signingCredentials: credentials);

// 3. Protected endpoint
[HttpGet("profile")]
[Authorize]
public IActionResult GetProfile()
{
    var userId = User.FindFirst("userId")?.Value;
    return Ok(new { userId });
}
```

---

## Soal 7–20: (Ringkasan)

**7.** Bagaimana cara logout dengan JWT?
> Revoke refresh token di database. Access token tidak bisa di-revoke (stateless).
> Solusi: access token berumur pendek (15-60 menit)

**8.** Apa itu RBAC?
> Role-Based Access Control: hak akses berdasarkan role (Admin, User, Manager)
> Di ASP.NET Core: `[Authorize(Roles = "Admin")]`

**9.** Apa itu Claim?
> Key-value pair yang ada di JWT payload. Contoh: userId, email, role

**10.** Bagaimana cara ambil user ID dari JWT di Controller?
> `User.FindFirst("userId")?.Value` atau `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`

**11.** Apa itu Policy-based Authorization?
> Authorization dengan aturan bisnis kompleks, lebih fleksibel dari Role-based

**12.** Apa itu OAuth2?
> Protokol authorization untuk third-party apps (Login with Google/GitHub)

**13.** Apa itu OpenID Connect?
> Layer authentication di atas OAuth2. OAuth2 = authorization, OIDC = authentication

**14.** Bagaimana cara simpan JWT di client?
> localStorage: mudah akses, rentan XSS
> HttpOnly Cookie: aman dari XSS, rentan CSRF
> Recommendation: HttpOnly Cookie + CSRF protection

**15.** Apa itu CSRF dan bagaimana mencegahnya?
> Cross-Site Request Forgery: request berbahaya dari site lain
> Pencegahan: Anti-forgery token, SameSite cookie, CORS yang ketat

**16.** Bagaimana cara implementasi 2FA?
> TOTP (Time-based One-Time Password): Google Authenticator
> Library: `Google.Authenticator` atau `OtpNet`

**17.** Apa itu JWT Blacklist?
> Menyimpan token yang di-revoke sebelum expire (misal saat logout)
> Simpan di Redis dengan TTL sesuai sisa waktu expire token

**18.** Apa itu Resource Owner Password Credentials Grant?
> OAuth2 flow untuk trusted first-party apps: kirim username+password langsung ke server

**19.** Bagaimana cara handle JWT di microservices?
> API Gateway validate JWT, forward decoded claims ke downstream services
> Atau: setiap service validate JWT sendiri (lebih aman)

**20.** Apa itu Refresh Token Rotation?
> Setiap kali refresh, token lama di-revoke dan buat token baru
> Jika refresh token dicuri, hacker hanya bisa pakai sekali sebelum legitimate user menggunakannya
