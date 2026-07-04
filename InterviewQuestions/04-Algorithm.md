# 🧮 04 — Algorithm Interview Questions (Coding Test)

---

## Soal 1: Two Sum

**Tingkat**: Easy | **Topik**: Array, HashMap

### Deskripsi
Diberikan array integer `nums` dan integer `target`, return indices dari dua angka yang jumlahnya sama dengan target.

### Kode Lengkap

```csharp
public int[] TwoSum(int[] nums, int target)
{
    // Gunakan Dictionary untuk O(1) lookup
    var map = new Dictionary<int, int>(); // value → index

    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i];

        if (map.ContainsKey(complement))
            return new[] { map[complement], i };

        map[nums[i]] = i;
    }

    return Array.Empty<int>(); // Tidak ditemukan
}

// Test:
// TwoSum([2,7,11,15], 9) → [0,1] (2+7=9)
// TwoSum([3,2,4], 6)     → [1,2] (2+4=6)
```

**Kompleksitas**: Time O(n), Space O(n)

---

## Soal 2: Valid Parentheses

**Tingkat**: Easy | **Topik**: Stack

### Deskripsi
Diberikan string s yang berisi karakter `()[]{}`, periksa apakah string valid (setiap kurung buka memiliki pasangan kurung tutup yang tepat).

### Kode Lengkap

```csharp
public bool IsValid(string s)
{
    var stack = new Stack<char>();

    var pairs = new Dictionary<char, char>
    {
        { ')', '(' },
        { ']', '[' },
        { '}', '{' }
    };

    foreach (var c in s)
    {
        if (!pairs.ContainsKey(c))
        {
            // Kurung buka: push ke stack
            stack.Push(c);
        }
        else
        {
            // Kurung tutup: cek apakah pasangannya ada di stack
            if (stack.Count == 0 || stack.Peek() != pairs[c])
                return false;

            stack.Pop();
        }
    }

    return stack.Count == 0; // Stack harus kosong jika valid
}

// Test:
// IsValid("()")   → true
// IsValid("()[]{}")  → true
// IsValid("(]")   → false
// IsValid("{[]}")  → true
```

**Kompleksitas**: Time O(n), Space O(n)

---

## Soal 3: Reverse Linked List

**Tingkat**: Easy | **Topik**: Linked List

### Kode Lengkap

```csharp
public class ListNode
{
    public int Val;
    public ListNode? Next;
    public ListNode(int val = 0, ListNode? next = null)
    {
        Val  = val;
        Next = next;
    }
}

// Iterative — O(n) time, O(1) space
public ListNode? ReverseList(ListNode? head)
{
    ListNode? prev    = null;
    ListNode? current = head;

    while (current != null)
    {
        var next     = current.Next; // Simpan next
        current.Next = prev;          // Balik pointer
        prev         = current;       // Geser prev
        current      = next;          // Geser current
    }

    return prev; // prev adalah head baru
}

// Recursive — O(n) time, O(n) space (rekursi stack)
public ListNode? ReverseListRecursive(ListNode? head)
{
    if (head == null || head.Next == null)
        return head;

    var newHead  = ReverseListRecursive(head.Next);
    head.Next.Next = head;
    head.Next    = null;
    return newHead;
}
```

---

## Soal 4: Maximum Subarray (Kadane's Algorithm)

**Tingkat**: Medium | **Topik**: Dynamic Programming, Array

### Deskripsi
Diberikan array integer, return jumlah subarray yang paling besar.

### Kode Lengkap

```csharp
public int MaxSubArray(int[] nums)
{
    int maxSum     = nums[0];
    int currentSum = nums[0];

    for (int i = 1; i < nums.Length; i++)
    {
        // Pilih: extend subarray sekarang ATAU mulai baru
        currentSum = Math.Max(nums[i], currentSum + nums[i]);
        maxSum     = Math.Max(maxSum, currentSum);
    }

    return maxSum;
}

// Test:
// MaxSubArray([-2,1,-3,4,-1,2,1,-5,4]) → 6 ([4,-1,2,1])
// MaxSubArray([1]) → 1
// MaxSubArray([5,4,-1,7,8]) → 23
```

**Kompleksitas**: Time O(n), Space O(1)

---

## Soal 5: FizzBuzz

**Tingkat**: Easy | **Topik**: Basic

### Deskripsi
Untuk angka 1-n: print "FizzBuzz" jika habis dibagi 3 dan 5, "Fizz" jika habis dibagi 3, "Buzz" jika habis dibagi 5, angkanya jika tidak keduanya.

### Kode Lengkap

```csharp
public IList<string> FizzBuzz(int n)
{
    var result = new List<string>();

    for (int i = 1; i <= n; i++)
    {
        if (i % 15 == 0)      result.Add("FizzBuzz");
        else if (i % 3 == 0)  result.Add("Fizz");
        else if (i % 5 == 0)  result.Add("Buzz");
        else                   result.Add(i.ToString());
    }

    return result;
}
```

---

## Soal 6: Binary Search

**Tingkat**: Easy | **Topik**: Binary Search

### Kode Lengkap

```csharp
public int Search(int[] nums, int target)
{
    int left = 0, right = nums.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2; // Hindari integer overflow

        if (nums[mid] == target) return mid;
        if (nums[mid] < target)  left  = mid + 1;
        else                      right = mid - 1;
    }

    return -1; // Tidak ditemukan
}

// Time: O(log n), Space: O(1)
```

---

## Soal 7: Fibonacci dengan Memoization

**Tingkat**: Easy | **Topik**: Recursion, DP

### Kode Lengkap

```csharp
// 1. Naive Recursion — O(2^n): SANGAT LAMBAT
int FibNaive(int n) => n <= 1 ? n : FibNaive(n - 1) + FibNaive(n - 2);

// 2. Memoization — O(n) time, O(n) space
Dictionary<int, long> _memo = new();
long FibMemo(int n)
{
    if (n <= 1) return n;
    if (_memo.TryGetValue(n, out var cached)) return cached;

    _memo[n] = FibMemo(n - 1) + FibMemo(n - 2);
    return _memo[n];
}

// 3. Iterative DP — O(n) time, O(1) space (TERBAIK)
long FibIterative(int n)
{
    if (n <= 1) return n;
    long prev = 0, curr = 1;

    for (int i = 2; i <= n; i++)
        (prev, curr) = (curr, prev + curr);

    return curr;
}
```

---

## Soal 8: Anagram Check

**Tingkat**: Easy | **Topik**: String, HashMap

### Kode Lengkap

```csharp
public bool IsAnagram(string s, string t)
{
    if (s.Length != t.Length) return false;

    var count = new int[26]; // Hanya lowercase a-z

    foreach (char c in s) count[c - 'a']++;
    foreach (char c in t) count[c - 'a']--;

    return count.All(x => x == 0);
}

// Alternatif: Dictionary
public bool IsAnagramDict(string s, string t)
{
    if (s.Length != t.Length) return false;

    var freq = new Dictionary<char, int>();
    foreach (var c in s) freq[c] = freq.GetValueOrDefault(c) + 1;
    foreach (var c in t)
    {
        if (!freq.ContainsKey(c) || freq[c] == 0) return false;
        freq[c]--;
    }

    return true;
}
```

---

## Soal 9: Palindrome Check

**Tingkat**: Easy | **Topik**: String, Two Pointer

### Kode Lengkap

```csharp
public bool IsPalindrome(string s)
{
    // Filter hanya alphanumeric, lowercase
    var filtered = new string(s.Where(char.IsLetterOrDigit)
                                .Select(char.ToLower)
                                .ToArray());

    int left = 0, right = filtered.Length - 1;

    while (left < right)
    {
        if (filtered[left] != filtered[right]) return false;
        left++;
        right--;
    }

    return true;
}

// Test:
// IsPalindrome("A man, a plan, a canal: Panama") → true
// IsPalindrome("race a car") → false
```

---

## Soal 10: Count Duplicates

**Tingkat**: Easy | **Topik**: Array, HashMap

### Kode Lengkap

```csharp
// Return angka yang muncul lebih dari sekali
public IList<int> FindDuplicates(int[] nums)
{
    var freq   = new Dictionary<int, int>();
    var result = new List<int>();

    foreach (var n in nums)
        freq[n] = freq.GetValueOrDefault(n) + 1;

    foreach (var (key, count) in freq)
        if (count > 1) result.Add(key);

    return result;
}
```

---

## Soal 11–20: (Ringkasan dengan Kode)

**11. Merge Sorted Arrays**
```csharp
// Merge dua sorted array menjadi satu sorted array
int[] Merge(int[] a, int[] b)
{
    var result = new int[a.Length + b.Length];
    int i = 0, j = 0, k = 0;
    while (i < a.Length && j < b.Length)
        result[k++] = a[i] <= b[j] ? a[i++] : b[j++];
    while (i < a.Length) result[k++] = a[i++];
    while (j < b.Length) result[k++] = b[j++];
    return result;
}
// Time: O(n+m), Space: O(n+m)
```

**12. Find Missing Number**
```csharp
// Array 0..n dengan satu angka yang hilang, return yang hilang
int MissingNumber(int[] nums)
{
    int n = nums.Length;
    int expected = n * (n + 1) / 2; // Sum 0..n
    return expected - nums.Sum();
}
// Time: O(n), Space: O(1)
```

**13. Reverse Words in String**
```csharp
string ReverseWords(string s)
    => string.Join(" ",
        s.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
         .Reverse());
```

**14. Group Anagrams**
```csharp
IList<IList<string>> GroupAnagrams(string[] strs)
{
    var groups = new Dictionary<string, List<string>>();
    foreach (var s in strs)
    {
        var key = new string(s.OrderBy(c => c).ToArray());
        groups.TryAdd(key, new List<string>());
        groups[key].Add(s);
    }
    return groups.Values.Cast<IList<string>>().ToList();
}
```

**15. First Non-Repeating Character**
```csharp
int FirstUniqChar(string s)
{
    var freq = new int[26];
    foreach (var c in s) freq[c - 'a']++;
    for (int i = 0; i < s.Length; i++)
        if (freq[s[i] - 'a'] == 1) return i;
    return -1;
}
```

**16. Rotate Array**
```csharp
// Rotate array k steps ke kanan
void Rotate(int[] nums, int k)
{
    k %= nums.Length;
    Array.Reverse(nums);
    Array.Reverse(nums, 0, k);
    Array.Reverse(nums, k, nums.Length - k);
}
// Time: O(n), Space: O(1)
```

**17. Climbing Stairs (DP)**
```csharp
// Bisa naik 1 atau 2 step, berapa cara untuk naik n step?
int ClimbStairs(int n)
{
    if (n <= 2) return n;
    int prev = 1, curr = 2;
    for (int i = 3; i <= n; i++)
        (prev, curr) = (curr, prev + curr);
    return curr;
}
// Sama dengan Fibonacci! Time: O(n), Space: O(1)
```

**18. Longest Common Prefix**
```csharp
string LongestCommonPrefix(string[] strs)
{
    if (strs.Length == 0) return "";
    var prefix = strs[0];
    for (int i = 1; i < strs.Length; i++)
        while (!strs[i].StartsWith(prefix))
            prefix = prefix[..^1];
    return prefix;
}
```

**19. Find Peak Element**
```csharp
// Return index peak element (lebih besar dari tetangga)
int FindPeakElement(int[] nums)
{
    int left = 0, right = nums.Length - 1;
    while (left < right)
    {
        int mid = (left + right) / 2;
        if (nums[mid] > nums[mid + 1]) right = mid;
        else left = mid + 1;
    }
    return left;
}
```

**20. Implement Stack using Queue**
```csharp
public class MyStack
{
    private Queue<int> _q1 = new();
    private Queue<int> _q2 = new();

    public void Push(int x)
    {
        _q2.Enqueue(x);
        while (_q1.Count > 0) _q2.Enqueue(_q1.Dequeue());
        (_q1, _q2) = (_q2, _q1);
    }

    public int Pop()    => _q1.Dequeue();
    public int Top()    => _q1.Peek();
    public bool Empty() => _q1.Count == 0;
}
```
