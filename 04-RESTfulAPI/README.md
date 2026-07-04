# 04-RESTfulAPI — RESTful API, HATEOAS, Content Negotiation, and Versioning

Modul ini mendemonstrasikan cara membangun RESTful API yang matang dan memenuhi standar kedewasaan REST tingkat tinggi (Richardson Maturity Model).

---

## 🎯 Target Materi & Pembelajaran

1. **Content Negotiation**: Mengizinkan client memilih representasi format data yang mereka inginkan (`application/json` atau `application/xml`) melalui header `Accept`.
2. **HATEOAS (Hypermedia as the Engine of Application State)**: Menyertakan link navigasi yang relevan secara dinamis dalam respon JSON, membantu client melakukan aksi tanpa perlu menebak atau hardcode URL.
3. **API Versioning**: Mengelola perubahan (breaking changes) pada API dengan memisahkan versi menggunakan URL path (`/api/v1/...` vs `/api/v2/...`), Query String, maupun HTTP Headers.

---

## 🛠️ Cara Menjalankan

1. Pastikan Anda berada di direktori modul:
   ```bash
   cd 04-RESTfulAPI/Source
   ```
2. Jalankan aplikasi:
   ```bash
   dotnet run
   ```
3. Gunakan berkas [Test.http](../Test.http) untuk melakukan pengetesan endpoint secara langsung dari editor VS Code atau REST Client favorit Anda.
4. Akses Swagger UI di peramban: `http://localhost:5000/swagger` untuk melihat pilihan dokumentasi API V1 dan V2.
