namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class DataAccessException : Exception
{
	public DataAccessException(string message)
		: base(message)
	{
	}

	public DataAccessException(string message, Exception innerException)
		: base("[Data Access Error] " + message, innerException)
	{
	}
}