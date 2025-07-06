namespace ProxyForge.Backend.Helpers;

public static class Paths
{
    private static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string ToolsDirectory = Path.Combine(BaseDirectory, "tools");
    public static readonly string DataDirectory = Path.Combine(BaseDirectory, "data");

    public static string CertbotExecutable =>
        Path.Combine(ToolsDirectory, OperatingSystem.IsWindows() ? "certbot.exe" : "certbot");

    public static void EnsureDataDirectoryExists()
    {
        if (!Directory.Exists(DataDirectory))
        {
            Directory.CreateDirectory(DataDirectory);
        }
    }
}
