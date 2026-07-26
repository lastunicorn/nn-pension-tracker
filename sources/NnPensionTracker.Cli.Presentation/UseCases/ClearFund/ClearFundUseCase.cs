using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ClearFund;

public class ClearFundUseCase : IUseCase<ClearFundRequest>
{
	private readonly IUnitOfWork unitOfWork;

	public ClearFundUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute(ClearFundRequest request, CancellationToken cancellationToken)
	{
		unitOfWork.FundNavRepository.Clear();
		await unitOfWork.SaveChangesAsync();

		CustomConsole.WriteLineSuccess("All fund records have been cleared.");
	}
}