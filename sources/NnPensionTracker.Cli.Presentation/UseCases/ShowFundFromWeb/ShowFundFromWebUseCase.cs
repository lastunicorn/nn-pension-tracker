using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ShowFundFromWeb;

public class ShowFundFromWebUseCase : IUseCase
{
	private readonly INnApiClient nnApiClient;

	private static readonly DateOnly UnixEpoch = new(1970, 1, 1);

	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }

	public int? Year { get; set; }

	public ShowFundFromWebUseCase(INnApiClient nnApiClient)
	{
		this.nnApiClient = nnApiClient ?? throw new ArgumentNullException(nameof(nnApiClient));
	}

	public Task Execute()
	{
		ValidateDateInterval();

		return DisplayFundNavsFromNnApi();
	}

	private void ValidateDateInterval()
	{
		if (Year != null && (FromDate != null || ToDate != null))
			throw new Exception("Please specify either 'year' or the 'from'/'to' interval, not both.");

		if (FromDate != null && ToDate != null && FromDate > ToDate)
			throw new Exception("The 'from' date cannot be greater than the 'to' date.");
	}

	private async Task DisplayFundNavsFromNnApi()
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

		DateOnly fromDate;
		DateOnly toDate;

		if (Year != null)
		{
			fromDate = new DateOnly(Year.Value, 1, 1);
			toDate = new DateOnly(Year.Value, 12, 31);
		}
		else if (FromDate != null || ToDate != null)
		{
			fromDate = FromDate ?? UnixEpoch;
			toDate = ToDate ?? today;
		}
		else
		{
			fromDate = new DateOnly(today.Year, 1, 1);
			toDate = today;
		}

		int numberOfPoints = toDate.DayNumber - fromDate.DayNumber + 1;

		Console.WriteLine($"Reading fund NAV values from {fromDate} to {toDate} ({numberOfPoints} points)");

		GraphData graphData = await nnApiClient.GetGraph(fromDate, toDate, numberOfPoints);

		IEnumerable<FundNav> fundNavs = graphData.Points
			.Select(x => new FundNav
			{
				Date = DateOnly.FromDateTime(x.Date),
				Value = x.Value
			})
			.OrderBy(x => x.Date)
			.ToList();

		DisplayFundRecords(fundNavs);
	}

	private static void DisplayFundRecords(IEnumerable<FundNav> fundNavs)
	{
		DataGrid dataGrid = new();

		dataGrid.Columns.Add("Date", HorizontalAlignment.Center);
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

		int count = 0;
		foreach (FundNav fundNav in fundNavs)
		{
			count++;
			
			dataGrid.Rows.Add(
				fundNav.Date,
				fundNav.Value);
		}
		
		dataGrid.Footer = $"Total: {count} records";

		dataGrid.Display();
	}
}