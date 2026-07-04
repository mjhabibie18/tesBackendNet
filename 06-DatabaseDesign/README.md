# 🗂️ 06 — Database Design

## Konsep Dasar

Database Design adalah proses merancang struktur database yang optimal, efisien, dan mudah dimaintain.

---

## ERD (Entity Relationship Diagram)

ERD adalah diagram yang menggambarkan entitas, atribut, dan relasi antar entitas.

```
┌─────────────┐         ┌─────────────┐
│   Category  │         │   Product   │
├─────────────┤         ├─────────────┤
│ PK: Id      │────────<│ PK: Id      │
│ Name        │  1 to N │ FK: CategoryId
│ Description │         │ Name        │
└─────────────┘         │ Price       │
                        │ Stock       │
                        └─────────────┘
```

---

## Primary Key (PK)

```sql
-- Surrogate key (recommended): auto-generated, tidak bermakna bisnis
Id INT IDENTITY(1,1) PRIMARY KEY

-- Natural key: punya makna bisnis
NIK CHAR(16) PRIMARY KEY  -- Nomor Induk Kependudukan
```

---

## Foreign Key (FK)

```sql
-- Mendefinisikan relasi antar tabel
ALTER TABLE Products
ADD CONSTRAINT FK_Products_Categories
FOREIGN KEY (CategoryId)
REFERENCES Categories(Id)
ON DELETE CASCADE  -- Hapus category = hapus semua product-nya
ON DELETE SET NULL -- Hapus category = CategoryId jadi NULL
```

---

## Normalisasi

| Bentuk Normal | Aturan |
|---------------|--------|
| **1NF** | Setiap sel hanya satu nilai (tidak ada array) |
| **2NF** | 1NF + tidak ada partial dependency pada composite PK |
| **3NF** | 2NF + tidak ada transitive dependency |
| **BCNF** | 3NF lebih ketat |

---

## Index

```sql
-- Clustered Index: urutan fisik tabel
-- Default: Primary Key otomatis jadi Clustered Index

-- Non-Clustered Index: struktur terpisah
CREATE INDEX IX_Products_Name ON Products(Name);
CREATE INDEX IX_Products_Price ON Products(Price);

-- Composite Index
CREATE INDEX IX_Orders_Status_Date ON Orders(Status, CreatedAt);

-- Unique Index: mencegah duplikat
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
```

---

## Constraint

```sql
-- NOT NULL
Name NVARCHAR(200) NOT NULL

-- UNIQUE
Email NVARCHAR(200) NOT NULL UNIQUE

-- CHECK
Price DECIMAL(18,2) CHECK (Price >= 0)
Age INT CHECK (Age >= 0 AND Age <= 150)

-- DEFAULT
IsDeleted BIT NOT NULL DEFAULT 0
CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
```

---

## 🎤 Tips Interview

**Q: "Kapan pakai Index?"**
```
Gunakan Index pada kolom yang sering di-WHERE, JOIN, ORDER BY.
Jangan index semua kolom: write operations jadi lambat karena index perlu update.
```

**Q: "Apa bedanya Clustered dan Non-Clustered Index?"**
```
Clustered: data fisik disusun sesuai index (hanya 1 per tabel)
Non-Clustered: struktur index terpisah dari data (bisa banyak per tabel)
```
