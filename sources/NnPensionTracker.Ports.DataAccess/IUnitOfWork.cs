namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IUnitOfWork
{
    IContributionRepository ContributionRepository { get; }

    IFundNavRepository FundNavRepository { get; }
    
    IDataLabelRepository DataLabelRepository { get; }

    Task SaveChangesAsync();
}