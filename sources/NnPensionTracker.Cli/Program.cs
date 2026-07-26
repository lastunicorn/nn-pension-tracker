using System.Globalization;
using System.Reflection;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando.Setup.Microsoft;
using DustInTheWind.NnPensionTracker.Cli.Presentation;
using Microsoft.Extensions.Configuration;

namespace DustInTheWind.NnPensionTracker.Cli;

// account [show] [--year <year>] [--from <month>] [--to <month>]
// account import [<pdf-file-path>] [--file <pdf-file-path>]
// account export [--format pp] [--year <year>]
// account clear

// fund [show] [--source web] [--year <year>] [--from <date>] [--to <date>]
// fund import [--source web|file] [--year <year>] [--from <date>] [--to <date>] [--file <csv-file-path>] [--verbose]
// fund export --file <csv-file-path> [--year <year>]
// fund clear

// help [<command-name>]

internal static class Program
{
	internal static async Task Main(string[] args)
	{
		try
		{
			Version version = Assembly.GetEntryAssembly()?.GetName().Version;
			CustomConsole.WriteLine($"NN Pension Tracker CLI {version?.ToString(3)}");

			DeploymentEnvironment deploymentEnvironment = new();
			IConfigurationRoot configuration = BuildConfiguration(deploymentEnvironment);
			ApplyCultureFromAppSettings(configuration);

			ConsoleTools.Commando.Application application = ApplicationBuilder.Create()
				.RegisterCommandsFromAssemblyContaining(typeof(NounVerbCommandParser))
				.UseCommandParser(typeof(NounVerbCommandParser))
				.ConfigureServices(x => Setup.ConfigureServices(x, deploymentEnvironment, configuration))
				.Build();

			await application.RunAsync(args);
		}
		catch (Exception ex)
		{
			CustomConsole.WriteLineError(ex);
		}
	}

	private static IConfigurationRoot BuildConfiguration(DeploymentEnvironment deploymentEnvironment)
	{
		ConfigurationBuilder configurationBuilder = new();

		foreach (string path in deploymentEnvironment.AppSettingsFilePaths)
			configurationBuilder.AddJsonFile(path, optional: true, reloadOnChange: false);

		return configurationBuilder.Build();
	}

	private static void ApplyCultureFromAppSettings(IConfiguration configuration)
	{
		if (TryReadCultureInfoFromAppSettings(configuration, out CultureInfo cultureInfo))
		{
			CultureInfo.CurrentCulture = cultureInfo;
			CultureInfo.CurrentUICulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		}
	}

	private static bool TryReadCultureInfoFromAppSettings(IConfiguration configuration, out CultureInfo cultureInfo)
	{
		string cultureName = configuration["CultureInfo"];

		if (string.IsNullOrWhiteSpace(cultureName))
		{
			cultureInfo = null;
			return false;
		}

		cultureInfo = CultureInfo.GetCultureInfo(cultureName);
		return true;
	}
}