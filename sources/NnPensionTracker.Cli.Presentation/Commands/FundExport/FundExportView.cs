using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands.FundExport;

internal class FundExportView : ViewBase<FundExportViewModel>
{
	public override void Display(FundExportViewModel viewModel)
	{
		Console.WriteLine();
		CustomConsole.WriteLineSuccess($"{viewModel.ExportedCount} fund NAV values were exported to: '{viewModel.FilePath}'");
	}
}