using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class HasStartDateTests
{
	[Fact]
	public void WhenStartDateIsNull_ReturnsFalse()
	{
		UnixDateInterval interval = new(null, null);
		interval.HasStartDate.Should().BeFalse();
	}

	[Fact]
	public void WhenStartDateIsSet_ReturnsTrue()
	{
		UnixDateInterval interval = new(new DateOnly(2024, 1, 1), null);
		interval.HasStartDate.Should().BeTrue();
	}
}