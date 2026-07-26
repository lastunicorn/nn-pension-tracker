using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class FromYearTests
{
	[Fact]
	public void ReturnsIntervalStartingOnJan1()
	{
		UnixDateInterval interval = UnixDateInterval.FromYear(2024);
		interval.HasStartDate.Should().BeTrue();
	}

	[Fact]
	public void ReturnsIntervalWithCorrectDayCount()
	{
		UnixDateInterval interval = UnixDateInterval.FromYear(2024); // 2024 is a leap year
		interval.DayCount.Should().Be(366);
	}

	[Fact]
	public void ReturnsIntervalWithCorrectDayCountForNonLeapYear()
	{
		UnixDateInterval interval = UnixDateInterval.FromYear(2023);
		interval.DayCount.Should().Be(365);
	}
}