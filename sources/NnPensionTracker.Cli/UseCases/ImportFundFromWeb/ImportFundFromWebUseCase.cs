using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.ApiAccess;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;

internal class ImportFundFromWebUseCase : IUseCase
{
	private readonly IUnitOfWork unitOfWork;
	private readonly INnApiClient nnApiClient;

	private static readonly DateOnly UnixEpoch = new(1970, 1, 1);

	public DateOnly? FromDate { get; init; }

	public DateOnly? ToDate { get; init; }

	public int? Year { get; init; }

	public ImportFundFromWebUseCase(IUnitOfWork unitOfWork, INnApiClient nnApiClient)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.nnApiClient = nnApiClient ?? throw new ArgumentNullException(nameof(nnApiClient));
	}

	public async Task Execute()
	{
		if (Year == null)
			throw new ArgumentNullException(nameof(Year));

		IEnumerable<FundNav> fundNavs = await ReadFromNnApi();

		ImportDiagnostics importDiagnostics = Import(fundNavs);
		DisplayImportDiagnostics($"Fund NAV values for {Year}", importDiagnostics);

		await unitOfWork.SaveChangesAsync();
	}

	private async Task<IEnumerable<FundNav>> ReadFromNnApi()
	{
		DateOnly fromDate = Year != null
			? new DateOnly(Year.Value, 1, 1)
			: FromDate ?? UnixEpoch;

		DateOnly toDate = Year != null
			? new DateOnly(Year.Value, 12, 31)
			: ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

		int numberOfPoints = toDate.DayNumber - fromDate.DayNumber + 1;

		GraphData graphData = await nnApiClient.GetGraph(fromDate, toDate, numberOfPoints);

		return graphData.Points
			.Select(x => new FundNav
			{
				Date = x.Date,
				Value = x.Value
			})
			.ToList();
	}

	private ImportDiagnostics Import(IEnumerable<FundNav> fundNavs)
	{
		ImportDiagnostics importDiagnostics = new();

		try
		{
			foreach (FundNav fundNav in fundNavs)
			{
				FundNav existingFundNav = unitOfWork.FundNavRepository.Get(fundNav.Date);

				if (existingFundNav == null)
				{
					unitOfWork.FundNavRepository.Add(fundNav);
					importDiagnostics.AddCount++;
				}
				else if (existingFundNav.Value == fundNav.Value)
				{
					importDiagnostics.SkipCount++;
				}
				else
				{
					existingFundNav.Value = fundNav.Value;
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

	private void DisplayImportDiagnostics(string title, ImportDiagnostics importDiagnostics)
	{
		DataGrid diagnosticsGrid = new()
		{
			Title = title,
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