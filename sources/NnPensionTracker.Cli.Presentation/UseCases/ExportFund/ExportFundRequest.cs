namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportFund;

public class ExportFundRequest
{
	public string FilePath { get; set; }

	public int? Year { get; set; }
}
