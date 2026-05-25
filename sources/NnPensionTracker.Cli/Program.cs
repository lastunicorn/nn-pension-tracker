using System.Globalization;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Arguments;
using DustInTheWind.NnPensionTracker.Cli.Presentation;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DustInTheWind.NnPensionTracker.Cli;

// account import [<pdf-file-path>] - Imports 
// account import [--file <pdf-file-path>]
// account clear
// account export [--format pp]
// account

// fund import --from 2026-01-01 --to 2026-12-31
// fund import --year 2026
// fund import --file "historical_2008.csv"
// fund clear
// fund

// help

internal static class Program
{
	internal static async Task Main(string[] args)
	{
		try
		{
			ApplyCultureFromAppSettings();

			Arguments arguments = new(args);
			await using ServiceProvider serviceProvider = CreateServiceProvider();

			IUseCase useCase = CreateUseCase(arguments, serviceProvider)
			                   ?? serviceProvider.GetRequiredService<HelpUseCase>();

			await useCase.Execute();
		}
		catch (Exception ex)
		{
			CustomConsole.WriteLineError(ex);
		}
	}

	private static void ApplyCultureFromAppSettings()
	{
		TryReadCultureInfoFromAppSettings(null, out CultureInfo cultureInfo);

		CultureInfo.CurrentCulture = cultureInfo;
		CultureInfo.CurrentUICulture = cultureInfo;
		CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
		CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
	}

	internal static bool TryReadCultureInfoFromAppSettings(string appSettingsPath, out CultureInfo cultureInfo)
	{
		string filePath = string.IsNullOrWhiteSpace(appSettingsPath)
			? Path.Combine(AppContext.BaseDirectory, "appsettings.json")
			: appSettingsPath;

		if (!File.Exists(filePath))
		{
			cultureInfo = CultureInfo.CurrentCulture;
			return false;
		}

		IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(filePath) ?? AppContext.BaseDirectory)
			.AddJsonFile(Path.GetFileName(filePath), optional: false, reloadOnChange: false)
			.Build();

		string cultureName = configuration["CultureInfo"];

		if (!string.IsNullOrWhiteSpace(cultureName))
		{
			cultureInfo = CultureInfo.GetCultureInfo(cultureName);
			return true;
		}

		cultureInfo = CultureInfo.CurrentCulture;
		return false;
	}

	private static ServiceProvider CreateServiceProvider()
	{
		ServiceCollection services = new();
		Setup.ConfigureServices(services);
		return services.BuildServiceProvider();
	}

	private static IUseCase CreateUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		if (arguments.Count == 0)
			return null;

		Argument noun = arguments[0];
		if (noun?.Type != ArgumentType.Ordinal)
			return null;

		return TryCreateImportAccountUseCase(arguments, serviceProvider)
		       ?? TryCreateExportAccountUseCase(arguments, serviceProvider)
		       ?? TryCreateClearAccountUseCase(arguments, serviceProvider)
		       ?? TryCreateShowAccountUseCase(arguments, serviceProvider)
		       ?? TryCreateImportFundFromFileUseCase(arguments, serviceProvider)
		       ?? TryCreateImportFundFromWebUseCase(arguments, serviceProvider)
		       ?? TryCreateShowFundFromWebUseCase(arguments, serviceProvider)
		       ?? TryCreateExportFundUseCase(arguments, serviceProvider)
		       ?? TryCreateClearFundUseCase(arguments, serviceProvider)
		       ?? TryCreateShowFundUseCase(arguments, serviceProvider)
		       ?? TryCreateHelpUseCase(serviceProvider);
	}

	private static IUseCase TryCreateImportAccountUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "import")
			return null;

		Argument fileArgument = arguments["file"] ?? arguments[2];

		ImportAccountUseCase useCase = serviceProvider.GetRequiredService<ImportAccountUseCase>();
		useCase.FilePath = fileArgument?.Value;
		return useCase;
	}

	private static IUseCase TryCreateExportAccountUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "export")
			return null;

		Argument formatArgument = arguments["format"];
		Argument yearArgument = arguments["year"];
		int? year = yearArgument != null
			? int.Parse(yearArgument.Value!)
			: null;

		ExportAccountUseCase useCase = serviceProvider.GetRequiredService<ExportAccountUseCase>();
		useCase.ExportFormat = formatArgument?.Value;
		useCase.Year = year;
		return useCase;
	}

	private static IUseCase TryCreateClearAccountUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "clear")
			return null;

		return serviceProvider.GetRequiredService<ClearAccountUseCase>();
	}

	private static IUseCase TryCreateShowAccountUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb != null && verb.Type == ArgumentType.Ordinal && verb.Value != "show")
			return null;

		Argument yearArgument = arguments["year"];
		int? year = yearArgument != null
			? int.Parse(yearArgument.Value!)
			: null;

		Argument monthArgument = arguments["month"];
		int? month = null;
		if (year.HasValue && monthArgument != null)
			month = int.Parse(monthArgument.Value!);

		ShowAccountUseCase useCase = serviceProvider.GetRequiredService<ShowAccountUseCase>();
		useCase.Year = year;
		useCase.Month = month;
		return useCase;
	}

	private static IUseCase TryCreateImportFundFromFileUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "import")
			return null;

		Argument sourceArgument = arguments["source"];

		// if --source argument is provided and is 'file'
		// OR
		// if --source argument is not provided and --file argument is provided

		bool isMatch = sourceArgument != null && sourceArgument.Value == "file";

		if (!isMatch)
			isMatch = sourceArgument == null && arguments["file"] != null;

		if (isMatch)
		{
			Argument fileArgument = arguments["file"];

			ImportFundFromFileUseCase useCase = serviceProvider.GetRequiredService<ImportFundFromFileUseCase>();
			useCase.FilePath = fileArgument?.Value;
			return useCase;
		}

		return null;
	}

	private static IUseCase TryCreateImportFundFromWebUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "import")
			return null;

		Argument sourceArgument = arguments["source"];

		// if --source argument is provided and is 'nn-api' or 'web'
		// OR
		// if --source argument is not provided and --year argument is provided

		bool isMatch = sourceArgument != null && (sourceArgument.Value == "nn-api" || sourceArgument.Value == "web");

		if (!isMatch)
			isMatch = sourceArgument == null && (arguments["year"] != null || arguments["from"] != null || arguments["to"] != null);

		if (isMatch)
		{
			Argument yearArgument = arguments["year"];
			int? year = yearArgument != null
				? int.Parse(yearArgument.Value!)
				: null;

			Argument fromArgument = arguments["from"];
			DateOnly? fromDate = fromArgument != null
				? DateOnly.Parse(fromArgument.Value!)
				: null;

			Argument toArgument = arguments["to"];
			DateOnly? toDate = toArgument != null
				? DateOnly.Parse(toArgument.Value!)
				: null;

			Argument verboseArgument = arguments["verbose"];

			bool verboseLogging = verboseArgument != null && (verboseArgument.Value == null || verboseArgument.Value == "true");

			ImportFundFromWebUseCase useCase = serviceProvider.GetRequiredService<ImportFundFromWebUseCase>();
			useCase.Year = year;
			useCase.FromDate = fromDate;
			useCase.ToDate = toDate;
			useCase.VerboseLogging = verboseLogging;
			return useCase;
		}

		return null;
	}

	private static IUseCase TryCreateExportFundUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "export")
			return null;

		Argument fileArgument = arguments["file"];

		if (fileArgument == null)
			return null;

		Argument yearArgument = arguments["year"];
		int? year = yearArgument != null
			? int.Parse(yearArgument.Value!)
			: null;

		ExportFundUseCase useCase = serviceProvider.GetRequiredService<ExportFundUseCase>();
		useCase.FilePath = fileArgument.Value;
		useCase.Year = year;
		return useCase;
	}

	private static IUseCase TryCreateClearFundUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "clear")
			return null;

		return serviceProvider.GetRequiredService<ClearFundUseCase>();
	}

	private static IUseCase TryCreateShowFundUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb != null && verb.Type == ArgumentType.Ordinal && verb.Value != "show")
			return null;

		Argument yearArgument = arguments["year"];
		int? year = yearArgument != null
			? int.Parse(yearArgument.Value!)
			: null;

		Argument fromArgument = arguments["from"];
		DateOnly? fromDate = fromArgument != null
			? DateOnly.Parse(fromArgument.Value!)
			: null;

		Argument toArgument = arguments["to"];
		DateOnly? toDate = toArgument != null
			? DateOnly.Parse(toArgument.Value!)
			: null;

		ShowFundUseCase useCase = serviceProvider.GetRequiredService<ShowFundUseCase>();
		useCase.Year = year;
		useCase.FromDate = fromDate;
		useCase.ToDate = toDate;
		return useCase;
	}

	private static IUseCase TryCreateShowFundFromWebUseCase(Arguments arguments, IServiceProvider serviceProvider)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "show")
			return null;

		Argument sourceArgument = arguments["source"];

		if (sourceArgument == null || (sourceArgument.Value != "nn-api" && sourceArgument.Value != "web"))
			return null;

		Argument yearArgument = arguments["year"];
		int? year = yearArgument != null
			? int.Parse(yearArgument.Value!)
			: null;

		Argument fromArgument = arguments["from"];
		DateOnly? fromDate = fromArgument != null
			? DateOnly.Parse(fromArgument.Value!)
			: null;

		Argument toArgument = arguments["to"];
		DateOnly? toDate = toArgument != null
			? DateOnly.Parse(toArgument.Value!)
			: null;

		ShowFundFromWebUseCase useCase = serviceProvider.GetRequiredService<ShowFundFromWebUseCase>();
		useCase.Year = year;
		useCase.FromDate = fromDate;
		useCase.ToDate = toDate;
		return useCase;
	}

	private static IUseCase TryCreateHelpUseCase(IServiceProvider serviceProvider)
	{
		return serviceProvider.GetRequiredService<HelpUseCase>();
	}
}