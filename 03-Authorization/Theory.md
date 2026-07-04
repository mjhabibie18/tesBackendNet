# Theory: Authorization & RBAC

## 1. Authentication vs Authorization
Dua konsep ini sering membingungkan, padahal fungsinya sangat berbeda:
- **Authentication (AuthN)**: Proses memverifikasi **siapa Anda**. Contoh: Login pakai email & password, FaceID, OTP. Outputnya adalah identitas (User ID, Name, Email).
- **Authorization (AuthZ)**: Proses memverifikasi **apa yang boleh Anda lakukan**. Contoh: Apakah Anda boleh menghapus data ini? Apakah Anda boleh masuk ke halaman admin?

Analoginya seperti di bandara:
- Menunjukkan paspor di imigrasi = Authentication.
- Membuka pintu VIP Lounge dengan tiket First Class = Authorization.

## 2. Role-Based Access Control (RBAC)
RBAC adalah metode otorisasi paling umum. Di sini, *permission* (hak akses) tidak diberikan langsung ke *user*, melainkan ke *role* (peran). User kemudian ditugaskan ke role tersebut.

- **Kelebihan**: Mudah dikelola. Jika ada pegawai baru (Manager), tinggal masukkan dia ke role `Manager`, otomatis dia punya semua hak akses Manager tanpa perlu di-set satu-satu.
- **Implementasi di JWT**: Role dimasukkan ke dalam *Payload (Claims)* di JWT (misal: `"role": "Admin"`). Server tinggal membaca JWT untuk tahu role user tanpa perlu query database lagi di setiap request.

## 3. Claim-Based Authorization & Policies
Terkadang Role saja tidak cukup. Misalnya: "Hanya user berusia > 21 tahun yang boleh akses". Di sinilah *Claim-Based* atau *Policy-Based Authorization* masuk.
Di ASP.NET Core, kita bisa membuat Policy kustom di `Program.cs` yang mengecek klaim spesifik pada token user, tidak peduli apa role-nya.

## 4. HTTP Status Code: 401 vs 403
Dalam interview, pertanyaan ini sangat sering muncul:
- **401 Unauthorized**: Sebenarnya nama yang lebih tepat adalah "Unauthenticated". Terjadi ketika user belum login, tidak mengirim token, atau tokennya invalid/expired.
- **403 Forbidden**: User SUDAH login dan tokennya VALID, tetapi ia mencoba mengakses *resource* yang bukan haknya (misalnya: User biasa mencoba buka halaman Admin).
