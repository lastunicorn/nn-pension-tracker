using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class DayCountTests
{
	[Fact]
	public void WhenBothDatesSet_ReturnsCorrectCount()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
		interval.DayCount.Should().Be(10);
	}

	[Fact]
	public void WhenSameDateForBoth_ReturnsOne()
	{
		DateOnly date = new(2024, 6, 15);
		UnixDateInterval interval = new(date, date);
		interval.DayCount.Should().Be(1);
	}

	[Fact]
	public void WhenStartDateIsNull_UsesUnixEpochAsStart()
	{
		DateOnly endDate = new(1970, 1, 11);
		UnixDateInterval interval = new(null, endDate);
		// From 1970-01-01 to 1970-01-11 inclusive = 11 days
		interval.DayCount.Should().Be(11);
	}

	[Fact]
	public void WhenEndDateIsNull_UsesDateOnlyMaxValueAsEnd()
	{
		DateOnly startDate = DateOnly.MaxValue;
		UnixDateInterval interval = new(startDate, null);
		interval.DayCount.Should().Be(1);
	}

	[Fact]
	public void WhenBothDatesAreNull_UsesEpochToMaxValue()
	{
		UnixDateInterval interval = new(null, null);
		int expected = DateOnly.MaxValue.DayNumber - new DateOnly(1970, 1, 1).DayNumber + 1;
		interval.DayCount.Should().Be(expected);
	}
}