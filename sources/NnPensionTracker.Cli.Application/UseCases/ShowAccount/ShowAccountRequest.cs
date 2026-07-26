using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowAccount;

public class ShowAccountRequest
{
	public int? Year { get; set; }

	public MonthDate? FromMonth { get; set; }

	public MonthDate? ToMonth { get; set; }
}
