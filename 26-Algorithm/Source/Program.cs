// ============================================================
// Nama File: Program.cs — Demo Algoritma & Struktur Data (LeetCode-Style)
// Folder: 26-Algorithm/Source/
// ============================================================
// 1. PENJELASAN FOLDER (Algorithm):
//    - Tujuan: Menyediakan implementasi C# dari algoritma dan struktur data yang paling sering muncul
//      dalam technical interview (LeetCode, HackerRank, Codility) khususnya untuk posisi Backend Developer.
//    - Kapan Digunakan: Sebagai referensi belajar dan latihan sebelum menghadapi sesi coding interview.
//    - Hubungan: Proyek ini berdiri sendiri (Console App) dan tidak bergantung pada modul lain.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menjalankan demo 4 algoritma penting dengan visualisasi output di konsol.
//    - Mengapa Diperlukan: Membandingkan secara langsung kompleksitas waktu (O(n) vs O(log n) vs O(2^n))
//      dengan mengukur waktu eksekusi nyata menggunakan Stopwatch.
//    - Jika Dihapus: Tidak ada demonstrasi algoritma yang dapat dijalankan pada modul ini.
//
// 3. NOTASI KOMPLEKSITAS WAKTU (Big-O):
//    - O(1)     : Konstan — waktu tetap tidak peduli besar input (akses elemen array via index).
//    - O(log n) : Logaritmik — sangat cepat, input dibagi dua setiap langkah (Binary Search).
//    - O(n)     : Linear — waktu bertumbuh proporsional dengan input (Two Sum, Memoization).
//    - O(n²)    : Kuadratik — nested loop, lambat untuk input besar (Bubble Sort naif).
//    - O(2^n)   : Eksponensial — sangat lambat, harus dihindari (Fibonacci rekursif naif).
// ============================================================

using System.Diagnostics;

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🚀 DEMO ALGORITMA & STRUKTUR DATA (LEETCODE PREPARATION)");
Console.WriteLine("============================================================\n");

// ── 1. Binary Search Demo (O(log n)) ─────────────────────────────────
Console.WriteLine("--- 1. BINARY SEARCH ---");
int[] sortedArray = { 3, 9, 12, 17, 24, 38, 45, 56, 72, 90 };
int target = 45;
int index  = BinarySearch(sortedArray, target);
Console.WriteLine($"Sorted Array: [{string.Join(", ", sortedArray)}]");
Console.WriteLine($"Cari angka {target} -> Ditemukan pada indeks: {index}");
Console.WriteLine();

// ── 2. Fibonacci Memoization vs Naive (O(n) vs O(2^n)) ──────────────
Console.WriteLine("--- 2. FIBONACCI (NAIVE VS MEMOIZATION) ---");
int fibN = 40; // Fibonacci ke-40

var MemoCache = new Dictionary<int, long>(); // Cache untuk memoization
var sw        = Stopwatch.StartNew();

long resultMemo = FibMemo(fibN);
sw.Stop();
var timeMemo = sw.ElapsedMilliseconds;

Console.WriteLine($"[Memoization O(n)]: Hasil Fib({fibN}) = {resultMemo} (Waktu: {timeMemo} ms)");
Console.WriteLine("[Naive O(2^n)]: Fib(40) naive membutuhkan miliaran operasi rekursif...");

// Demo Naive hanya sampai Fib(30) untuk menghindari hang (Fib(40) bisa > 10 detik)
sw.Restart();
long resultNaive = FibNaive(30);
sw.Stop();
Console.WriteLine($"[Naive O(2^n)]: Hasil Fib(30) = {resultNaive} (Waktu: {sw.ElapsedMilliseconds} ms)");
Console.WriteLine();

// ── 3. Two Sum Demo (O(n)) ────────────────────────────────────────────
Console.WriteLine("--- 3. TWO SUM (LEETCODE #1) ---");
int[] nums          = { 2, 7, 11, 15 };
int twoSumTarget    = 9;
int[] twoSumIndices = TwoSum(nums, twoSumTarget);
Console.WriteLine($"Nums Array: [{string.Join(", ", nums)}], Target: {twoSumTarget}");
Console.WriteLine($"Indeks hasil penjumlahan: [{string.Join(", ", twoSumIndices)}]");
Console.WriteLine();

// ── 4. Valid Parentheses (Stack Demo) ────────────────────────────────
Console.WriteLine("--- 4. VALID PARENTHESES (LEETCODE #20) ---");
string expression1 = "{[()]}";
string expression2 = "([)]";
Console.WriteLine($"Apakah '{expression1}' Valid? {IsValidParentheses(expression1)}"); // True
Console.WriteLine($"Apakah '{expression2}' Valid? {IsValidParentheses(expression2)}"); // False

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Algoritma Selesai!");
Console.WriteLine("============================================================");

// =========================================================================
// IMPLEMENTASI FUNGSI ALGORITMA (Local Functions — C# Top-Level Statements)
// =========================================================================
// CATATAN C# Top-Level Statements:
// File ini menggunakan fitur C# 9+ dimana kode dapat ditulis langsung tanpa class/method pembungkus.
// Fungsi yang dideklarasikan di bawah ini adalah "local functions" yang dapat membaca variabel di luar scope-nya (closure).

// ── ALGORITMA 1: BINARY SEARCH ───────────────────────────────────────
// PRINSIP: Divide and Conquer — bagi array menjadi dua setiap iterasi.
// SYARAT: Array HARUS sudah terurut (sorted) agar teknik ini bekerja.
// KOMPLEKSITAS: O(log n) — untuk array 1.000.000 elemen, hanya butuh ~20 langkah.
// CONTOH PENERAPAN: Pencarian di phone book, binary search tree, range query database index.
int BinarySearch(int[] arr, int target)
{
    int left  = 0;
    int right = arr.Length - 1;

    while (left <= right)
    {
        // PENTING: `left + (right - left) / 2` bukan `(left + right) / 2`
        // Alasan: (left + right) bisa overflow integer jika nilai keduanya sangat besar.
        int mid = left + (right - left) / 2;

        if (arr[mid] == target) return mid;   // Ditemukan! Kembalikan indeks.
        
        if (arr[mid] < target)
            left = mid + 1;   // Target ada di sisi kanan → abaikan setengah kiri
        else
            right = mid - 1;  // Target ada di sisi kiri → abaikan setengah kanan
    }

    return -1; // Target tidak ada dalam array
}

// ── ALGORITMA 2A: FIBONACCI NAIVE (REKURSIF) ─────────────────────────
// PRINSIP: Rekursi langsung tanpa cache.
// MASALAH: Menghitung FibNaive(40) melakukan FibNaive(39)+FibNaive(38), dan masing-masing
//          memanggil dirinya lagi → pohon rekursi berbentuk eksponensial.
// KOMPLEKSITAS: O(2^n) — sangat lambat untuk n > 35.
long FibNaive(int n)
{
    if (n <= 1) return n; // Base case: Fib(0)=0, Fib(1)=1
    return FibNaive(n - 1) + FibNaive(n - 2); // Rekursi berganda
}

// ── ALGORITMA 2B: FIBONACCI MEMOIZATION (DYNAMIC PROGRAMMING) ────────
// PRINSIP: Simpan hasil komputasi yang sudah dilakukan (cache) agar tidak dihitung ulang.
// KOMPLEKSITAS: O(n) — setiap nilai Fib(i) hanya dihitung SEKALI, sisanya diambil dari cache.
// INI ADALAH INTI dari Dynamic Programming: "Jangan hitung yang sudah pernah dihitung."
long FibMemo(int n)
{
    if (n <= 1) return n; // Base case
    
    // Jika sudah ada di cache, langsung kembalikan hasilnya (O(1) lookup)
    if (MemoCache.ContainsKey(n)) return MemoCache[n];

    // Hitung dan simpan ke cache sebelum dikembalikan
    MemoCache[n] = FibMemo(n - 1) + FibMemo(n - 2);
    return MemoCache[n];
}

// ── ALGORITMA 3: TWO SUM (LEETCODE #1) ───────────────────────────────
// MASALAH: Diberikan array nums dan nilai target, temukan dua indeks i dan j
//          sehingga nums[i] + nums[j] == target.
// SOLUSI OPTIMAL: Gunakan HashMap untuk menyimpan nilai yang sudah dilihat.
//   Untuk setiap nums[i], cari apakah "complement" (target - nums[i]) sudah ada di map.
// KOMPLEKSITAS: O(n) — satu kali iterasi array + O(1) lookup di HashMap.
// SOLUSI NAIF (Nested Loop): O(n²) — dua kali iterasi, jauh lebih lambat.
int[] TwoSum(int[] nums, int target)
{
    // HashMap: Key = nilai elemen, Value = indeks posisinya dalam array
    var map = new Dictionary<int, int>();

    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i]; // Nilai yang kita cari

        // Jika complement sudah pernah kita lihat sebelumnya → pasangan ditemukan!
        if (map.ContainsKey(complement))
        {
            return new int[] { map[complement], i };
        }

        // Simpan nilai nums[i] beserta indeksnya ke map untuk referensi ke depan
        map[nums[i]] = i;
    }

    return Array.Empty<int>(); // Tidak ada pasangan yang ditemukan
}

// ── ALGORITMA 4: VALID PARENTHESES (LEETCODE #20) ────────────────────
// MASALAH: Cek apakah string berisi kurung pembuka-penutup yang valid dan berpasangan.
// SOLUSI: Gunakan Stack untuk melacak kurung yang belum ditutup.
//   - Kurung buka: push ke stack.
//   - Kurung tutup: pop dari stack dan verifikasi pasangannya cocok.
// KOMPLEKSITAS: O(n) — iterasi string satu kali.
// STRUKTUR DATA: Stack (LIFO — Last In, First Out) adalah solusi alami untuk masalah ini.
bool IsValidParentheses(string s)
{
    var stack = new Stack<char>(); // Stack untuk menyimpan kurung buka yang belum ditutup
    
    foreach (char c in s)
    {
        if (c == '(' || c == '[' || c == '{')
        {
            stack.Push(c); // Kurung buka → masukkan ke stack (menunggu pasangannya)
        }
        else
        {
            // Kurung tutup: stack harus ada isinya (kurung buka yang menunggu)
            if (stack.Count == 0) return false; // Kurung tutup tanpa pasangan → invalid

            char top = stack.Pop(); // Ambil kurung buka terakhir yang belum dipasangkan

            // Periksa apakah kurung tutup cocok dengan kurung buka yang ada di stack
            if (c == ')' && top != '(') return false;
            if (c == ']' && top != '[') return false;
            if (c == '}' && top != '{') return false;
        }
    }

    // Jika stack kosong setelah semua karakter diproses → semua kurung berpasangan → valid
    return stack.Count == 0;
}
