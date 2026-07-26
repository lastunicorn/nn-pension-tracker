namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromWeb;

public class ImportFundFromWebRequest
{
	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }

	public int? Year { get; set; }

	public bool VerboseLogging { get; set; }
}
