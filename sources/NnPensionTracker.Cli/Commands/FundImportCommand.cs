using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromFile;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromWeb;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Commands;

[NamedCommand("fund-import", Description = "Imports fund NAV values from NN's website (--year or --from/--to) or from a CSV file (--file).")]
[CommandOrder(22)]
internal class FundImportCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	[NamedParameter("source", IsMandatory = false, Description = "The source of the fund values: 'web', 'nn-api' or 'file'. When not specified, it is inferred: 'file' if --file is provided, 'web' otherwise.")]
	public string Source { get; set; }

	[NamedParameter("file", IsMandatory = false, Description = "The path of the CSV file containing the fund values. The file must have the format of the historical values file downloadable from NN's website.")]
	public string FilePath { get; set; }

	[NamedParameter("year", IsMandatory = false, Description = "Imports the fund values for the specified year from NN's website.")]
	public int? Year { get; set; }

	[NamedParameter("from", IsMandatory = false, Description = "The start date of the interval to be imported from NN's website.")]
	public DateOnly? FromDate { get; set; }

	[NamedParameter("to", IsMandatory = false, Description = "The end date of the interval to be imported from NN's website.")]
	public DateOnly? ToDate { get; set; }

	[NamedParameter("verbose", IsMandatory = false, Description = "Displays detailed information about each imported value.")]
	public bool VerboseLogging { get; set; }

	public FundImportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		bool importFromFile = Source == "file" || (Source == null && FilePath != null);

		if (importFromFile)
		{
			ImportFundFromFileRequest request = new()
			{
				FilePath = FilePath
			};

			await requestBus.SendAsync(request);
		}
		else if (Source == null || Source == "web" || Source == "nn-api")
		{
			ImportFundFromWebRequest request = new()
			{
				Year = Year,
				FromDate = FromDate,
				ToDate = ToDate,
				VerboseLogging = VerboseLogging
			};

			await requestBus.SendAsync(request);
		}
		else
		{
			throw new Exception($"Unknown source: '{Source}'. Supported sources: 'web', 'nn-api', 'file'.");
		}
	}
}
