using System.Diagnostics;
using ProxyForge.Backend.Builders;
using ProxyForge.Backend.Contracts;

namespace ProxyForge.Backend.Services;

public sealed class Certbot : ICertbot
{
    public async Task<CertbotResult> RunAsync(CertbotRequest request, CancellationToken cancellationToken = default)
    {
        var builder = new CertbotCommandBuilder()
            .UseCommand(CertbotCommand.CertOnly)
            .AddDomains(request.Domains)
            .AddPlugin(request.Plugin)
            .WithEmail(request.Email)
            .UseStaging()
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
}
