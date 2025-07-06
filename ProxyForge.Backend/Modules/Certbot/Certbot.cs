using System.Diagnostics;
using System.Text.RegularExpressions;
using ProxyForge.Backend.Builders;
using ProxyForge.Backend.Contracts;
using ProxyForge.Backend.Helpers;

namespace ProxyForge.Backend.Modules.Certbot;

public sealed partial class Certbot : ICertbot
{
    public enum CertbotRenewalResult
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

    private static readonly string CertbotDataDirectory = Path.Combine(Paths.DataDirectory, "certbot");

    private static readonly List<(Regex Pattern, CertbotRenewalResult Result)> StdOutPatterns = new()
    {
        (CertificateNotDueRegex(), CertbotRenewalResult.NotDueForRenewal),
        (CongratulationsRegex(), CertbotRenewalResult.Success),
        (NoCertificatesFoundRegex(), CertbotRenewalResult.NoCertificatesFound),
        (NoRenewalsAttemptedRegex(), CertbotRenewalResult.NoRenewalsAttempted),
        (AllRenewalsFailedRegex(), CertbotRenewalResult.AllRenewalsFailed),
        (SomeRenewalsFailedRegex(), CertbotRenewalResult.SomeRenewalsFailed),
        (ErrorRegex(), CertbotRenewalResult.Error),
        (FailedRegex(), CertbotRenewalResult.Error)
    };

    public async Task<CertbotResult> RunAsync(CertbotRequest request, CancellationToken cancellationToken = default)
    {
        var builder = new CertbotCommandBuilder()
            .UseCommand(CertbotCommand.CertOnly)
            .WithCustomDirs(Path.Combine(CertbotDataDirectory, "config"), Path.Combine(CertbotDataDirectory, "work"), Path.Combine(CertbotDataDirectory, "logs"))
            .AddDomains(request.Domains)
            .AddPlugin(request.Plugin)
            .WithEmail(request.Email)
            .UseStaging()
            .NonInteractive()
            .AgreeTos();

        if (request.UseStaging)
        {
            builder.UseStaging();
        }

        if (request.ForceRenewal)
        {
            builder.ForceRenewal();
        }

        builder.UseVenvPath(request.VenvPath);

        var psi = builder.Build();
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;

        using var process = new Process();
        process.StartInfo = psi;
        process.EnableRaisingEvents = true;

        var stdoutBuffer = new List<string>();
        var stderrBuffer = new List<string>();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stdoutBuffer.Add(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stderrBuffer.Add(e.Data);
            }
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        return new CertbotResult
        {
            ExitCode = process.ExitCode,
            StdOut = string.Join('\n', stdoutBuffer),
            StdErr = string.Join('\n', stderrBuffer)
        };
    }

    public static CertbotRenewalResult ParseRenewalStdOut(string stdout)
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
}