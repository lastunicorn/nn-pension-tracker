using DustInTheWind.NnPensionTracker.Domain;

namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IDataLabelRepository
{
	IAsyncEnumerable<DataLabel> GetAll();
}