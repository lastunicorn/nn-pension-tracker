using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class Database
{
	public List<Contribution> Contributions { get; } = [];

	public List<FundNav> FundNavs { get; } = [];
	
	public List<DataLabel> DataLabels { get; } = [];

	public async Task OpenAsync()
	{
		ContributionPersister contributionPersister = new();
		Contributions.AddRange(await contributionPersister.LoadAsync());

		FundRecordPersister fundRecordPersister = new();
		FundNavs.AddRange(await fundRecordPersister.LoadAsync());
		
		DataLabelParser dataLabelParser = new();
		DataLabels.AddRange(await dataLabelParser.LoadAsync());
	}

	public async Task SaveAllAsync()
	{
		ContributionPersister contributionPersister = new();
		await contributionPersister.SaveAsync(Contributions);

		FundRecordPersister fundRecordPersister = new();
		await fundRecordPersister.SaveAsync(FundNavs);
		
		DataLabelParser dataLabelParser = new();
		await dataLabelParser.SaveAsync(DataLabels);
	}
}