using Microsoft.AspNetCore.Mvc;
using ProxyForge.Backend.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ProxyForge.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController(ILogger<TestController> logger) : ControllerBase
{
    private readonly ILogger<TestController> _logger = logger;

    [HttpGet("generate")]
    public string Get()
    {
        var config = new TraefikConfiguration
        {
            EntryPoints = new Dictionary<string, EntryPoint>
            {
                ["web"] = new()
                {
                    Address = ":80"
                }
            },
            Providers = new Providers
            {
                File = new FileProvider
                {
                    Directory = "./dynamic"
                }
            },
            Http = new HttpConfiguration
            {
                Routers = new Dictionary<string, Router>
                {
                    ["my-reverse-proxy"] = new()
                    {
                        Rule = "Host(`localhost`)",
                        Service = "my-backend",
                        EntryPoints = ["web"]
                    }
                },
                Services = new Dictionary<string, Service>
                {
                    ["my-backend"] = new()
                    {
                        LoadBalancer = new LoadBalancer
                        {
                            Servers =
                            [
                                new Server
                                {
                                    Url = "http://localhost:8080"
                                }
                            ],
                            PassHostHeader = true
                        }
                    }
                }
            }
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(config);
        System.IO.File.WriteAllText("/etc/traefik/dynamic/test.yaml", yaml);

        return yaml;
    }
}
