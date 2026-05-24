namespace DustInTheWind.NnPensionTracker.Cli.Ports.DataAccess;

public interface IEntityPersister<T>
{
    Task<IEnumerable<T>> LoadAsync();
    
    Task SaveAsync(IEnumerable<T> entities);
}