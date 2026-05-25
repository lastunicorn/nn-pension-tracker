namespace DustInTheWind.NnPensionTracker.Domain;

/// <summary>
/// Fund Net Asset Value
/// </summary>
public record FundNav
{
	public DateOnly Date { get; set; }

	public decimal Value { get; set; }
}