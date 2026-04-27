namespace TrueCodeTest.CurrencyFetcher.Options;

public sealed class CurrencyFetcherOptions
{
    public const string SectionName = "CurrencyFetcher";

    public int IntervalSeconds { get; set; } = 3600;
    public string SourceUrl { get; set; } = "http://www.cbr.ru/scripts/XML_daily.asp";
    public int HttpTimeoutSeconds { get; set; } = 30;
    public int StartupDelaySeconds { get; set; } = 5;
}
