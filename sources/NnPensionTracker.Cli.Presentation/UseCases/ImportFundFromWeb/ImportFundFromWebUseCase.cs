using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromWeb;

/// <summary>
/// Imports fund values from the NN API.
/// </summary>
public class ImportFundFromWebUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;
	private readonly INnApiClient nnApiClient;

	private static readonly DateOnly UnixEpoch = new(1970, 1, 1);

	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }

	public int? Year { get; set; }

	public bool VerboseLogging { get; set; }

	public ImportFundFromWebUseCase(IUnitOfWork unitOfWork, INnApiClient nnApiClient)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.nnApiClient = nnApiClient ?? throw new ArgumentNullException(nameof(nnApiClient));
	}

	public async Task Execute()
	{
		ValidateDateInterval();

		GraphData graphData = await ReadFromNnApi();

		ImportDiagnostics importDiagnostics = await AddToStorage(graphData);
		DisplayImportDiagnostics(importDiagnostics);

		await unitOfWork.SaveChangesAsync();
	}

	private void ValidateDateInterval()
	{
		if (Year == null && FromDate == null && ToDate == null)
			throw new Exception("A date interval must be specified. Either a 'year' or both 'from' and 'to' dates.");

		if (Year != null && (FromDate != null || ToDate != null))
			throw new Exception("Please specify either 'year' or the 'from'/'to' interval, not both.");
	}

	private async Task<GraphData> ReadFromNnApi()
	{
		DateOnly fromDate = Year != null
			? new DateOnly(Year.Value, 1, 1)
			: FromDate ?? UnixEpoch;

		DateOnly toDate = Year != null
			? new DateOnly(Year.Value, 12, 31)
			: ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

		int numberOfPoints = toDate.DayNumber - fromDate.DayNumber + 1;

		Console.WriteLine($"Reading Romanian NN Mandatory Pension values.");
		Console.WriteLine($"From {fromDate} to {toDate} ({numberOfPoints} points)");

		return await nnApiClient.GetGraph(fromDate, toDate, numberOfPoints);
	}

	private async Task<ImportDiagnostics> AddToStorage(GraphData graphData)
	{
		ImportDiagnostics importDiagnostics = new();

		try
		{
			foreach (NnGraphPoint nnGraphPoint in graphData.Points)
			{
				FundNav existingFundNav = await unitOfWork.FundNavRepository.GetAsync(DateOnly.FromDateTime(nnGraphPoint.Date));

				if (existingFundNav == null)
				{
					if (VerboseLogging)
						CustomConsole.WriteLine($"[{nnGraphPoint.Date} - {nnGraphPoint.Value}] Adding fund NAV value.");

					unitOfWork.FundNavRepository.Add(new FundNav
					{
						Date = DateOnly.FromDateTime(nnGraphPoint.Date),
						Value = nnGraphPoint.Value
					});
					importDiagnostics.AddCount++;
				}
				else if (existingFundNav.Value == nnGraphPoint.Value)
				{
					if (VerboseLogging)
						CustomConsole.WriteLineWarning($"[{nnGraphPoint.Date} - {nnGraphPoint.Value}] Duplicate fund NAV value. Skipping.");

					importDiagnostics.SkipCount++;
				}
				else
				{
					if (VerboseLogging)
						CustomConsole.WriteLineWarning($"[{nnGraphPoint.Date} - {nnGraphPoint.Value}] Fund NAV value already exists: [{existingFundNav.Date} - {existingFundNav.Value}]");

					existingFundNav.Value = nnGraphPoint.Value;
					importDiagnostics.UpdateCount++;
				}
			}
		}
		catch (Exception ex)
		{
			importDiagnostics.Error = ex;
		}

		return importDiagnostics;
	}

	private void DisplayImportDiagnostics(ImportDiagnostics importDiagnostics)
	{
		DataGrid diagnosticsGrid = new()
		{
			Margin = new Thickness(0, 1, 0, 1)
		};

		diagnosticsGrid.Columns.Add("Name", HorizontalAlignment.Left);
		diagnosticsGrid.Columns.Add("Value", HorizontalAlignment.Right);

		diagnosticsGrid.Rows.Add("Add", importDiagnostics.AddCount);
		diagnosticsGrid.Rows.Add("Update", importDiagnostics.UpdateCount);
		diagnosticsGrid.Rows.Add("Skip", importDiagnostics.SkipCount);

		diagnosticsGrid.Display();

		if (importDiagnostics.Error != null)
			CustomConsole.WriteLineError($"Error importing fund values: {importDiagnostics.Error}");
	}
}