using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowAccount;

public class ShowAccountUseCase : IUseCase<ShowAccountRequest, ShowAccountResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public ShowAccountUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<ShowAccountResponse> Execute(ShowAccountRequest request, CancellationToken cancellationToken)
	{
		IAsyncEnumerable<Contribution> source;

		if (request.FromMonth.HasValue || request.ToMonth.HasValue)
			source = unitOfWork.ContributionRepository.GetByMonthDateInterval(request.FromMonth, request.ToMonth);
		else if (request.Year.HasValue)
			source = unitOfWork.ContributionRepository.GetByYear(request.Year.Value);
		else
			source = unitOfWork.ContributionRepository.GetAll();

		List<Contribution> contributions = [];

		await foreach (Contribution contribution in source)
			contributions.Add(contribution);

		return new ShowAccountResponse
		{
			Contributions = contributions
		};
	}
}