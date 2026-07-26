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
    public static void ConfigureServices(IServiceCollection services, DeploymentEnvironment deploymentEnvironment, IConfigurationRoot configuration)
    {
        services.AddSingleton(deploymentEnvironment);
        services.AddSingleton(configuration);

        services.AddSingleton(x =>
        {
            DeploymentEnvironment environment = x.GetRequiredService<DeploymentEnvironment>();
            Database database = new();
            database.OpenAsync(environment.DataDirectoryPath).GetAwaiter().GetResult();
            return database;
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IFileSystemService, FileSystemService>();
        services.AddTransient<INnApiClient, NnApiClient>();

        services.AddUseCaseEngine(options =>
        {
            options.AddFromAssemblyContaining<ImportAccountUseCase>();
        });
    }
}
