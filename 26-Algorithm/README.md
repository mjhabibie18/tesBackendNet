# 🧮 26 — Algorithm & Data Structure

## Mengapa Algorithm Penting?

Algorithm sering muncul di technical test, terutama untuk posisi backend di perusahaan tech.

---

## Big O Notation

```
O(1)       → Constant: akses array by index
O(log n)   → Logarithmic: Binary Search
O(n)       → Linear: Linear Search, iterasi array
O(n log n) → QuasiLinear: Merge Sort, Quick Sort
O(n²)      → Quadratic: Bubble Sort, nested loop
O(2^n)     → Exponential: Fibonacci naive recursive
```

---

## Data Structures

### Array

```csharp
// O(1) access by index, O(n) search
int[] arr = { 1, 2, 3, 4, 5 };
int val = arr[2]; // O(1)

// List<T> — dynamic array
var list = new List<int> { 1, 2, 3 };
list.Add(4);    // O(1) amortized
list.Remove(2); // O(n)
```

### Stack (LIFO)

```csharp
var stack = new Stack<int>();
stack.Push(1); // Tambah ke atas
stack.Push(2);
var top = stack.Pop();  // Ambil dari atas → 2
var peek = stack.Peek(); // Lihat tanpa hapus → 1

// Use case: undo/redo, backtracking, function call stack
```

### Queue (FIFO)

```csharp
var queue = new Queue<int>();
queue.Enqueue(1); // Tambah ke belakang
queue.Enqueue(2);
var front = queue.Dequeue(); // Ambil dari depan → 1

// Use case: task queue, BFS, request processing
```

### Dictionary (HashMap)

```csharp
// O(1) average untuk insert, delete, lookup
var map = new Dictionary<string, int>();
map["apple"] = 5;
map["banana"] = 3;

bool exists = map.ContainsKey("apple"); // true
int count = map["apple"]; // 5

// Use case: memoization, frequency count, cache
```

---

## Sorting Algorithms

```csharp
// C# built-in sort: O(n log n) — introsort (combination quicksort + heapsort)
int[] arr = { 5, 2, 8, 1, 9 };
Array.Sort(arr); // { 1, 2, 5, 8, 9 }

// LINQ sort
var sorted = arr.OrderBy(x => x).ToList();

// Custom sort
var products = products.OrderBy(p => p.Price)
                        .ThenBy(p => p.Name)
                        .ToList();
```

---

## Binary Search

```csharp
// O(log n) — array HARUS sudah sorted!
int BinarySearch(int[] arr, int target)
{
    int left = 0, right = arr.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (arr[mid] == target) return mid;
        if (arr[mid] < target) left = mid + 1;
        else right = mid - 1;
    }

    return -1; // Tidak ditemukan
}
```

---

## Recursion & Fibonacci

```csharp
// ❌ Naive: O(2^n) — sangat lambat!
int FibNaive(int n) => n <= 1 ? n : FibNaive(n-1) + FibNaive(n-2);

// ✅ Memoization: O(n) — cache hasil
Dictionary<int, long> memo = new();
long FibMemo(int n)
{
    if (n <= 1) return n;
    if (memo.ContainsKey(n)) return memo[n];

    memo[n] = FibMemo(n-1) + FibMemo(n-2);
    return memo[n];
}

// ✅ Dynamic Programming: O(n) — iteratif
long FibDP(int n)
{
    if (n <= 1) return n;
    long prev = 0, curr = 1;
    for (int i = 2; i <= n; i++)
        (prev, curr) = (curr, prev + curr);
    return curr;
}
```

---

## Contoh Soal Interview

### Two Sum
```csharp
// Given array dan target, return indices dua angka yang jumlahnya = target
int[] TwoSum(int[] nums, int target)
{
    var map = new Dictionary<int, int>(); // value → index

    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i];
        if (map.ContainsKey(complement))
            return new[] { map[complement], i };

        map[nums[i]] = i;
    }

    return Array.Empty<int>();
}
// Time: O(n), Space: O(n)
```

### Reverse String
```csharp
string Reverse(string s)
{
    var chars = s.ToCharArray();
    Array.Reverse(chars);
    return new string(chars);
}
// Time: O(n), Space: O(n)
```

---

## 🏋️ Latihan (LeetCode-style)

- [ ] Two Sum (Easy)
- [ ] Valid Parentheses (Easy)
- [ ] Maximum Subarray (Medium)
- [ ] Binary Search (Easy)
- [ ] Merge Intervals (Medium)
- [ ] Linked List Cycle (Easy)
