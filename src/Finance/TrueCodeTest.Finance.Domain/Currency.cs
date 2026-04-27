namespace TrueCodeTest.Finance.Domain;

public sealed class Currency
{
    public int Id { get; set; }
    public string CharCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public int Nominal { get; set; }
    public DateTime UpdatedAt { get; set; }
}
