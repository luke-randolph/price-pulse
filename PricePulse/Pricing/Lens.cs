using PricePulse.Models;

namespace PricePulse.Pricing;

public enum PriceLens
{
    Nominal,      // raw value, as reported
    RealDollars,  // inflation-adjusted to the latest month ("today's dollars")
    TimePrice     // price ÷ average hourly wage = hours of work
}

public record LensPoint(DateTime Date, decimal Value);

public static class Lens
{
    public const string CpiSeriesId = "CPIAUCNS";        // CPI-U, not seasonally adjusted
    public const string WageSeriesId = "CEU0500000008";  // production & nonsupervisory wage, NSA, back to 1964

    private const decimal HoursPerWorkWeek = 40m;
    private const decimal HoursPerWorkYear = 2080m;      // 40 hr × 52 wk

    public static IReadOnlyList<PriceLens> LensesFor(SeriesKind kind) => kind switch
    {
        SeriesKind.Price => new[] { PriceLens.Nominal, PriceLens.RealDollars, PriceLens.TimePrice },
        SeriesKind.PriceIndex => new[] { PriceLens.Nominal, PriceLens.RealDollars },
        SeriesKind.Wage => new[] { PriceLens.Nominal, PriceLens.RealDollars },
        SeriesKind.Indicator => new[] { PriceLens.Nominal },
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };

    // Dollar-valued kinds format as currency; index kinds as a plain number.
    public static bool IsDollar(SeriesKind kind) => kind is SeriesKind.Price or SeriesKind.Wage;

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

    // Formats a lens value for display, honouring the series' unit kind.
    public static string FormatValue(decimal value, PriceLens lens, SeriesKind kind) => lens switch
    {
        PriceLens.TimePrice => FormatWorkTime(value),
        _ when IsDollar(kind) => FormatDollars(value),
        _ => value.ToString("N1")  // index number
    };

    // Big-ticket dollars (homes, wages × year) read better without cents.
    public static string FormatDollars(decimal value) =>
        Math.Abs(value) >= 1000m ? value.ToString("C0") : value.ToString("C2");

    // Natural unit for hours of work: minutes / hours / weeks / years.
    public static string FormatWorkTime(decimal hours)
    {
        var minutes = hours * 60m;
        if (minutes < 60m) return $"{minutes:0} min";
        if (hours < HoursPerWorkWeek) return $"{hours:0.0} hr";
        if (hours < HoursPerWorkYear) return $"{hours / HoursPerWorkWeek:0.0} wk";
        return $"{hours / HoursPerWorkYear:0.0} yr";
    }

    // JS Y-axis formatter, mirroring the C# formatters for the chart ticks (renders browser-side).
    public static string AxisFormatter(PriceLens lens, SeriesKind kind)
    {
        if (lens == PriceLens.TimePrice)
        {
            return "function (val) { var m = val * 60; if (m < 60) return m.toFixed(0) + ' min'; if (val < 40) return val.toFixed(1) + ' hr'; if (val < 2080) return (val / 40).toFixed(1) + ' wk'; return (val / 2080).toFixed(1) + ' yr'; }";
        }

        return IsDollar(kind)
            ? "function (val) { return val >= 1000 ? '$' + Math.round(val).toLocaleString() : '$' + val.toFixed(2); }"
            : "function (val) { return val.toFixed(1); }";
    }

    // A "per X" unit ("per dozen", "per gallon") is a clean denominator we can fold into labels.
    // Descriptor units ("price index", "U.S. median · MSPUS") are not, so they stay out of the axis.
    private static bool IsPerUnit(string units) => units.StartsWith("per ", StringComparison.OrdinalIgnoreCase);

    // Y-axis title for the active lens: dollars/index/work-time, with the "per X" unit folded in when
    // it reads cleanly (e.g. "$ per dozen"). Non-"per" units fall back to the plain measure.
    public static string AxisTitle(PriceLens lens, SeriesKind kind, string units)
    {
        if (lens == PriceLens.TimePrice) return "Work time";
        if (IsDollar(kind))
        {
            return IsPerUnit(units) ? $"$ {units}"
                : lens == PriceLens.RealDollars ? "Today's dollars" : "Dollars";
        }
        return "Index";
    }

    // JS tooltip formatter: same value formatting as the axis ticks, plus the "per X" unit suffix so a
    // hovered point reads e.g. "$4.99 per dozen" or "12 min per dozen". Index series get no suffix.
    public static string TooltipFormatter(PriceLens lens, SeriesKind kind, string units)
    {
        var suffix = IsPerUnit(units) ? " " + units : "";
        var value = lens == PriceLens.TimePrice
            ? "var m = val * 60; var t = m < 60 ? m.toFixed(0) + ' min' : val < 40 ? val.toFixed(1) + ' hr' : val < 2080 ? (val / 40).toFixed(1) + ' wk' : (val / 2080).toFixed(1) + ' yr';"
            : IsDollar(kind)
                ? "var t = val >= 1000 ? '$' + Math.round(val).toLocaleString() : '$' + val.toFixed(2);"
                : "var t = val.toFixed(1);";
        return "function (val) { " + value + " return t + '" + suffix + "'; }";
    }
}
