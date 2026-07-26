using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ShowAccount;

public class ShowAccountResponse
{
    public List<Contribution> Contributions { get; set; }
}
