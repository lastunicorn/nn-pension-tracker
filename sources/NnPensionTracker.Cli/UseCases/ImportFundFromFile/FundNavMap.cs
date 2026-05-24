using CsvHelper.Configuration;
using DustInTheWind.NnPensionTracker.Cli.Domain;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromFile;

internal class FundNavMap : ClassMap<FundNav>
{
    public FundNavMap()
    {
        Map(x => x.Date).Name("Date");
        Map(x => x.Value).Name("Quote");
    }
}