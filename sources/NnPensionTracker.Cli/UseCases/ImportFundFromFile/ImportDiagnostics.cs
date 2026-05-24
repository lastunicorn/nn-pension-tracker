namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromFile;

internal class ImportDiagnostics
{
    public int AddCount { get; set; }

    public int UpdateCount { get; set; }

    public int SkipCount { get; set; }
    
    public Exception Error { get; set; }
}

