using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class ContributionRepository
{
    private readonly Database database;

    public ContributionRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public IEnumerable<Contribution> GetAll()
    {
        return database.Contributions;
    }

    public IEnumerable<Contribution> GetByYear(int year)
    {
        return database.Contributions.Where(x => x.Month.Year == year);
    }

    public Contribution Get(MonthDate contributionMonth)
    {
        return database.Contributions.FirstOrDefault(x => x.Month == contributionMonth);
    }

    public void Add(Contribution contribution)
    {
        if (contribution == null) throw new ArgumentNullException(nameof(contribution));

        database.Contributions.Add(contribution);
    }

    public void Clear()
    {
        database.Contributions.Clear();
    }
}