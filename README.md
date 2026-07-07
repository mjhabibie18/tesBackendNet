<div align="center">

# 🚀 TesBackendNet

**Repository Belajar, Latihan & Template Backend Developer**
**ASP.NET Core (.NET 9) · SQL Server · Entity Framework Core**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Developer-CC2927?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge)](https://docs.microsoft.com/ef/core/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![RabbitMQ](https://img.shields.io/badge/Rabbitmq-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

</div>

---

## 📑 Daftar Isi
- [🎯 Tujuan Repository](#-tujuan-repository)
- [📖 Penjelasan Proyek](#-penjelasan-proyek)
- [🏆 Apa yang Ingin Dicapai (Goals)](#-apa-yang-ingin-dicapai-goals)
- [🗂️ Struktur Repository](#️-struktur-repository)
- [📦 Setiap Modul Berisi](#-setiap-modul-berisi)
- [⚡ Quick Start](#-quick-start)
- [🗄️ Konfigurasi Database](#️-konfigurasi-database)
- [🔗 Cara Menggabungkan Modul](#-cara-menggabungkan-modul)
- [📚 Materi yang Tersedia](#-materi-yang-tersedia)
- [🎤 Interview Questions](#-interview-questions)
- [🛠️ Tech Stack](#️-tech-stack)
- [📖 Cara Belajar yang Disarankan](#-cara-belajar-yang-disarankan)
- [🤝 Kontribusi](#-kontribusi)

---

## 🎯 Tujuan Repository

Repository ini dirancang sebagai **panduan belajar sekaligus template profesional** untuk Backend Developer menggunakan ASP.NET Core. Digunakan untuk:

| Tujuan | Deskripsi |
|--------|-----------|
| 📚 **Belajar** | Materi lengkap setiap topik Backend |
| 🧪 **Technical Test** | Template siap pakai saat coding test |
| 🎤 **Interview Prep** | 100+ soal interview + pembahasan lengkap |
| 📋 **Template** | Copy-paste snippet langsung digunakan |
| 🗂️ **Portfolio** | Repository profesional untuk GitHub |

---

## 📖 Penjelasan Proyek

**TesBackendNet** adalah repositori modular yang berisi kumpulan materi, latihan, dan *template* backend yang dipisahkan berdasarkan topik spesifik.
Setiap modul di dalam proyek ini memiliki struktur lengkap yang terdiri dari:
- **Teori Mendalam (`Theory.md`)** beserta diagram/alur logika.
- **Cheat Sheet (`CheatSheet.md`)** berupa *snippet* kode siap salin-tempel.
- **Source Code (`Source/`)** — Project ASP.NET Core yang **100% bisa langsung dijalankan**, dengan komentar **bergaya teks-buku** yang menjelaskan *apa, mengapa, dan bagaimana* setiap baris kode ditulis. Cocok digunakan sebagai bahan belajar mandiri.
- **File Test (`Test.http`)** untuk menguji coba *endpoint* secara langsung.

Sifat proyek ini sangat modular, artinya Anda bisa dengan mudah mengambil modul A (misal: CRUD) dan menggabungkannya dengan modul B (misal: JWT Auth) untuk menyelesaikan sebuah *study case* dengan sangat cepat.

---

## 🏆 Apa yang Ingin Dicapai (Goals)

Proyek ini berusaha memfasilitasi 5 hal utama bagi penggunanya:
1. **Pusat Pembelajaran (Belajar):** Menyajikan materi lengkap secara bertahap mulai dari hal fundamental hingga tingkat lanjut (*Microservices* dan *Cloud*).
2. **Kesiapan Technical Test:** Menyediakan *template* aplikasi siap pakai sehingga saat ada tes *coding*, developer tidak perlu membuat struktur aplikasi dari nol.
3. **Persiapan Wawancara (Interview Prep):** Menyediakan lebih dari 100 studi kasus pertanyaan wawancara teknis beserta solusi kodenya.
4. **Repositori Template Praktis:** Menjadi bank kode yang berisi implementasi standar (seperti struktur validasi, penanganan *error* global, dan *design pattern*).
5. **Standardisasi *Clean Code* & Portofolio:** Menunjukkan bentuk repositori berkualitas profesional standar industri untuk referensi GitHub.

---

## 🗂️ Struktur Repository

```
TesBackendNet/
│
├── 📁 01-CRUD/                    ✅ Fase 1
├── 📁 02-Authentication/          ✅ Fase 1
├── 📁 03-Authorization/           ✅ Fase 1
├── 📁 04-RESTfulAPI/              ✅ Fase 1
├── 📁 05-Database/                ✅ Fase 1
├── 📁 06-DatabaseDesign/          ✅ Fase 2
├── 📁 07-Validation/              ✅ Fase 1
├── 📁 08-ErrorHandling/           ✅ Fase 1
├── 📁 09-ApiResponse/             ✅ Fase 2
├── 📁 10-FileUpload/              ✅ Fase 2
├── 📁 11-Security/                ✅ Fase 2
├── 📁 12-Git/                     ✅ Fase 2
├── 📁 13-OOP/                     ✅ Fase 2
├── 📁 14-CleanCode/               ✅ Fase 2
├── 📁 15-DesignPattern/           ✅ Fase 2
├── 📁 16-Framework/               ✅ Fase 2
├── 📁 17-ORM/                     ✅ Fase 2
├── 📁 18-Testing/                 ✅ Fase 3
├── 📁 19-Caching/                 ✅ Fase 3
├── 📁 20-Queue/                   ✅ Fase 3
├── 📁 21-Docker/                  ✅ Fase 4
├── 📁 22-CICD/                    ✅ Fase 4
├── 📁 23-Logging/                 ✅ Fase 3
├── 📁 24-Environment/             ✅ Fase 3
├── 📁 25-Documentation/           ✅ Fase 3
├── 📁 26-Algorithm/               ✅ Fase 4
├── 📁 27-Concurrency/             ✅ Fase 4
├── 📁 28-Microservices/           ✅ Fase 4
├── 📁 29-Cloud/                   ✅ Fase 4
├── 📁 30-Debugging/               ✅ Fase 4
│
├── 📁 InterviewQuestions/         ✅ 100+ Soal
│
├── 🐳 docker-compose.yml          (Opsional)
└── 📄 README.md
```

> **Legend:** ✅ Selesai · 🔄 Dalam Progress · ⏳ Belum Dimulai

---

## 📦 Setiap Modul Berisi

```
XX-ModuleName/
├── README.md        → Overview, kapan/mengapa digunakan, tips interview
├── Theory.md        → Teori mendalam + diagram alur
├── CheatSheet.md    → Snippet copy-paste untuk coding test
├── Source/          → ASP.NET Core project yang DAPAT DIJALANKAN
├── Test.http        → Semua endpoint siap test di VS Code
└── Assets/          → Diagram, ERD, Flowchart
```

---

## ⚡ Quick Start

### Prerequisites

```bash
# Cek versi .NET
dotnet --version   # Harus 8.0+

# Cek SQL Server (LocalDB)
sqllocaldb info    # Atau gunakan SQL Server Developer Edition
```

### Cara Menjalankan Setiap Modul

```bash
# 1. Masuk ke folder modul
cd 01-CRUD/Source

# 2. Restore packages
dotnet restore

# 3. Update database (buat tabel)
dotnet ef database update

# 4. Jalankan
dotnet run

# 5. Buka Swagger UI
# https://localhost:PORT/swagger
```

### Cara Menjalankan dengan Docker (Opsional)

```bash
# Jalankan SQL Server + Redis + RabbitMQ via Docker
docker-compose up -d

# Cek status
docker-compose ps

# Hentikan
docker-compose down
```

---

## 🗄️ Konfigurasi Database

### Opsi 1: SQL Server LocalDB (Bawaan Visual Studio) - *Rekomendasi*

Buka `appsettings.Development.json` di setiap modul dan ubah connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TesBackendNet_NAMA_MODUL;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```

### Opsi 2: SQL Server Developer/Express

Jika Anda menginstal SQL Server secara terpisah, gunakan server `.` atau `.\\SQLEXPRESS`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TesBackendNet_NAMA_MODUL;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Opsi 3: Docker SQL Server

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1434;Database=TesBackendNet_NAMA_MODUL;User Id=sa;Password=TesBackendNet@2024!;TrustServerCertificate=True;"
  }
}
```

---

## 🔗 Cara Menggabungkan Modul

Contoh soal: **"Buat CRUD Product dengan JWT Authentication dan Role Admin"**

```text
1. Ambil ProductController.cs dari → 01-CRUD/Source/Controllers/
2. Ambil JWT setup dari           → 02-Authentication/Source/
3. Ambil Role Policy dari         → 03-Authorization/Source/
4. Gabungkan di project baru
5. Selesai! 🎉
```

**Bagaimana Databasenya jika digabung?**

Jika Anda membangun aplikasi gabungan (Monolitik), Anda tidak perlu menggunakan database yang terpisah-pisah. Cukup satukan semuanya ke dalam **satu database utama**. 

Berikut adalah panduan langkah demi langkahnya:

**1. Buat Satu Connection String Saja**
Di file `appsettings.json` pada *project* gabungan Anda, gunakan satu nama database akhir.
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TokoOnlineDB;Trusted_Connection=True;MultipleActiveResultSets=true;"
```

**2. Kumpulkan Semua Model (Class)**
Salin seluruh *class* entitas dari berbagai modul (misalnya `User.cs` dari Auth, `Product.cs` dari CRUD) ke dalam satu folder `Models` di *project* baru Anda.

**3. Daftarkan ke Dalam Satu DbContext Utama**
Kumpulkan semua tabel tersebut ke dalam satu file `AppDbContext`.
```csharp
public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    // Tabel dari Modul Auth
    public DbSet<User> Users { get; set; }
    
    // Tabel dari Modul CRUD
    public DbSet<Product> Products { get; set; }
}
```

**4. Jalankan Migrasi**
Buka terminal dan jalankan perintah Entity Framework berikut:
```bash
dotnet ef migrations add InitialGabungan
dotnet ef database update
```
**Selesai!** EF Core akan otomatis mendeteksi kode Anda dan menciptakan **satu database utuh (`TokoOnlineDB`)** yang langsung berisi tabel `Users` dan `Products`. Anda kini bisa dengan bebas membuat relasi/JOIN antar tabel tersebut.

---

## 📚 Materi yang Tersedia

### Fase 1 — Fundamental (Most Asked in Technical Test)

| Modul | Topik | Status |
|-------|-------|--------|
| [01-CRUD](./01-CRUD/) | Create, Read, Update, Delete, Soft Delete, Pagination, Search, Filter, Sort | ✅ |
| [02-Authentication](./02-Authentication/) | JWT, Refresh Token, Login, Register, bcrypt | ✅ |
| [03-Authorization](./03-Authorization/) | RBAC, Policy, Permission, Middleware Guard | ✅ |
| [04-RESTfulAPI](./04-RESTfulAPI/) | HTTP Methods, Status Codes, REST Principles | ✅ |
| [05-Database](./05-Database/) | SQL, JOIN, Transaction, EF Core | ✅ |
| [07-Validation](./07-Validation/) | FluentValidation, Data Annotations | ✅ |
| [08-ErrorHandling](./08-ErrorHandling/) | Global Exception Handler, ProblemDetails | ✅ |

### Fase 2 — Architecture & Best Practices

| Modul | Topik | Status |
|-------|-------|--------|
| [06-DatabaseDesign](./06-DatabaseDesign/) | Normalization, ERD, Indexing, Relationships | ✅ |
| [09-ApiResponse](./09-ApiResponse/) | Standard Response Wrapper, Pagination Metadata | ✅ |
| [10-FileUpload](./10-FileUpload/) | IFormFile, Validation, Local Storage, S3 | ✅ |
| [11-Security](./11-Security/) | CORS, Rate Limiting, XSS, CSRF, Helmet | ✅ |
| [12-Git](./12-Git/) | Branching Strategy, Commit Convention | ✅ |
| [13-OOP](./13-OOP/) | SOLID, Abstraction, Polymorphism, Interfaces | ✅ |
| [14-CleanCode](./14-CleanCode/) | Naming Convention, Refactoring, YAGNI, DRY | ✅ |
| [15-DesignPattern](./15-DesignPattern/) | Repository, Unit of Work, Factory, Builder | ✅ |
| [16-Framework](./16-Framework/) | Middleware, Dependency Injection, Hosted Services | ✅ |
| [17-ORM](./17-ORM/) | Dapper, EF Core Performance, Lazy Loading | ✅ |

### Fase 3 — Advanced Concepts & Performance

| Modul | Topik | Status |
|-------|-------|--------|
| [18-Testing](./18-Testing/) | Unit Testing, Integration Testing, xUnit, Moq | ✅ |
| [19-Caching](./19-Caching/) | In-Memory, Distributed Cache, Redis | ✅ |
| [20-Queue](./20-Queue/) | Message Brokers, RabbitMQ, Background Tasks | ✅ |
| [23-Logging](./23-Logging/) | Serilog, Structured Logging, Seq | ✅ |
| [24-Environment](./24-Environment/) | Configuration, Secret Manager, AppSettings | ✅ |
| [25-Documentation](./25-Documentation/) | Swagger, OpenAPI, XML Comments | ✅ |

### Fase 4 — DevOps & Scalability

| Modul | Topik | Status |
|-------|-------|--------|
| [21-Docker](./21-Docker/) | Containerization, Dockerfile, Docker Compose | ✅ |
| [22-CICD](./22-CICD/) | GitHub Actions, Automated Testing, Deployment | ✅ |
| [26-Algorithm](./26-Algorithm/) | Data Structures, Time Complexity, LeetCode | ✅ |
| [27-Concurrency](./27-Concurrency/) | async/await, Task, Threading, Mutex | ✅ |
| [28-Microservices](./28-Microservices/) | API Gateway, Service Discovery, gRPC | ✅ |
| [29-Cloud](./29-Cloud/) | Azure/AWS, App Service, Blob Storage | ✅ |
| [30-Debugging](./30-Debugging/) | Profiling, Memory Leaks, Diagnostic Tools | ✅ |

---

## 🎤 Interview Questions

Lihat folder [InterviewQuestions/](./InterviewQuestions/) untuk **100+ soal interview** dengan:
- ✅ Deskripsi soal
- ✅ Requirement
- ✅ Hint
- ✅ Solusi lengkap
- ✅ Pembahasan
- ✅ Kode C# siap pakai
- ✅ Alternatif solusi

### 🧑‍💻 Coding Challenges Interaktif

Jalankan `InterviewQuestions/Source/` sebagai **Console App** untuk 30 soal coding C# yang langsung tereksekusi:

```bash
cd InterviewQuestions/Source
dotnet run
```

Mencakup kategori: **String Manipulation**, **Array & Collection**, **LINQ**, **Recursion & DP**, **OOP & Design Patterns** (Singleton, Builder, Strategy, ISP).

---

## 🛠️ Tech Stack

| Kategori | Teknologi |
|----------|-----------|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server Developer/Express |
| Auth | JWT Bearer + Refresh Token |
| Validation | FluentValidation |
| Logging | Serilog |
| Testing | xUnit + Moq |
| Caching | Redis (StackExchange.Redis) |
| Queue | RabbitMQ / Hangfire |
| Documentation | Swagger / OpenAPI |

---

## 📖 Cara Belajar yang Disarankan

```
1. Baca README.md      → Pahami konsep & tujuan modul
2. Baca Theory.md      → Pelajari teori mendalam + diagram
3. Baca Source/        → Pelajari implementasi kode langkah demi langkah
                          (Komentar teks-buku menjelaskan setiap baris)
4. Jalankan Test.http  → Coba semua endpoint di VS Code REST Client
5. Baca CheatSheet.md  → Hafalkan snippet penting untuk interview
6. dotnet run          → Jalankan InterviewQuestions/Source untuk latihan 30 soal coding
```

> 💡 **Tips:** Setiap file Source Code memiliki komentar bergaya teks-buku. Baca komentar dari atas ke bawah — Anda akan langsung memahami desain arsitektur, keputusan teknis, dan konsep yang diterapkan tanpa perlu buku referensi tambahan.

---

## 🤝 Kontribusi

Repository ini bersifat personal untuk persiapan interview. Namun jika Anda ingin berkontribusi:
1. Fork repository ini
2. Buat branch baru: `git checkout -b feature/nama-fitur`
3. Commit: `git commit -m 'Add: deskripsi perubahan'`
4. Push: `git push origin feature/nama-fitur`
5. Buat Pull Request

---

## 📄 Lisensi

MIT License — bebas digunakan untuk pembelajaran.

---

<div align="center">

**Muhammad Jurnalies Habibie**

*"The best way to learn is to build something."*

</div>
