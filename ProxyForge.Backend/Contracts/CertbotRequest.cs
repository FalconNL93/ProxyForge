namespace ProxyForge.Backend.Contracts;

public sealed class CertbotRequest
{
    public required string[] Domains { get; init; }
    public required ICertbotPlugin Plugin { get; init; }
    public required string Email { get; init; }
    public bool UseStaging { get; init; }
    public bool ForceRenewal { get; init; }
    public string VenvPath { get; init; } = ".venv";
}
