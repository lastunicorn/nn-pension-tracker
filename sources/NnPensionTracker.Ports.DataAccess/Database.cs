using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class Database
{
	public List<Contribution> Contributions { get; } = [];

	public List<FundNav> FundRecords { get; } = [];

	public async Task OpenAsync()
	{
		ContributionPersister contributionPersister = new();
		Contributions.AddRange(await contributionPersister.LoadAsync());

		FundRecordPersister fundRecordPersister = new();
		FundRecords.AddRange(await fundRecordPersister.LoadAsync());
	}

	public async Task SaveAllAsync()
	{
		ContributionPersister contributionPersister = new();
		await contributionPersister.SaveAsync(Contributions);

		FundRecordPersister fundRecordPersister = new();
		await fundRecordPersister.SaveAsync(FundRecords);
	}
}