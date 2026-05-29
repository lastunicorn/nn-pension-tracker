namespace DustInTheWind.NnPensionTracker.Cli;

internal class DeploymentEnvironment
{
    private const string AppDirectoryName = "nn-pension-tracker";
    private const string PortableMarkerFileName = "portable";

    public bool IsPortable { get; } = File.Exists(Path.Combine(AppContext.BaseDirectory, PortableMarkerFileName));

    public IEnumerable<string> AppSettingsFilePaths { get; }

    public string DataDirectoryPath { get; }

    public DeploymentEnvironment()
    {
        AppSettingsFilePaths = ResolveAppSettingsFilePaths();
        DataDirectoryPath = ResolveDataDirectoryPath();
    }

    private IEnumerable<string> ResolveAppSettingsFilePaths()
    {
        const string fileName = "appsettings.json";

        if (IsPortable)
        {
            yield return Path.Combine(AppContext.BaseDirectory, fileName);
            yield break;
        }

        if (OperatingSystem.IsWindows())
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            yield return Path.Combine(commonAppData, AppDirectoryName, fileName);

            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            yield return Path.Combine(userAppData, AppDirectoryName, fileName);
        }
        else if (OperatingSystem.IsLinux())
        {
            yield return Path.Combine("/etc", AppDirectoryName, fileName);

            string userConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            yield return Path.Combine(userConfigDir, AppDirectoryName, fileName);
        }
    }

    private string ResolveDataDirectoryPath()
    {
        if (IsPortable)
            return Path.Combine(AppContext.BaseDirectory, "Data");

        if (OperatingSystem.IsWindows())
        {
            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(userAppData, AppDirectoryName, "Data");
        }

        if (OperatingSystem.IsLinux())
        {
            string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userHome, AppDirectoryName, "Data");
        }

        return Path.Combine(AppContext.BaseDirectory, "Data");
    }
}
