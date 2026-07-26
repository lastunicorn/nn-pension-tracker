using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFundFromWeb;

public class ShowFundFromWebUseCase : IUseCase<ShowFundFromWebRequest, ShowFundFromWebResponse>
{
	private readonly INnApiClient nnApiClient;

	private static readonly DateOnly UnixEpoch = new(1970, 1, 1);

	public ShowFundFromWebUseCase(INnApiClient nnApiClient)
	{
		this.nnApiClient = nnApiClient ?? throw new ArgumentNullException(nameof(nnApiClient));
	}

	public Task<ShowFundFromWebResponse> Execute(ShowFundFromWebRequest request, CancellationToken cancellationToken)
	{
		ValidateDateInterval(request);

		return RetrieveFundNavsFromNnApi(request);
	}

	private static void ValidateDateInterval(ShowFundFromWebRequest request)
	{
		if (request.Year != null && (request.FromDate != null || request.ToDate != null))
			throw new Exception("Please specify either 'year' or the 'from'/'to' interval, not both.");

		if (request.FromDate != null && request.ToDate != null && request.FromDate > request.ToDate)
			throw new Exception("The 'from' date cannot be greater than the 'to' date.");
	}

	private async Task<ShowFundFromWebResponse> RetrieveFundNavsFromNnApi(ShowFundFromWebRequest request)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

		DateOnly fromDate;
		DateOnly toDate;

		if (request.Year != null)
		{
			fromDate = new DateOnly(request.Year.Value, 1, 1);
			toDate = new DateOnly(request.Year.Value, 12, 31);
		}
		else if (request.FromDate != null || request.ToDate != null)
		{
			fromDate = request.FromDate ?? UnixEpoch;
			toDate = request.ToDate ?? today;
		}
		else
		{
			fromDate = new DateOnly(today.Year, 1, 1);
			toDate = today;
		}

		int numberOfPoints = toDate.DayNumber - fromDate.DayNumber + 1;

		Console.WriteLine($"Reading fund NAV values from {fromDate} to {toDate} ({numberOfPoints} points)");

		GraphData graphData = await nnApiClient.GetGraph(fromDate, toDate, numberOfPoints);

		List<FundNav> fundNavs = graphData.Points
			.Select(x => new FundNav
			{
				Date = DateOnly.FromDateTime(x.Date),
				Value = x.Value
			})
			.OrderBy(x => x.Date)
			.ToList();

		return new ShowFundFromWebResponse
		{
			FundNavs = fundNavs
		};
	}
}
