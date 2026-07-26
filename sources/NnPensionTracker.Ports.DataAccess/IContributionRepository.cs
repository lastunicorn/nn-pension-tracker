using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IContributionRepository
{
	IAsyncEnumerable<Contribution> GetAll();

	IAsyncEnumerable<Contribution> GetByYear(int year);

	IAsyncEnumerable<Contribution> GetByMonthDateInterval(MonthDate? fromMonth, MonthDate? toMonth);

	Task<Contribution> GetAsync(MonthDate contributionMonth);

	void Add(Contribution contribution);

	void Clear();
}