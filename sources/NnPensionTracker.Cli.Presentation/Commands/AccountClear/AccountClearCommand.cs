using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ClearAccount;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands.AccountClear;

[NamedCommand("account-clear", Description = "Clears all contribution records from the database.")]
[CommandOrder(14)]
internal class AccountClearCommand : IConsoleCommand<AccountClearViewModel>
{
	private readonly RequestBus requestBus;

	public AccountClearCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<AccountClearViewModel> Execute()
	{
		await requestBus.SendAsync(new ClearAccountRequest());

		return new AccountClearViewModel();
	}
}