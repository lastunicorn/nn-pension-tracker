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
		IEnumerable<Contribution> contributions = database.Contributions
			.Where(x => x.Month.Year == year);

		foreach (Contribution contribution in contributions)
		{
			yield return contribution;
			await Task.Yield();
		}
	}

	public async IAsyncEnumerable<Contribution> GetByYearMonth(int year, int? month)
	{
		IEnumerable<Contribution> contributions = database.Contributions
			.Where(x => x.Month.Year == year);

		if (month.HasValue)
			contributions = contributions.Where(x => x.Month.Month == month.Value);

		foreach (Contribution contribution in contributions)
		{
			yield return contribution;
			await Task.Yield();
		}
	}

	public Task<Contribution> GetAsync(MonthDate contributionMonth)
	{
		Contribution contribution = database.Contributions.FirstOrDefault(x => x.Month == contributionMonth);
		return Task.FromResult(contribution);
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