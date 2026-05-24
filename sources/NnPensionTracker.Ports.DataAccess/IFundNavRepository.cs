using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IFundNavRepository
{
	FundNav Get(DateTime date);
	
	IAsyncEnumerable<FundNav> GetAll();
	
	IAsyncEnumerable<FundNav> GetByYear(int year);
	
	void Add(FundNav fundNav);
	
	void Clear();
}