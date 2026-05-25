using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IFundNavRepository
{
	IAsyncEnumerable<FundNav> GetAll();
	
	IAsyncEnumerable<FundNav> GetByYear(int year);
	
	Task<FundNav> GetAsync(DateOnly date);
	
	void Add(FundNav fundNav);
	
	void Clear();
}