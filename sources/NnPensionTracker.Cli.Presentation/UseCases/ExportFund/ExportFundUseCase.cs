using System.Globalization;
using CsvHelper;
using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportFund;

/// <summary>
/// Exports fund NAV values to a CSV file.
/// The CSV file structure:
///     - "Date": date
///     - "Quote": number
/// </summary>
public class ExportFundUseCase : IUseCase<ExportFundRequest>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IFileSystemService fileSystemService;
	private int count;

	public ExportFundUseCase(IUnitOfWork unitOfWork, IFileSystemService fileSystemService)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
	}

	public async Task Execute(ExportFundRequest request, CancellationToken cancellationToken)
	{
		if (request.FilePath == null)
			throw new ArgumentNullException(nameof(request.FilePath));

		IAsyncEnumerable<FundNav> fundNavs = RetrieveFundNavsFromStorage(request.Year);
		await ExportFundNavsToCsv(fundNavs, request.FilePath);

		Console.WriteLine();
		CustomConsole.WriteLineSuccess($"{count} fund NAV values were exported to: '{request.FilePath}'");
	}

	private IAsyncEnumerable<FundNav> RetrieveFundNavsFromStorage(int? year)
	{
		return year != null
			? unitOfWork.FundNavRepository.GetByYear(year.Value)
			: unitOfWork.FundNavRepository.GetAll();
	}

	private async Task ExportFundNavsToCsv(IAsyncEnumerable<FundNav> fundNavs, string filePath)
	{
		await using StreamWriter writer = fileSystemService.OpenStreamWriter(filePath);
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