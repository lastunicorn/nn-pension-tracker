using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFund;

public class ShowFundUseCase : IUseCase<ShowFundRequest, ShowFundResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly INnApiClient nnApiClient;

	private static readonly DateOnly UnixEpoch = new(1970, 1, 1);

	public ShowFundUseCase(IUnitOfWork unitOfWork, INnApiClient nnApiClient)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.nnApiClient = nnApiClient ?? throw new ArgumentNullException(nameof(nnApiClient));
	}

	public async Task<ShowFundResponse> Execute(ShowFundRequest request, CancellationToken cancellationToken)
	{
		ValidateDateInterval(request);

		List<FundNav> fundNavs = request.Source == FundNavSource.Web
			? await RetrieveFromNnApi(request)
			: await RetrieveFromDatabase(request);

		return new ShowFundResponse
		{
			FundNavs = fundNavs
		};
	}

	private static void ValidateDateInterval(ShowFundRequest request)
	{
		if (request.Year != null && (request.FromDate != null || request.ToDate != null))
			throw new Exception("Please specify either 'year' or the 'from'/'to' interval, not both.");

		if (request.FromDate != null && request.ToDate != null && request.FromDate > request.ToDate)
			throw new Exception("The 'from' date cannot be greater than the 'to' date.");
	}

	private async Task<List<FundNav>> RetrieveFromDatabase(ShowFundRequest request)
	{
		IAsyncEnumerable<FundNav> source;

		if (request.FromDate.HasValue || request.ToDate.HasValue)
			source = unitOfWork.FundNavRepository.GetByDateInterval(request.FromDate, request.ToDate);
		else if (request.Year.HasValue)
			source = unitOfWork.FundNavRepository.GetByYear(request.Year.Value);
		else
			source = unitOfWork.FundNavRepository.GetAll();

		List<FundNav> fundNavs = [];

		await foreach (FundNav fundNav in source)
			fundNavs.Add(fundNav);

		return fundNavs
			.OrderBy(x => x.Date)
			.ToList();
	}

	private async Task<List<FundNav>> RetrieveFromNnApi(ShowFundRequest request)
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

		GraphData graphData = await nnApiClient.GetGraph(fromDate, toDate, numberOfPoints);

		return graphData.Points
			.Select(x => new FundNav
			{
				Date = DateOnly.FromDateTime(x.Date),
				Value = x.Value
			})
			.OrderBy(x => x.Date)
			.ToList();
	}
}