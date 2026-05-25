using System.Text.Json;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class DataLabelParser
{
	private const string DatabasePath = "Data";
	private const string FileName = "data-labels.json";

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true,
		Converters = { new MonthDateJsonConverter() }
	};

	public async Task<IEnumerable<DataLabel>> LoadAsync()
	{
		string filePath = Path.Combine(DatabasePath, FileName);

		if (!File.Exists(filePath))
			return [];

		try
		{
			string json = await File.ReadAllTextAsync(filePath);
			return JsonSerializer.Deserialize<List<DataLabel>>(json, jsonSerializerOptions) ?? [];
		}
		catch (Exception ex)
		{
			throw new DataAccessException($"Failed to load data labels from file: {filePath}", ex);
		}
	}

	public Task SaveAsync(IEnumerable<DataLabel> contributions)
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
			throw new DataAccessException($"Failed to save data labels to file: {filePath}", ex);
		}
	}
}