namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromWeb;

internal class ImportDiagnostics
{
    public int AddCount { get; set; }

    public int UpdateCount { get; set; }

    public int SkipCount { get; set; }
    
    public Exception Error { get; set; }
}

