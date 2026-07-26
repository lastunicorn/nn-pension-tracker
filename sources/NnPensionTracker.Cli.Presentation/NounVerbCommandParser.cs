using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.Parsing;
using DustInTheWind.ConsoleTools.Commando.RequestModel;
using ExecutionContext = DustInTheWind.ConsoleTools.Commando.MetadataModel.ExecutionContext;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation;

// Commando matches commands by a single-token name (the first bare argument), so the
// noun-verb grammar ("account import") is preserved by folding the leading noun and verb
// into one command name ("account-import") whenever a command with that name is registered.
public class NounVerbCommandParser : ICommandParser
{
	private readonly ExecutionContext executionContext;
	private readonly CommandParser commandParser = new();

	public NounVerbCommandParser(ExecutionContext executionContext)
	{
		this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
	}

	public CommandRequest Parse(string[] args)
	{
		string[] processedArgs = FoldNounVerbPair(args);
		return commandParser.Parse(processedArgs);
	}

	private string[] FoldNounVerbPair(string[] args)
	{
		if (args is not { Length: >= 2 })
			return args;

		if (!IsBareWord(args[0]) || !IsBareWord(args[1]))
			return args;

		string foldedCommandName = $"{args[0]}-{args[1]}";

		bool commandExists = executionContext.Commands.GetAllByName(foldedCommandName).Any();
		if (!commandExists)
			return args;

		string[] foldedArgs = new string[args.Length - 1];
		foldedArgs[0] = foldedCommandName;
		Array.Copy(args, 2, foldedArgs, 1, args.Length - 2);

		return foldedArgs;
	}

	private static bool IsBareWord(string arg)
	{
		return !string.IsNullOrEmpty(arg) && !arg.StartsWith('-') && !arg.StartsWith('/');
	}
}
