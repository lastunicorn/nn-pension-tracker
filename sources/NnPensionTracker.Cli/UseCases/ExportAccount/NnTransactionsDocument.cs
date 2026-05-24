using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ExportAccount;

internal sealed class NnTransactionsDocument : IDisposable, IAsyncDisposable
{
    private readonly IList<string> labels;
    private readonly StreamWriter streamWriter;

    public NnTransactionsDocument(StreamWriter streamWriter, IList<string> labels)
    {
        this.streamWriter = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
        this.labels = labels;
        
        this.streamWriter.WriteLine("Security Name,Ticker Symbol,Date,Time,Value,Shares,Type,Fees,Note");
    }

    public Task WriteAsync(Contribution contribution)
    {
        string date = $"{contribution.PaidInMonth.Year:00}-{contribution.PaidInMonth.Month:00}-01";
        string note = labels == null
            ? string.Empty
            : string.Join("; ", labels.Zip(contribution.ToStringArray(), (h, v) => $"{h}={v}"));

        return streamWriter.WriteLineAsync($"NN,NN,{date},08:05,{contribution.GrossValue},{contribution.UnitCount},Buy,{contribution.AdministrationFee},\"{note}\"");
    }

    public void Dispose()
    {
        streamWriter?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (streamWriter != null)
            await streamWriter.DisposeAsync();
    }
}