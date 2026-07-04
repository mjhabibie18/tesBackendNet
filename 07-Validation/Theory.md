# Teori Validasi Data di ASP.NET Core

Validasi adalah pilar penting dalam keamanan backend untuk memastikan integritas data dan mencegah serangan cyber (seperti SQL Injection atau XSS).

---

## 1. Mengapa Validasi Server-Side Wajib?
*   **Bypass Client-Side**: Validasi browser (HTML5/JS) sangat mudah dilewati menggunakan tools seperti Postman, cURL, atau dengan mematikan JavaScript di browser.
*   **Keamanan Data**: Server-side adalah gerbang utama sebelum data masuk ke database. Jika data kotor masuk, database dapat mengalami kegagalan integritas (*data corruption*).

---

## 2. Data Annotations vs FluentValidation
Dalam ekosistem ASP.NET Core, ada dua pendekatan populer:

### Data Annotations
*   **Deskripsi**: Menggunakan atribut bawaan C# (seperti `[Required]`, `[Range]`) di atas properti model.
*   **Kelebihan**: Cepat ditulis, menyatu dengan DTO.
*   **Kelemahan**: Sulit untuk logika validasi kompleks (misalnya validasi kondisional yang bergantung pada properti lain), mengotori DTO dengan kode *meta-programming*.

### FluentValidation
*   **Deskripsi**: Library eksternal pihak ketiga yang menggunakan *Fluent Interface* dan lambda expression untuk menyusun aturan validasi secara terpisah.
*   **Kelebihan**: Aturan validasi bersih terpisah dari DTO, mendukung logika kompleks/kondisional, sangat mudah di-*unit test*.
*   **Kelemahan**: Memerlukan instalasi *package* tambahan.
