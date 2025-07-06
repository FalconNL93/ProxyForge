using System.Text.RegularExpressions;
using ProxyForge.Backend.CertbotPlugins;
using ProxyForge.Backend.Contracts;
using ProxyForge.Backend.Helpers;

namespace ProxyForge.Backend.Services.HostedServices;

/// <summary>
///     Background service that periodically renews certificates using Certbot and logs the result.
/// </summary>
public sealed partial class CertbotRenewalBackgroundService(
    ICertbot certbot,
    ILogger<CertbotRenewalBackgroundService> logger
) : BackgroundService
{
    private static readonly List<(Regex Pattern, CertbotRenewalResult Result)> StdOutPatterns =
    [
        (CertificateNotDueRegex(), CertbotRenewalResult.NotDueForRenewal),
        (CongratulationsRegex(), CertbotRenewalResult.Success),
        (NoCertificatesFoundRegex(), CertbotRenewalResult.NoCertificatesFound),
        (NoRenewalsAttemptedRegex(), CertbotRenewalResult.NoRenewalsAttempted),
        (AllRenewalsFailedRegex(), CertbotRenewalResult.AllRenewalsFailed),
        (SomeRenewalsFailedRegex(), CertbotRenewalResult.SomeRenewalsFailed),
        (ErrorRegex(), CertbotRenewalResult.Error),
        (FailedRegex(), CertbotRenewalResult.Error)
    ];

    [GeneratedRegex(@"Certificate not yet due for renewal", RegexOptions.IgnoreCase)]
    private static partial Regex CertificateNotDueRegex();

    [GeneratedRegex(@"Congratulations! Your certificate and chain have been saved at:", RegexOptions.IgnoreCase)]
    private static partial Regex CongratulationsRegex();

    [GeneratedRegex(@"No certificates found", RegexOptions.IgnoreCase)]
    private static partial Regex NoCertificatesFoundRegex();

    [GeneratedRegex(@"No renewals were attempted", RegexOptions.IgnoreCase)]
    private static partial Regex NoRenewalsAttemptedRegex();

    [GeneratedRegex(@"All renewal attempts failed", RegexOptions.IgnoreCase)]
    private static partial Regex AllRenewalsFailedRegex();

    [GeneratedRegex(@"The following certs could not be renewed:", RegexOptions.IgnoreCase)]
    private static partial Regex SomeRenewalsFailedRegex();

    [GeneratedRegex(@"error", RegexOptions.IgnoreCase)]
    private static partial Regex ErrorRegex();

    [GeneratedRegex(@"failed", RegexOptions.IgnoreCase)]
    private static partial Regex FailedRegex();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string[] domains = ["testapi.falcon-net.nl"];
        logger.LogInformation("Certbot renewal background service started");

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

                logger.LogInformation("Running Certbot renewal for {Domains}...", string.Join(", ", domains));
                logger.LogDebug("ACME Request: {@Request}", request);

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);

                var result = await certbot.RunAsync(request, linkedCts.Token);
                var stdOutResult = ParseCertbotStdOut(result.StdOut);

                LogCertbotResult(stdOutResult, result, logger);
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

        logger.LogInformation("Certbot renewal background service stopping");
    }

    /// <summary>
    ///     Logs the result of a Certbot renewal attempt based on the parsed result.
    /// </summary>
    private static void LogCertbotResult(CertbotRenewalResult resultType, CertbotResult result, ILogger logger)
    {
        switch (resultType)
        {
            case CertbotRenewalResult.NotDueForRenewal:
                logger.LogInformation("Certificate not yet due for renewal; no action taken");
                break;
            case CertbotRenewalResult.Success:
                logger.LogInformation("Certbot renewal succeeded ({ReturnCode})", result.ExitCode);
                logger.LogInformation("StdOut: {StdOut}", result.StdOut);
                break;
            case CertbotRenewalResult.NoCertificatesFound:
                logger.LogWarning("No certificates found for renewal.");
                break;
            case CertbotRenewalResult.NoRenewalsAttempted:
                logger.LogWarning("No renewals were attempted.");
                break;
            case CertbotRenewalResult.AllRenewalsFailed:
                logger.LogError("All renewal attempts failed. StdOut: {StdOut}", result.StdOut);
                break;
            case CertbotRenewalResult.SomeRenewalsFailed:
                logger.LogWarning("Some certificates could not be renewed. StdOut: {StdOut}", result.StdOut);
                break;
            case CertbotRenewalResult.Error:
                logger.LogError("Certbot renewal failed ({ReturnCode})", result.ExitCode);
                logger.LogError("StdErr: {StdErr}", result.StdErr);
                break;
            case CertbotRenewalResult.Unknown:
            default:
                logger.LogInformation("Certbot renewal result unknown. StdOut: {StdOut}", result.StdOut);
                break;
        }
    }

    private static CertbotRenewalResult ParseCertbotStdOut(string stdout)
    {
        foreach (var (pattern, result) in StdOutPatterns)
        {
            if (pattern.IsMatch(stdout))
            {
                return result;
            }
        }

        return CertbotRenewalResult.Unknown;
    }

    private enum CertbotRenewalResult
    {
        Success,
        NotDueForRenewal,
        NoCertificatesFound,
        NoRenewalsAttempted,
        SomeRenewalsFailed,
        AllRenewalsFailed,
        Error,
        Unknown
    }
}
