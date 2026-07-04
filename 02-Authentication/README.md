# 🔐 02 — Authentication (Autentikasi)

## Apa itu Authentication?

**Authentication** adalah proses memverifikasi **siapa Anda** (identitas).

```
Authentication = "Siapa kamu?"
Authorization  = "Kamu boleh apa?"

Contoh:
  Login dengan email + password → Authentication (verify identity)
  Hanya admin yang bisa hapus data → Authorization (check permission)
```

> **Authentication harus selesai SEBELUM Authorization**

---

## 🕐 Kapan Authentication Digunakan?

- ✅ Setiap aplikasi yang punya sistem login
- ✅ API yang tidak boleh diakses publik
- ✅ Menyimpan data personal user
- ✅ Membedakan hak akses antar user

---

## 🔑 Metode Authentication yang Dicover

| Metode | Deskripsi | Kapan Digunakan |
|--------|-----------|-----------------|
| **JWT (JSON Web Token)** | Token stateless berbasis JSON | REST API, Mobile, SPA |
| **Session** | State disimpan di server | Web tradisional |
| **Cookie** | State disimpan di browser | Web tradisional |
| **Refresh Token** | Token untuk perbarui Access Token | Semua yang pakai JWT |
| **Password Hashing** | Enkripsi satu arah untuk password | Selalu! |

---

## 🔄 Flow Authentication JWT

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│   CLIENT    │         │   SERVER    │         │  DATABASE   │
└──────┬──────┘         └──────┬──────┘         └──────┬──────┘
       │                       │                        │
       │  1. POST /auth/register                        │
       │  { email, password }  │                        │
       │──────────────────────>│                        │
       │                       │  2. Hash password       │
       │                       │  (bcrypt)              │
       │                       │──────────────────────>│
       │                       │  3. Save user          │
       │                       │<──────────────────────│
       │  4. Return 201        │                        │
       │<──────────────────────│                        │
       │                       │                        │
       │  5. POST /auth/login  │                        │
       │  { email, password }  │                        │
       │──────────────────────>│                        │
       │                       │  6. Find user          │
       │                       │──────────────────────>│
       │                       │  7. Return user        │
       │                       │<──────────────────────│
       │                       │  8. Verify password    │
       │                       │  (bcrypt compare)      │
       │                       │  9. Generate JWT       │
       │                       │     + Refresh Token    │
       │                       │  10. Save Refresh Token│
       │                       │──────────────────────>│
       │  11. Return tokens    │                        │
       │  { accessToken,       │                        │
       │    refreshToken }     │                        │
       │<──────────────────────│                        │
       │                       │                        │
       │  12. GET /api/products│                        │
       │  Authorization:       │                        │
       │  Bearer {accessToken} │                        │
       │──────────────────────>│                        │
       │                       │  13. Validate JWT      │
       │                       │  (signature, expiry)   │
       │  14. Return data      │                        │
       │<──────────────────────│                        │
```

---

## 🪙 Apa itu JWT?

JWT (JSON Web Token) adalah **token stateless** berbentuk string dengan 3 bagian:

```
Header.Payload.Signature

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

### Struktur JWT

```
┌─────────────────────────────────────────────┐
│  HEADER (Base64)                            │
│  {                                          │
│    "alg": "HS256",   ← algoritma signature  │
│    "typ": "JWT"                             │
│  }                                          │
├─────────────────────────────────────────────┤
│  PAYLOAD (Base64) — berisi "claims"         │
│  {                                          │
│    "sub": "123",     ← subject (user ID)    │
│    "email": "...",   ← data custom          │
│    "role": "Admin",  ← role user            │
│    "iat": 1234567,   ← issued at            │
│    "exp": 1234567    ← expiration           │
│  }                                          │
├─────────────────────────────────────────────┤
│  SIGNATURE                                  │
│  HMACSHA256(base64(header) + "." +          │
│    base64(payload), secretKey)              │
│  ← memastikan token tidak dimanipulasi     │
└─────────────────────────────────────────────┘
```

### Mengapa JWT?

```
✓ Stateless: server tidak perlu simpan session
✓ Self-contained: semua info ada di token
✓ Cross-domain: bisa dipakai lintas domain
✓ Mobile-friendly: mudah disimpan di mobile
✓ Microservices: satu token bisa dipakai banyak service
```

---

## 🔄 Access Token vs Refresh Token

```
Access Token:
  - Umur pendek: 15 menit - 1 jam
  - Dikirim di setiap request (Authorization header)
  - Jika expire: user harus minta token baru menggunakan Refresh Token

Refresh Token:
  - Umur panjang: 7 hari - 30 hari
  - Disimpan di database (bisa di-revoke)
  - Dikirim HANYA ke endpoint /auth/refresh
  - Jika expire: user harus login ulang
```

---

## 🔒 Password Hashing

```
JANGAN PERNAH simpan password dalam plain text!

Hash: password → hash string (satu arah, tidak bisa dibalik)
     "password123" → "$2a$11$abc123..."

Verify: hash password input, bandingkan dengan hash di database
     bcrypt.Verify("password123", "$2a$11$abc123...") → true/false

Mengapa bcrypt?
  - Slow by design: brute force menjadi sangat lambat
  - Salt otomatis: sama password → hash berbeda (mencegah rainbow table)
  - Work factor: bisa tingkatkan kesulitan seiring waktu
```

---

## ✅ Best Practice

1. **Password hashing**: Selalu hash password dengan bcrypt atau Argon2
2. **Short-lived Access Token**: 15-60 menit
3. **Revocable Refresh Token**: Simpan di database, bisa di-revoke
4. **HTTPS only**: JWT sensitif, selalu pakai HTTPS
5. **Jangan simpan secret di JWT payload**: payload bisa di-decode siapapun
6. **Validate semua claims**: exp, iss, aud
7. **Logout = revoke Refresh Token**: invalidate refresh token di database

---

## ⚠️ Kesalahan yang Sering Dilakukan

```
❌ Simpan password dalam plain text
❌ Menggunakan MD5/SHA1 untuk hash password (tidak aman)
❌ Access Token terlalu lama (24 jam+)
❌ Tidak ada Refresh Token — user harus login ulang setiap jam
❌ Simpan JWT di localStorage (rentan XSS)
❌ Secret key lemah atau hardcoded
❌ Tidak validate expiration JWT
❌ Tidak handle logout (tidak revoke token)
```

---

## 🎤 Tips Interview

**Q: "Apa bedanya Authentication dan Authorization?"**
```
Authentication = siapa kamu? (verifikasi identitas)
Authorization  = kamu boleh apa? (cek hak akses)
Auth harus selesai sebelum AuthZ.
```

**Q: "Mengapa JWT lebih baik dari Session?"**
```
JWT (Stateless):
  + Tidak perlu session storage di server
  + Scale horizontal mudah (tidak perlu shared session)
  + Cocok untuk microservices
  - Tidak bisa di-revoke sebelum expire (gunakan Refresh Token)

Session (Stateful):
  + Bisa di-invalidate kapan saja
  + Tidak expose data di client
  - Perlu shared storage untuk multi-server
```

**Q: "Bagaimana cara logout dengan JWT?"**
```
1. Di client: hapus token dari storage
2. Di server: masukkan Access Token ke blacklist ATAU
              hapus/invalidate Refresh Token di database
Catatan: Access Token tidak bisa benar-benar di-revoke karena stateless.
Solusi: buat Access Token berumur pendek (15 menit).
```

**Q: "Kenapa password harus di-hash?"**
```
Jika database bocor (SQL injection, breach):
  - Plain text: semua password langsung terbaca
  - Hashed: hacker perlu brute force setiap hash
  - bcrypt dengan work factor 11: brute force ~100 tahun per password
```

---

## 🏋️ Latihan & Challenge

### Level 1 (Basic)
- [ ] Implementasi Register + Login dengan JWT
- [ ] Buat protected endpoint dengan [Authorize]

### Level 2 (Intermediate)
- [ ] Tambahkan Refresh Token
- [ ] Implementasi Logout (revoke refresh token)

### Level 3 (Advanced)
- [ ] Two-Factor Authentication (2FA)
- [ ] OAuth2 dengan Google/GitHub
- [ ] Rate limiting untuk login (prevent brute force)
