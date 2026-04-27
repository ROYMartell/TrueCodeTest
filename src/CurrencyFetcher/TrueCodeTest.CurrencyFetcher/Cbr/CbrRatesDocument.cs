using System.Xml.Serialization;

namespace TrueCodeTest.CurrencyFetcher.Cbr;

[XmlRoot("ValCurs")]
public sealed class CbrRatesDocument
{
    [XmlAttribute("Date")]
    public string? Date { get; set; }

    [XmlAttribute("name")]
    public string? Name { get; set; }

    [XmlElement("Valute")]
    public List<CbrValute> Valutes { get; set; } = new();
}

public sealed class CbrValute
{
    [XmlAttribute("ID")]
    public string? Id { get; set; }

    [XmlElement("NumCode")]
    public string NumCode { get; set; } = string.Empty;

    [XmlElement("CharCode")]
    public string CharCode { get; set; } = string.Empty;

    [XmlElement("Nominal")]
    public string Nominal { get; set; } = "1";

    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("Value")]
    public string Value { get; set; } = "0";

    [XmlElement("VunitRate")]
    public string? VunitRate { get; set; }
}
