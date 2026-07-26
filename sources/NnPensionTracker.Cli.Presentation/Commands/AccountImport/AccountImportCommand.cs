using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ImportAccount;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands.AccountImport;

[NamedCommand("account-import", Description = "Imports the contribution records from an NN contribution statement PDF file.")]
[CommandOrder(12)]
internal class AccountImportCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	[NamedParameter("file", IsMandatory = false, Description = "The path of the NN contribution statement PDF file.")]
	public string FilePath { get; set; }

	[AnonymousParameter(Order = 1, IsMandatory = false, DisplayName = "pdf-file-path", Description = "The path of the NN contribution statement PDF file. Alternative to --file.")]
	public string FilePathOperand { get; set; }

	public AccountImportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		ImportAccountRequest request = new()
		{
			FilePath = FilePath ?? FilePathOperand
		};

		await requestBus.SendAsync(request);
	}
}