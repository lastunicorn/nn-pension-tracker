using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;
using DustInTheWind.NnPensionTracker.Ports.DataAccess;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ShowAccount;

public class ShowAccountUseCase : IUseCase
{
    private readonly IUnitOfWork unitOfWork;

    public int? Year { get; set; }

    public int? Month { get; set; }

    public ShowAccountUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    public async Task Execute()
    {
        IAsyncEnumerable<Contribution> source;

        if (Year.HasValue)
        {
            if (Month.HasValue)
                source = unitOfWork.ContributionRepository.GetByYearMonth(Year.Value, Month.Value);
            else
                source = unitOfWork.ContributionRepository.GetByYear(Year.Value);
        }
        else
        {
            source = unitOfWork.ContributionRepository.GetAll();
        }

        await DisplayContributions(source);
    }

    private async Task DisplayContributions(IAsyncEnumerable<Contribution> contributions)
    {
        DataGrid dataGrid = new()
        {
            EmptyGridMessage = "No data"
        };

        dataGrid.Columns.Add("Month", HorizontalAlignment.Center);
        dataGrid.Columns.Add("Gross Value", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Administration Fee", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Net Value", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Unit Value", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Unit Count", HorizontalAlignment.Right);
        dataGrid.Columns.Add("Paid in Month", HorizontalAlignment.Center);

        await foreach (Contribution contribution in contributions)
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