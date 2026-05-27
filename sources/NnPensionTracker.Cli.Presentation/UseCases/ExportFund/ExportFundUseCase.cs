using System.Globalization;
using CsvHelper;
using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportFund;

/// <summary>
/// Exports fund NAV values to a CSV file.
/// The CSV file structure:
///     - "Date": date
///     - "Quote": number
/// </summary>
public class ExportFundUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IFileSystemService fileSystemService;
	private int count;

	public string FilePath { get; set; }

	public int? Year { get; set; }

	public ExportFundUseCase(IUnitOfWork unitOfWork, IFileSystemService fileSystemService)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
	}

	public async Task Execute()
	{
		if (FilePath == null)
			throw new ArgumentNullException(nameof(FilePath));

		IAsyncEnumerable<FundNav> fundNavs = RetrieveFundNavsFromStorage();
		await ExportFundNavsToCsv(fundNavs);

		Console.WriteLine();
		CustomConsole.WriteLineSuccess($"{count} fund NAV values were exported to: '{FilePath}'");
	}

	private IAsyncEnumerable<FundNav> RetrieveFundNavsFromStorage()
	{
		return Year != null
			? unitOfWork.FundNavRepository.GetByYear(Year.Value)
			: unitOfWork.FundNavRepository.GetAll();
	}

	private async Task ExportFundNavsToCsv(IAsyncEnumerable<FundNav> fundNavs)
	{
		await using StreamWriter writer = fileSystemService.OpenStreamWriter(FilePath);
		await using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

		csv.Context.RegisterClassMap<FundNavMap>();
		csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
		csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd"];

		csv.WriteHeader<FundNav>();
		await csv.NextRecordAsync();

		await foreach (FundNav fundNav in fundNavs)
		{
			count++;
			csv.WriteRecord(fundNav);
			await csv.NextRecordAsync();
		}
	}
}