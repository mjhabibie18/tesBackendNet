// ============================================================
// Nama File: Encapsulation.cs — Demo Pilar OOP: Enkapsulasi (Encapsulation)
// Folder: 13-OOP/Source/Pillars/
// ============================================================
// 1. PENJELASAN FOLDER (OOP/Pillars):
//    - Tujuan: Menerapkan 4 pilar dasar Pemrograman Berorientasi Objek (OOP) secara konkrit.
//    - Kapan Digunakan: Saat mendesain struktur logika sistem agar modular, aman, dan mudah dipelihara.
//    - Hubungan: Menjadi fondasi dasar bagi semua pemodelan kelas data (seperti entitas Product, User) di seluruh modul proyek.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan contoh nyata pilar Enkapsulasi (pembungkusan data dan pembatasan akses langsung).
//    - Mengapa Diperlukan: Menunjukkan cara melindungi state/data internal objek dari manipulasi ilegal luar yang merusak integritas data.
//    - Hubungan File: Dipanggil dan diuji coba di Program.cs pada modul OOP.
//    - Jika Dihapus: Kehilangan materi praktek enkapsulasi pada kurikulum belajar OOP.
// ============================================================

namespace TesBackendNet.OOP.Pillars;

/// <summary>
/// TUJUAN CLASS:
/// Merepresentasikan rekening bank (BankAccount) yang menerapkan enkapsulasi penuh.
/// 
/// ALASAN MENGGUNAKAN ENKAPSULASI DI SINI:
/// Saldo rekening (_balance) adalah informasi sensitif. Jika variabel _balance dibuat public, 
/// pihak luar bisa mengisi saldo negatif sesuka hati (misal: `account.balance = -99999`). 
/// Dengan menyembunyikan field tersebut dan mewajibkan perubahan melalui method teratur, 
/// kita menjamin integritas aturan bisnis bank selalu terjaga.
/// 
/// DEPENDENCY: Tidak ada (Pure domain logic).
/// </summary>
public class BankAccount
{
    // Field internal disembunyikan (private) agar tidak diakses langsung dari luar class.
    private decimal _balance; 

    /// <summary>
    /// FUNGSI PROPERTY: Mengekspos saldo secara aman ke dunia luar.
    /// ALASAN TIPE DATA (decimal): 
    /// - Menggunakan 'decimal' (128-bit) karena memiliki presisi tinggi tanpa pembulatan biner floating-point yang tidak akurat.
    /// - Tipe data float/double menggunakan representasi biner pecahan yang dapat menyebabkan selisih sen keuangan (contoh: 0.1 + 0.2 = 0.30000000000000004).
    /// - Oleh karena itu, decimal adalah standar industri wajib untuk semua nominal uang dan transaksi finansial.
    /// KAPAN DIGUNAKAN: Digunakan oleh sistem luar untuk menampilkan saldo saat ini (Read-Only).
    /// </summary>
    public decimal Balance => _balance;

    /// <summary>
    /// FUNGSI METHOD: Memasukkan dana (deposit) ke rekening.
    /// PARAMETER: amount (nominal yang didepositkan).
    /// 
    /// ALUR EKSEKUSI & VALIDASI:
    /// 1. Memeriksa apakah nominal lebih kecil atau sama dengan nol.
    /// 2. Jika ya: Lempar ArgumentException untuk memblokir proses ilegal secara instan.
    /// 3. Jika valid: Tambahkan nominal tersebut ke saldo internal `_balance`.
    /// 
    /// BEST PRACTICE:
    /// Terapkan "Fail-Fast Principle". Periksa semua validasi parameter di baris-baris awal method
    /// sebelum mengeksekusi logika bisnis utama.
    /// </summary>
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Nominal deposit harus lebih besar dari 0.");
        
        _balance += amount;
    }

    /// <summary>
    /// FUNGSI METHOD: Menarik dana (withdraw) dari rekening.
    /// PARAMETER: amount (nominal yang ditarik).
    /// 
    /// ALUR EKSEKUSI & VALIDASI:
    /// 1. Validasi masukan negatif/nol (Fail-Fast).
    /// 2. Validasi kecukupan saldo: Memastikan dana yang ditarik tidak melebihi saldo saat ini.
    /// 3. Jika tidak cukup: Lempar InvalidOperationException karena status objek tidak memenuhi syarat penarikan.
    /// 4. Jika lolos: Kurangi saldo internal `_balance`.
    /// </summary>
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Nominal penarikan harus lebih besar dari 0.");

        if (amount > _balance)
            throw new InvalidOperationException("Saldo tidak mencukupi untuk melakukan penarikan.");

        _balance -= amount;
    }
}
