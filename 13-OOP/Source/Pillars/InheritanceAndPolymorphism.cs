// ============================================================
// Nama File: InheritanceAndPolymorphism.cs — Demo Pilar OOP: Pewarisan (Inheritance) & Polimorfisme (Polymorphism)
// Folder: 13-OOP/Source/Pillars/
// ============================================================
// 1. PENJELASAN FOLDER (OOP/Pillars):
//    - Tujuan: Menerapkan 4 pilar dasar OOP (Enkapsulasi, Pewarisan, Polimorfisme, Abstraksi).
//    - Kapan Digunakan: Saat memodelkan objek domain bisnis yang memiliki hierarki dan perilaku bervariasi.
//    - Hubungan: Mendasari penulisan kelas Exception kustom di modul Error Handling (seperti AppException mewarisi Exception).
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyajikan kode demonstrasi pewarisan properti dan implementasi perilaku dinamis (polimorfisme).
//    - Mengapa Diperlukan: Pemahaman tentang keyword 'abstract', 'virtual', dan 'override' sangat mendasar agar developer bisa membangun arsitektur class yang reusable.
//    - Hubungan File: Dipanggil dan diuji coba di Program.cs pada modul OOP.
// ============================================================

namespace TesBackendNet.OOP.Pillars;

/// <summary>
/// TUJUAN CLASS:
/// Kelas induk abstrak (Base/Parent Class) yang mendefinisikan struktur data umum dan perilaku makhluk hidup (Animal).
/// 
/// ALASAN MENGGUNAKAN CLASS ABSTRAK (abstract class):
/// Class ini ditandai 'abstract' karena kita tidak ingin siapa pun membuat instansi langsung dari kelas 'Animal' 
/// (misal: `new Animal()`), sebab 'Animal' hanyalah konsep umum. Hanya hewan spesifik (seperti Dog atau Cat) 
/// yang boleh dibuatkan objeknya.
/// </summary>
public abstract class Animal
{
    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan nama hewan.
    /// ALASAN TIPE DATA (string): Nama berwujud karakter alfanumerik.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI METHOD: Perilaku hewan mengeluarkan suara.
    /// NILAI KEMBALIAN: String representasi suara.
    /// 
    /// ALASAN MENGGUNAKAN METHOD ABSTRAK (abstract method):
    /// Setiap hewan bersuara dengan cara yang berbeda, sehingga kelas induk 'Animal' tidak memiliki implementasi default.
    /// Menandainya sebagai 'abstract' mewajibkan setiap kelas turunan untuk mengimplementasikan suara khasnya masing-masing.
    /// </summary>
    public abstract string Speak();

    /// <summary>
    /// FUNGSI METHOD: Perilaku hewan saat tidur.
    /// 
    /// ALASAN MENGGUNAKAN METHOD VIRTUAL (virtual method):
    /// Berbeda dengan 'abstract' yang tidak memiliki kode awal, method 'virtual' menyediakan implementasi default 
    /// di kelas induk. Kelas turunan dipersilakan (opsional) untuk menimpa/mengubah perilaku ini (override) 
    /// jika diperlukan, atau menggunakan perilaku bawaan ini secara langsung.
    /// </summary>
    public virtual void Sleep()
    {
        Console.WriteLine($"{Name} sedang tidur... zzz");
    }
}

/// <summary>
/// TUJUAN CLASS:
/// Merepresentasikan Dog yang mewarisi (Inheritance) kelas induk Animal.
/// 
/// CARA KERJA PEWARISAN:
/// Dog mewarisi properti 'Name' dan method 'Sleep()' dari Animal secara otomatis tanpa perlu menulis ulang kodenya (DRY).
/// </summary>
public class Dog : Animal
{
    /// <summary>
    /// FUNGSI METHOD: Mengimplementasikan suara anjing.
    /// 
    /// PENGGUNAAN KEYWORD 'override':
    /// Digunakan untuk menimpa dan mengisi perilaku konkrit dari abstract method 'Speak()' milik kelas induk.
    /// Ini memicu runtime polimorfisme, di mana referensi 'Animal' akan mengeluarkan suara anjing saat objek Dog dipanggil.
    /// </summary>
    public override string Speak() => "Guk Guk!";
}

/// <summary>
/// TUJUAN CLASS:
/// Merepresentasikan Cat yang mewarisi kelas induk Animal.
/// </summary>
public class Cat : Animal
{
    /// <summary>
    /// FUNGSI METHOD: Mengimplementasikan suara kucing.
    /// </summary>
    public override string Speak() => "Meong Meong!";

    /// <summary>
    /// FUNGSI METHOD: Menimpa perilaku tidur bawaan (Override Virtual Method).
    /// 
    /// ALASAN OVERRIDE DI SINI:
    /// Kucing memiliki kebiasaan mendengkur pelan sebelum tidur, sehingga kita memodifikasi 
    /// logika bawaan dari kelas induk untuk mencerminkan perilaku ini secara spesifik.
    /// </summary>
    public override void Sleep()
    {
        Console.WriteLine($"{Name} mendengkur pelan sebelum tidur.");
    }
}
