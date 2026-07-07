namespace PricePulse.Data;

public enum PriceLens
{
    Nominal,      // raw dollars, as reported
    RealDollars,  // inflation-adjusted to the latest month ("today's dollars")
    TimePrice     // price ÷ average hourly wage = hours of work
}

public record LensPoint(DateTime Date, decimal Value);

public static class Lens
{
    public const string CpiSeriesId = "CPIAUCNS";        // CPI-U, not seasonally adjusted
    public const string WageSeriesId = "CEU0500000008";  // production & nonsupervisory wage, NSA, back to 1964

    public static string? ReferenceSeriesId(PriceLens lens) => lens switch
    {
        PriceLens.RealDollars => CpiSeriesId,
        PriceLens.TimePrice => WageSeriesId,
        _ => null
    };

    // Projects observations through a lens. Derived lenses drop points that have no reference
    // value for their month. The time-price curve always uses the average-worker wage;
    // personalization is a separate point-in-time calc (TimePriceAt), never applied across history.
    public static IReadOnlyList<LensPoint> Apply(
        PriceLens lens,
        IReadOnlyList<Observation> nominal,
        IReadOnlyList<Observation>? reference)
    {
        var priced = nominal
            .Where(o => o.Value is not null)
            .Select(o => new LensPoint(o.Date.ToDateTime(TimeOnly.MinValue), o.Value!.Value))
            .ToList();

        if (lens == PriceLens.Nominal || reference is null)
        {
            return priced;
        }

        // Key reference values by calendar month so monthly series line up regardless of exact day.
        var byMonth = reference
            .Where(o => o.Value is { } v && v != 0)
            .GroupBy(o => (o.Date.Year, o.Date.Month))
            .ToDictionary(g => g.Key, g => g.Last().Value!.Value);

        // Base = most recent reference value, so real dollars read "in today's dollars".
        var baseValue = reference
            .Where(o => o.Value is { } v && v != 0)
            .OrderBy(o => o.Date)
            .LastOrDefault()?.Value;

        var result = new List<LensPoint>();
        foreach (var p in priced)
        {
            if (!byMonth.TryGetValue((p.Date.Year, p.Date.Month), out var refValue))
            {
                continue;
            }

            var value = lens switch
            {
                PriceLens.RealDollars when baseValue is { } b => p.Value * b / refValue,
                PriceLens.TimePrice => p.Value / refValue,
                _ => p.Value
            };

            result.Add(p with { Value = value });
        }

        return result;
    }

    // Hours of work a price costs at a specific hourly wage — powers the personalized headline.
    public static decimal TimePriceAt(decimal price, decimal hourlyWage) =>
        hourlyWage > 0 ? price / hourlyWage : 0;

    public static DateTime? BaseMonth(IReadOnlyList<Observation> reference) =>
        reference
            .Where(o => o.Value is { } v && v != 0)
            .OrderBy(o => o.Date)
            .LastOrDefault()?.Date.ToDateTime(TimeOnly.MinValue);

    public static string Format(decimal value, PriceLens lens) => lens switch
    {
        PriceLens.TimePrice => FormatWorkTime(value),
        _ => value.ToString("C2")
    };

    // Natural unit for hours of work: minutes / hours / 40-hour weeks.
    public static string FormatWorkTime(decimal hours)
    {
        var minutes = hours * 60m;
        if (minutes < 60m) return $"{minutes:0} min";
        if (hours < 40m) return $"{hours:0.0} hr";
        return $"{hours / 40m:0.0} wk";
    }

    // JS Y-axis formatter, mirroring FormatWorkTime for the chart ticks (renders browser-side).
    public static string AxisFormatter(PriceLens lens) => lens == PriceLens.TimePrice
        ? "function (val) { var m = val * 60; if (m < 60) return m.toFixed(0) + ' min'; if (val < 40) return val.toFixed(1) + ' hr'; return (val / 40).toFixed(1) + ' wk'; }"
        : "function (val) { return '$' + val.toFixed(2); }";
}
