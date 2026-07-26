using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowAccount;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands;

[NamedCommand("account-show", Description = "Displays the contributions from the current account, optionally filtered by year or by a month interval.")]
[CommandOrder(11)]
internal class AccountShowCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(Order = 1, IsMandatory = false, DisplayName = "verb", Description = "Optional 'show' verb. It is the default action, so it may be omitted.")]
	public string Verb { get; set; }

	[NamedParameter("year", IsMandatory = false, Description = "Displays only the contributions from the specified year.")]
	public int? Year { get; set; }

	[NamedParameter("from", IsMandatory = false, Description = "Displays only the contributions starting with the specified month (MM/yyyy, a date or a year).")]
	public string FromMonth { get; set; }

	[NamedParameter("to", IsMandatory = false, Description = "Displays only the contributions up to the specified month (MM/yyyy, a date or a year).")]
	public string ToMonth { get; set; }

	public AccountShowCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		if (Verb != null && Verb != "show")
			throw new Exception($"Unknown command: account {Verb}");

		ShowAccountRequest request = new()
		{
			Year = Year,
			FromMonth = ParseMonth(FromMonth, "from", 1),
			ToMonth = ParseMonth(ToMonth, "to", 12)
		};

		await requestBus.SendAsync(request);
	}

	private static MonthDate? ParseMonth(string text, string parameterName, int defaultMonth)
	{
		if (text == null)
			return null;

		if (MonthDate.TryParse(text, out MonthDate monthDate))
			return monthDate;

		if (DateTime.TryParse(text, out DateTime date))
			return new MonthDate(date.Year, date.Month);

		if (int.TryParse(text, out int year))
			return new MonthDate(year, defaultMonth);

		throw new FormatException($"Invalid '{parameterName}' month date format. Expected format is MM/yyyy or a valid date.");
	}
}
