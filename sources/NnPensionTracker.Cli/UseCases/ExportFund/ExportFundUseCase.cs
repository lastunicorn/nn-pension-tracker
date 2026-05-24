using System.Globalization;
using CsvHelper;
using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ExportFund;

/// <summary>
/// Exports fund NAV values to a CSV file.
/// The CSV file structure:
///     - "Date": date
///     - "Quote": number
/// </summary>
internal class ExportFundUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IFileSystemService fileSystemService;
	private int count;

	public string FilePath { get; init; }

	public int? Year { get; init; }

	public ExportFundUseCase(IUnitOfWork unitOfWork, IFileSystemService fileSystemService)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
	}

	public async Task Execute()
	{
		if (FilePath == null)
			throw new ArgumentNullException(nameof(FilePath));

		IEnumerable<FundNav> fundNavs = RetrieveFundNavsFromStorage();
		await ExportFundNavsToCsv(fundNavs);

		Console.WriteLine();
		CustomConsole.WriteLineSuccess($"{count} fund NAV values were exported to: '{FilePath}'");
	}

	private IEnumerable<FundNav> RetrieveFundNavsFromStorage()
	{
		IEnumerable<FundNav> fundNavs = Year != null
			? unitOfWork.FundNavRepository.GetByYear(Year.Value)
			: unitOfWork.FundNavRepository.GetAll();

		foreach (FundNav fundNav in fundNavs)
		{
			count++;
			yield return fundNav;
		}
	}

	private async Task ExportFundNavsToCsv(IEnumerable<FundNav> fundNavs)
	{
		await using StreamWriter writer = fileSystemService.OpenStreamWriter(FilePath);
		await using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

		csv.Context.RegisterClassMap<FundNavMap>();
		csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
		csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd"];
		await csv.WriteRecordsAsync(fundNavs);
	}
}