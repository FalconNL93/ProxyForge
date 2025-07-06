using ProxyForge.Backend.Contracts;

namespace ProxyForge.Backend.CertbotPlugins;

public sealed class DnsCloudflarePlugin(string credentialsFile) : ICertbotPlugin
{
    public IEnumerable<string> GetArguments()
    {
        return
        [
            "--dns-cloudflare",
            $"--dns-cloudflare-credentials {credentialsFile}"
        ];
    }
}