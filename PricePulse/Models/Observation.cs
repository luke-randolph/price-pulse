namespace PricePulse.Models;

public class Observation
{
    public required string SeriesId { get; set; }
    public DateOnly Date { get; set; }
    public decimal? Value { get; set; }
}
