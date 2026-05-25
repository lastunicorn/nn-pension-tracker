using DustInTheWind.NN.Toolkit.MandatoryPrivatePension.Pdf;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.ConsoleUtils;

internal readonly record struct TableExtractionApproachPretty
{
    private readonly TableExtractionApproach value;

    private TableExtractionApproachPretty(TableExtractionApproach value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value switch
        {
            TableExtractionApproach.Unknown => "Unknown",
            TableExtractionApproach.NurminenAlgorithm => "Nurminen's algorithm",
            TableExtractionApproach.WholePage => "Whole page",
            _ => value.ToString()
        };
    }

    public static implicit operator TableExtractionApproachPretty(TableExtractionApproach value)
    {
        return new TableExtractionApproachPretty(value);
    }

    public static implicit operator TableExtractionApproach(TableExtractionApproachPretty yesNoBool)
    {
        return yesNoBool.value;
    }
}