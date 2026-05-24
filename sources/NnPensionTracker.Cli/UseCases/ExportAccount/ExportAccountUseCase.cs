using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ExportAccount;

/// <summary>
/// Exports contributions to CSV files in the format required by Portfolio Performance application.
/// nn_transactions.csv -> "Security Name,Ticker Symbol,Date,Time,Value,Shares,Type,Fees,Note"
/// nn_cash_transactions.csv -> "Type,Cash Account,Date,Time,Value,Note"
/// </summary>
internal class ExportAccountUseCase : IUseCase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystemService fileSystemService;

    public string ExportFormat { get; set; }

    public int? Year { get; set; }

    public ExportAccountUseCase(IUnitOfWork unitOfWork , IFileSystemService fileSystemService)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    public async Task Execute()
    {
        string exportFormatSafe = ExportFormat ?? "pp";

        switch (exportFormatSafe.ToLower())
        {
            case "pp":
                IAsyncEnumerable<Contribution> contributions = Year != null
                    ? unitOfWork.ContributionRepository.GetByYear(Year.Value)
                    : unitOfWork.ContributionRepository.GetAll();

                await ExportToCsv(contributions);
                break;

            default:
                Console.WriteLine($"Export format '{ExportFormat}' is not supported.");
                break;
        }
    }

    private async Task ExportToCsv(IAsyncEnumerable<Contribution> contributions)
    {
        string[] labels =
        [
            "Luna",
            "Contribuție brută (lei)",
            "Comision de administrare (lei)",
            "Contribuție netă (lei)",
            "Valoare unitate de fond (lei)",
            "Număr unități de fond",
            "Plătită în luna"
        ];
        
        StreamWriter nnTransactionsStreamWriter = fileSystemService.OpenStreamWriter("nn_transactions.csv");
        StreamWriter nnCashTransactionsStreamWriter =  fileSystemService.OpenStreamWriter("nn_cash_transactions.csv");
        
        await using NnTransactionsDocument nnTransactionsDocument = new(nnTransactionsStreamWriter, labels);
        await using NnCashTransactionsDocument nnCashTransactionsDocument = new(nnCashTransactionsStreamWriter);

        await foreach (Contribution contribution in contributions)
        {
            await nnTransactionsDocument.WriteAsync(contribution);
            await nnCashTransactionsDocument.WriteAsync(contribution);
        }

        Console.WriteLine();
        Console.WriteLine("Account contributions were exported to CSV files:");
        Console.WriteLine("  - nn_transactions.csv");
        Console.WriteLine("  - nn_cash_transactions.csv");
    }
}