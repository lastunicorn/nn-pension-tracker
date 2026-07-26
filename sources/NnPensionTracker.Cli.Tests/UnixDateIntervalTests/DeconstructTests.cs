using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class DeconstructTests
{
	[Fact]
	public void DeconstructsIntoBothDates()
	{
		DateOnly start = new(2024, 1, 1);
		DateOnly end = new(2024, 12, 31);
		UnixDateInterval interval = new(start, end);

		(DateOnly? s, DateOnly? e) = interval;

		s.Should().Be(start);
		e.Should().Be(end);
	}

	[Fact]
	public void DeconstructsNullDates()
	{
		UnixDateInterval interval = new(null, null);

		(DateOnly? s, DateOnly? e) = interval;

		s.Should().BeNull();
		e.Should().BeNull();
	}
}