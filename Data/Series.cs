namespace PricePulse.Data;

public class Series
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Units { get; set; }
    public required string Icon { get; set; }

    // Reference series are fetched and stored like any other series but hidden from the consumer-goods catalog.
    public bool IsReference { get; set; }

    public List<Observation> Observations { get; set; } = new();
}
