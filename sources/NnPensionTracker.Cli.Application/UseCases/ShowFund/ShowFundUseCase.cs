using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFund;

public class ShowFundUseCase : IUseCase<ShowFundRequest>
{
	private readonly IUnitOfWork unitOfWork;

	public ShowFundUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute(ShowFundRequest request, CancellationToken cancellationToken)
	{
		List<FundNav> fundRecords = [];

		IAsyncEnumerable<FundNav> source;

		if (request.FromDate.HasValue || request.ToDate.HasValue)
			source = unitOfWork.FundNavRepository.GetByDateInterval(request.FromDate, request.ToDate);
		else if (request.Year.HasValue)
			source = unitOfWork.FundNavRepository.GetByYear(request.Year.Value);
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