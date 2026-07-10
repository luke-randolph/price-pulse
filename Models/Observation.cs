namespace PricePulse.Models;

public class Observation
{
    public int Id { get; set; }
    public required string SeriesId { get; set; }
    public Series? Series { get; set; }
    public DateOnly Date { get; set; }
    public decimal? Value { get; set; }
}
