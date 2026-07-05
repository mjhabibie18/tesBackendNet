// ============================================================
// Nama File: CleanCode.cs — Contoh Kode Bersih (Clean Code)
// Folder: 14-CleanCode/Source/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengimplementasikan ulang logika yang sama dengan DirtyCode.cs menggunakan prinsip Clean Code.
//    - Mengapa Diperlukan: Menunjukkan bahwa kode yang rapi tidak harus lebih panjang atau kompleks, hanya lebih mudah dibaca dan dipahami.
//    - Hubungan File: Pasangan langsung dari DirtyCode.cs. Selalu baca keduanya berdampingan untuk memahami perbedaannya.
//    - Jika Dihapus: Tidak ada solusi Clean Code yang dapat dibandingkan dengan DirtyCode.cs.
// ============================================================

namespace TesBackendNet.CleanCode;

/// <summary>
/// TUJUAN CLASS:
/// Implementasi ulang logika dari DirtyCode.cs menggunakan prinsip-prinsip Clean Code:
/// 
/// 1. MEANINGFUL NAMES: Nama class, method, dan variabel mengungkapkan niat secara eksplisit.
/// 2. NAMED CONSTANTS: Magic number diganti dengan konstanta bernama yang menjelaskan konteks bisnisnya.
/// 3. SMALL METHODS (SRP): Setiap method memiliki satu tanggung jawab tunggal dan dapat diuji secara independen.
/// 4. DRY (Don't Repeat Yourself): Logika validasi dipisahkan ke method tersendiri sehingga tidak perlu disalin ulang.
/// </summary>
public class CleanCode
{
    /// <summary>
    /// TUJUAN CLASS:
    /// Service yang bertanggung jawab untuk proses registrasi pengguna baru.
    /// Nama 'UserAccountService' langsung menjelaskan tujuan class ini tanpa perlu dokumentasi tambahan.
    /// </summary>
    public class UserAccountService
    {
        private readonly List<string> _users = new();

        // ── PRINSIP: Named Constants (Mengganti Magic Number) ────────
        // Sekarang jelas bahwa angka 17 adalah "usia minimum yang diperlukan".
        // Jika kebijakan berubah menjadi 18, cukup ubah di satu tempat ini.
        private const int MinimumRequiredAge      = 17;
        private const int MinimumUsernameLength   = 3;

        /// <summary>
        /// FUNGSI METHOD: Mengorkestrasikan proses registrasi pengguna baru.
        /// PARAMETER: username, password, age, email — semua dengan nama yang jelas.
        /// 
        /// ALASAN METODE KECIL (Small Methods):
        /// RegisterUser hanya bertanggung jawab mengatur alur. Logika detail didelegasikan ke:
        ///  - IsValidUsername() → validasi format username
        ///  - IsAgeEligible() → validasi usia
        ///  - SaveUserToDatabase() → persistensi data
        ///  - SendWelcomeEmail() → notifikasi
        /// Jika logika email berubah, kita hanya mengubah SendWelcomeEmail() tanpa menyentuh RegisterUser().
        /// </summary>
        public void RegisterUser(string username, string password, int age, string email)
        {
            // ── 1. Validasi Input ───────────────────────────────────
            if (!IsValidUsername(username))
            {
                Console.WriteLine("[Clean]: Username tidak valid.");
                return;
            }

            if (!IsAgeEligible(age))
            {
                Console.WriteLine("[Clean]: Pendaftar belum cukup umur.");
                return;
            }

            // ── 2. Proses Bisnis & Simpan ke Database ────────────────
            SaveUserToDatabase(username);

            // ── 3. Kirim Notifikasi Email ────────────────────────────
            SendWelcomeEmail(email);
        }

        /// <summary>
        /// FUNGSI METHOD: Memvalidasi format username.
        /// NILAI KEMBALIAN: bool — true jika valid, false jika tidak.
        /// 
        /// ALASAN STATIC:
        /// Method ini tidak menggunakan state instance (tidak mengakses _users atau field lain),
        /// sehingga ditandai `static` untuk menghemat overhead alokasi memori.
        /// </summary>
        private static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username) && username.Length >= MinimumUsernameLength;
        }

        /// <summary>
        /// FUNGSI METHOD: Memeriksa apakah usia memenuhi syarat minimum.
        /// NILAI KEMBALIAN: bool — true jika memenuhi syarat.
        /// </summary>
        private static bool IsAgeEligible(int age)
        {
            return age >= MinimumRequiredAge;
        }

        /// <summary>
        /// FUNGSI METHOD: Menyimpan data pengguna ke lapisan penyimpanan.
        /// </summary>
        private void SaveUserToDatabase(string username)
        {
            _users.Add(username);
            Console.WriteLine($"[Clean DB]: User '{username}' berhasil disimpan ke database.");
        }

        /// <summary>
        /// FUNGSI METHOD: Mengirimkan email sambutan ke pengguna yang baru mendaftar.
        /// </summary>
        private void SendWelcomeEmail(string email)
        {
            Console.WriteLine($"[Clean Email]: Welcome email berhasil dikirim ke {email}.");
        }
    }
}
