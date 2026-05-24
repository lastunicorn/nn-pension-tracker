namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public interface IEntityPersister<T>
{
    Task<IEnumerable<T>> LoadAsync();
    
    Task SaveAsync(IEnumerable<T> entities);
}