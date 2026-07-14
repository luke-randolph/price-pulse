using PricePulse.Models;

namespace PricePulse.Services;

// Read-side API for the pages. Everything is served from PriceStore (in memory), so reads are
// synchronous and never touch FRED.
public class PriceService
{
    private readonly PriceStore _store;

    public PriceService(PriceStore store)
    {
        _store = store;
    }

    // Reference series (CPI, wages) power the lenses but are not consumer goods, so keep them out of the catalog.
    public IReadOnlyList<Series> GetSeries() =>
        SeriesCatalog.All
            .Where(s => !s.IsReference)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToList();

    public IReadOnlyList<Observation> GetObservations(string seriesId) =>
        _store.GetObservations(seriesId);

    // False only when the startup warm-up hasn't populated the cache yet (e.g. FRED was
    // unavailable). Pages use it to show a "loading" state instead of a wall of empty values.
    public bool IsWarm => _store.IsWarm;
}
