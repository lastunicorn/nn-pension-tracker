using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class Database
{
    private string connectionString;

    public List<Contribution> Contributions { get => IsOpen ? field : throw new DatabaseNotOpenException(); } = [];

    public List<FundNav> FundNavs { get => IsOpen ? field : throw new DatabaseNotOpenException(); } = [];

    public List<DataLabel> DataLabels { get => IsOpen ? field : throw new DatabaseNotOpenException(); } = [];

    private bool IsOpen => connectionString is not null;

    public async Task OpenAsync(string connectionString)
    {
        this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        Contributions.Clear();
        FundNavs.Clear();
        DataLabels.Clear();

        ContributionPersister contributionPersister = new(connectionString);
        Contributions.AddRange(await contributionPersister.LoadAsync());

        FundRecordPersister fundRecordPersister = new(connectionString);
        FundNavs.AddRange(await fundRecordPersister.LoadAsync());

        DataLabelParser dataLabelParser = new(connectionString);
        DataLabels.AddRange(await dataLabelParser.LoadAsync());
    }

    public async Task SaveAllAsync()
    {
        if (!IsOpen)
            throw new DatabaseNotOpenException();

        ContributionPersister contributionPersister = new(connectionString!);
        await contributionPersister.SaveAsync(Contributions);

        FundRecordPersister fundRecordPersister = new(connectionString!);
        await fundRecordPersister.SaveAsync(FundNavs);

        DataLabelParser dataLabelParser = new(connectionString!);
        await dataLabelParser.SaveAsync(DataLabels);
    }
}