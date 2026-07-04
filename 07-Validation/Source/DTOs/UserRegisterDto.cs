// ============================================================
// Nama File: UserRegisterDto.cs — DTO Pendaftaran User
// Folder: 07-Validation/Source/DTOs/
// ============================================================
// 1. PENJELASAN FOLDER (Validation):
//    - Tujuan: Menerapkan validasi input data dari klien sebelum diproses.
//    - Kapan Digunakan: Setiap kali sistem menerima input eksternal.
//    - Hubungan: Menjadi gerbang pelindung sebelum data masuk ke Service/Repository.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mentransfer data pendaftaran dari client.
//    - Mengapa Diperlukan: Mencegah over-posting attack dan memisahkan struktur data internal (Entity) dari eksternal (DTO).
//    - Hubungan File: Divalidasi oleh UserRegisterValidator.cs dan diproses di UsersController.cs.
//    - Jika Dihapus: Controller harus menerima Entity langsung dari request, melanggar pola Separation of Concerns.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace TesBackendNet.Validation.DTOs;

/// <summary>
/// TUJUAN CLASS:
/// Merepresentasikan skema data mentah (payload) yang dikirim oleh klien saat melakukan registrasi.
/// 
/// ALASAN PENGGUNAAN DTO (Data Transfer Object):
/// DTO digunakan agar struktur API luar tidak bergantung pada database schema (Entity). Jika database berubah,
/// kita tidak perlu mengubah kontrak request API yang telah disepakati oleh klien (Loose Coupling).
/// 
/// PRINSIP OOP & DESIGN PATTERN:
/// - Single Responsibility Principle (SRP): Class ini hanya bertanggung jawab memindahkan data tanpa logika bisnis.
/// - Data Encapsulation: Menggunakan auto-implemented properties untuk mengekspos data secara aman.
/// </summary>
public class UserRegisterDto
{
    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan alamat email pengguna untuk pengiriman notifikasi dan login.
    /// ALASAN TIPE DATA (string): Email terdiri dari karakter alfanumerik beserta simbol khusus (seperti @).
    /// METADATA ANNOTATIONS:
    /// - [Required]: Validasi dasar tingkat runtime agar model state langsung mendeteksi jika email bernilai null/empty.
    /// - [EmailAddress]: Menggunakan pola Regex bawaan .NET untuk memvalidasi pola alamat email standar.
    /// </summary>
    [Required(ErrorMessage = "Email tidak boleh kosong")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan nama unik (identitas) pengguna di sistem.
    /// ALASAN TIPE DATA (string): Username merupakan string alfanumerik.
    /// KAPAN DIGUNAKAN: Digunakan saat otentikasi login selain menggunakan alamat email.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan kata sandi mentah (plain text) yang dimasukkan pengguna.
    /// ALASAN TIPE DATA (string): Mengakomodasi kombinasi simbol, angka, serta huruf besar/kecil.
    /// KAPAN DIGUNAKAN: Hanya digunakan saat proses validasi di memori, sebelum kemudian di-hash secara aman.
    /// WARNING: Jangan pernah menyimpan nilai property ini langsung ke database dalam format plain text!
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Mengonfirmasi ulang kata sandi untuk mencegah kesalahan ketik pengguna.
    /// ALASAN TIPE DATA (string): Harus bertipe data sama dengan properti Password agar bisa dibandingkan.
    /// KIBLAT HUBUNGAN: Sangat terikat pada validasi silang (cross-field validation) di UserRegisterValidator.cs.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan nomor telepon pengguna untuk verifikasi 2FA atau keperluan kontak.
    /// ALASAN TIPE DATA (string): Nomor telepon tidak digunakan untuk operasi matematika, serta sering kali diawali oleh simbol + atau angka 0 di depan yang akan hilang jika bertipe numeric.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan usia pengguna guna memastikan batasan umur legal pendaftaran.
    /// ALASAN TIPE DATA (int): Usia bernilai numerik bilangan bulat positif tanpa pecahan desimal.
    /// KAPAN DIGUNAKAN: Diperiksa pada tingkat FluentValidation untuk memastikan minimal usia terpenuhi.
    /// </summary>
    public int Age { get; set; }
}
