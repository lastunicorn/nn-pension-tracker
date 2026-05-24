using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class ContributionRepository : IContributionRepository
{
    private readonly Database database;

    public ContributionRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async IAsyncEnumerable<Contribution> GetAll()
    {
        foreach (Contribution contribution in database.Contributions)
        {
            yield return contribution;
            await Task.Yield();
        }
    }

    public async IAsyncEnumerable<Contribution> GetByYear(int year)
    {
        foreach (Contribution contribution in database.Contributions)
        {
            if (contribution.Month.Year != year)
                continue;

            yield return contribution;
            await Task.Yield();
        }
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