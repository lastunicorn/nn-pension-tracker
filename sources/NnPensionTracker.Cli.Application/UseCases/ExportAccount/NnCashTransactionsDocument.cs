using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.Application.UseCases.ExportAccount;

internal sealed class NnCashTransactionsDocument : IDisposable, IAsyncDisposable
{
    private readonly StreamWriter streamWriter;
    private readonly CsvWriter csvWriter;

    public NnCashTransactionsDocument(StreamWriter streamWriter)
    {
        this.streamWriter = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));

        CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };

        csvWriter = new CsvWriter(this.streamWriter, csvConfiguration);

        csvWriter.WriteField("Type");
        csvWriter.WriteField("Cash Account");
        csvWriter.WriteField("Date");
        csvWriter.WriteField("Time");
        csvWriter.WriteField("Value");
        csvWriter.WriteField("Note");
        csvWriter.NextRecord();
    }

    public Task WriteAsync(Contribution contribution)
    {
        string date = $"{contribution.PaidInMonth.Year:00}-{contribution.PaidInMonth.Month:00}-01";

        csvWriter.WriteField("Deposit");
        csvWriter.WriteField("NN");
        csvWriter.WriteField(date);
        csvWriter.WriteField("08:00");
        csvWriter.WriteField(contribution.GrossValue);
        csvWriter.WriteField($"Luna: {contribution.Month}", true);
        return csvWriter.NextRecordAsync();
    }

    public void Dispose()
    {
        csvWriter?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (streamWriter != null)
            await streamWriter.DisposeAsync();
    }
}