using System.Globalization;
using CsvHelper;
using DustInTheWind.ConsoleTools;
using DustInTheWind.NnPensionTracker.Cli.Domain;
using DustInTheWind.NnPensionTracker.Cli.Ports.DataAccess;
using DustInTheWind.NnPensionTracker.Cli.Ports.FileSystemAccess;
using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromFile;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ExportFund;

internal class ExportFundUseCase : IUseCase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystemService fileSystemService;

    public string FilePath { get; init; }
    
    public ExportFundUseCase(IUnitOfWork unitOfWork, IFileSystemService fileSystemService)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    public async Task Execute()
    {
        if (FilePath == null)
            throw new ArgumentNullException(nameof(FilePath));

        IEnumerable<FundNav> fundNavs = unitOfWork.FundNavRepository.GetAll();

        await using StreamWriter writer = fileSystemService.OpenStreamWriter(FilePath);
        await using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<FundNavMap>();
        csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = ["yyyy-MM-dd"];
        await csv.WriteRecordsAsync(fundNavs);

        Console.WriteLine();
        CustomConsole.WriteLineSuccess($"Fund NAV values were exported to: {FilePath}");
    }
}