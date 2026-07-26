using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ClearFund;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands;

[NamedCommand("fund-clear", Description = "Clears all fund NAV values from the database.")]
[CommandOrder(24)]
internal class FundClearCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	public FundClearCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		await requestBus.SendAsync(new ClearFundRequest());
	}
}
