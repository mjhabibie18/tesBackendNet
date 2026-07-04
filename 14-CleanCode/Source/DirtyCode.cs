// ============================================================
// DirtyCode.cs — Contoh Kode Kotor (Melanggar Clean Code)
// ============================================================
// File ini mendemonstrasikan penamaan variabel buruk, magic numbers,
// method gemuk (fat method), dan duplikasi kode.
// ============================================================

namespace TesBackendNet.CleanCode;

public class DirtyCode
{
    // UAS = User Account Service (Sangat membingungkan)
    public class UAS
    {
        // Variabel penampung user
        public List<string> u = new();

        // ❌ Method terlalu panjang, melakukan validasi, registrasi, db save, email sending sekaligus
        public void ProcessUserAndSaveAndSendEmail(string un, string p, int age, string email)
        {
            // Validasi (Duplikasi di tempat lain jika ada)
            if (string.IsNullOrEmpty(un) || un.Length < 3)
            {
                Console.WriteLine("Bad: Username invalid");
                return;
            }

            // Magic number: 17
            if (age < 17)
            {
                Console.WriteLine("Bad: Umur tidak cukup");
                return;
            }

            // Simpan ke database dummy
            u.Add(un);
            Console.WriteLine($"Bad: Menyimpan {un} ke DB...");

            // Logic email yang dicampur aduk
            Console.WriteLine($"Bad: Mengirim email registrasi ke {email}...");
        }
    }
}
