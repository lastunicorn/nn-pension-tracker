using DustInTheWind.NnPensionTracker.Cli.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ClearAccount;

internal class ClearAccountUseCase : IUseCase
{
    private readonly IUnitOfWork unitOfWork;

    public ClearAccountUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Execute()
    {
        unitOfWork.ContributionRepository.Clear();
        await unitOfWork.SaveChangesAsync();

        Console.WriteLine("All contributions have been cleared from the database.");
    }
}