namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IUnitOfWork
{
    IContributionRepository ContributionRepository { get; }

    IFundNavRepository FundNavRepository { get; }

    Task SaveChangesAsync();
}