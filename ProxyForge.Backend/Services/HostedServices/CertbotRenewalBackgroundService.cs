using ProxyForge.Backend.CertbotPlugins;
using ProxyForge.Backend.Contracts;
using ProxyForge.Backend.Helpers;

namespace ProxyForge.Backend.Services.HostedServices;

public sealed class CertbotRenewalBackgroundService(
    ICertbot certbot,
    ILogger<CertbotRenewalBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Certbot renewal background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new CertbotRequest
                {
                    Domains = ["testapi.falcon-net.nl"],
                    Plugin = new DnsCloudflarePlugin(Path.Combine(Paths.DataDirectory, "cloudflare.ini")),
                    Email = "admin@falcon-net.nl",
                    UseStaging = true,
                    ForceRenewal = false,
                    VenvPath = ".venv"
                };

                logger.LogInformation("Running Certbot renewal...");

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);

                var result = await certbot.RunAsync(request, linkedCts.Token);

                if (result.Success)
                {
                    logger.LogInformation("Certbot renewal succeeded ({ReturnCode})", result.ExitCode);
                    logger.LogInformation("StdOut: {StdOut}", result.StdOut);
                }
                else
                {
                    logger.LogError("Certbot renewal failed ({ReturnCode})", result.ExitCode);
                    logger.LogError("StdErr: {StdErr}", result.StdErr);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Certbot renewal task was cancelled by host");
                break;
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Certbot process timed out after 5 minutes");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during Certbot renewal");
            }

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }

        logger.LogInformation("Certbot renewal background service stopping");
    }
}
