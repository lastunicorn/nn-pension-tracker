using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFund;

public class ShowFundUseCase : IUseCase<ShowFundRequest, ShowFundResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public ShowFundUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<ShowFundResponse> Execute(ShowFundRequest request, CancellationToken cancellationToken)
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

		return new ShowFundResponse
		{
			FundNavs = fundRecords
		};
	}
}
