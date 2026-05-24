using DustInTheWind.ConsoleTools.Arguments;
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
using NnPensionTracker.Ports.FileSystemAccess;
using System.Globalization;
using System.Text.Json;

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
	internal static Task Main(string[] args)
	{
		ApplyCultureFromAppSettings();

		Arguments arguments = new(args);

		IUseCase useCase = CreateUseCase(arguments) ?? new HelpUseCase();
		return useCase.Execute();
	}

	private static IUseCase CreateUseCase(Arguments arguments)
	{
		if (arguments.Count == 0)
			return null;

		Argument noun = arguments[0];
		if (noun?.Type != ArgumentType.Ordinal)
			return null;

		return TryCreateImportAccountUseCase(arguments)
		       ?? TryCreateExportAccountUseCase(arguments)
		       ?? TryCreateClearAccountUseCase(arguments)
		       ?? TryCreateShowAccountUseCase(arguments)
		       ?? TryCreateImportFundFromFileUseCase(arguments)
		       ?? TryCreateImportFundFromWebUseCase(arguments)
		       ?? TryCreateExportFundUseCase(arguments)
		       ?? TryCreateClearFundUseCase(arguments)
		       ?? TryCreateShowFundUseCase(arguments)
		       ?? TryCreateHelpUseCase();
	}

	private static IUseCase TryCreateImportAccountUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "import")
			return null;

		Argument fileArgument = arguments["file"] ?? arguments[2];

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		return new ImportAccountUseCase(unitOfWork)
		{
			FilePath = fileArgument?.Value
		};
	}

	private static IUseCase TryCreateExportAccountUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "export")
			return null;

		Argument formatArgument = arguments["format"];

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		FileSystemService fileSystemService = new();
		return new ExportAccountUseCase(unitOfWork, fileSystemService)
		{
			ExportFormat = formatArgument?.Value
		};
	}

	private static IUseCase TryCreateClearAccountUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "clear")
			return null;

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		return new ClearAccountUseCase(unitOfWork);
	}

	private static IUseCase TryCreateShowAccountUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb != null && verb.Value != "show")
			return null;

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		return new ShowAccountUseCase(unitOfWork);
	}

	private static IUseCase TryCreateImportFundFromFileUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "import")
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

			Database database = OpenDatabase();
			UnitOfWork unitOfWork = new(database);
			FileSystemService fileSystemService = new();
			return new ImportFundFromFileUseCase(unitOfWork, fileSystemService)
			{
				FilePath = fileArgument?.Value
			};
		}

		return null;
	}

	private static IUseCase TryCreateImportFundFromWebUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "import")
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
			Argument fromArgument = arguments["from"];
			Argument toArgument = arguments["to"];

			int? year = yearArgument != null
				? int.Parse(yearArgument.Value!)
				: null;

			DateOnly? fromDate = fromArgument != null
				? DateOnly.Parse(fromArgument.Value!)
				: null;

			DateOnly? toDate = toArgument != null
				? DateOnly.Parse(toArgument.Value!)
				: null;

			Database database = OpenDatabase();
			UnitOfWork unitOfWork = new(database);
			NnApiClient nnApiClient = new();
			return new ImportFundFromWebUseCase(unitOfWork, nnApiClient)
			{
				Year = year,
				FromDate = fromDate,
				ToDate = toDate
			};
		}

		return null;
	}

	private static IUseCase TryCreateExportFundUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "export")
			return null;

		Argument fileArgument = arguments["file"];

		if (fileArgument == null)
			return null;

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		FileSystemService fileSystemService = new();
		return new ExportFundUseCase(unitOfWork, fileSystemService)
		{
			FilePath = fileArgument.Value
		};
	}

	private static IUseCase TryCreateClearFundUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb?.Value != "clear")
			return null;

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		return new ClearFundUseCase(unitOfWork);
	}

	private static IUseCase TryCreateShowFundUseCase(Arguments arguments)
	{
		Argument noun = arguments[0];

		if (noun?.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb != null && verb.Value != "show")
			return null;

		Database database = OpenDatabase();
		UnitOfWork unitOfWork = new(database);
		return new ShowFundUseCase(unitOfWork);
	}

	private static IUseCase TryCreateHelpUseCase()
	{
		return new HelpUseCase();
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

		using FileStream stream = File.OpenRead(filePath);
		using JsonDocument document = JsonDocument.Parse(stream);

		if (TryReadCultureName(document.RootElement, out string cultureName))
		{
			cultureInfo = CultureInfo.GetCultureInfo(cultureName);
			return true;
		}

		cultureInfo = CultureInfo.CurrentCulture;
		return false;
	}

	private static void ApplyCultureFromAppSettings()
	{
		TryReadCultureInfoFromAppSettings(null, out CultureInfo cultureInfo);

		CultureInfo.CurrentCulture = cultureInfo;
		CultureInfo.CurrentUICulture = cultureInfo;
		CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
		CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
	}

	private static bool TryReadCultureName(JsonElement rootElement, out string cultureName)
	{
		if (rootElement.TryGetProperty("CultureInfo", out JsonElement cultureInfoElement))
		{
			cultureName = cultureInfoElement.GetString() ?? string.Empty;
			if (!string.IsNullOrWhiteSpace(cultureName))
				return true;
		}

		cultureName = string.Empty;
		return false;
	}

	private static Database OpenDatabase()
	{
		Database database = new();
		database.OpenAsync().GetAwaiter().GetResult();
		return database;
	}
}