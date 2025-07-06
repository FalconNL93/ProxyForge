using System.Diagnostics;
using ProxyForge.Backend.Contracts;
using ProxyForge.Backend.Helpers;

namespace ProxyForge.Backend.Builders;

public sealed class CertbotCommandBuilder
{
    private readonly List<string> _arguments = [];
    private readonly HashSet<string> _domains = [];
    private readonly List<ICertbotPlugin> _plugins = [];
    private bool _agreeTos;
    private CertbotCommand _command = CertbotCommand.CertOnly;
    private string? _email;
    private bool _forceRenewal;
    private bool _staging;
    private string _venvPath = ".venv";

    public CertbotCommandBuilder UseCommand(CertbotCommand command)
    {
        _command = command;
        return this;
    }

    public CertbotCommandBuilder AddDomain(string domain)
    {
        _domains.Add(domain);
        return this;
    }

    public CertbotCommandBuilder AddDomains(params string[] domains)
    {
        foreach (var domain in domains)
        {
            _domains.Add(domain);
        }

        return this;
    }

    public CertbotCommandBuilder AddPlugin(ICertbotPlugin plugin)
    {
        _plugins.Add(plugin);
        return this;
    }

    public CertbotCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CertbotCommandBuilder AgreeTos()
    {
        _agreeTos = true;
        return this;
    }

    public CertbotCommandBuilder UseStaging()
    {
        _staging = true;
        return this;
    }

    public CertbotCommandBuilder ForceRenewal()
    {
        _forceRenewal = true;
        return this;
    }

    public CertbotCommandBuilder UseVenvPath(string path)
    {
        _venvPath = path;
        return this;
    }

    public ProcessStartInfo Build()
    {
        _arguments.Add(GetCommandString(_command));

        foreach (var domain in _domains)
        {
            _arguments.Add($"-d {domain}");
        }

        foreach (var plugin in _plugins)
        {
            _arguments.AddRange(plugin.GetArguments());
        }

        if (!string.IsNullOrWhiteSpace(_email))
        {
            _arguments.Add($"--email \"{_email}\"");
        }

        if (_agreeTos)
        {
            _arguments.Add("--agree-tos");
        }

        if (_staging)
        {
            _arguments.Add("--staging");
        }

        if (_forceRenewal)
        {
            _arguments.Add("--force-renewal");
        }

        var certbotPath = Paths.CertbotExecutable;
        return new ProcessStartInfo
        {
            FileName = certbotPath,
            Arguments = string.Join(" ", _arguments),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }

    private static string GetCommandString(CertbotCommand command)
    {
        return command switch
        {
            CertbotCommand.CertOnly => "certonly",
            CertbotCommand.Renew => "renew",
            CertbotCommand.Revoke => "revoke",
            CertbotCommand.Register => "register",
            CertbotCommand.Delete => "delete",
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };
    }
}
