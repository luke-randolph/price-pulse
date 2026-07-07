using System.Globalization;
using System.Net.Http.Json;

namespace PricePulse.Fred;

public class FredClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public FredClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Fred:ApiKey"]
            ?? throw new InvalidOperationException("Fred:ApiKey is not configured. Set it with: dotnet user-secrets set \"Fred:ApiKey\" \"<key>\"");
    }

    public async Task<IReadOnlyList<PricePoint>> GetSeriesAsync(string seriesId, CancellationToken ct = default)
    {
        var url = $"fred/series/observations?series_id={seriesId}&api_key={_apiKey}&file_type=json";

        using var response = await _http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"FRED request for series '{seriesId}' failed ({(int)response.StatusCode}): {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<FredObservationsResponse>(ct)
            ?? throw new InvalidOperationException($"FRED returned no data for series '{seriesId}'.");

        return payload.Observations.Select(ToPricePoint).ToList();
    }

    private static PricePoint ToPricePoint(FredObservation observation)
    {
        var date = DateOnly.ParseExact(observation.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        decimal? value = decimal.TryParse(observation.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;

        return new PricePoint(date, value);
    }
}
