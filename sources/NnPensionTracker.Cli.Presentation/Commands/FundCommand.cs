using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands;

// The "fund" noun used without a verb defaults to the show action.
[NamedCommand("fund", Description = "Displays the fund NAV values from the database. Same as 'fund show'.")]
[CommandOrder(20)]
internal class FundCommand : FundShowCommand
{
	public FundCommand(RequestBus requestBus)
		: base(requestBus)
	{
	}
}
