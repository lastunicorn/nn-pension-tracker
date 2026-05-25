using System.Collections.ObjectModel;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.Help;

internal class HelpItemDescription : Collection<string>
{
	public override string ToString()
	{
		return string.Join(Environment.NewLine, Items);
	}

	public static implicit operator HelpItemDescription(List<string> descriptions)
	{
		HelpItemDescription result = [];

		foreach (string description in descriptions)
			result.Add(description);

		return result;
	}

	public static implicit operator List<string>(HelpItemDescription description)
	{
		return description.ToList();
	}

	public static implicit operator HelpItemDescription(string description)
	{
		return
		[
			description
		];
	}
}