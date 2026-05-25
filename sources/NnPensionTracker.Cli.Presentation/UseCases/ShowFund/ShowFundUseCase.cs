using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowFund;

public class ShowFundUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;

	public int? Year { get; set; }

	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }

	public ShowFundUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute()
	{
		List<FundNav> fundRecords = [];

		IAsyncEnumerable<FundNav> source;

		if (FromDate.HasValue || ToDate.HasValue)
			source = unitOfWork.FundNavRepository.GetByDateInterval(FromDate, ToDate);
		else if (Year.HasValue)
			source = unitOfWork.FundNavRepository.GetByYear(Year.Value);
		else
			source = unitOfWork.FundNavRepository.GetAll();

		await foreach (FundNav fundRecord in source)
			fundRecords.Add(fundRecord);

		fundRecords = fundRecords
			.OrderBy(x => x.Date)
			.ToList();
		
		DisplayFundRecords(fundRecords);
	}

	private void DisplayFundRecords(IEnumerable<FundNav> fundRecords)
	{
		DataGrid dataGrid = new()
		{
			EmptyGridMessage = "No data"
		};

		dataGrid.Columns.Add("Date", HorizontalAlignment.Center);
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);
		dataGrid.EmptyGridMessage = "No fund records found.";

		foreach (FundNav fundRecord in fundRecords)
		{
			dataGrid.Rows.Add(
				fundRecord.Date,
				fundRecord.Value);
		}

		dataGrid.Display();
	}
}