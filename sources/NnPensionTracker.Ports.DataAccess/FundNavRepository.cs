using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class FundNavRepository : IFundNavRepository
{
    private readonly Database database;

    public FundNavRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public FundNav Get(DateTime date)
    {
        return database.FundRecords.FirstOrDefault(x => x.Date == date);
    }

    public async IAsyncEnumerable<FundNav> GetAll()
    {
        foreach (FundNav fundNav in database.FundRecords)
        {
            yield return fundNav;
            await Task.Yield();
        }
    }

    public async IAsyncEnumerable<FundNav> GetByYear(int year)
    {
        foreach (FundNav fundNav in database.FundRecords)
        {
            if (fundNav.Date.Year != year)
                continue;

            yield return fundNav;
            await Task.Yield();
        }
    }

    public void Add(FundNav fundNav)
    {
        if (fundNav == null) throw new ArgumentNullException(nameof(fundNav));

        database.FundRecords.Add(fundNav);
    }

    public void Clear()
    {
        database.FundRecords.Clear();
    }
}
