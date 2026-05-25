using DustInTheWind.NnPensionTracker.Cli.Presentation.ConsoleUtils;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.Help;

public class HelpUseCase : IUseCase
{
	public Task Execute()
	{
		Display([
			new HelpItem
			{
				Command = "account [show] [--year <year> [--month <month>]]",
				Description =
				[
					"Displays the contributions from the current account.",
					"If --year is specified, only contributions from that year are displayed.",
					"If --month is specified (requires --year), only contributions from that specific month are displayed."
				]
			},
			new HelpItem
			{
				Command = "account import --file <pdf-file-path>",
				Description = "Imports the contribution records from an NN contribution statement PDF file."
			},
			new HelpItem
			{
				Command = "account export [--format <format-name>] [--year <year>]",
				Description =
				[
					"Exports the contribution records from the database to a file in the specified format.",
					"Supported formats are: 'pp' (CSV files for PortfolioPerformance)",
					"Format default value is 'pp'.",
					"If --year is specified, only contributions from that year are exported."
				]
			},
			new HelpItem
			{
				Command = "account clear",
				Description = "Clears all contribution records from the database."
			},
			new HelpItem
			{
				Command = "fund [show] [--year <year>] [--from <date>] [--to <date>]",
				Description =
				[
					"Displays the fund values from the database.",
					"If --year is specified, only values from that year are displayed.",
					"If --from and/or --to are specified, only values in that interval are displayed."
				]
			},
			new HelpItem
			{
				Command = "fund show --source web [--year <year> | --from <date> --to <date>]",
				Description =
				[
					"Displays fund values from NN's website without importing them into the database.",
					"When no date interval is specified, it displays values from the current year."
				]
			},
			new HelpItem
			{
				Command = "fund import [--source web] --from <date> --to <date>",
				Description =
				[
					"Imports the fund values for the specified date range from NN's website.",
					"The --source option can be used to specify the source of the fund values. When --from and --to are specified, the source is automatically set to 'web'."
				]
			},
			new HelpItem
			{
				Command = "fund import [--source web] --year <year>",
				Description =
				[
					"Imports the fund values for the specified year from NN's website.",
					"The --source option can be used to specify the source of the fund values. When --year is specified, the source is automatically set to 'web'."
				]
			},
			new HelpItem
			{
				Command = "fund import [--source file] --file <file-path>",
				Description =
				[
					"Imports the fund values from a CSV file. The file must have the same format as the historical fund values CSV file that can be downloaded from NN's website.",
					"The --source option can be used to specify the source of the fund values. When --file is specified, the source is automatically set to 'file'."
				]
			},
			new HelpItem
			{
				Command = "fund export --file <file-path> [--year <year>]",
				Description = "Exports the fund values into a CSV file. If --year is specified, only values from that year are exported."
			},
			new HelpItem
			{
				Command = "fund clear",
				Description = "Clears all fund values from the database."
			},
			new HelpItem
			{
				Command = "help",
				Description = "Displays this help message."
			}
		]);

		return Task.CompletedTask;
	}

	private static void Display(List<HelpItem> helpItems)
	{
		XConsole xConsole = XConsole.Create()
			.WriteLine("Usage:");

		foreach (HelpItem helpItem in helpItems)
		{
			xConsole
				.WriteLine()
				.With(null, null)
				.WriteLine(helpItem.Command)
				.With(ConsoleColor.DarkGray, null);

			if (helpItem.Description != null)
				foreach (string descriptionLine in helpItem.Description)
					xConsole.WriteLine("  " + descriptionLine);
		}
	}
}