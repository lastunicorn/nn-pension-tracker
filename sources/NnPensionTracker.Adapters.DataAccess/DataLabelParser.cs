using System.Text.Json;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class DataLabelParser
{
	private readonly string dataDirectoryPath;
	private const string FileName = "data-labels.json";

	public DataLabelParser(string dataDirectoryPath)
	{
		this.dataDirectoryPath = dataDirectoryPath ?? throw new ArgumentNullException(nameof(dataDirectoryPath));
	}

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true,
		Converters = { new MonthDateJsonConverter() }
	};

	public async Task<IEnumerable<DataLabel>> LoadAsync()
	{
		string filePath = Path.Combine(dataDirectoryPath, FileName);

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
			throw new DataAccessException($"Failed to save data labels to file: {filePath}", ex);
		}
	}
}