using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IContributionRepository
{
	IAsyncEnumerable<Contribution> GetAll();

	IAsyncEnumerable<Contribution> GetByYear(int year);
	
	Contribution Get(MonthDate contributionMonth);
	
	void Add(Contribution contribution);
	
	void Clear();
}