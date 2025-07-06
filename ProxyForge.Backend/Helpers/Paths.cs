namespace ProxyForge.Backend.Helpers;

public static class Paths
{
    public static readonly string ToolsDirectory =
        Path.Combine(AppContext.BaseDirectory, "tools");

    public static string CertbotExecutable =>
        Path.Combine(ToolsDirectory,
            OperatingSystem.IsWindows() ? "certbot.exe" : "certbot");
}
