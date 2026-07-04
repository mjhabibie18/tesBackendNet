# Phase 3: Authorization (Role-Based Access Control)

Modul ini adalah kelanjutan dari Phase 2 (Authentication). Setelah sistem mengetahui *siapa* Anda (Authentication), sistem perlu mengetahui *apa yang boleh Anda lakukan* (Authorization).

## 🎯 Tujuan Pembelajaran
1. Memahami perbedaan Authentication (AuthN) dan Authorization (AuthZ).
2. Memahami konsep Role-Based Access Control (RBAC).
3. Menerapkan `[Authorize(Roles = "...")]` di ASP.NET Core.
4. Membuat Policy-Based Authorization yang lebih advanced.
5. Memahami HTTP Status Codes yang berkaitan dengan keamanan (401 vs 403).

## 🚀 Cara Menjalankan Project
1. Buka terminal di folder `Source/`.
2. Pastikan appsettings.Development.json sudah sesuai dengan SQL Server Anda.
3. Jalankan `dotnet run`. 
   *(Project ini menggunakan `EnsureCreated()` di `Program.cs` untuk membuat DB & Seed Data otomatis, jadi Anda tidak perlu menjalankan `dotnet ef database update` kecuali Anda mengubah struktur model).*

## 📚 Seed Data (Otomatis dibuat)
Saat dijalankan, database akan otomatis diisi oleh 3 user dengan role yang berbeda:
- **Admin**: `admin@example.com` | Pass: `Password123!`
- **Manager**: `manager@example.com` | Pass: `Password123!`
- **User**: `user@example.com` | Pass: `Password123!`

Anda dapat mencoba login menggunakan ketiga akun di atas melalui `Test.http` atau Swagger UI untuk melihat perbedaan hak akses pada endpoint `/api/admin/...`.
