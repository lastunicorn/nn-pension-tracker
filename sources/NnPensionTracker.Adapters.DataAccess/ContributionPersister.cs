using System.Text.Json;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class ContributionPersister : IEntityPersister<Contribution>
{
	private readonly string dataDirectoryPath;
	private const string FileName = "contributions.json";

	public ContributionPersister(string dataDirectoryPath)
	{
		this.dataDirectoryPath = dataDirectoryPath ?? throw new ArgumentNullException(nameof(dataDirectoryPath));
	}

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true,
		Converters = { new MonthDateJsonConverter() }
	};

	public async Task<IEnumerable<Contribution>> LoadAsync()
	{
		string filePath = Path.Combine(dataDirectoryPath, FileName);

		if (!File.Exists(filePath))
			return [];

		try
		{
			string json = await File.ReadAllTextAsync(filePath);
			return JsonSerializer.Deserialize<List<Contribution>>(json, jsonSerializerOptions) ?? [];
		}
		catch (Exception ex)
		{
			throw new DataAccessException($"Failed to load contributions from file: {filePath}", ex);
		}
	}

	public Task SaveAsync(IEnumerable<Contribution> contributions)
	{
		if (!Directory.Exists(dataDirectoryPath))
			Directory.CreateDirectory(dataDirectoryPath);

		string filePath = Path.Combine(dataDirectoryPath, FileName);

		try
		{
			string json = JsonSerializer.Serialize(contributions, jsonSerializerOptions);
			return File.WriteAllTextAsync(filePath, json);
		}
		catch (Exception ex)
		{
			throw new DataAccessException($"Failed to save contributions to file: {filePath}", ex);
		}
	}
}