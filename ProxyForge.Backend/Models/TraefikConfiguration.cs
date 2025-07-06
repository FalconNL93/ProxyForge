using YamlDotNet.Serialization;

namespace ProxyForge.Backend.Models;

public class TraefikConfiguration
{
    [YamlMember(Alias = "entryPoints")]
    public Dictionary<string, EntryPoint> EntryPoints { get; set; }

    [YamlMember(Alias = "providers")]
    public Providers Providers { get; set; }

    [YamlMember(Alias = "api")]
    public Api Api { get; set; }

    [YamlMember(Alias = "log")]
    public Log Log { get; set; }

    [YamlMember(Alias = "accessLog")]
    public AccessLog AccessLog { get; set; }

    [YamlMember(Alias = "metrics")]
    public Metrics Metrics { get; set; }

    [YamlMember(Alias = "tracing")]
    public Tracing Tracing { get; set; }

    [YamlMember(Alias = "http")]
    public HttpConfiguration Http { get; set; }

    [YamlMember(Alias = "tls")]
    public TlsConfiguration Tls { get; set; }

    [YamlMember(Alias = "experimental")]
    public Experimental Experimental { get; set; }
}

public class EntryPoint
{
    [YamlMember(Alias = "address")]
    public string Address { get; set; }

    [YamlMember(Alias = "http")]
    public EntryPointHttp Http { get; set; }
}

public class EntryPointHttp
{
    [YamlMember(Alias = "redirections")]
    public Redirections Redirections { get; set; }
}

public class Redirections
{
    [YamlMember(Alias = "entryPoint")]
    public EntryPointRedirection EntryPoint { get; set; }
}

public class EntryPointRedirection
{
    [YamlMember(Alias = "to")]
    public string To { get; set; }

    [YamlMember(Alias = "scheme")]
    public string Scheme { get; set; }

    [YamlMember(Alias = "permanent")]
    public bool Permanent { get; set; }
}

public class Providers
{
    [YamlMember(Alias = "file")]
    public FileProvider File { get; set; }

    [YamlMember(Alias = "docker")]
    public DockerProvider Docker { get; set; }

    [YamlMember(Alias = "kubernetesCRD")]
    public KubernetesProvider KubernetesCRD { get; set; }

    [YamlMember(Alias = "consulCatalog")]
    public ConsulCatalogProvider ConsulCatalog { get; set; }
}

public class FileProvider
{
    [YamlMember(Alias = "directory")]
    public string Directory { get; set; }

    [YamlMember(Alias = "watch")]
    public bool Watch { get; set; }
}

public class DockerProvider
{
    [YamlMember(Alias = "endpoint")]
    public string Endpoint { get; set; }

    [YamlMember(Alias = "exposedByDefault")]
    public bool ExposedByDefault { get; set; }
}

public class KubernetesProvider
{
    [YamlMember(Alias = "endpoint")]
    public string Endpoint { get; set; }
}

public class ConsulCatalogProvider
{
    [YamlMember(Alias = "endpoint")]
    public string Endpoint { get; set; }
}

public class Api
{
    [YamlMember(Alias = "dashboard")]
    public bool Dashboard { get; set; }
}

public class Log
{
    [YamlMember(Alias = "level")]
    public string Level { get; set; }
}

public class AccessLog
{
    [YamlMember(Alias = "filePath")]
    public string FilePath { get; set; }

    [YamlMember(Alias = "format")]
    public string Format { get; set; }

    [YamlMember(Alias = "filters")]
    public AccessLogFilters Filters { get; set; }
}

public class AccessLogFilters
{
    [YamlMember(Alias = "statusCodes")]
    public List<string> StatusCodes { get; set; }

    [YamlMember(Alias = "retryAttempts")]
    public bool RetryAttempts { get; set; }
}

public class Metrics
{
    [YamlMember(Alias = "prometheus")]
    public PrometheusMetrics Prometheus { get; set; }
}

public class PrometheusMetrics
{
    [YamlMember(Alias = "buckets")]
    public List<double> Buckets { get; set; }

    [YamlMember(Alias = "entryPoint")]
    public string EntryPoint { get; set; }
}

public class Tracing
{
    [YamlMember(Alias = "serviceName")]
    public string ServiceName { get; set; }

    [YamlMember(Alias = "jaeger")]
    public JaegerTracing Jaeger { get; set; }
}

public class JaegerTracing
{
    [YamlMember(Alias = "samplingServerURL")]
    public string SamplingServerUrl { get; set; }

    [YamlMember(Alias = "localAgentHostPort")]
    public string LocalAgentHostPort { get; set; }
}

public class HttpConfiguration
{
    [YamlMember(Alias = "routers")]
    public Dictionary<string, Router> Routers { get; set; }

    [YamlMember(Alias = "services")]
    public Dictionary<string, Service> Services { get; set; }

    [YamlMember(Alias = "middlewares")]
    public Dictionary<string, Middleware> Middlewares { get; set; }
}

public class Router
{
    [YamlMember(Alias = "rule")]
    public string Rule { get; set; }

    [YamlMember(Alias = "service")]
    public string Service { get; set; }

    [YamlMember(Alias = "entryPoints")]
    public List<string> EntryPoints { get; set; }

    [YamlMember(Alias = "middlewares")]
    public List<string> Middlewares { get; set; }

    [YamlMember(Alias = "tls")]
    public RouterTls Tls { get; set; }
}

public class RouterTls
{
    [YamlMember(Alias = "certResolver")]
    public string CertResolver { get; set; }

    [YamlMember(Alias = "domains")]
    public List<TlsDomain> Domains { get; set; }
}

public class TlsDomain
{
    [YamlMember(Alias = "main")]
    public string Main { get; set; }

    [YamlMember(Alias = "sans")]
    public List<string> Sans { get; set; }
}

public class Service
{
    [YamlMember(Alias = "loadBalancer")]
    public LoadBalancer LoadBalancer { get; set; }
}

public class LoadBalancer
{
    [YamlMember(Alias = "servers")]
    public List<Server> Servers { get; set; }

    [YamlMember(Alias = "passHostHeader")]
    public bool PassHostHeader { get; set; }
}

public class Server
{
    [YamlMember(Alias = "url")]
    public string Url { get; set; }

    [YamlMember(Alias = "weight")]
    public int Weight { get; set; }
}

public class Middleware
{
    [YamlMember(Alias = "addPrefix")]
    public AddPrefix AddPrefix { get; set; }

    [YamlMember(Alias = "stripPrefix")]
    public StripPrefix StripPrefix { get; set; }

    [YamlMember(Alias = "basicAuth")]
    public BasicAuth BasicAuth { get; set; }
}

public class AddPrefix
{
    [YamlMember(Alias = "prefix")]
    public string Prefix { get; set; }
}

public class StripPrefix
{
    [YamlMember(Alias = "prefixes")]
    public List<string> Prefixes { get; set; }
}

public class BasicAuth
{
    [YamlMember(Alias = "users")]
    public List<string> Users { get; set; }
}

public class TlsConfiguration
{
    [YamlMember(Alias = "certificatesResolvers")]
    public Dictionary<string, CertificatesResolver> CertificatesResolvers { get; set; }

    [YamlMember(Alias = "options")]
    public Dictionary<string, TlsOptions> Options { get; set; }
}

public class CertificatesResolver
{
    [YamlMember(Alias = "acme")]
    public AcmeConfiguration Acme { get; set; }
}

public class AcmeConfiguration
{
    [YamlMember(Alias = "email")]
    public string Email { get; set; }

    [YamlMember(Alias = "storage")]
    public string Storage { get; set; }

    [YamlMember(Alias = "dnsChallenge")]
    public DnsChallenge DnsChallenge { get; set; }
}

public class DnsChallenge
{
    [YamlMember(Alias = "provider")]
    public string Provider { get; set; }

    [YamlMember(Alias = "delayBeforeCheck")]
    public int DelayBeforeCheck { get; set; }

    [YamlMember(Alias = "resolvers")]
    public List<string> Resolvers { get; set; }
}

public class TlsOptions
{
    [YamlMember(Alias = "minVersion")]
    public string MinVersion { get; set; }

    [YamlMember(Alias = "cipherSuites")]
    public List<string> CipherSuites { get; set; }

    [YamlMember(Alias = "clientAuth")]
    public ClientAuth ClientAuth { get; set; }
}

public class ClientAuth
{
    [YamlMember(Alias = "caFiles")]
    public List<string> CaFiles { get; set; }

    [YamlMember(Alias = "clientAuthType")]
    public string ClientAuthType { get; set; }
}

public class Experimental
{
    [YamlMember(Alias = "plugins")]
    public Dictionary<string, Plugin> Plugins { get; set; }
}

public class Plugin
{
    [YamlMember(Alias = "moduleName")]
    public string ModuleName { get; set; }

    [YamlMember(Alias = "version")]
    public string Version { get; set; }

    [YamlMember(Alias = "config")]
    public Dictionary<string, object> Config { get; set; }
}
