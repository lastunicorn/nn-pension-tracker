using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class Database
{
	private string connectionString = string.Empty;

	public List<Contribution> Contributions { get; } = [];

	public List<FundNav> FundNavs { get; } = [];

	public List<DataLabel> DataLabels { get; } = [];

	public async Task OpenAsync(string connectionString)
	{
		this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

		ContributionPersister contributionPersister = new(connectionString);
		Contributions.AddRange(await contributionPersister.LoadAsync());

		FundRecordPersister fundRecordPersister = new(connectionString);
		FundNavs.AddRange(await fundRecordPersister.LoadAsync());

		DataLabelParser dataLabelParser = new(connectionString);
		DataLabels.AddRange(await dataLabelParser.LoadAsync());
	}

	public async Task SaveAllAsync()
	{
		ContributionPersister contributionPersister = new(connectionString);
		await contributionPersister.SaveAsync(Contributions);

		FundRecordPersister fundRecordPersister = new(connectionString);
		await fundRecordPersister.SaveAsync(FundNavs);

		DataLabelParser dataLabelParser = new(connectionString);
		await dataLabelParser.SaveAsync(DataLabels);
	}
}