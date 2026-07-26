namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ExportFund;

public class ExportFundRequest
{
	public string FilePath { get; set; }

	public int? Year { get; set; }
}