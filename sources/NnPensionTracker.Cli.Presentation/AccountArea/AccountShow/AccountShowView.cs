using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.AccountArea.AccountShow;

internal class AccountShowView : ViewBase<AccountShowViewModel>
{
	public override void Display(AccountShowViewModel viewModel)
	{
		DataGrid dataGrid = new()
		{
			EmptyGridMessage = "No data"
		};

		dataGrid.Columns.Add("Month", HorizontalAlignment.Center);
		dataGrid.Columns.Add("Gross Value", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Administration Fee", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Net Value", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Unit Value", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Unit Count", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Paid in Month", HorizontalAlignment.Center);

		foreach (Contribution contribution in viewModel.Contributions)
		{
			dataGrid.Rows.Add(
				contribution.Month,
				contribution.GrossValue,
				contribution.AdministrationFee,
				contribution.NetValue,
				contribution.UnitValue,
				contribution.UnitCount,
				contribution.PaidInMonth);
		}

		dataGrid.Display();
	}
}