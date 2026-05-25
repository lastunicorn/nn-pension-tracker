using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NnPensionTracker.Domain;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;
using NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromFile;

/// <summary>
/// Imports fund values from a CSV file.
/// The CSV file structure:
///     - "Date": date
///     - "Quote": number
/// </summary>
public class ImportFundFromFileUseCase : IUseCase
{
    public string FilePath { get; set; }

    private readonly IUnitOfWork unitOfWork;
    private readonly IFileSystemService fileSystemService;

    public ImportFundFromFileUseCase(IUnitOfWork unitOfWork, IFileSystemService fileSystemService)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    public async Task Execute()
    {
        string filePathSafe = FilePath ?? Environment.CurrentDirectory;

        IEnumerable<string> filePaths = fileSystemService.IsDirectory(filePathSafe)
            ? fileSystemService.GetFiles(filePathSafe, "*.csv")
            : [filePathSafe];

        ImportDiagnostics totalDiagnostics = new();

        foreach (string path in filePaths)
        {
            IEnumerable<FundNav> fundNavs = ReadFromFile(path);
                  ImportDiagnostics importDiagnostics = await AddToStorage(fundNavs);
            DisplayImportDiagnostics(Path.GetFileName(path), importDiagnostics);

            totalDiagnostics.AddCount += importDiagnostics.AddCount;
            totalDiagnostics.UpdateCount += importDiagnostics.UpdateCount;
            totalDiagnostics.SkipCount += importDiagnostics.SkipCount;
        }

        DisplayImportDiagnostics("Total", totalDiagnostics);

        await unitOfWork.SaveChangesAsync();
    }

    private IEnumerable<FundNav> ReadFromFile(string path)
    {
        Console.WriteLine($"Importing {path} ...");

        try
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture);

            using StreamReader streamReader = fileSystemService.OpenStreamReader(path);
            using CsvReader csv = new(streamReader, config);
            csv.Context.RegisterClassMap<FundNavMap>();

            return csv.GetRecords<FundNav>().ToList();
        }
        catch (Exception ex)
        {
            CustomConsole.WriteLineError($"Error reading file '{path}': {ex}");
            return Enumerable.Empty<FundNav>();
        }
    }

      private async Task<ImportDiagnostics> AddToStorage(IEnumerable<FundNav> fundNavs)
    {
        ImportDiagnostics importDiagnostics = new();

        try
        {
            foreach (FundNav fundNav in fundNavs)
            {
                        FundNav existingFundNav = await unitOfWork.FundNavRepository.GetAsync(fundNav.Date);

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