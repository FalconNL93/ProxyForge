using ProxyForge.Backend.Contracts;

namespace ProxyForge.Backend.Modules.Certbot;

public static class CertbotResultExtensions
{
    public static Certbot.CertbotRenewalResult GetRenewalResult(this CertbotResult result)
    {
        return Certbot.ParseRenewalStdOut(result.StdOut);
    }
}