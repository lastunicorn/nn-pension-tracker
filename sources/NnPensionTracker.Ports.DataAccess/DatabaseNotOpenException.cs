namespace DustInTheWind.NnPensionTracker.Ports.DataAccess;

public class DatabaseNotOpenException : DataAccessException
{
    public DatabaseNotOpenException()
        : base("Open the database before using it.")
    {
    }
}