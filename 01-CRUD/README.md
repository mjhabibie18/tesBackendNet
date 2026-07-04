# 📦 01 — CRUD (Create, Read, Update, Delete)

## Apa itu CRUD?

**CRUD** adalah singkatan dari **Create, Read, Update, Delete** — empat operasi dasar yang wajib dikuasai setiap Backend Developer. Hampir **100% technical test** meminta Anda membuat CRUD.

> **CRUD = Fondasi Backend Development**

```
CREATE  → Menambahkan data baru           (HTTP POST)
READ    → Mengambil/membaca data           (HTTP GET)
UPDATE  → Mengubah data yang sudah ada    (HTTP PUT / PATCH)
DELETE  → Menghapus data                  (HTTP DELETE)
```

---

## 🕐 Kapan CRUD Digunakan?

CRUD digunakan **setiap saat** dalam pengembangan aplikasi:

- ✅ Saat membuat fitur manajemen produk
- ✅ Saat membuat user management
- ✅ Saat membuat sistem blog/artikel
- ✅ Saat membuat inventory system
- ✅ Saat membuat order management
- ✅ Hampir semua fitur aplikasi adalah CRUD

---

## 🤔 Mengapa CRUD Penting?

| Alasan | Penjelasan |
|--------|-----------|
| **Fondasi** | Semua fitur kompleks dibangun di atas CRUD |
| **Universal** | Berlaku di semua bahasa & framework |
| **Interview** | Selalu muncul di technical test |
| **REST** | CRUD adalah implementasi praktis REST API |
| **Database** | Mapping langsung ke SQL: INSERT, SELECT, UPDATE, DELETE |

---

## 📚 Fitur yang Dicover di Modul Ini

| Fitur | Deskripsi |
|-------|-----------|
| **Create** | Menambah data baru |
| **Read (All)** | Mengambil semua data |
| **Read (ById)** | Mengambil data berdasarkan ID |
| **Update** | Mengubah data |
| **Delete (Hard)** | Menghapus data permanen |
| **Soft Delete** | Menandai data sebagai dihapus (tidak benar-benar hapus) |
| **Pagination** | Membagi data menjadi halaman |
| **Search** | Mencari data berdasarkan keyword |
| **Filter** | Menyaring data berdasarkan kriteria |
| **Sorting** | Mengurutkan data |

---

## ✅ Kelebihan Pola CRUD + Repository + Service

```
✓ Separation of Concerns (SoC) — setiap class punya satu tanggung jawab
✓ Testable — mudah di-unit test karena pakai interface
✓ Maintainable — mudah diperbaiki dan dikembangkan
✓ Scalable — mudah ditambah fitur baru
✓ Industry Standard — dipakai di perusahaan besar
```

## ❌ Kekurangan

```
✗ Boilerplate — banyak file yang harus dibuat
✗ Over-engineering untuk project kecil
✗ Learning curve untuk pemula
```

---

## 📖 Studi Kasus

### Skenario Technical Test yang Umum:

```
"Buat REST API untuk manajemen Product dengan fitur:
- CRUD Product (name, description, price, stock)
- Pagination & Search
- Soft Delete
- Response format yang konsisten"
```

Modul ini menjawab skenario tersebut **secara lengkap**.

---

## 🏗️ Arsitektur yang Digunakan

```
Request
   ↓
Controller        ← menerima HTTP request, validasi input dasar
   ↓
Service           ← business logic, validasi bisnis
   ↓
Repository        ← akses database via EF Core
   ↓
Database (SQL Server)
```

### Kenapa 3 Layer?

| Layer | Tanggung Jawab | Tanpa Layer Ini |
|-------|----------------|-----------------|
| Controller | Handle HTTP, return response | Logic berantakan di satu tempat |
| Service | Business rules | Controller jadi terlalu besar |
| Repository | Database access | Susah ganti database/ORM |

---

## ✨ Best Practice

1. **Selalu gunakan DTO** — jangan expose Model langsung ke response
2. **Gunakan async/await** — untuk semua operasi database
3. **Return tipe yang konsisten** — gunakan wrapper `ApiResponse<T>`
4. **Soft Delete > Hard Delete** — untuk data penting, jangan hapus permanen
5. **Gunakan interface** — agar mudah di-mock saat testing
6. **Validasi di Service layer** — jangan hanya di Controller
7. **Pagination wajib** — jangan return semua data sekaligus

---

## ⚠️ Kesalahan yang Sering Dilakukan

```
❌ Langsung akses DbContext di Controller
❌ Return Model langsung (expose struktur database)
❌ Tidak menggunakan async/await
❌ Return semua data tanpa pagination
❌ Hard delete untuk data penting (misal: user, order)
❌ Tidak handle null (data tidak ditemukan)
❌ Magic string di everywhere (tidak pakai constant)
❌ Tidak ada logging
```

---

## 🎤 Tips Interview

### Pertanyaan yang Sering Ditanyakan:

**Q: "Apa bedanya PUT dan PATCH?"**
```
PUT   → Replace SELURUH resource (kirim semua field)
PATCH → Update SEBAGIAN resource (kirim hanya field yang diubah)
```

**Q: "Apa itu Soft Delete? Kapan digunakan?"**
```
Soft Delete = tandai data sebagai "deleted" (IsDeleted = true)
tanpa benar-benar menghapus dari database.

Gunakan ketika:
- Data punya audit trail
- Mungkin perlu restore
- Relasi dengan tabel lain
- Compliance requirement (GDPR, dll)
```

**Q: "Mengapa menggunakan Repository Pattern?"**
```
- Memisahkan logic akses database dari business logic
- Mudah di-unit test (bisa di-mock)
- Kalau ganti ORM, cukup ubah Repository, tidak perlu ubah Service
```

**Q: "Apa itu DTO?"**
```
Data Transfer Object — object yang digunakan untuk transfer data
antara layer, berbeda dari Model/Entity yang merepresentasikan
struktur database.

Kenapa perlu DTO?
- Tidak expose field sensitif (password, dll)
- Bisa bentuk data sesuai kebutuhan client
- Versioning API lebih mudah
```

---

## 🧪 Tips Coding Test

```
1. Buat struktur folder dulu sebelum coding
2. Mulai dari Model → DbContext → Repository → Service → Controller
3. Selalu gunakan async/await
4. Buat ApiResponse wrapper dari awal
5. Tambahkan Swagger untuk dokumentasi otomatis
6. Test dengan Test.http atau Postman
7. Jika ada waktu: tambahkan pagination & search
```

---

## 🏋️ Latihan & Challenge

### Level 1 (Basic)
- [ ] Buat CRUD untuk entity `Category`
- [ ] Tambahkan relasi Product → Category

### Level 2 (Intermediate)
- [ ] Implementasikan Soft Delete
- [ ] Tambahkan Pagination, Search, Sort

### Level 3 (Advanced)
- [ ] Tambahkan bulk operations (create/update/delete multiple)
- [ ] Implementasikan ETag untuk optimistic concurrency
- [ ] Tambahkan audit fields (CreatedBy, UpdatedBy)

---

## 📁 Struktur File Modul Ini

```
01-CRUD/
├── README.md         ← File ini
├── Theory.md         ← Teori mendalam
├── CheatSheet.md     ← Snippet copy-paste
├── Source/           ← Project yang bisa dijalankan
│   └── (ASP.NET Core Web API)
├── Test.http         ← Test semua endpoint
└── Assets/           ← Diagram
```

---

## 🔗 Modul Terkait

- [02-Authentication](../02-Authentication/) — Tambahkan JWT ke CRUD
- [03-Authorization](../03-Authorization/) — Tambahkan Role ke CRUD
- [07-Validation](../07-Validation/) — Validasi input CRUD
- [08-ErrorHandling](../08-ErrorHandling/) — Handle error di CRUD
- [09-ApiResponse](../09-ApiResponse/) — Standarisasi response
