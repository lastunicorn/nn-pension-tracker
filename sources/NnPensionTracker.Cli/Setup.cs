using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ClearAccount;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ClearFund;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ExportAccount;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ExportFund;
using DustInTheWind.NnPensionTracker.Cli.UseCases.Help;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportAccount;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromFile;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ShowAccount;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ShowFund;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
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
		services.AddTransient<HelpUseCase>();
	}
}