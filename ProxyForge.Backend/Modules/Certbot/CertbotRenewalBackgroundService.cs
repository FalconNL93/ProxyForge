using ProxyForge.Backend.CertbotPlugins;
using ProxyForge.Backend.Contracts;
using ProxyForge.Backend.Helpers;

namespace ProxyForge.Backend.Modules.Certbot;

public sealed class CertbotRenewalBackgroundService : BackgroundService
{
    private readonly ICertbot _certbot;
    private readonly ILogger<CertbotRenewalBackgroundService> _logger;

    public CertbotRenewalBackgroundService(ICertbot certbot, ILogger<CertbotRenewalBackgroundService> logger)
    {
        _certbot = certbot;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string[] domains = ["testapi.falcon-net.nl"];
        _logger.LogInformation("Certbot renewal background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new CertbotRequest
                {
                    Domains = domains,
                    Plugin = new DnsCloudflarePlugin(Path.Combine(Paths.DataDirectory, "cloudflare.ini")),
                    Email = "admin@falcon-net.nl",
                    UseStaging = true,
                    ForceRenewal = false,
                    VenvPath = ".venv"
                };

                _logger.LogInformation("Running Certbot renewal for {Domains}...", string.Join(", ", domains));
                _logger.LogDebug("ACME Request: {@Request}", request);

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);

                var result = await _certbot.RunAsync(request, linkedCts.Token);
                var stdOutResult = result.GetRenewalResult();

                LogCertbotResult(stdOutResult, result, _logger);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Certbot renewal task was cancelled by host");
                break;
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Certbot process timed out after 5 minutes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Certbot renewal");
            }
            finally
            {
                // Wait 12 hours before next renewal attempt, unless cancelled
                try
                {
                    await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        _logger.LogInformation("Certbot renewal background service stopping");
    }

    private static void LogCertbotResult(Certbot.CertbotRenewalResult resultType, CertbotResult result, ILogger logger)
    {
        switch (resultType)
        {
            case Certbot.CertbotRenewalResult.NotDueForRenewal:
                logger.LogInformation("Certificate not yet due for renewal; no action taken");
                break;
            case Certbot.CertbotRenewalResult.Success:
                logger.LogInformation("Certbot renewal succeeded ({ReturnCode})", result.ExitCode);
                logger.LogInformation("StdOut: {StdOut}", result.StdOut);
                break;
            case Certbot.CertbotRenewalResult.NoCertificatesFound:
                logger.LogWarning("No certificates found for renewal.");
                break;
            case Certbot.CertbotRenewalResult.NoRenewalsAttempted:
                logger.LogWarning("No renewals were attempted.");
                break;
            case Certbot.CertbotRenewalResult.AllRenewalsFailed:
                logger.LogError("All renewal attempts failed. StdOut: {StdOut}", result.StdOut);
                break;
            case Certbot.CertbotRenewalResult.SomeRenewalsFailed:
                logger.LogWarning("Some certificates could not be renewed. StdOut: {StdOut}", result.StdOut);
                break;
            case Certbot.CertbotRenewalResult.Error:
                logger.LogError("Certbot renewal failed ({ReturnCode})", result.ExitCode);
                logger.LogError("StdErr: {StdErr}", result.StdErr);
                break;
            case Certbot.CertbotRenewalResult.Unknown:
            default:
                logger.LogInformation("Certbot renewal result unknown. StdOut: {StdOut}", result.StdOut);
                break;
        }
    }
}