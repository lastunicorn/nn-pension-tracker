using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ClearFund;

public class ClearFundUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;

	public ClearFundUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute()
	{
		unitOfWork.FundNavRepository.Clear();
		await unitOfWork.SaveChangesAsync();

		CustomConsole.WriteLineSuccess("All fund records have been cleared.");
	}
}