using PricePulse.Models;

namespace PricePulse.Services;

// In-memory home for all FRED observations, shared as a singleton. FredDataLoader swaps in a fresh
// snapshot atomically, so reads never lock and never see a torn map.
public class PriceStore
{
    private volatile IReadOnlyDictionary<string, IReadOnlyList<Observation>> _snapshot =
        new Dictionary<string, IReadOnlyList<Observation>>();

    public IReadOnlyDictionary<string, IReadOnlyList<Observation>> Snapshot => _snapshot;

    public IReadOnlyList<Observation> GetObservations(string seriesId) =>
        _snapshot.TryGetValue(seriesId, out var observations) ? observations : Array.Empty<Observation>();

    public void Replace(IReadOnlyDictionary<string, IReadOnlyList<Observation>> snapshot) =>
        _snapshot = snapshot;

    public bool IsWarm => _snapshot.Count > 0;
}
