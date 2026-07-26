using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ExportAccount;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.AccountArea.AccountExport;

[NamedCommand("account-export", Description = "Exports the contribution records from the database to a file. Supported formats: 'pp' (CSV files for PortfolioPerformance).")]
[CommandOrder(13)]
internal class AccountExportCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	[NamedParameter("format", IsMandatory = false, Description = "The export format. Supported formats: 'pp' (default).")]
	public string ExportFormat { get; set; }

	[NamedParameter("year", IsMandatory = false, Description = "Exports only the contributions from the specified year.")]
	public int? Year { get; set; }

	public AccountExportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		ExportAccountRequest request = new()
		{
			ExportFormat = ExportFormat,
			Year = Year
		};

		await requestBus.SendAsync(request);
	}
}