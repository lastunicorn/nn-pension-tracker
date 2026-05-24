using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ShowFund;

internal class ShowFundUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;

	public ShowFundUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute()
	{
		List<FundNav> fundRecords = [];

		await foreach (FundNav fundRecord in unitOfWork.FundNavRepository.GetAll())
			fundRecords.Add(fundRecord);

		fundRecords = fundRecords
			.OrderBy(x => x.Date)
			.ToList();
		DisplayFundRecords(fundRecords);
	}

	private void DisplayFundRecords(IEnumerable<FundNav> fundRecords)
	{
		DataGrid dataGrid = new();

		dataGrid.Columns.Add("Date", HorizontalAlignment.Center);
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

		foreach (FundNav fundRecord in fundRecords)
		{
			dataGrid.Rows.Add(
				fundRecord.Date,
				fundRecord.Value);
		}

		dataGrid.Display();
	}
}