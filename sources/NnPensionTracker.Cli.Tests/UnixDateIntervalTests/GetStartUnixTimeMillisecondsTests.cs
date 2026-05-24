using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class GetStartUnixTimeMillisecondsTests
{
	[Fact]
	public void WhenStartDateIsUnixEpoch_ReturnsZero()
	{
		UnixDateInterval interval = new(new DateOnly(1970, 1, 1), null);
		interval.GetStartUnixTimeMilliseconds().Should().Be(0);
	}

	[Fact]
	public void WhenStartDateIsNull_ReturnsZero()
	{
		UnixDateInterval interval = new(null, null);
		interval.GetStartUnixTimeMilliseconds().Should().Be(0);
	}

	[Fact]
	public void WhenStartDateIsSet_ReturnsCorrectMilliseconds()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), null);
		long expected = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
		interval.GetStartUnixTimeMilliseconds().Should().Be(expected);
	}

	[Fact]
	public void WhenStartDateIsAfterEpoch_ReturnsPositiveValue()
	{
		UnixDateInterval interval = new(new DateOnly(2000, 1, 1), null);
		interval.GetStartUnixTimeMilliseconds().Should().BePositive();
	}
}