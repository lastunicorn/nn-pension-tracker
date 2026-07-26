using DustInTheWind.ConsoleTools.Commando.RequestModel;
using DustInTheWind.NnPensionTracker.Cli;
using DustInTheWind.NnPensionTracker.Cli.Presentation;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.NounVerbCommandParserTests;

public class ParseTests
{
	private readonly NounVerbCommandParser parser = new(ExecutionContextFactory.Create());

	[Fact]
	public void WhenNounVerbPairMatchesRegisteredCommand_ThenCommandNameIsFoldedName()
	{
		CommandRequest commandRequest = parser.Parse(["account", "import"]);

		commandRequest.CommandName.Should().Be("account-import");
		commandRequest.Operands.Should().BeEmpty();
	}

	[Fact]
	public void WhenNounVerbPairMatchesRegisteredCommand_ThenRemainingArgumentsArePreserved()
	{
		CommandRequest commandRequest = parser.Parse(["account", "import", "statement.pdf", "--file", "other.pdf"]);

		commandRequest.CommandName.Should().Be("account-import");
		commandRequest.Operands.Should().ContainSingle(x => x.Value == "statement.pdf");
		commandRequest.Options.Should().ContainSingle(x => x.Name == "file" && x.Value == "other.pdf");
	}

	[Fact]
	public void WhenVerbDoesNotFormRegisteredCommand_ThenNounIsCommandNameAndVerbIsOperand()
	{
		CommandRequest commandRequest = parser.Parse(["account", "unknownverb"]);

		commandRequest.CommandName.Should().Be("account");
		commandRequest.Operands.Should().ContainSingle(x => x.Value == "unknownverb");
	}

	[Fact]
	public void WhenSecondArgumentIsAnOption_ThenNoFoldingIsPerformed()
	{
		CommandRequest commandRequest = parser.Parse(["account", "--year", "2025"]);

		commandRequest.CommandName.Should().Be("account");
		commandRequest.Options.Should().ContainSingle(x => x.Name == "year" && x.Value == "2025");
	}

	[Fact]
	public void WhenSingleArgumentIsProvided_ThenItBecomesTheCommandName()
	{
		CommandRequest commandRequest = parser.Parse(["account"]);

		commandRequest.CommandName.Should().Be("account");
	}

	[Fact]
	public void WhenNoArgumentsAreProvided_ThenRequestIsEmpty()
	{
		CommandRequest commandRequest = parser.Parse([]);

		commandRequest.IsEmpty.Should().BeTrue();
	}

	[Fact]
	public void WhenArgsIsNull_ThenRequestIsEmpty()
	{
		CommandRequest commandRequest = parser.Parse(null!);

		commandRequest.IsEmpty.Should().BeTrue();
	}
}