using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.MetadataModel;
using ExecutionContext = DustInTheWind.ConsoleTools.Commando.MetadataModel.ExecutionContext;

namespace NN.Toolkit.Cli.Tests.NounVerbCommandParserTests;

[NamedCommand("account")]
internal class AccountFakeCommand : IConsoleCommand
{
	public Task Execute()
	{
		return Task.CompletedTask;
	}
}

[NamedCommand("account-import")]
internal class AccountImportFakeCommand : IConsoleCommand
{
	public Task Execute()
	{
		return Task.CompletedTask;
	}
}

internal static class ExecutionContextFactory
{
	public static ExecutionContext Create()
	{
		ExecutionContext executionContext = new();
		executionContext.Commands.Add(new CommandMetadata(typeof(AccountFakeCommand)));
		executionContext.Commands.Add(new CommandMetadata(typeof(AccountImportFakeCommand)));
		return executionContext;
	}
}
