namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IUnitOfWork
{
    ContributionRepository ContributionRepository { get; }

    FundNavRepository FundNavRepository { get; }

    Task SaveChangesAsync();
}