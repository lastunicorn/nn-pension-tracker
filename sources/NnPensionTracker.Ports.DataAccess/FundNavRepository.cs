using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class FundNavRepository : IFundNavRepository
{
	private readonly Database database;

	public FundNavRepository(Database database)
	{
		this.database = database ?? throw new ArgumentNullException(nameof(database));
	}

	public Task<FundNav> GetAsync(DateOnly date)
	{
		FundNav fundNav = database.FundNavs.FirstOrDefault(x => x.Date == date);
		return Task.FromResult(fundNav);
	}

	public async IAsyncEnumerable<FundNav> GetAll()
	{
		foreach (FundNav fundNav in database.FundNavs)
		{
			yield return fundNav;
			await Task.Yield();
		}
	}

	public async IAsyncEnumerable<FundNav> GetByYear(int year)
	{
		IEnumerable<FundNav> fundNavs = database.FundNavs
			.Where(fundNav => fundNav.Date.Year == year);

		foreach (FundNav fundNav in fundNavs)
		{
			yield return fundNav;
			await Task.Yield();
		}
	}

	public async IAsyncEnumerable<FundNav> GetByDateInterval(DateOnly? fromDate, DateOnly? toDate)
	{
		IEnumerable<FundNav> fundNavs = database.FundNavs;

		if (fromDate.HasValue)
			fundNavs = fundNavs.Where(x => x.Date >= fromDate.Value);

		if (toDate.HasValue)
			fundNavs = fundNavs.Where(x => x.Date <= toDate.Value);

		foreach (FundNav fundNav in fundNavs)
		{
			yield return fundNav;
			await Task.Yield();
		}
	}

	public void Add(FundNav fundNav)
	{
		if (fundNav == null) throw new ArgumentNullException(nameof(fundNav));

		database.FundNavs.Add(fundNav);
	}

	public void Clear()
	{
		database.FundNavs.Clear();
	}
}