using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.UseCases.ExportAccount;

internal sealed class NnCashTransactionsDocument : IDisposable, IAsyncDisposable
{
    private readonly StreamWriter streamWriter;

    public NnCashTransactionsDocument(StreamWriter streamWriter)
    {
        this.streamWriter = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
        streamWriter.WriteLine("Type,Cash Account,Date,Time,Value,Note");
    }

    public Task WriteAsync(Contribution contribution)
    {
        string date = $"{contribution.PaidInMonth.Year:00}-{contribution.PaidInMonth.Month:00}-01";
        return streamWriter.WriteLineAsync($"Deposit,NN,{date},08:00,{contribution.GrossValue},\"Luna: {contribution.Month}\"");
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