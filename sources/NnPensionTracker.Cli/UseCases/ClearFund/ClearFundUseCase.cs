using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Cli.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ClearFund;

internal class ClearFundUseCase : IUseCase
{
	private readonly UnitOfWork unitOfWork;

	public ClearFundUseCase(UnitOfWork unitOfWork)
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