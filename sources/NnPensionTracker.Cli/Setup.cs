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
using DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DustInTheWind.NnPensionTracker.Cli;

internal static class Setup
{
    public static void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton(services =>
        {
            DeploymentEnvironment deploymentEnvironment = services.GetRequiredService<DeploymentEnvironment>();
            Database database = new();
            database.OpenAsync(deploymentEnvironment.DataDirectoryPath).GetAwaiter().GetResult();
            return database;
        });

        services.AddSingleton(services =>
        {
            DeploymentEnvironment deploymentEnvironment = services.GetRequiredService<DeploymentEnvironment>();
            ConfigurationBuilder configurationBuilder = new();

            foreach (string path in deploymentEnvironment.AppSettingsFilePaths)
                configurationBuilder.AddJsonFile(path, optional: true, reloadOnChange: false);

            return configurationBuilder.Build();
        });

        services.AddSingleton<DeploymentEnvironment>();

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