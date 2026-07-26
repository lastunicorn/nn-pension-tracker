using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ClearAccount;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands;

[NamedCommand("account-clear", Description = "Clears all contribution records from the database.")]
[CommandOrder(14)]
internal class AccountClearCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	public AccountClearCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		await requestBus.SendAsync(new ClearAccountRequest());
	}
}
