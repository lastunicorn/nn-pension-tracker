using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

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

	public async IAsyncEnumerable<Contribution> GetByMonthDateInterval(MonthDate? fromMonth, MonthDate? toMonth)
	{
		IEnumerable<Contribution> contributions = database.Contributions;

		if (fromMonth.HasValue)
		{
			MonthDate from = fromMonth.Value;
			contributions = contributions.Where(x =>
				x.Month.Year > from.Year ||
				(x.Month.Year == from.Year && x.Month.Month >= from.Month));
		}

		if (toMonth.HasValue)
		{
			MonthDate to = toMonth.Value;
			contributions = contributions.Where(x =>
				x.Month.Year < to.Year ||
				(x.Month.Year == to.Year && x.Month.Month <= to.Month));
		}

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