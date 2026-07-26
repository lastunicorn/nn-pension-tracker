namespace DustInTheWind.NnPensionTracker.Cli;

internal class DeploymentEnvironment
{
	private const string DatabaseDirectoryName = "Data";
	private readonly bool isPortable;
	private readonly string appDirectoryName;

	public IEnumerable<string> AppSettingsFilePaths { get; }

	public string DataDirectoryPath { get; }

	public DeploymentEnvironment()
	{
		isPortable = DetectIsPortable();
		appDirectoryName = ResolveAppDirectoryName();

		AppSettingsFilePaths = ResolveAppSettingsFilePaths();
		DataDirectoryPath = ResolveDataDirectoryPath();
	}

	private bool DetectIsPortable()
	{
		string portableMarkerFileName = "portable";
		string portableMarkerFilePath = Path.Combine(AppContext.BaseDirectory, portableMarkerFileName);
		return File.Exists(portableMarkerFilePath);
	}

	private string ResolveAppDirectoryName()
	{
		if (isPortable)
			return string.Empty;

		if (OperatingSystem.IsWindows())
			return "Nn Pension Tracker";

		if (OperatingSystem.IsLinux())
			return "nn-pension-tracker";

		return string.Empty;
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
			return Path.Combine(AppContext.BaseDirectory, DatabaseDirectoryName);

		if (OperatingSystem.IsWindows())
		{
			string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			return Path.Combine(userAppData, appDirectoryName, DatabaseDirectoryName);
		}

		if (OperatingSystem.IsLinux())
		{
			string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			return Path.Combine(userHome, appDirectoryName, DatabaseDirectoryName);
		}

		return Path.Combine(AppContext.BaseDirectory, DatabaseDirectoryName);
	}
}