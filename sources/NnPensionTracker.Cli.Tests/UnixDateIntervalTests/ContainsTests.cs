using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class ContainsTests
{
	[Fact]
	public void WhenDateIsWithinBounds_ReturnsTrue()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.Contains(new DateOnly(2024, 6, 15)).Should().BeTrue();
	}

	[Fact]
	public void WhenDateEqualsStartDate_ReturnsTrue()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.Contains(new DateOnly(2024, 1, 1)).Should().BeTrue();
	}

	[Fact]
	public void WhenDateEqualsEndDate_ReturnsTrue()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.Contains(new DateOnly(2024, 12, 31)).Should().BeTrue();
	}

	[Fact]
	public void WhenDateIsBeforeStartDate_ReturnsFalse()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.Contains(new DateOnly(2023, 12, 31)).Should().BeFalse();
	}

	[Fact]
	public void WhenDateIsAfterEndDate_ReturnsFalse()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.Contains(new DateOnly(2025, 1, 1)).Should().BeFalse();
	}

	[Fact]
	public void WhenStartDateIsNull_AnyDateBeforeEndDateReturnsTrue()
	{
		UnixDateInterval interval = new(null, new DateOnly(2024, 12, 31));
		interval.Contains(new DateOnly(1900, 1, 1)).Should().BeTrue();
	}

	[Fact]
	public void WhenEndDateIsNull_AnyDateAfterStartDateReturnsTrue()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), null);
		interval.Contains(new DateOnly(9999, 12, 31)).Should().BeTrue();
	}

	[Fact]
	public void WhenBothDatesAreNull_AlwaysReturnsTrue()
	{
		UnixDateInterval interval = new(null, null);
		interval.Contains(new DateOnly(2024, 6, 15)).Should().BeTrue();
	}
}