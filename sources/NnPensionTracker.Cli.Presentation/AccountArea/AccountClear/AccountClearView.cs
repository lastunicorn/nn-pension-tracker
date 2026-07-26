using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.AccountArea.AccountClear;

internal class AccountClearView : ViewBase<AccountClearViewModel>
{
	public override void Display(AccountClearViewModel viewModel)
	{
		Console.WriteLine("All contributions have been cleared from the database.");
	}
}