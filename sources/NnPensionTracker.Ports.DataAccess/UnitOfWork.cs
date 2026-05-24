namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class UnitOfWork: IUnitOfWork
{
    private readonly Database database;

    public ContributionRepository ContributionRepository => field ??= new ContributionRepository(database);

    public FundNavRepository FundNavRepository => field ??= new FundNavRepository(database);

    public UnitOfWork(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task SaveChangesAsync()
    {
        return database.SaveAllAsync();
    }
}