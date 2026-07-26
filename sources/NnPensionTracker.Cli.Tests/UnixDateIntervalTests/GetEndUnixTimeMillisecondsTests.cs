using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class GetEndUnixTimeMillisecondsTests
{
	[Fact]
	public void WhenEndDateIsSet_ReturnsLastMillisecondOfDay()
	{
		UnixDateInterval interval = new(null, new DateOnly(2024, 1, 1));
		DateTimeOffset startOfDay = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
		long expectedEndOfDay = startOfDay.AddTicks(TimeSpan.TicksPerDay - TimeSpan.TicksPerMillisecond).ToUnixTimeMilliseconds();
		interval.GetEndUnixTimeMilliseconds().Should().Be(expectedEndOfDay);
	}

	[Fact]
	public void WhenEndDateIsNull_UsesDateOnlyMaxValue()
	{
		UnixDateInterval interval = new(null, null);
		DateTimeOffset startOfDay = new(DateOnly.MaxValue.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
		long expectedEndOfDay = startOfDay.AddTicks(TimeSpan.TicksPerDay - TimeSpan.TicksPerMillisecond).ToUnixTimeMilliseconds();
		interval.GetEndUnixTimeMilliseconds().Should().Be(expectedEndOfDay);
	}

	[Fact]
	public void EndIsAlwaysGreaterThanStart_WhenBothDatesSet()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		interval.GetEndUnixTimeMilliseconds().Should().BeGreaterThan(interval.GetStartUnixTimeMilliseconds());
	}
}