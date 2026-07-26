using System.Text.Json;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class FundRecordPersister : IEntityPersister<FundNav>
{
	private readonly string dataDirectoryPath;
	private const string FileName = "fund-navs.json";

	public FundRecordPersister(string dataDirectoryPath)
	{
		this.dataDirectoryPath = dataDirectoryPath ?? throw new ArgumentNullException(nameof(dataDirectoryPath));
	}

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true
	};

	public async Task<IEnumerable<FundNav>> LoadAsync()
	{
		string filePath = Path.Combine(dataDirectoryPath, FileName);

		if (!File.Exists(filePath))
			return [];

		try
		{
			string json = await File.ReadAllTextAsync(filePath);
			return JsonSerializer.Deserialize<List<FundNav>>(json, jsonSerializerOptions) ?? [];
		}
		catch (Exception ex)
		{
			throw new DataAccessException($"Failed to load fund records from file: {filePath}", ex);
		}
	}

	public Task SaveAsync(IEnumerable<FundNav> fundRecords)
	{
		if (!Directory.Exists(dataDirectoryPath))
			Directory.CreateDirectory(dataDirectoryPath);

		string filePath = Path.Combine(dataDirectoryPath, FileName);

		try
		{
			string json = JsonSerializer.Serialize(fundRecords, jsonSerializerOptions);
			return File.WriteAllTextAsync(filePath, json);
		}
		catch (Exception ex)
		{
			throw new DataAccessException($"Failed to save fund records to file: {filePath}", ex);
		}
	}
}