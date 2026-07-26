using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportAccount;

/// <summary>
/// Exports contributions to CSV files in the format required by Portfolio Performance application.
/// nn_transactions.csv -> "Security Name,Ticker Symbol,Date,Time,Value,Shares,Type,Fees,Note"
/// nn_cash_transactions.csv -> "Type,Cash Account,Date,Time,Value,Note"
/// </summary>
public class ExportAccountUseCase : IUseCase<ExportAccountRequest>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IFileSystemService fileSystemService;

	public ExportAccountUseCase(IUnitOfWork unitOfWork, IFileSystemService fileSystemService)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
	}

	public async Task Execute(ExportAccountRequest request, CancellationToken cancellationToken)
	{
		string exportFormatSafe = request.ExportFormat ?? "pp";

		switch (exportFormatSafe.ToLower())
		{
			case "pp":
				IAsyncEnumerable<Contribution> contributions = request.Year != null
					? unitOfWork.ContributionRepository.GetByYear(request.Year.Value)
					: unitOfWork.ContributionRepository.GetAll();

				await ExportToCsv(contributions);
				break;

			default:
				Console.WriteLine($"Export format '{request.ExportFormat}' is not supported.");
				break;
		}
	}

	private async Task ExportToCsv(IAsyncEnumerable<Contribution> contributions)
	{
		List<DataLabel> labels = await unitOfWork.DataLabelRepository.GetAll()
			.ToListAsync();

		StreamWriter nnTransactionsStreamWriter = fileSystemService.OpenStreamWriter("nn_transactions.csv");
		StreamWriter nnCashTransactionsStreamWriter = fileSystemService.OpenStreamWriter("nn_cash_transactions.csv");

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