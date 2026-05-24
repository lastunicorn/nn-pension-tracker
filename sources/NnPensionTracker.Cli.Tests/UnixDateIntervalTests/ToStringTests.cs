using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class ToStringTests
{
	[Fact]
	public void WhenBothDatesSet_FormatsCorrectly()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.ToString().Should().Be("2024-01-01 — 2024-12-31");
	}

	[Fact]
	public void WhenStartDateIsNull_ShowsEllipsis()
	{
		UnixDateInterval interval = new(null, new DateOnly(2024, 12, 31));
		interval.ToString().Should().StartWith("...");
	}

	[Fact]
	public void WhenEndDateIsNull_ShowsEllipsis()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), null);
		interval.ToString().Should().EndWith("...");
	}

	[Fact]
	public void WhenBothDatesAreNull_ShowsBothEllipses()
	{
		UnixDateInterval interval = new(null, null);
		interval.ToString().Should().Be("... — ...");
	}
}