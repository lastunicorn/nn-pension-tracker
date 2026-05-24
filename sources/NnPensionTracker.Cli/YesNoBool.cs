namespace DustInTheWind.NnPensionTracker.Cli;

internal readonly record struct YesNoBool
{
    private readonly bool value;

    private YesNoBool(bool value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value ? "Yes" : "No";
    }

    public static implicit operator YesNoBool(bool value)
    {
        return new YesNoBool(value);
    }

    public static implicit operator bool(YesNoBool yesNoBool)
    {
        return yesNoBool.value;
    }
}