using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.FundArea.FundClear;

internal class FundClearView : ViewBase<FundClearViewModel>
{
	public override void Display(FundClearViewModel viewModel)
	{
		CustomConsole.WriteLineSuccess("All fund records have been cleared.");
	}
}