using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension.Pdf;
using DustInTheWind.NnPensionTracker.Cli.Application.ConsoleUtils;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportAccount;

/// <summary>
/// Imports contributions from a PDF file.
/// The PDF file must be a Mandatory Private Pension contributions document downloaded from NN Direct mobile app.
/// </summary>
public class ImportAccountUseCase : IUseCase<ImportAccountRequest>
{
	private readonly IUnitOfWork unitOfWork;

	public ImportAccountUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute(ImportAccountRequest request, CancellationToken cancellationToken)
	{
		if (request.FilePath == null)
			throw new ArgumentNullException(nameof(request.FilePath));

		DocumentLoadResult documentLoadResult = ParseDocument(request.FilePath);
		DisplayParsingDiagnostics(documentLoadResult.Diagnostics);

		AddColumnNamesToStorage(documentLoadResult.Document.Header);
		
		ImportDiagnostics importDiagnostics = await AddContributionsToStorage(documentLoadResult.Document);
		DisplayImportDiagnostics(importDiagnostics);

		await unitOfWork.SaveChangesAsync();
	}

	private void AddColumnNamesToStorage(ContributionsHeader documentHeader)
	{
		string[] propertyNames =
		[
			nameof(Contribution.Month),
			nameof(Contribution.GrossValue),
			nameof(Contribution.AdministrationFee),
			nameof(Contribution.NetValue),
			nameof(Contribution.UnitValue),
			nameof(Contribution.UnitCount),
			nameof(Contribution.PaidInMonth)
		];

		for (int i = 0; i < documentHeader.Count && i < propertyNames.Length; i++)
		{
			unitOfWork.DataLabelRepository.AddOrUpdate(new DataLabel
			{
				Key = $"{nameof(Contribution)}.{propertyNames[i]}",
				Value = documentHeader[i]
			});
		}
	}

	private DocumentLoadResult ParseDocument(string filePath)
	{
		Console.WriteLine($"Parsing document '{filePath}'");
		return ContributionsDocument.LoadFromFile(filePath);
	}

	private static void DisplayParsingDiagnostics(DocumentParsingDiagnostics diagnostics)
	{
		DataGrid diagnosticsGrid = new()
		{
			Margin = new Thickness(0, 1, 0, 1)
		};

		diagnosticsGrid.Columns.Add($"Pages ({diagnostics.Pages.Count})");
		diagnosticsGrid.Columns.Add("Extraction Algorithm");
		diagnosticsGrid.Columns.Add("Table Count", HorizontalAlignment.Right);
		diagnosticsGrid.Columns.Add("Row Count", HorizontalAlignment.Right);

		foreach (PageParsingDiagnostics page in diagnostics.Pages)
		{
			string pageNumber = $"Page {page.PageIndex}";
			TableExtractionApproachPretty tableExtractionApproach = page.TableExtractionApproach;
			int tableCount = page.Tables.Count;
			int rowCount = page.Tables
				.Select(x => x.RowCount)
				.Sum();

			diagnosticsGrid.Rows.Add(pageNumber, tableExtractionApproach, tableCount, rowCount);
		}

		int totalRowCount = diagnostics.Pages
			.SelectMany(x => x.Tables)
			.Select(x => x.RowCount)
			.Sum();
		diagnosticsGrid.Footer = "Row Count: " + totalRowCount;

		diagnosticsGrid.Display();
	}

	private async Task<ImportDiagnostics> AddContributionsToStorage(ContributionsDocument contributionsDocument)
	{
		Console.WriteLine($"Importing {contributionsDocument.Count} contributions into database.");

		ImportDiagnostics importDiagnostics = new();

		foreach (Contribution contribution in contributionsDocument)
		{
			Contribution existingContribution = await unitOfWork.ContributionRepository.GetAsync(contribution.Month);

			if (existingContribution == null)
			{
				unitOfWork.ContributionRepository.Add(contribution);
				importDiagnostics.AddCount++;
			}
			else
			{
				if (existingContribution.Equals(contribution))
				{
					importDiagnostics.SkipCount++;
				}
				else
				{
					existingContribution.GrossValue = contribution.GrossValue;
					existingContribution.AdministrationFee = contribution.AdministrationFee;
					existingContribution.NetValue = contribution.NetValue;
					existingContribution.UnitValue = contribution.UnitValue;
					existingContribution.UnitCount = contribution.UnitCount;
					existingContribution.PaidInMonth = contribution.PaidInMonth;

					importDiagnostics.UpdateCount++;
				}
			}
		}

		Console.WriteLine();
		Console.WriteLine("Data imported successfully.");

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
	}
}