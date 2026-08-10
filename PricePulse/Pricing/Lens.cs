using System.Globalization;
using System.Text.Json;
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
    public const decimal HoursPerWorkYear = 2080m;       // 40 hr × 52 wk
    private const decimal MinutesPerHour = 60m;
    private const decimal WholeDollarThreshold = 1000m;
    private const string Locale = "en-US";

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

    public static decimal? PercentChangeOverYears(IReadOnlyList<LensPoint> points, int years)
    {
        if (points.Count == 0)
        {
            return null;
        }

        var latest = points[^1];
        var then = points.LastOrDefault(p => p.Date <= latest.Date.AddYears(-years));

        return then is null || then.Value == 0
            ? null
            : (latest.Value - then.Value) / then.Value * 100m;
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
        Math.Abs(value) >= WholeDollarThreshold ? value.ToString("C0") : value.ToString("C2");

    // Natural unit for hours of work: minutes / hours / weeks / years.
    public static string FormatWorkTime(decimal hours)
    {
        var minutes = hours * MinutesPerHour;
        if (minutes < MinutesPerHour) return $"{minutes:0} min";
        if (hours < HoursPerWorkWeek) return $"{hours:0.0} hr";
        if (hours < HoursPerWorkYear) return $"{hours / HoursPerWorkWeek:0.0} wk";
        return $"{hours / HoursPerWorkYear:0.0} yr";
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

    // ApexCharts labels ticks and tooltips browser-side, so these emit the JS equivalents of
    // FormatWorkTime / FormatDollars / FormatValue from the same constants.
    public static string AxisFormatter(PriceLens lens, SeriesKind kind) =>
        JsFormatter(lens, kind, suffix: "");

    // The "per X" suffix makes a hovered point read "$4.99 per dozen". Other units get none.
    public static string TooltipFormatter(PriceLens lens, SeriesKind kind, string units) =>
        JsFormatter(lens, kind, IsPerUnit(units) ? " " + units : "");

    private static string JsFormatter(PriceLens lens, SeriesKind kind, string suffix)
    {
        var result = suffix.Length == 0 ? "t" : $"t + {JsString(suffix)}";
        return $"function (val) {{ {JsValue(lens, kind)} return {result}; }}";
    }

    private static string JsValue(PriceLens lens, SeriesKind kind) => lens switch
    {
        PriceLens.TimePrice =>
            $"var m = val * {Js(MinutesPerHour)}; " +
            $"var t = m < {Js(MinutesPerHour)} ? m.toFixed(0) + ' min' " +
            $": val < {Js(HoursPerWorkWeek)} ? val.toFixed(1) + ' hr' " +
            $": val < {Js(HoursPerWorkYear)} ? (val / {Js(HoursPerWorkWeek)}).toFixed(1) + ' wk' " +
            $": (val / {Js(HoursPerWorkYear)}).toFixed(1) + ' yr';",
        _ when IsDollar(kind) =>
            $"var t = val >= {Js(WholeDollarThreshold)} " +
            $"? '$' + Math.round(val).toLocaleString('{Locale}') : '$' + val.toFixed(2);",
        _ =>
            $"var t = val.toLocaleString('{Locale}', {{ minimumFractionDigits: 1, maximumFractionDigits: 1 }});"
    };

    private static string Js(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    private static string JsString(string value) => JsonSerializer.Serialize(value);
}
