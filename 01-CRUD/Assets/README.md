# Assets — CRUD Module

Folder ini berisi diagram dan gambar untuk modul CRUD.

## Daftar Diagram

### 1. Request Flow Diagram
```
HTTP Client
    │
    ▼ HTTP Request (GET/POST/PUT/DELETE)
[Middleware Pipeline]
    │ GlobalExceptionMiddleware
    │ HTTPS Redirection
    │ CORS
    │ Authorization
    ▼
[ProductController]
    │ Route Matching
    │ Model Binding
    │ Model Validation (DataAnnotations)
    ▼
[ProductService]
    │ Business Logic
    │ Validasi Bisnis (cek duplikat, dll)
    │ Mapping DTO ↔ Entity
    ▼
[ProductRepository]
    │ Query Builder (LINQ)
    │ Search / Filter / Sort / Pagination
    ▼
[AppDbContext (EF Core)]
    │ SQL Generation
    │ Connection Management
    ▼
[SQL Server Database]
    │ Execute SQL
    │ Return Data
    ▼ (balik ke atas)
[HTTP Response JSON]
```

### 2. Database Schema
```sql
CREATE TABLE Products (
    Id          INT             IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200)   NOT NULL,
    Description NVARCHAR(2000)  NULL,
    Price       DECIMAL(18,2)   NOT NULL,
    Stock       INT             NOT NULL DEFAULT 0,
    IsDeleted   BIT             NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2       NULL,
    DeletedAt   DATETIME2       NULL
);

-- Index untuk pencarian dan soft delete
CREATE INDEX IX_Products_Name      ON Products (Name);
CREATE INDEX IX_Products_IsDeleted ON Products (IsDeleted);
```

### 3. Pagination Formula
```
totalPages      = CEIL(totalCount / pageSize)
offset (SKIP)   = (page - 1) * pageSize
fetch  (TAKE)   = pageSize
hasNextPage     = page < totalPages
hasPreviousPage = page > 1
```

## Cara Generate Diagram

Untuk membuat diagram visual, gunakan tools berikut:
- **draw.io** (https://draw.io) — ERD, flowchart
- **PlantUML** — sequence diagram
- **Mermaid** — diagram as code (bisa di markdown GitHub)
