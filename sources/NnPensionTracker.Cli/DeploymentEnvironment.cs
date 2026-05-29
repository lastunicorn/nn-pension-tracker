namespace DustInTheWind.NnPensionTracker.Cli;

internal class DeploymentEnvironment
{
    private readonly bool isPortable;
    private readonly string appDirectoryName;

    public IEnumerable<string> AppSettingsFilePaths { get; }

    public string DataDirectoryPath { get; }

    public DeploymentEnvironment()
    {
        string portableMarkerFileName = "portable";
        string portableMarkerFilePath = Path.Combine(AppContext.BaseDirectory, portableMarkerFileName);
        isPortable = File.Exists(portableMarkerFilePath);

        if (isPortable)
            appDirectoryName = string.Empty;
        else if (OperatingSystem.IsWindows())
            appDirectoryName = "Nn Pension Tracker";
        else if (OperatingSystem.IsLinux())
            appDirectoryName = ".nn-pension-tracker";
        else
            appDirectoryName = string.Empty;

        AppSettingsFilePaths = ResolveAppSettingsFilePaths();
        DataDirectoryPath = ResolveDataDirectoryPath();
    }

    private IEnumerable<string> ResolveAppSettingsFilePaths()
    {
        const string fileName = "appsettings.json";

        if (isPortable)
        {
            yield return Path.Combine(AppContext.BaseDirectory, fileName);
            yield break;
        }

        if (OperatingSystem.IsWindows())
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            yield return Path.Combine(commonAppData, appDirectoryName, fileName);

            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            yield return Path.Combine(userAppData, appDirectoryName, fileName);
        }
        else if (OperatingSystem.IsLinux())
        {
            yield return Path.Combine("/etc", appDirectoryName, fileName);

            string userConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            yield return Path.Combine(userConfigDir, appDirectoryName, fileName);
        }
    }

    private string ResolveDataDirectoryPath()
    {
        if (isPortable)
            return Path.Combine(AppContext.BaseDirectory, "Data");

        if (OperatingSystem.IsWindows())
        {
            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(userAppData, appDirectoryName, "Data");
        }

        if (OperatingSystem.IsLinux())
        {
            string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userHome, appDirectoryName, "Data");
        }

        return Path.Combine(AppContext.BaseDirectory, "Data");
    }
}
