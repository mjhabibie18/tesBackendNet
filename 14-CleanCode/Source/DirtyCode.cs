// ============================================================
// Nama File: DirtyCode.cs — Contoh Kode Kotor (Anti-Pattern)
// Folder: 14-CleanCode/Source/
// ============================================================
// 1. PENJELASAN FOLDER (CleanCode):
//    - Tujuan: Memperbandingkan kode yang buruk (Dirty Code) dengan kode yang bersih (Clean Code).
//    - Kapan Digunakan: Sebagai referensi saat melakukan code review atau refactoring.
//    - Hubungan: File ini adalah pasangan dari CleanCode.cs yang menyelesaikan masalah yang sama dengan cara yang benar.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendokumentasikan anti-pattern penulisan kode yang sering ditemukan di codebase nyata.
//    - Mengapa Diperlukan: Memahami kode buruk sama pentingnya dengan memahami kode baik, agar dapat mengenali dan menghindarinya.
//    - Jika Dihapus: Tidak ada perbandingan konkret yang memperlihatkan perbedaan nyata antara kode buruk dan bersih.
// ============================================================

namespace TesBackendNet.CleanCode;

/// <summary>
/// TUJUAN CLASS:
/// Kumpulan contoh penulisan kode yang melanggar prinsip Clean Code.
/// 
/// ANTI-PATTERN YANG DIDEMONSTRASIKAN:
/// 1. Penamaan Misterius (Cryptic Naming): Nama class/variabel yang tidak bermakna (UAS, u, un, p).
/// 2. Magic Number: Angka `17` tanpa penjelasan konteks bisnis apapun.
/// 3. Fat Method (God Method): Satu method mengerjakan validasi, penyimpanan, dan pengiriman email sekaligus.
/// 4. Duplikasi Kode: Logika validasi username/usia berpotensi disalin-tempel di banyak tempat.
/// </summary>
public class DirtyCode
{
    // ❌ ANTI-PATTERN 1: Nama class yang tidak bermakna
    // "UAS" tidak memberitahu developer apa yang dilakukan class ini.
    // Developer baru harus menebak atau membaca seluruh kode untuk memahaminya.
    public class UAS
    {
        // ❌ ANTI-PATTERN 2: Nama variabel satu huruf dan tidak bermakna
        // `u` tidak menjelaskan bahwa ini adalah daftar pengguna terdaftar.
        public List<string> u = new();

        // ❌ ANTI-PATTERN 3: Method terlalu gemuk (Fat Method)
        // Method ini melakukan 4 tanggung jawab sekaligus:
        //   1. Validasi username
        //   2. Validasi usia
        //   3. Penyimpanan ke database
        //   4. Pengiriman email
        // Jika salah satu logika berubah, kita harus mengubah method ini dan berisiko merusak bagian lainnya.
        //
        // ❌ ANTI-PATTERN 4: Nama parameter yang tidak jelas
        // `un` (username?), `p` (password?), padahal dapat langsung ditulis `username`, `password`.
        public void ProcessUserAndSaveAndSendEmail(string un, string p, int age, string email)
        {
            if (string.IsNullOrEmpty(un) || un.Length < 3)
            {
                Console.WriteLine("Bad: Username invalid");
                return;
            }

            // ❌ ANTI-PATTERN 5: Magic Number (17)
            // Mengapa 17? Apakah ini usia minimal? Apakah akan berubah menjadi 18 nanti?
            // Angka ini tersebar di berbagai tempat di kode, dan jika harus diubah, developer 
            // harus mencari semua kemunculannya secara manual (rentan terlewat).
            if (age < 17)
            {
                Console.WriteLine("Bad: Umur tidak cukup");
                return;
            }

            // Logika penyimpanan DB dan pengiriman email yang tercampur aduk dalam satu method
            u.Add(un);
            Console.WriteLine($"Bad: Menyimpan {un} ke DB...");
            Console.WriteLine($"Bad: Mengirim email registrasi ke {email}...");
        }
    }
}
