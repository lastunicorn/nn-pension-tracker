using DustInTheWind.NnPensionTracker.Cli;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.NounVerbCommandParserTests;

public class ConstructorTests
{
	[Fact]
	public void WhenExecutionContextIsNull_ThrowsArgumentNullException()
	{
		Action act = () =>
		{
			_ = new NounVerbCommandParser(null!);
		};

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void WhenExecutionContextIsProvided_DoesNotThrow()
	{
		Action act = () =>
		{
			_ = new NounVerbCommandParser(ExecutionContextFactory.Create());
		};

		act.Should().NotThrow();
	}
}
