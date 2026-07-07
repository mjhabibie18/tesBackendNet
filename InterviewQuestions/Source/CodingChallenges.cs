// ============================================================
// Nama File: CodingChallenges.cs — Kumpulan Soal Coding Interview
// Folder: InterviewQuestions/Source/
// ============================================================
// 1. PENJELASAN FOLDER (InterviewQuestions/Source):
//    - Tujuan: Menyediakan solusi C# untuk 30+ soal coding interview yang paling sering muncul.
//    - Kapan Digunakan: Persiapan technical test dan sesi live coding interview.
//    - Hubungan: Dapat dijalankan sebagai Console App. Setiap class berisi satu kategori soal.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengimplementasikan solusi dari berbagai topik:
//      String Manipulation, Array, LINQ, OOP, Recursion, dan Pattern Matching.
//    - Mengapa Diperlukan: Copy-paste siap pakai saat sesi coding dengan waktu terbatas.
//    - Cara Menjalankan: `dotnet run` dari folder ini.
// ============================================================

// ── JALANKAN SEMUA DEMO ──────────────────────────────────────
Console.Clear();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("====================================================");
Console.WriteLine("  CODING INTERVIEW CHALLENGES — C# SOLUTIONS");
Console.WriteLine("====================================================");
Console.ResetColor();

StringChallenges.RunAll();
ArrayChallenges.RunAll();
LinqChallenges.RunAll();
RecursionChallenges.RunAll();
OopChallenges.RunAll();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n====================================================");
Console.WriteLine("  SEMUA DEMO SELESAI!");
Console.WriteLine("====================================================");
Console.ResetColor();

// =========================================================================
// KATEGORI 1: STRING MANIPULATION
// =========================================================================
public static class StringChallenges
{
    public static void RunAll()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n===== KATEGORI 1: STRING MANIPULATION =====");
        Console.ResetColor();

        // ── SOAL 1: Reverse a String ─────────────────────────────
        // Pertanyaan: Balikkan urutan karakter dalam string.
        // Contoh: "hello" → "olleh"
        // Teknik: `ToCharArray()` + `Array.Reverse()` + `new string(...)`
        string original = "hello world";
        char[] chars = original.ToCharArray();
        Array.Reverse(chars);
        string reversed = new string(chars);
        Console.WriteLine($"1. Reverse String: '{original}' → '{reversed}'");

        // ── SOAL 2: Check Palindrome ─────────────────────────────
        // Pertanyaan: Apakah sebuah string merupakan palindrome (sama jika dibaca terbalik)?
        // Contoh: "racecar" → true, "hello" → false
        // Teknik: Bandingkan string asli dengan yang sudah di-reverse.
        string word = "racecar";
        bool isPalindrome = word.SequenceEqual(word.Reverse());
        Console.WriteLine($"2. Palindrome: '{word}' → {isPalindrome}");

        // ── SOAL 3: Count Vowels ─────────────────────────────────
        // Pertanyaan: Hitung jumlah huruf vokal dalam sebuah string.
        // Teknik: LINQ Count + Contains pada set vokal.
        string sentence = "The quick brown fox";
        int vowelCount = sentence.Count(c => "aeiouAEIOU".Contains(c));
        Console.WriteLine($"3. Count Vowels: '{sentence}' → {vowelCount} vokal");

        // ── SOAL 4: Remove Duplicates ────────────────────────────
        // Pertanyaan: Hapus karakter duplikat dari string, pertahankan urutan pertama kali muncul.
        // Contoh: "programming" → "progamin"
        // Teknik: LINQ Distinct() menjaga urutan kemunculan pertama tiap karakter.
        string withDupes = "programming";
        string noDupes = new string(withDupes.Distinct().ToArray());
        Console.WriteLine($"4. Remove Duplicates: '{withDupes}' → '{noDupes}'");

        // ── SOAL 5: Count Words in Sentence ─────────────────────
        // Pertanyaan: Hitung jumlah kata dalam sebuah kalimat.
        // Teknik: Split berdasarkan spasi, filter empty entries.
        string text = "  The quick brown fox jumps  ";
        int wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        Console.WriteLine($"5. Word Count: '{text.Trim()}' → {wordCount} kata");

        // ── SOAL 6: Anagram Check ────────────────────────────────
        // Pertanyaan: Apakah dua string merupakan anagram (mengandung karakter yang sama, hanya urutannya berbeda)?
        // Contoh: "listen" dan "silent" → true
        // Teknik: Sort kedua string dan bandingkan.
        string s1 = "listen", s2 = "silent";
        bool isAnagram = s1.OrderBy(c => c).SequenceEqual(s2.OrderBy(c => c));
        Console.WriteLine($"6. Anagram: '{s1}' & '{s2}' → {isAnagram}");

        // ── SOAL 7: Find Most Frequent Character ─────────────────
        // Pertanyaan: Temukan karakter yang paling sering muncul dalam string.
        // Teknik: GroupBy karakter, OrderByDescending count, ambil First.
        string str = "programming";
        char mostFrequent = str.GroupBy(c => c).OrderByDescending(g => g.Count()).First().Key;
        Console.WriteLine($"7. Most Frequent Char: '{str}' → '{mostFrequent}'");

        // ── SOAL 8: FizzBuzz ─────────────────────────────────────
        // Pertanyaan: Cetak "Fizz" untuk kelipatan 3, "Buzz" untuk kelipatan 5, "FizzBuzz" untuk keduanya.
        // Teknik: Ternary operator / switch expression.
        var fizzBuzz = Enumerable.Range(1, 20)
            .Select(i => i % 15 == 0 ? "FizzBuzz" : i % 3 == 0 ? "Fizz" : i % 5 == 0 ? "Buzz" : i.ToString());
        Console.WriteLine($"8. FizzBuzz (1-20): {string.Join(", ", fizzBuzz)}");
    }
}

// =========================================================================
// KATEGORI 2: ARRAY & COLLECTION
// =========================================================================
public static class ArrayChallenges
{
    public static void RunAll()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n===== KATEGORI 2: ARRAY & COLLECTION =====");
        Console.ResetColor();

        // ── SOAL 9: Find Second Largest Number ───────────────────
        // Pertanyaan: Temukan angka terbesar kedua dalam array.
        // Teknik: OrderByDescending + Distinct + Skip(1) + First.
        int[] nums = { 12, 35, 1, 10, 34, 1 };
        int secondLargest = nums.OrderByDescending(x => x).Distinct().Skip(1).First();
        Console.WriteLine($"9. Second Largest: [{string.Join(", ", nums)}] → {secondLargest}");

        // ── SOAL 10: Two Sum ─────────────────────────────────────
        // Pertanyaan: Temukan dua indeks dengan jumlah sama dengan target. (LeetCode #1)
        // Teknik: HashMap O(n) — simpan komplemen.
        int[] twoSumArr = { 2, 7, 11, 15 };
        int target = 9;
        var map = new Dictionary<int, int>();
        int[] result = Array.Empty<int>();
        for (int i = 0; i < twoSumArr.Length; i++)
        {
            int comp = target - twoSumArr[i];
            if (map.ContainsKey(comp)) { result = new[] { map[comp], i }; break; }
            map[twoSumArr[i]] = i;
        }
        Console.WriteLine($"10. Two Sum: [{string.Join(", ", twoSumArr)}] target={target} → [{string.Join(", ", result)}]");

        // ── SOAL 11: Remove Duplicates from Sorted Array ─────────
        // Pertanyaan: Hapus duplikat dari array terurut menggunakan in-place.
        // Teknik: LINQ Distinct + ToArray.
        int[] sorted = { 0, 0, 1, 1, 1, 2, 2, 3, 3, 4 };
        int[] unique = sorted.Distinct().ToArray();
        Console.WriteLine($"11. Remove Dup (Sorted): [{string.Join(", ", unique)}]");

        // ── SOAL 12: Rotate Array ─────────────────────────────────
        // Pertanyaan: Rotasikan array ke kanan sebanyak k langkah.
        // Contoh: [1,2,3,4,5], k=2 → [4,5,1,2,3]
        // Teknik: Skip + Take + Concat.
        int[] arr = { 1, 2, 3, 4, 5 };
        int k = 2;
        int n = arr.Length;
        int[] rotated = arr.Skip(n - k).Concat(arr.Take(n - k)).ToArray();
        Console.WriteLine($"12. Rotate Array k={k}: [{string.Join(", ", arr)}] → [{string.Join(", ", rotated)}]");

        // ── SOAL 13: Missing Number ──────────────────────────────
        // Pertanyaan: Dalam array 0..n dengan satu angka hilang, temukan angka tersebut.
        // Teknik: Rumus matematika: n*(n+1)/2 − sum(array).
        int[] withMissing = { 3, 0, 1 };
        int expectedSum = withMissing.Length * (withMissing.Length + 1) / 2;
        int missing = expectedSum - withMissing.Sum();
        Console.WriteLine($"13. Missing Number: [{string.Join(", ", withMissing)}] → {missing}");

        // ── SOAL 14: Majority Element ────────────────────────────
        // Pertanyaan: Temukan elemen yang muncul lebih dari n/2 kali. (LeetCode #169)
        // Teknik: GroupBy + OrderByDescending + First.
        int[] majority = { 3, 2, 3 };
        int majorElem = majority.GroupBy(x => x).OrderByDescending(g => g.Count()).First().Key;
        Console.WriteLine($"14. Majority Element: [{string.Join(", ", majority)}] → {majorElem}");

        // ── SOAL 15: Product Except Self ─────────────────────────
        // Pertanyaan: Kembalikan array dimana output[i] = product semua elemen kecuali nums[i].
        // Teknik: Prefix + Suffix product tanpa pembagian.
        int[] prod = { 1, 2, 3, 4 };
        int[] output = new int[prod.Length];
        output[0] = 1;
        for (int i = 1; i < prod.Length; i++) output[i] = output[i - 1] * prod[i - 1];
        int right = 1;
        for (int i = prod.Length - 1; i >= 0; i--) { output[i] *= right; right *= prod[i]; }
        Console.WriteLine($"15. Product Except Self: [{string.Join(", ", prod)}] → [{string.Join(", ", output)}]");
    }
}

// =========================================================================
// KATEGORI 3: LINQ
// =========================================================================
public static class LinqChallenges
{
    record Employee(string Name, string Dept, decimal Salary);

    public static void RunAll()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n===== KATEGORI 3: LINQ QUERIES =====");
        Console.ResetColor();

        var employees = new List<Employee>
        {
            new("Alice",   "Engineering", 12_000_000),
            new("Bob",     "Marketing",    8_000_000),
            new("Charlie", "Engineering", 15_000_000),
            new("Diana",   "HR",           7_500_000),
            new("Eve",     "Marketing",   10_000_000),
            new("Frank",   "Engineering",  9_000_000),
        };

        // ── SOAL 16: Group by Department ─────────────────────────
        // Pertanyaan: Kelompokkan karyawan berdasarkan departemen.
        var byDept = employees.GroupBy(e => e.Dept)
                              .Select(g => new { Dept = g.Key, Count = g.Count() });
        Console.WriteLine("16. Group by Department:");
        foreach (var d in byDept) Console.WriteLine($"    {d.Dept}: {d.Count} karyawan");

        // ── SOAL 17: Average Salary per Department ────────────────
        // Pertanyaan: Hitung rata-rata gaji per departemen.
        var avgSalary = employees.GroupBy(e => e.Dept)
                                 .Select(g => new { Dept = g.Key, Avg = g.Average(e => e.Salary) });
        Console.WriteLine("17. Avg Salary per Dept:");
        foreach (var d in avgSalary) Console.WriteLine($"    {d.Dept}: Rp{d.Avg:N0}");

        // ── SOAL 18: Top 3 Highest Salary ────────────────────────
        // Pertanyaan: Temukan 3 karyawan bergaji tertinggi.
        var top3 = employees.OrderByDescending(e => e.Salary).Take(3);
        Console.WriteLine("18. Top 3 Highest Salary:");
        foreach (var e in top3) Console.WriteLine($"    {e.Name}: Rp{e.Salary:N0}");

        // ── SOAL 19: Filter by Salary Range ──────────────────────
        var highEarners = employees.Where(e => e.Salary >= 10_000_000)
                                   .Select(e => e.Name);
        Console.WriteLine($"19. Salary >= 10jt: {string.Join(", ", highEarners)}");

        // ── SOAL 20: Count Engineers ─────────────────────────────
        int engCount = employees.Count(e => e.Dept == "Engineering");
        Console.WriteLine($"20. Engineering Count: {engCount}");

        // ── SOAL 21: Any Department with < 8jt avg ───────────────
        bool hasPoorDept = employees.GroupBy(e => e.Dept)
                                    .Any(g => g.Average(e => e.Salary) < 8_000_000);
        Console.WriteLine($"21. Any dept avg < 8jt: {hasPoorDept}");

        // ── SOAL 22: Flatten nested list (SelectMany) ─────────────
        var teams = new List<List<string>>
        {
            new() { "Alice", "Bob" },
            new() { "Charlie", "Diana" }
        };
        var allMembers = teams.SelectMany(t => t).ToList();
        Console.WriteLine($"22. SelectMany flatten: [{string.Join(", ", allMembers)}]");
    }
}

// =========================================================================
// KATEGORI 4: RECURSION & DYNAMIC PROGRAMMING
// =========================================================================
public static class RecursionChallenges
{
    public static void RunAll()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n===== KATEGORI 4: RECURSION & DP =====");
        Console.ResetColor();

        // ── SOAL 23: Factorial ────────────────────────────────────
        // Pertanyaan: Hitung n! secara rekursif.
        // Base case: 0! = 1. Recursive case: n! = n * (n-1)!
        Console.WriteLine($"23. Factorial(6) = {Factorial(6)}");

        // ── SOAL 24: Power (Fast Exponentiation) ─────────────────
        // Pertanyaan: Hitung x^n secara efisien O(log n).
        // Teknik: Divide and Conquer — x^n = (x^(n/2))^2.
        Console.WriteLine($"24. Power(2, 10) = {Power(2, 10)}");

        // ── SOAL 25: Fibonacci dengan Memoization ────────────────
        var memo = new Dictionary<int, long>();
        Console.WriteLine($"25. Fibonacci(40) = {Fib(40, memo)}");

        // ── SOAL 26: GCD (Greatest Common Divisor) ────────────────
        // Teknik: Algoritma Euclidean — GCD(a,b) = GCD(b, a%b) hingga b=0.
        Console.WriteLine($"26. GCD(48, 18) = {Gcd(48, 18)}");
    }

    static long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

    static long Power(long x, int n)
    {
        if (n == 0) return 1;
        if (n % 2 == 0) { long half = Power(x, n / 2); return half * half; }
        return x * Power(x, n - 1);
    }

    static long Fib(int n, Dictionary<int, long> memo)
    {
        if (n <= 1) return n;
        if (memo.ContainsKey(n)) return memo[n];
        return memo[n] = Fib(n - 1, memo) + Fib(n - 2, memo);
    }

    static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}

// =========================================================================
// KATEGORI 5: OOP & DESIGN PATTERNS
// =========================================================================
public static class OopChallenges
{
    public static void RunAll()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n===== KATEGORI 5: OOP & DESIGN PATTERNS =====");
        Console.ResetColor();

        // ── SOAL 27: Singleton Pattern ────────────────────────────
        // Pertanyaan: Implementasikan Singleton yang thread-safe.
        var s1 = AppSettings.Instance;
        var s2 = AppSettings.Instance;
        Console.WriteLine($"27. Singleton Same Instance: {ReferenceEquals(s1, s2)}");

        // ── SOAL 28: Builder Pattern ─────────────────────────────
        // Pertanyaan: Buat objek kompleks secara bertahap menggunakan Builder.
        var person = new PersonBuilder()
            .SetName("Habibie")
            .SetAge(25)
            .SetEmail("habibie@email.com")
            .Build();
        Console.WriteLine($"28. Builder Pattern: {person}");

        // ── SOAL 29: Strategy Pattern ─────────────────────────────
        // Pertanyaan: Tukar algoritma sorting tanpa mengubah kode klien.
        int[] numbers = { 5, 3, 1, 4, 2 };
        var sorter = new Sorter(new BubbleSortStrategy());
        sorter.Sort(numbers);
        Console.WriteLine($"29. Strategy (Bubble Sort): [{string.Join(", ", numbers)}]");

        // ── SOAL 30: Interface Segregation + Dependency Inversion ─
        // Pertanyaan: Demonstrasikan SOLID principles.
        INotificationSender email = new EmailSender();
        INotificationSender sms   = new SmsSender();
        email.Send("Pesanan Anda telah dikonfirmasi!");
        sms.Send("Kode OTP Anda: 123456");
        Console.WriteLine("30. SOLID: Interface Segregation + DIP selesai. (Lihat output di atas)");
    }
}

// ── Supporting Classes ────────────────────────────────────────

// Soal 27: Singleton (Thread-Safe dengan Lazy<T>)
// Lazy<T> menjamin inisialisasi thread-safe tanpa double-check lock manual.
public sealed class AppSettings
{
    private static readonly Lazy<AppSettings> _lazy = new(() => new AppSettings());
    public static AppSettings Instance => _lazy.Value;
    private AppSettings() { }
    public string AppName { get; } = "TesBackendNet";
}

// Soal 28: Builder Pattern
public class Person
{
    public string Name  { get; init; } = "";
    public int    Age   { get; init; }
    public string Email { get; init; } = "";
    public override string ToString() => $"{Name}, {Age} tahun, {Email}";
}

public class PersonBuilder
{
    private string _name = "";
    private int    _age;
    private string _email = "";

    public PersonBuilder SetName(string name)   { _name = name; return this; }
    public PersonBuilder SetAge(int age)         { _age = age;   return this; }
    public PersonBuilder SetEmail(string email)  { _email = email; return this; }
    public Person Build() => new Person { Name = _name, Age = _age, Email = _email };
}

// Soal 29: Strategy Pattern
public interface ISortStrategy { void Sort(int[] arr); }

public class BubbleSortStrategy : ISortStrategy
{
    public void Sort(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
            for (int j = 0; j < arr.Length - i - 1; j++)
                if (arr[j] > arr[j + 1]) (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
    }
}

public class Sorter
{
    private readonly ISortStrategy _strategy;
    public Sorter(ISortStrategy strategy) => _strategy = strategy;
    public void Sort(int[] arr) => _strategy.Sort(arr);
}

// Soal 30: Interface Segregation + Dependency Inversion
public interface INotificationSender { void Send(string message); }
public class EmailSender : INotificationSender { public void Send(string msg) => Console.WriteLine($"   [Email] {msg}"); }
public class SmsSender   : INotificationSender { public void Send(string msg) => Console.WriteLine($"   [SMS]   {msg}"); }
