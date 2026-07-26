using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowFund;
using DustInTheWind.RequestR;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.Commands.FundShow;

[NamedCommand("fund-show", Description = "Displays the fund NAV values from the database or, when --source web is specified, directly from NN's website.")]
[CommandOrder(21)]
internal class FundShowCommand : IConsoleCommand<FundShowViewModel>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(Order = 1, IsMandatory = false, DisplayName = "verb", Description = "Optional 'show' verb. It is the default action, so it may be omitted.")]
	public string Verb { get; set; }

	[NamedParameter("source", IsMandatory = false, Description = "The source of the fund values: 'web'. When not specified, the values are read from the database.")]
	public FundNavSource Source { get; set; }

	[NamedParameter("year", IsMandatory = false, Description = "Displays only the fund values from the specified year.")]
	public int? Year { get; set; }

	[NamedParameter("from", IsMandatory = false, Description = "Displays only the fund values starting with the specified date.")]
	public DateOnly? FromDate { get; set; }

	[NamedParameter("to", IsMandatory = false, Description = "Displays only the fund values up to the specified date.")]
	public DateOnly? ToDate { get; set; }

	public FundShowCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<FundShowViewModel> Execute()
	{
		if (Verb != null && Verb != "show")
			throw new Exception($"Unknown command: fund {Verb}");

		ShowFundRequest request = new()
		{
			Source = Source,
			Year = Year,
			FromDate = FromDate,
			ToDate = ToDate
		};

		ShowFundResponse response = await requestBus.SendAsync<ShowFundRequest, ShowFundResponse>(request);

		return new FundShowViewModel
		{
			FundNavs = response.FundNavs
		};
	}
}