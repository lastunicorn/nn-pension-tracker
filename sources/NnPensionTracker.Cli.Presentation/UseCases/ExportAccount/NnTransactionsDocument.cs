using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Cli.Presentation.UseCases.ExportAccount;

internal sealed class NnTransactionsDocument : IDisposable, IAsyncDisposable
{
    private readonly IList<string> labels;
    private readonly StreamWriter streamWriter;
    private readonly CsvWriter csvWriter;

    public NnTransactionsDocument(StreamWriter streamWriter, IList<string> labels)
    {
        this.streamWriter = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
        this.labels = labels;

        CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };

        csvWriter = new CsvWriter(this.streamWriter, csvConfiguration);

        csvWriter.WriteField("Security Name");
        csvWriter.WriteField("Ticker Symbol");
        csvWriter.WriteField("Date");
        csvWriter.WriteField("Time");
        csvWriter.WriteField("Value");
        csvWriter.WriteField("Shares");
        csvWriter.WriteField("Type");
        csvWriter.WriteField("Fees");
        csvWriter.WriteField("Note");
        csvWriter.NextRecord();
    }

    public async Task WriteAsync(Contribution contribution)
    {
        string date = $"{contribution.PaidInMonth.Year:00}-{contribution.PaidInMonth.Month:00}-01";
        string note = labels == null
            ? string.Empty
            : string.Join("; ", labels.Zip(contribution.ToStringArray(), (h, v) => $"{h}={v}"));

        csvWriter.WriteField("NN");
        csvWriter.WriteField("NN");
        csvWriter.WriteField(date);
        csvWriter.WriteField("08:05");
        csvWriter.WriteField(contribution.GrossValue);
        csvWriter.WriteField(contribution.UnitCount);
        csvWriter.WriteField("Buy");
        csvWriter.WriteField(contribution.AdministrationFee);
        csvWriter.WriteField(note, true);
        await csvWriter.NextRecordAsync();
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