// ============================================================
// Program.cs — Simulasi Aliran Kerja Git CLI (Interactive)
// ============================================================
// Proyek ini memenuhi persyaratan sebagai proyek mandiri yang dapat
// dijalankan secara interaktif untuk melatih pemahaman alur kerja Git.
// ============================================================

using System;
using System.Collections.Generic;

class Program
{
    private static readonly List<string> WorkingDirectory = new() { "ProductController.cs", "ProductService.cs", "appsettings.json" };
    private static readonly List<string> StagingArea = new();
    private static readonly List<Commit> LocalRepository = new();
    private static bool _isPushedToRemote = false;

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("============================================================");
            Console.WriteLine("🐙 GIT WORKFLOW SIMULATOR CLI — PERSAPAN CODING TEST & INTERVIEW");
            Console.WriteLine("============================================================");
            Console.ResetColor();

            // Render Visualisasi 3 Area Utama Git
            RenderGitAreas();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nPilih perintah Git untuk disimulasikan:");
            Console.WriteLine("1. git status             - Cek status file di Working Directory & Staging Area");
            Console.WriteLine("2. git add <file>         - Pindahkan file dari Working Directory ke Staging Area");
            Console.WriteLine("3. git commit -m <msg>    - Simpan file yang ada di Staging ke Local History");
            Console.WriteLine("4. git log                - Tampilkan riwayat commit local");
            Console.WriteLine("5. git push origin main   - Kirim commit lokal ke Remote Repository (GitHub)");
            Console.WriteLine("6. Reset Simulator        - Kembalikan keadaan awal file");
            Console.WriteLine("0. Keluar");
            Console.Write("\nMasukkan pilihan (0-6): ");
            Console.ResetColor();

            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    GitStatus();
                    break;
                case "2":
                    GitAddPrompt();
                    break;
                case "3":
                    GitCommitPrompt();
                    break;
                case "4":
                    GitLog();
                    break;
                case "5":
                    GitPush();
                    break;
                case "6":
                    ResetSimulator();
                    break;
                case "0":
                    Console.WriteLine("Terima kasih telah menggunakan Git Simulator!");
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Pilihan tidak valid!");
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine("\nTekan [ENTER] untuk melanjutkan...");
            Console.ReadLine();
        }
    }

    private static void RenderGitAreas()
    {
        Console.WriteLine("\n[1] WORKING DIR (Untracked/Modified)  => [2] STAGING AREA (Staged)  => [3] LOCAL REPO (Committed)");
        Console.WriteLine("-------------------------------------     -------------------------     --------------------------");

        // Baris 1: Working Dir
        Console.Write(" ");
        if (WorkingDirectory.Count == 0) Console.Write("(clean)".PadRight(38));
        else Console.Write(string.Join(", ", WorkingDirectory).PadRight(38));

        // Baris 2: Staging Area
        Console.Write(" | ");
        if (StagingArea.Count == 0) Console.Write("(empty)".PadRight(23));
        else Console.Write(string.Join(", ", StagingArea).PadRight(23));

        // Baris 3: Local Repo History
        Console.Write(" | ");
        if (LocalRepository.Count == 0) Console.Write("No commits yet");
        else Console.Write($"Latest: \"{LocalRepository[^1].Message}\" (Hash: {LocalRepository[^1].Hash[..7]})");
        
        Console.WriteLine();
        Console.WriteLine(new string('=', 95));

        // Status Sinkronisasi Remote
        Console.Write("REMOTE REPO Status: ");
        if (_isPushedToRemote)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("UP TO DATE dengan Local Repository (GitHub)");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("AHEAD (Ada commit lokal yang belum di-push ke GitHub!)");
        }
        Console.ResetColor();
    }

    private static void GitStatus()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("=== $ git status ===");
        Console.ResetColor();

        if (WorkingDirectory.Count == 0 && StagingArea.Count == 0)
        {
            Console.WriteLine("On branch main\nnothing to commit, working tree clean");
            return;
        }

        if (StagingArea.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Changes to be committed:");
            Console.WriteLine("  (use \"git restore --staged <file>...\" to unstage)");
            foreach (var file in StagingArea)
            {
                Console.WriteLine($"\tnew file:   {file}");
            }
            Console.ResetColor();
        }

        if (WorkingDirectory.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Changes not staged for commit:");
            Console.WriteLine("  (use \"git add <file>...\" to update what will be committed)");
            foreach (var file in WorkingDirectory)
            {
                Console.WriteLine($"\tmodified:   {file}");
            }
            Console.ResetColor();
        }
    }

    private static void GitAddPrompt()
    {
        if (WorkingDirectory.Count == 0)
        {
            Console.WriteLine("Semua file sudah masuk Staging Area.");
            return;
        }

        Console.WriteLine("Pilih file yang ingin di-add:");
        for (int i = 0; i < WorkingDirectory.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {WorkingDirectory[i]}");
        }
        Console.WriteLine($"{WorkingDirectory.Count + 1}. ALL FILES (git add .)");
        Console.Write("Pilih nomor file: ");
        
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            if (index == WorkingDirectory.Count + 1)
            {
                StagingArea.AddRange(WorkingDirectory);
                WorkingDirectory.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Seluruh file berhasil dipindahkan ke Staging Area!");
            }
            else if (index > 0 && index <= WorkingDirectory.Count)
            {
                string file = WorkingDirectory[index - 1];
                StagingArea.Add(file);
                WorkingDirectory.RemoveAt(index - 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"File '{file}' berhasil dipindahkan ke Staging Area!");
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }
        Console.ResetColor();
    }

    private static void GitCommitPrompt()
    {
        if (StagingArea.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Gagal commit: Staging area kosong! Jalankan 'git add' terlebih dahulu.");
            Console.ResetColor();
            return;
        }

        Console.Write("Masukkan pesan commit: ");
        string? message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message)) message = "Minor updates";

        var commit = new Commit(message, new List<string>(StagingArea));
        LocalRepository.Add(commit);
        StagingArea.Clear();
        _isPushedToRemote = false; // Ada commit baru yang belum di-push

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[main {commit.Hash[..7]}] {commit.Message}");
        Console.WriteLine($" {commit.Files.Count} files changed, committed successfully to local history.");
        Console.ResetColor();
    }

    private static void GitLog()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("=== $ git log ===");
        Console.ResetColor();

        if (LocalRepository.Count == 0)
        {
            Console.WriteLine("Belum ada riwayat commit.");
            return;
        }

        for (int i = LocalRepository.Count - 1; i >= 0; i--)
        {
            var commit = LocalRepository[i];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"commit {commit.Hash}");
            Console.ResetColor();
            Console.WriteLine($"Author: Developer <dev@tesbackend.net>");
            Console.WriteLine($"Date:   {commit.Date}");
            Console.WriteLine($"\n    {commit.Message}");
            Console.WriteLine("    Files: " + string.Join(", ", commit.Files));
            Console.WriteLine(new string('-', 50));
        }
    }

    private static void GitPush()
    {
        if (LocalRepository.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Tidak ada commit lokal yang bisa di-push.");
            Console.ResetColor();
            return;
        }

        if (_isPushedToRemote)
        {
            Console.WriteLine("Everything up-to-date.");
            return;
        }

        Console.WriteLine("Connecting to origin/main (GitHub)...");
        System.Threading.Thread.Sleep(1000);
        
        _isPushedToRemote = true;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Push Berhasil! Seluruh riwayat lokal sinkron dengan GitHub.");
        Console.ResetColor();
    }

    private static void ResetSimulator()
    {
        WorkingDirectory.Clear();
        WorkingDirectory.AddRange(new[] { "ProductController.cs", "ProductService.cs", "appsettings.json" });
        StagingArea.Clear();
        LocalRepository.Clear();
        _isPushedToRemote = false;
        Console.WriteLine("Simulator di-reset ke kondisi awal.");
    }
}

class Commit
{
    public string Hash { get; }
    public string Message { get; }
    public DateTime Date { get; }
    public List<string> Files { get; }

    public Commit(string message, List<string> files)
    {
        Hash = Guid.NewGuid().ToString().Replace("-", "");
        Message = message;
        Date = DateTime.Now;
        Files = files;
    }
}
