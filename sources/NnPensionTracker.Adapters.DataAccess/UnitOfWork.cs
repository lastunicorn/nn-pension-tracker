using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class UnitOfWork : IUnitOfWork
{
	private readonly Database database;

	public IContributionRepository ContributionRepository => field ??= new ContributionRepository(database);

	public IFundNavRepository FundNavRepository => field ??= new FundNavRepository(database);
	public IDataLabelRepository DataLabelRepository => field ??= new DataLabelRepository(database);

	public UnitOfWork(Database database)
	{
		this.database = database ?? throw new ArgumentNullException(nameof(database));
	}

	public Task SaveChangesAsync()
	{
		return database.SaveAllAsync();
	}
}