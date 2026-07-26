using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportFund;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands;

[NamedCommand("fund-export", Description = "Exports the fund NAV values from the database into a CSV file.")]
[CommandOrder(23)]
internal class FundExportCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	[NamedParameter("file", IsMandatory = true, Description = "The path of the CSV file to be created.")]
	public string FilePath { get; set; }

	[NamedParameter("year", IsMandatory = false, Description = "Exports only the fund values from the specified year.")]
	public int? Year { get; set; }

	public FundExportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		ExportFundRequest request = new()
		{
			FilePath = FilePath,
			Year = Year
		};

		await requestBus.SendAsync(request);
	}
}
