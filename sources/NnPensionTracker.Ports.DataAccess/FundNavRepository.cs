using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class FundNavRepository
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

    public IEnumerable<FundNav> GetAll()
    {
        return database.FundRecords;
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


