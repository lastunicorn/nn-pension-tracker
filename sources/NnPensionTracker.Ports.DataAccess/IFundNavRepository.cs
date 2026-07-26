using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IFundNavRepository
{
	IAsyncEnumerable<FundNav> GetAll();

	IAsyncEnumerable<FundNav> GetByYear(int year);

	IAsyncEnumerable<FundNav> GetByDateInterval(DateOnly? fromDate, DateOnly? toDate);

	Task<FundNav> GetAsync(DateOnly date);

	void Add(FundNav fundNav);

	void Clear();
}