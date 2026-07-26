namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowFundFromWeb;

public class ShowFundFromWebRequest
{
	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }

	public int? Year { get; set; }
}
