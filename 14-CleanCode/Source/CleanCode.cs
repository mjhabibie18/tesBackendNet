// ============================================================
// CleanCode.cs — Contoh Kode Bersih (Clean Code)
// ============================================================
// Menyelesaikan masalah di DirtyCode.cs dengan penamaan bermakna,
// konstanta bernama, dan pemecahan method berdasarkan tugasnya (SRP).
// ============================================================

namespace TesBackendNet.CleanCode;

public class CleanCode
{
    public class UserAccountService
    {
        private readonly List<string> _users = new();

        // Mengganti magic number dengan konstanta yang jelas maknanya
        private const int MinimumRequiredAge = 17;
        private const int MinimumUsernameLength = 3;

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

            // ── 2. Proses Bisnis & DB ────────────────────────────────
            SaveUserToDatabase(username);

            // ── 3. Pengiriman Email ─────────────────────────────────
            SendWelcomeEmail(email);
        }

        private static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username) && username.Length >= MinimumUsernameLength;
        }

        private static bool IsAgeEligible(int age)
        {
            return age >= MinimumRequiredAge;
        }

        private void SaveUserToDatabase(string username)
        {
            _users.Add(username);
            Console.WriteLine($"[Clean DB]: User '{username}' berhasil disimpan ke database.");
        }

        private void SendWelcomeEmail(string email)
        {
            Console.WriteLine($"[Clean Email]: Welcome email berhasil dikirim ke {email}.");
        }
    }
}
