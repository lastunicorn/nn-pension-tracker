namespace DustInTheWind.NnPensionTracker.Cli.Ports.DataAccess;

internal interface IUnitOfWork
{
    ContributionRepository ContributionRepository { get; }

    FundNavRepository FundNavRepository { get; }

    Task SaveChangesAsync();
}