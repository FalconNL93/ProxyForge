namespace ProxyForge.Backend.Contracts;

public interface ICertbot
{
    Task<CertbotResult> RunAsync(CertbotRequest request, CancellationToken cancellationToken = default);
}
