using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportAccount;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;
using DustInTheWind.RequestR.Extensions.Microsoft.DependencyInjection;
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

        services.AddUseCaseEngine(options =>
        {
            options.AddFromAssemblyContaining<ImportAccountUseCase>();
        });
    }
}
