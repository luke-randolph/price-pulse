using PricePulse.Models;

namespace PricePulse.Services;

// In-memory home for all FRED observations, shared as a singleton. FredDataLoader swaps in a fresh
// snapshot atomically, so reads never lock and never see a torn map.
public class PriceStore
{
    private readonly ILogger<PriceStore> _logger;

    private volatile IReadOnlyDictionary<string, IReadOnlyList<Observation>> _snapshot =
        new Dictionary<string, IReadOnlyList<Observation>>();

    public PriceStore(ILogger<PriceStore> logger)
    {
        _logger = logger;
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Observation>> Snapshot => _snapshot;

    public IReadOnlyList<Observation> GetObservations(string seriesId) =>
        _snapshot.TryGetValue(seriesId, out var observations) ? observations : Array.Empty<Observation>();

    public event Action? Changed;

    // Subscribers are isolated: a circuit torn down mid-refresh must not stop the others being
    // notified, nor surface to DataSyncService as a failed refresh.
    public void Replace(IReadOnlyDictionary<string, IReadOnlyList<Observation>> snapshot)
    {
        _snapshot = snapshot;

        foreach (var subscriber in Changed?.GetInvocationList().Cast<Action>() ?? [])
        {
            try
            {
                subscriber();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "A price-cache subscriber threw while being notified.");
            }
        }
    }

    public bool IsWarm => _snapshot.Count > 0;
}
