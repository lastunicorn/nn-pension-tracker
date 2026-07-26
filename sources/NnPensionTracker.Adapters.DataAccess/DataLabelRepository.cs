using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class DataLabelRepository : IDataLabelRepository
{
	private readonly Database database;

	public DataLabelRepository(Database database)
	{
		this.database = database ?? throw new ArgumentNullException(nameof(database));
	}

	public async IAsyncEnumerable<DataLabel> GetAll()
	{
		foreach (DataLabel dataLabel in database.DataLabels)
		{
			yield return dataLabel;
			await Task.Yield();
		}
	}

	public void AddOrUpdate(DataLabel dataLabel)
	{
		if (dataLabel == null) throw new ArgumentNullException(nameof(dataLabel));

		DataLabel existing = database.DataLabels.FirstOrDefault(x => x.Key == dataLabel.Key);

		if (existing == null)
			database.DataLabels.Add(dataLabel);
		else
			existing.Value = dataLabel.Value;
	}
}