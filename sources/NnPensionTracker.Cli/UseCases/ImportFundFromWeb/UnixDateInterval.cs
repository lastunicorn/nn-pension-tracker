namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ImportFundFromWeb;

internal record struct UnixDateInterval
{
	private DateOnly? startDate;
	private DateOnly? endDate;

	public bool HasStartDate => startDate.HasValue;

	public bool HasEndDate => endDate.HasValue;

	public static UnixDateInterval FromYear(int year)
	{
		return new UnixDateInterval
		{
			startDate = new DateOnly(year, 1, 1),
			endDate = new DateOnly(year, 12, 31)
		};
	}

	public UnixDateInterval(DateOnly? startDate, DateOnly? endDate)
	{
		if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
			throw new ArgumentException($"StartDate ({startDate.Value:yyyy-MM-dd}) must be less than or equal to EndDate ({endDate.Value:yyyy-MM-dd}).");

		this.startDate = startDate;
		this.endDate = endDate;
	}

	private static readonly DateOnly UnixEpoch = new(1970, 1, 1);

	public int? DayCount
	{
		get
		{
			DateOnly start = startDate ?? UnixEpoch;
			DateOnly end = endDate ?? DateOnly.MaxValue;
			return end.DayNumber - start.DayNumber + 1;
		}
	}

	public bool Contains(DateOnly date)
	{
		if (startDate.HasValue && date < startDate.Value)
			return false;

		if (endDate.HasValue && date > endDate.Value)
			return false;

		return true;
	}

	public void Deconstruct(out DateOnly? start, out DateOnly? end)
	{
		start = startDate;
		end = endDate;
	}

	public long GetStartUnixTimeMilliseconds()
	{
		DateOnly date = startDate ?? UnixEpoch;
		DateTimeOffset dateTimeOffset = new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
		return dateTimeOffset.ToUnixTimeMilliseconds();
	}

	public long GetEndUnixTimeMilliseconds()
	{
		DateOnly date = endDate ?? DateOnly.MaxValue;
		DateTimeOffset startOfDay = new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
		DateTimeOffset endOfDay = startOfDay.AddTicks(TimeSpan.TicksPerDay - TimeSpan.TicksPerMillisecond);
		return endOfDay.ToUnixTimeMilliseconds();
	}
	
	public override string ToString()
	{
		return $"{startDate?.ToString("yyyy-MM-dd") ?? "..."} — {endDate?.ToString("yyyy-MM-dd") ?? "..."}";
	}
}