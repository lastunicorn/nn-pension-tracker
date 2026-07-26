namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowFund;

public class ShowFundRequest
{
	public int? Year { get; set; }

	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }
}
