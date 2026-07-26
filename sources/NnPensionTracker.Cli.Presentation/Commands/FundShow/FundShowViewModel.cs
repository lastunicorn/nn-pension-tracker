using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands.FundShow;

internal class FundShowViewModel
{
	public List<FundNav> FundNavs { get; set; }

	public bool IsFromWeb { get; set; }
}
