using CsvHelper.Configuration;
using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportFundFromFile;

internal class FundNavMap : ClassMap<FundNav>
{
	public FundNavMap()
	{
		Map(x => x.Date).Name("Date");
		Map(x => x.Value).Name("Quote");
	}
}