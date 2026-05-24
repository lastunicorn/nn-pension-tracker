using DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.UnixDateIntervalTests;

public class ConstructorTests
{
	[Fact]
	public void WhenBothDatesAreNull_DoesNotThrow()
	{
		Action act = () => new UnixDateInterval(null, null);
		act.Should().NotThrow();
	}

	[Fact]
	public void WhenStartDateEqualsEndDate_DoesNotThrow()
	{
		DateOnly date = new(2024, 6, 15);
		Action act = () => new UnixDateInterval(date, date);
		act.Should().NotThrow();
	}

	[Fact]
	public void WhenStartDateBeforeEndDate_DoesNotThrow()
	{
		Action act = () => new UnixDateInterval(new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31));
		act.Should().NotThrow();
	}

	[Fact]
	public void WhenStartDateAfterEndDate_ThrowsArgumentException()
	{
		Action act = () => new UnixDateInterval(new DateOnly(2024, 12, 31), new DateOnly(2024, 1, 1));
		act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void WhenOnlyStartDateProvided_DoesNotThrow()
	{
		Action act = () => new UnixDateInterval(new DateOnly(2024, 1, 1), null);
		act.Should().NotThrow();
	}

	[Fact]
	public void WhenOnlyEndDateProvided_DoesNotThrow()
	{
		Action act = () => new UnixDateInterval(null, new DateOnly(2024, 12, 31));
		act.Should().NotThrow();
	}
}