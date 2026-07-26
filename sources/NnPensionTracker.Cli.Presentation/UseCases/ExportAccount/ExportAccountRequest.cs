namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportAccount;

public class ExportAccountRequest
{
	public string ExportFormat { get; set; }

	public int? Year { get; set; }
}
