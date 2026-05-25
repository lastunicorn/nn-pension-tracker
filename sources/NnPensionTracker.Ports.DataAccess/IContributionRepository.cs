using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IContributionRepository
{
	IAsyncEnumerable<Contribution> GetAll();

	IAsyncEnumerable<Contribution> GetByYear(int year);
	
	IAsyncEnumerable<Contribution> GetByYearMonth(int year, int? month);
	
	Task<Contribution> GetAsync(MonthDate contributionMonth);
	
	void Add(Contribution contribution);
	
	void Clear();
}