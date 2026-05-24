namespace DustInTheWind.NnPensionTracker.Cli.Domain;

/// <summary>
/// Fund Net Asset Value
/// </summary>
public record FundNav
{
	public DateTime Date { get; set; }

	public decimal Value { get; set; }
}