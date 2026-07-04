// ============================================================
// JobController.cs — Controller Pemicu Background Jobs
// ============================================================

using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TesBackendNet.Queue.Services;

namespace TesBackendNet.Queue.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public JobController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    // ================================================================
    // 1. Fire-and-Forget Job (Dijalankan sekali secara instan di background)
    // ================================================================
    
    [HttpPost("welcome")]
    public IActionResult TriggerWelcomeJob([FromQuery] string email, [FromQuery] string username)
    {
        // Enqueue job ke Hangfire. Thread utama HTTP akan langsung mengembalikan 202 Accepted.
        var jobId = _backgroundJobClient.Enqueue<IEmailService>(service => 
            service.SendWelcomeEmailAsync(email, username));

        return Accepted(new
        {
            Success = true,
            JobId = jobId,
            Message = "Request pendaftaran diterima. Email akan dikirim di background."
        });
    }

    // ================================================================
    // 2. Delayed Job (Dijalankan sekali di background setelah delay waktu tertentu)
    // ================================================================
    
    [HttpPost("invoice/{orderId:int}")]
    public IActionResult TriggerInvoiceJob(int orderId, [FromQuery] string email)
    {
        // Menjadwalkan pengiriman invoice setelah delay 10 detik
        var jobId = _backgroundJobClient.Schedule<IEmailService>(
            service => service.SendInvoiceEmailAsync(email, orderId),
            TimeSpan.FromSeconds(10));

        return Accepted(new
        {
            Success = true,
            JobId = jobId,
            Message = $"Order #{orderId} diproses. Invoice akan dikirim secara otomatis 10 detik dari sekarang."
        });
    }

    // ================================================================
    // 3. Recurring Job (Dijalankan berulang kali secara terjadwal berdasarkan Cron)
    // ================================================================
    
    [HttpPost("backup/register")]
    public IActionResult RegisterBackupJob()
    {
        // Mendaftarkan job rekursif yang berjalan setiap menit
        _recurringJobManager.AddOrUpdate<IEmailService>(
            "db-backup-every-minute",
            service => service.RunDatabaseBackup(),
            Cron.Minutely);

        return Ok(new
        {
            Success = true,
            Message = "Job backup database rutin (setiap 1 menit) berhasil diaktifkan."
        });
    }

    [HttpDelete("backup/remove")]
    public IActionResult RemoveBackupJob()
    {
        _recurringJobManager.RemoveIfExists("db-backup-every-minute");
        return Ok(new { Success = true, Message = "Job backup database rutin berhasil dihentikan." });
    }
}
