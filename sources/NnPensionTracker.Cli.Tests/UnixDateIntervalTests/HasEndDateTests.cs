using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class HasEndDateTests
{
	[Fact]
	public void WhenEndDateIsNull_ReturnsFalse()
	{
		UnixDateInterval interval = new(null, null);
		interval.HasEndDate.Should().BeFalse();
	}

	[Fact]
	public void WhenEndDateIsSet_ReturnsTrue()
	{
		UnixDateInterval interval = new(null, new DateOnly(2024, 12, 31));
		interval.HasEndDate.Should().BeTrue();
	}
}