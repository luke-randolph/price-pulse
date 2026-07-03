namespace PricePulse.Data;

public class Series
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Units { get; set; }
    public required string Icon { get; set; }

    public List<Observation> Observations { get; set; } = new();
}
