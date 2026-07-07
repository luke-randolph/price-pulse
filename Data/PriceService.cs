using Microsoft.EntityFrameworkCore;
using PricePulse.Fred;

namespace PricePulse.Data;

public class PriceService
{
    private readonly PriceContext _db;
    private readonly FredClient _fred;

    public PriceService(PriceContext db, FredClient fred)
    {
        _db = db;
        _fred = fred;
    }

    public async Task<IReadOnlyList<Series>> GetSeriesAsync(CancellationToken ct = default)
    {
        // Reference series (CPI, wages) power the lenses but are not consumer goods, so keep them out of the catalog.
        return await _db.Series
            .Where(s => !s.IsReference)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Observation>> GetObservationsAsync(string seriesId, CancellationToken ct = default)
    {
        var stored = await _db.Observations
            .Where(o => o.SeriesId == seriesId)
            .OrderBy(o => o.Date)
            .ToListAsync(ct);

        if (stored.Count > 0)
        {
            return stored;
        }

        var points = await _fred.GetSeriesAsync(seriesId, ct);

        var observations = points
            .Select(p => new Observation { SeriesId = seriesId, Date = p.Date, Value = p.Value })
            .ToList();

        _db.Observations.AddRange(observations);
        await _db.SaveChangesAsync(ct);

        return observations;
    }
}
