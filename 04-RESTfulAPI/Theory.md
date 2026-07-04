# Teori RESTful API Tingkat Lanjut

Materi ini membahas konsep arsitektur REST, Richardson Maturity Model, Content Negotiation, API Versioning, dan HATEOAS.

---

## 1. Richardson Maturity Model (RMM)
Untuk mengukur seberapa "RESTful" suatu API, Leonard Richardson memperkenalkan model kedewasaan dengan 4 tingkat (Level 0 - Level 3):

*   **Level 0: The Swamp of POX (Plain Old XML)**
    *   Satu endpoint tunggal (misal: `/api/service`) dengan satu HTTP method (biasanya POST).
    *   Request body menentukan aksi apa yang dilakukan (mirip SOAP/RPC).
*   **Level 1: Resources**
    *   Memperkenalkan konsep *Resource* dengan URI unik untuk setiap entitas (misal: `/api/products`, `/api/products/1`).
    *   Namun, interaksinya masih menggunakan satu HTTP Method tunggal.
*   **Level 2: HTTP Verbs**
    *   Menggunakan HTTP Verbs secara benar (GET untuk read, POST untuk create, PUT untuk update, DELETE untuk hapus).
    *   Menggunakan HTTP Status Code yang tepat (201 Created, 404 Not Found, 400 Bad Request).
*   **Level 3: Hypermedia Controls (HATEOAS)**
    *   Tingkat tertinggi. Response dari server mengandung link relasional untuk mengarahkan client ke aksi-aksi berikutnya yang valid.

---

## 2. Content Negotiation
Proses di mana client dan server bernegosiasi format representasi data terbaik untuk dikirimkan melalui jaringan.
*   **Request Header `Accept`**: Client memberi tahu server format apa yang dia inginkan (misal: `Accept: application/xml` atau `Accept: application/json`).
*   **Response Header `Content-Type`**: Server memberi tahu client format data yang dia kirimkan kembali (misal: `Content-Type: application/json; charset=utf-8`).
*   **HTTP 406 Not Acceptable**: Server mengembalikan status ini jika client meminta format data yang tidak disupport oleh server.

---

## 3. HATEOAS (Hypermedia as the Engine of Application State)
Prinsip di mana client berinteraksi dengan aplikasi web sepenuhnya melalui hypermedia yang disediakan secara dinamis dalam response body.
*   **Keuntungan**:
    *   Mencegah *tight coupling* antara client dan server (client tidak perlu tahu struktur URL server sebelumnya).
    *   Server bisa mengubah URL internal tanpa mematahkan kode client.
*   **Kelemahan**:
    *   Response body menjadi lebih besar (overhead data).
    *   Implementasi di sisi client cenderung lebih rumit.

---

## 4. API Versioning
Ketika sistem berkembang, struktur API sering kali harus diubah (breaking change). Versioning memfasilitasi integrasi client lama agar tetap berjalan sementara client baru menggunakan fitur baru.

Ada 3 strategi utama versioning:
1.  **URL Path Versioning** (Paling populer):
    *   Format: `/api/v1/products`
    *   Kelebihan: Sangat jelas dibaca di URL, mudah di-cache oleh proxy.
2.  **Query String Versioning**:
    *   Format: `/api/products?api-version=1.0`
    *   Kelebihan: URL dasar tetap bersih, fleksibel untuk parameter default.
3.  **Header / Accept Versioning (Media Type)**:
    *   Format: Header `X-Version: 1.0` atau `Accept: application/vnd.company.v1+json`
    *   Kelebihan: Bersih secara konseptual REST (URI merepresentasikan resource, bukan versi).
