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

    // False until the first refresh lands, so pages can show a "loading" state instead of empty values.
    public bool IsWarm => _store.IsWarm;

    // Raised on the refresh thread. Subscribers must unsubscribe — this lives on the singleton store,
    // so a handler left attached keeps its component alive for the life of the app.
    public event Action? Changed
    {
        add => _store.Changed += value;
        remove => _store.Changed -= value;
    }
}
