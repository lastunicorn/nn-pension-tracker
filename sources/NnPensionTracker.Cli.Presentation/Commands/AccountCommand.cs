using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands;

// The "account" noun used without a verb defaults to the show action.
[NamedCommand("account", Description = "Displays the contributions from the current account. Same as 'account show'.")]
[CommandOrder(10)]
internal class AccountCommand : AccountShowCommand
{
	public AccountCommand(RequestBus requestBus)
		: base(requestBus)
	{
	}
}
