using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ClearAccount;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ClearFund;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportAccount;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportFund;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.Help;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportAccount;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromFile;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromWeb;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowAccount;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowFund;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowFundFromWeb;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Cli;

internal static class Setup
{
	public static void ConfigureServices(ServiceCollection services)
	{
		services.AddSingleton(_ =>
		{
			Database database = new();
			database.OpenAsync().GetAwaiter().GetResult();
			return database;
		});

		services.AddSingleton(_ =>
		{
			IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: false);

			if (OperatingSystem.IsLinux())
			{
				// system config
				configurationBuilder.AddJsonFile(Path.Combine("/etc/nn-pension-tracker", "appsettings.json"), optional: true, reloadOnChange: false);

				// user config
				string userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config/nn-pension-tracker", "appsettings.json");
				configurationBuilder.AddJsonFile(userConfigPath, optional: true, reloadOnChange: false);
			}

			if (OperatingSystem.IsWindows())
			{
				// system config
				// tbd
				
				// user config
				string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				configurationBuilder.AddJsonFile(Path.Combine(applicationDataPath, "appsettings.json"), optional: true, reloadOnChange: false);
			}

			return configurationBuilder.Build();
		});

		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddTransient<IFileSystemService, FileSystemService>();
		services.AddTransient<INnApiClient, NnApiClient>();

		services.AddTransient<ImportAccountUseCase>();
		services.AddTransient<ExportAccountUseCase>();
		services.AddTransient<ClearAccountUseCase>();
		services.AddTransient<ShowAccountUseCase>();
		services.AddTransient<ImportFundFromFileUseCase>();
		services.AddTransient<ImportFundFromWebUseCase>();
		services.AddTransient<ExportFundUseCase>();
		services.AddTransient<ClearFundUseCase>();
		services.AddTransient<ShowFundUseCase>();
		services.AddTransient<ShowFundFromWebUseCase>();
		services.AddTransient<HelpUseCase>();
	}
}