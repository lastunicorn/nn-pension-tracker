using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFund;

public class ShowFundResponse
{
	public List<FundNav> FundNavs { get; set; }
}
