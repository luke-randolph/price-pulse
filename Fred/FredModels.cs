using System.Text.Json.Serialization;

namespace PricePulse.Fred;

public record FredObservationsResponse(
    [property: JsonPropertyName("observations")] IReadOnlyList<FredObservation> Observations);

public record FredObservation(
    [property: JsonPropertyName("date")] string Date,
    [property: JsonPropertyName("value")] string Value);

public record PricePoint(DateOnly Date, decimal? Value);
