// ============================================================
// Program.cs — Demo Algoritma & Struktur Data (LeetCode-style)
// ============================================================

using System.Diagnostics;

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🚀 DEMO ALGORITMA & STRUKTUR DATA (LEETCODE PREPARATION)");
Console.WriteLine("============================================================\n");

// ── 1. Binary Search Demo (O(log n)) ─────────────────────────
Console.WriteLine("--- 1. BINARY SEARCH ---");
int[] sortedArray = { 3, 9, 12, 17, 24, 38, 45, 56, 72, 90 };
int target = 45;
int index = BinarySearch(sortedArray, target);
Console.WriteLine($"Sorted Array: [{string.Join(", ", sortedArray)}]");
Console.WriteLine($"Cari angka {target} -> Ditemukan pada indeks: {index}");
Console.WriteLine();

// ── 2. Fibonacci Memoization vs Naive (O(n) vs O(2^n)) ───────
Console.WriteLine("--- 2. FIBONACCI (NAIVE VS MEMOIZATION) ---");
int fibN = 40; // Fibonacci ke-40

var sw = Stopwatch.StartNew();
long resultMemo = FibMemo(fibN);
sw.Stop();
var timeMemo = sw.ElapsedMilliseconds;

Console.WriteLine($"[Memoization O(n)]: Hasil Fib({fibN}) = {resultMemo} (Waktu: {timeMemo} ms)");

Console.WriteLine("[Naive O(2^n)]: Menghitung Fib(40) naive membutuhkan miliaran operasi...");
// Kami tidak menjalankan FibNaive(40) disini karena akan lag/hang komputer user (~10-20 detik).
// Sebaliknya kita demokan FibNaive(30) saja untuk visualisasi beda waktu.
sw.Restart();
long resultNaive = FibNaive(30);
sw.Stop();
Console.WriteLine($"[Naive O(2^n)]: Hasil Fib(30) = {resultNaive} (Waktu: {sw.ElapsedMilliseconds} ms)");
Console.WriteLine();

// ── 3. Two Sum Demo (O(n)) ────────────────────────────────────
Console.WriteLine("--- 3. TWO SUM (LEETCODE #1) ---");
int[] nums = { 2, 7, 11, 15 };
int twoSumTarget = 9;
int[] twoSumIndices = TwoSum(nums, twoSumTarget);
Console.WriteLine($"Nums Array: [{string.Join(", ", nums)}], Target: {twoSumTarget}");
Console.WriteLine($"Indeks hasil penjumlahan: [{string.Join(", ", twoSumIndices)}]");
Console.WriteLine();

// ── 4. Valid Parentheses (Stack Demo) ─────────────────────────
Console.WriteLine("--- 4. VALID PARENTHESES (LEETCODE #20) ---");
string expression1 = "{[()]}";
string expression2 = "([)]";
Console.WriteLine($"Apakah '{expression1}' Valid? {IsValidParentheses(expression1)}"); // True
Console.WriteLine($"Apakah '{expression2}' Valid? {IsValidParentheses(expression2)}"); // False

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Algoritma Selesai!");
Console.WriteLine("============================================================");

// =========================================================================
// ── IMPLEMENTASI FUNGSI ALGORITMA ────────────────────────────────────────
// =========================================================================

// 1. Binary Search
static int BinarySearch(int[] arr, int target)
{
    int left = 0;
    int right = arr.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2; // Menghindari overflow int

        if (arr[mid] == target) return mid;
        
        if (arr[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return -1; // Tidak ditemukan
}

// 2a. Fibonacci Naive (O(2^n))
static long FibNaive(int n)
{
    if (n <= 1) return n;
    return FibNaive(n - 1) + FibNaive(n - 2);
}

// 2b. Fibonacci Memoization (O(n))
static readonly Dictionary<int, long> MemoCache = new();
static long FibMemo(int n)
{
    if (n <= 1) return n;
    if (MemoCache.ContainsKey(n)) return MemoCache[n];

    MemoCache[n] = FibMemo(n - 1) + FibMemo(n - 2);
    return MemoCache[n];
}

// 3. Two Sum
static int[] TwoSum(int[] nums, int target)
{
    var map = new Dictionary<int, int>(); // Menyimpan: [Value, Index]

    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i];
        if (map.ContainsKey(complement))
        {
            return new int[] { map[complement], i };
        }
        map[nums[i]] = i;
    }

    return Array.Empty<int>();
}

// 4. Valid Parentheses
static bool IsValidParentheses(string s)
{
    var stack = new Stack<char>();
    
    foreach (char c in s)
    {
        if (c == '(' || c == '[' || c == '{')
        {
            stack.Push(c);
        }
        else
        {
            if (stack.Count == 0) return false;
            char top = stack.Pop();
            
            if (c == ')' && top != '(') return false;
            if (c == ']' && top != '[') return false;
            if (c == '}' && top != '{') return false;
        }
    }

    return stack.Count == 0;
}
