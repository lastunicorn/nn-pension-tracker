using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Cli.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ShowAccount;

internal class ShowAccountUseCase : IUseCase
{
    private readonly IUnitOfWork unitOfWork;

    public ShowAccountUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    public Task Execute()
    {
        IEnumerable<Contribution> contributions = unitOfWork.ContributionRepository.GetAll();
        DisplayContributions(contributions);
        
        return Task.CompletedTask;
    }

    private void DisplayContributions(IEnumerable<Contribution> contributions)
    {
        DataGrid dataGrid = new();

        dataGrid.Columns.Add("Month", HorizontalAlignment.Center);
        dataGrid.Columns.Add("Gross Value", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Administration Fee", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Net Value", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Unit Value", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Unit Count", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Paid in Month", HorizontalAlignment.Center);

        foreach (Contribution contribution in contributions)
        {
            dataGrid.Rows.Add(
                contribution.Month,
                contribution.GrossValue,
                contribution.AdministrationFee,
                contribution.NetValue,
                contribution.UnitValue,
                contribution.UnitCount,
                contribution.PaidInMonth);
        }

        dataGrid.Display();
    }
}