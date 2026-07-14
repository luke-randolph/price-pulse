using PricePulse.Fred;
using PricePulse.Models;

namespace PricePulse.Services;

// Fetches every catalog series from FRED and publishes the result to PriceStore. This is the only
// path that talks to FRED. Runs once at startup (warm-up) and periodically via DataSyncService.
public class FredDataLoader
{
    private readonly FredClient _fred;
    private readonly PriceStore _store;
    private readonly ILogger<FredDataLoader> _logger;

    public FredDataLoader(FredClient fred, PriceStore store, ILogger<FredDataLoader> logger)
    {
        _fred = fred;
        _store = store;
        _logger = logger;
    }

    // Returns the total number of observations now in the store. A series that fails to fetch keeps
    // whatever data it already had, so a transient FRED hiccup never blanks out a warm cache.
    public async Task<int> RefreshAsync(CancellationToken ct = default)
    {
        using var gate = new SemaphoreSlim(6);

        var fetches = SeriesCatalog.All.Select(async series =>
        {
            await gate.WaitAsync(ct);
            try
            {
                var points = await _fred.GetSeriesAsync(series.Id, ct);
                IReadOnlyList<Observation>? observations = points
                    .OrderBy(p => p.Date)
                    .Select(p => new Observation { SeriesId = series.Id, Date = p.Date, Value = p.Value })
                    .ToList();
                return (series.Id, observations);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FRED fetch failed for series {SeriesId}", series.Id);
                return (series.Id, observations: (IReadOnlyList<Observation>?)null);
            }
            finally
            {
                gate.Release();
            }
        });

        var results = await Task.WhenAll(fetches);

        var snapshot = new Dictionary<string, IReadOnlyList<Observation>>(_store.Snapshot);
        foreach (var (id, observations) in results)
        {
            if (observations is not null)
            {
                snapshot[id] = observations;
            }
        }

        _store.Replace(snapshot);
        return snapshot.Values.Sum(list => list.Count);
    }
}
