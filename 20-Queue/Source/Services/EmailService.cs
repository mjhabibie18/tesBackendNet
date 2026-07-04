// ============================================================
// EmailService.cs — Service Pengiriman Email (Simulasi)
// ============================================================

namespace TesBackendNet.Queue.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email, string username);
    Task SendInvoiceEmailAsync(string email, int orderId);
    void RunDatabaseBackup();
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string email, string username)
    {
        _logger.LogInformation("[Email Queue] Memulai proses pengiriman welcome email ke {Email}...", email);
        
        // Simulasi delay pengerjaan task berat
        await Task.Delay(3000);

        _logger.LogInformation("[Email Queue] SUKSES: Welcome email telah dikirim ke {Username} ({Email})", username, email);
    }

    public async Task SendInvoiceEmailAsync(string email, int orderId)
    {
        _logger.LogInformation("[Email Queue] Memulai pembuatan invoice PDF & kirim email untuk Order #{OrderId}...", orderId);

        await Task.Delay(5000);

        _logger.LogInformation("[Email Queue] SUKSES: Invoice Order #{OrderId} terkirim ke {Email}", orderId, email);
    }

    public void RunDatabaseBackup()
    {
        _logger.LogInformation("[Backup Job] Memulai backup database otomatis...");
        
        // Simulasi backup
        Thread.Sleep(2000);

        _logger.LogInformation("[Backup Job] SUKSES: Database berhasil di-backup ke storage cloud pada {Time}", DateTime.UtcNow);
    }
}
