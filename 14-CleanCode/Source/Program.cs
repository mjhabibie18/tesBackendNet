// ============================================================
// Program.cs — Entry Point Demo Clean Code
// ============================================================

using static TesBackendNet.CleanCode.DirtyCode;
using static TesBackendNet.CleanCode.CleanCode;

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🚀 DEMO PENERAPAN CLEAN CODE VS DIRTY CODE");
Console.WriteLine("============================================================\n");

// ── 1. Demo Kode Kotor (Dirty Code) ───────────────────────────
Console.WriteLine("--- [DIRTY CODE RUN] ---");
var dirtyService = new UAS();
dirtyService.ProcessUserAndSaveAndSendEmail("hb", "pass123", 16, "habibie@mail.com"); // Gagal: Username terlalu pendek & Umur < 17
dirtyService.ProcessUserAndSaveAndSendEmail("habibie", "pass123", 25, "habibie@mail.com"); // Sukses
Console.WriteLine();

// ── 2. Demo Kode Bersih (Clean Code) ──────────────────────────
Console.WriteLine("--- [CLEAN CODE RUN] ---");
var cleanService = new UserAccountService();
cleanService.RegisterUser("hb", "pass123", 16, "habibie@mail.com"); // Gagal
cleanService.RegisterUser("habibie", "pass123", 25, "habibie@mail.com"); // Sukses

Console.WriteLine("\n============================================================");
Console.WriteLine("Perhatikan bagaimana Clean Code membagi tugas menjadi method");
Console.WriteLine("kecil yang terfokus (SRP), menggunakan nama konstanta yang jelas,");
Console.WriteLine("dan menghasilkan output log yang lebih rapi.");
Console.WriteLine("============================================================");
