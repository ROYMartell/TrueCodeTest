using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using TrueCodeTest.CurrencyFetcher.Options;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.CurrencyFetcher.Cbr;

public sealed class CbrClient(HttpClient http, IOptions<CurrencyFetcherOptions> options, ILogger<CbrClient> logger)
{
    private static readonly XmlSerializer Serializer = new(typeof(CbrRatesDocument));
    private static readonly CultureInfo RuCulture = CultureInfo.GetCultureInfo("ru-RU");

    private readonly CurrencyFetcherOptions _options = options.Value;

    public async Task<IReadOnlyList<Currency>> FetchAsync(CancellationToken ct)
    {
        logger.LogDebug("Fetching rates from {Url}", _options.SourceUrl);

        using var response = await http.GetAsync(_options.SourceUrl, ct);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        // CBR XML is encoded in windows-1251
        var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251) ?? Encoding.UTF8;
        var xml = encoding.GetString(bytes);

        using var reader = new StringReader(xml);
        var doc = (CbrRatesDocument?)Serializer.Deserialize(reader);
        if (doc is null || doc.Valutes.Count == 0)
        {
            logger.LogWarning("CBR response contains no currencies");
            return Array.Empty<Currency>();
        }

        var now = DateTime.UtcNow;
        var currencies = new List<Currency>(doc.Valutes.Count);
        foreach (var v in doc.Valutes)
        {
            if (!int.TryParse(v.NumCode, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numCode))
                continue;
            if (!int.TryParse(v.Nominal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var nominal) || nominal <= 0)
                nominal = 1;
            if (!decimal.TryParse(v.Value, NumberStyles.Float, RuCulture, out var value)
                && !decimal.TryParse(v.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                logger.LogWarning("Failed to parse rate for {Code}: {Value}", v.CharCode, v.Value);
                continue;
            }

            var rate = Math.Round(value / nominal, 6, MidpointRounding.ToEven);

            currencies.Add(new Currency
            {
                Id = numCode,
                CharCode = v.CharCode,
                Name = v.Name,
                Rate = rate,
                Nominal = nominal,
                UpdatedAt = now,
            });
        }

        return currencies;
    }
}
