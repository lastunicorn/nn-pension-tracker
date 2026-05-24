using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ExportAccount;

internal class ExportAccountUseCase : IUseCase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystemService fileSystemService;

    public string ExportFormat { get; init; }

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
                IEnumerable<Contribution> contributions = unitOfWork.ContributionRepository.GetAll();
                await ExportToCsv(contributions);
                break;

            default:
                Console.WriteLine($"Export format '{ExportFormat}' is not supported.");
                break;
        }
    }

    private async Task ExportToCsv(IEnumerable<Contribution> contributions)
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
        
        StreamWriter nnTransactionsStreamWriter = fileSystemService.OpenStreamWriter("NN_transactions.csv");
        StreamWriter nnCashTransactionsStreamWriter =  fileSystemService.OpenStreamWriter("NN_cash_transactions.csv");
        
        await using NnTransactionsDocument nnTransactionsDocument = new(nnTransactionsStreamWriter, labels);
        await using NnCashTransactionsDocument nnCashTransactionsDocument = new(nnCashTransactionsStreamWriter);

        foreach (Contribution contribution in contributions)
        {
            await nnTransactionsDocument.WriteAsync(contribution);
            await nnCashTransactionsDocument.WriteAsync(contribution);
        }

        Console.WriteLine();
        Console.WriteLine("Account contributions were exported to CSV files:");
        Console.WriteLine("  - NN_transactions.csv");
        Console.WriteLine("  - NN_cash_transactions.csv");
    }
}