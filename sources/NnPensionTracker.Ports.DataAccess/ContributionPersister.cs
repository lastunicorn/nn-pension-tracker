using System.Text.Json;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class ContributionPersister : IEntityPersister<Contribution>
{
	private const string DatabasePath = "Data";
	private const string FileName = "contributions.json";

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true,
		Converters = { new MonthDateJsonConverter() }
	};

	public async Task<IEnumerable<Contribution>> LoadAsync()
	{
		string filePath = Path.Combine(DatabasePath, FileName);

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
		if (!Directory.Exists(DatabasePath))
			Directory.CreateDirectory(DatabasePath);

		string filePath = Path.Combine(DatabasePath, FileName);

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