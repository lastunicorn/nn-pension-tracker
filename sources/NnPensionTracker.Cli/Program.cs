using System.Globalization;
using System.Reflection;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Arguments;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
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
using DustInTheWind.RequestR;
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
			Version version = Assembly.GetEntryAssembly()?.GetName().Version;
			CustomConsole.WriteLine($"NN Pension Tracker CLI {version?.ToString(3)}");

			await using ServiceProvider serviceProvider = CreateServiceProvider();
			IConfiguration configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();
			ApplyCultureFromAppSettings(configuration);

			RequestBus requestBus = serviceProvider.GetRequiredService<RequestBus>();
			Arguments arguments = new(args);

			Func<Task> action = CreateAction(arguments, requestBus)
			                     ?? (() => requestBus.SendAsync(new HelpRequest()));

			await action();
		}
		catch (Exception ex)
		{
			CustomConsole.WriteLineError(ex);
		}
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

	private static ServiceProvider CreateServiceProvider()
	{
		ServiceCollection services = new();
		Setup.ConfigureServices(services);
		return services.BuildServiceProvider();
	}

	private static Func<Task> CreateAction(Arguments arguments, RequestBus requestBus)
	{
		if (arguments.Count == 0)
			return null;

		Argument noun = arguments[0];
		if (noun?.Type != ArgumentType.Ordinal)
			return null;

		return TryCreateImportAccountAction(arguments, requestBus)
		       ?? TryCreateExportAccountAction(arguments, requestBus)
		       ?? TryCreateClearAccountAction(arguments, requestBus)
		       ?? TryCreateShowAccountAction(arguments, requestBus)
		       ?? TryCreateImportFundFromFileAction(arguments, requestBus)
		       ?? TryCreateImportFundFromWebAction(arguments, requestBus)
		       ?? TryCreateShowFundFromWebAction(arguments, requestBus)
		       ?? TryCreateExportFundAction(arguments, requestBus)
		       ?? TryCreateClearFundAction(arguments, requestBus)
		       ?? TryCreateShowFundAction(arguments, requestBus);
	}

	private static Func<Task> TryCreateImportAccountAction(Arguments arguments, RequestBus requestBus)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "import")
			return null;

		Argument fileArgument = arguments["file"] ?? arguments[2];

		ImportAccountRequest request = new()
		{
			FilePath = fileArgument?.Value
		};
		return () => requestBus.SendAsync(request);
	}

	private static Func<Task> TryCreateExportAccountAction(Arguments arguments, RequestBus requestBus)
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

		ExportAccountRequest request = new()
		{
			ExportFormat = formatArgument?.Value,
			Year = year
		};
		return () => requestBus.SendAsync(request);
	}

	private static Func<Task> TryCreateClearAccountAction(Arguments arguments, RequestBus requestBus)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "account")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "clear")
			return null;

		return () => requestBus.SendAsync(new ClearAccountRequest());
	}

	private static Func<Task> TryCreateShowAccountAction(Arguments arguments, RequestBus requestBus)
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

		Argument fromArgument = arguments["from"];
		MonthDate? fromMonth;
		if (fromArgument != null)
		{
			if (MonthDate.TryParse(fromArgument.Value!, out MonthDate fromMonthValue))
				fromMonth = fromMonthValue;
			else if (DateTime.TryParse(fromArgument.Value!, out DateTime fromDate))
				fromMonth = new MonthDate(fromDate.Year, fromDate.Month);
			else if (int.TryParse(fromArgument.Value!, out int fromYear))
				fromMonth = new MonthDate(fromYear, 1);
			else
				throw new FormatException("Invalid 'from' month date format. Expected format is MM/yyyy or a valid date.");
		}
		else
		{
			fromMonth = null;
		}

		Argument toArgument = arguments["to"];
		MonthDate? toMonth;
		if (toArgument != null)
		{
			if (MonthDate.TryParse(toArgument.Value!, out MonthDate toMonthValue))
				toMonth = toMonthValue;
			else if (DateTime.TryParse(toArgument.Value!, out DateTime toDate))
				toMonth = new MonthDate(toDate.Year, toDate.Month);
			else if (int.TryParse(toArgument.Value!, out int toYear))
				toMonth = new MonthDate(toYear, 12);
			else
				throw new FormatException("Invalid 'to' month date format. Expected format is MM/yyyy or a valid date.");
		}
		else
		{
			toMonth = null;
		}

		ShowAccountRequest request = new()
		{
			Year = year,
			FromMonth = fromMonth,
			ToMonth = toMonth
		};
		return () => requestBus.SendAsync(request);
	}

	private static Func<Task> TryCreateImportFundFromFileAction(Arguments arguments, RequestBus requestBus)
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

			ImportFundFromFileRequest request = new()
			{
				FilePath = fileArgument?.Value
			};
			return () => requestBus.SendAsync(request);
		}

		return null;
	}

	private static Func<Task> TryCreateImportFundFromWebAction(Arguments arguments, RequestBus requestBus)
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

			ImportFundFromWebRequest request = new()
			{
				Year = year,
				FromDate = fromDate,
				ToDate = toDate,
				VerboseLogging = verboseLogging
			};
			return () => requestBus.SendAsync(request);
		}

		return null;
	}

	private static Func<Task> TryCreateExportFundAction(Arguments arguments, RequestBus requestBus)
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

		ExportFundRequest request = new()
		{
			FilePath = fileArgument.Value,
			Year = year
		};
		return () => requestBus.SendAsync(request);
	}

	private static Func<Task> TryCreateClearFundAction(Arguments arguments, RequestBus requestBus)
	{
		Argument noun = arguments[0];

		if (noun == null || noun.Type != ArgumentType.Ordinal || noun.Value != "fund")
			return null;

		Argument verb = arguments[1];

		if (verb == null || verb.Type != ArgumentType.Ordinal || verb.Value != "clear")
			return null;

		return () => requestBus.SendAsync(new ClearFundRequest());
	}

	private static Func<Task> TryCreateShowFundAction(Arguments arguments, RequestBus requestBus)
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

		ShowFundRequest request = new()
		{
			Year = year,
			FromDate = fromDate,
			ToDate = toDate
		};
		return () => requestBus.SendAsync(request);
	}

	private static Func<Task> TryCreateShowFundFromWebAction(Arguments arguments, RequestBus requestBus)
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

		ShowFundFromWebRequest request = new()
		{
			Year = year,
			FromDate = fromDate,
			ToDate = toDate
		};
		return () => requestBus.SendAsync(request);
	}
}
