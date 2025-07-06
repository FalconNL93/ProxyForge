using ProxyForge.Backend.CertbotPlugins;
using ProxyForge.Backend.Contracts;

namespace ProxyForge.Backend.Services.HostedServices;

public sealed class CertbotRenewalBackgroundService(
    ICertbot certbot,
    ILogger<CertbotRenewalBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Certbot renewal background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new CertbotRequest
                {
                    Domains = ["example.com", "*.example.com"],
                    Plugin = new DnsCloudflarePlugin("/etc/letsencrypt/cloudflare.ini"),
                    Email = "admin@example.com",
                    UseStaging = false,
                    ForceRenewal = false,
                    VenvPath = ".venv"
                };

                logger.LogInformation("Running Certbot renewal...");

                var result = await certbot.RunAsync(request, stoppingToken);

                if (result.Success)
                {
                    logger.LogInformation("Certbot renewal succeeded.");
                    logger.LogDebug(result.StdOut);
                }
                else
                {
                    logger.LogError("Certbot renewal failed.");
                    logger.LogError(result.StdErr);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Certbot renewal task was cancelled.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during Certbot renewal.");
            }

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }

        logger.LogInformation("Certbot renewal background service stopping.");
    }
}
