namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class DatabaseNotOpenException : DataAccessException
{
    public DatabaseNotOpenException()
        : base("Open the database before using it.")
    {
    }
}