namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFund;

public class ShowFundRequest
{
	public int? Year { get; set; }

	public DateOnly? FromDate { get; set; }

	public DateOnly? ToDate { get; set; }
}
