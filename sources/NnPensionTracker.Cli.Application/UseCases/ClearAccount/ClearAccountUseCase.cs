using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ClearAccount;

public class ClearAccountUseCase : IUseCase<ClearAccountRequest>
{
	private readonly IUnitOfWork unitOfWork;

	public ClearAccountUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute(ClearAccountRequest request, CancellationToken cancellationToken)
	{
		unitOfWork.ContributionRepository.Clear();
		await unitOfWork.SaveChangesAsync();
	}
}