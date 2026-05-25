using System.Text.Json;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class FundRecordPersister : IEntityPersister<FundNav>
{
	private const string DatabasePath = "Data";
	private const string FileName = "fund-navs.json";

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true
	};

	public async Task<IEnumerable<FundNav>> LoadAsync()
	{
		string filePath = Path.Combine(DatabasePath, FileName);

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
		if (!Directory.Exists(DatabasePath))
			Directory.CreateDirectory(DatabasePath);

		string filePath = Path.Combine(DatabasePath, FileName);

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