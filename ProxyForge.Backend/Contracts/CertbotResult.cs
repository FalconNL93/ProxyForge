namespace ProxyForge.Backend.Contracts;

public sealed class CertbotResult
{
    public required int ExitCode { get; init; }
    public required string StdOut { get; init; }
    public required string StdErr { get; init; }
    public bool Success => ExitCode == 0;
}
