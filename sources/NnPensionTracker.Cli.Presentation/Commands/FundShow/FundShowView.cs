using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands.FundShow;

internal class FundShowView : ViewBase<FundShowViewModel>
{
	public override void Display(FundShowViewModel viewModel)
	{
		DataGrid dataGrid = new()
		{
			EmptyGridMessage = viewModel.IsFromWeb ? "No data" : "No fund records found."
		};

		dataGrid.Columns.Add("Date", HorizontalAlignment.Center);
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

		foreach (FundNav fundNav in viewModel.FundNavs)
		{
			dataGrid.Rows.Add(
				fundNav.Date,
				fundNav.Value);
		}

		if (viewModel.IsFromWeb)
			dataGrid.Footer = $"Total: {viewModel.FundNavs.Count} records";

		dataGrid.Display();
	}
}
