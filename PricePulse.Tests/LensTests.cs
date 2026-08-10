using PricePulse.Models;
using PricePulse.Pricing;

namespace PricePulse.Tests;

public class LensTests
{
    private static Observation Obs(int year, int month, decimal? value) =>
        new() { Date = new DateOnly(year, month, 1), Value = value };

    // --- Nominal projection ---

    [Fact]
    public void Apply_Nominal_returnsValuesAsIsAndDropsNulls()
    {
        var nominal = new[] { Obs(2020, 1, 10m), Obs(2020, 2, null), Obs(2020, 3, 12m) };

        var result = Lens.Apply(PriceLens.Nominal, nominal, reference: null);

        Assert.Equal(2, result.Count);
        Assert.Equal(new DateTime(2020, 1, 1), result[0].Date);
        Assert.Equal(10m, result[0].Value);
        Assert.Equal(12m, result[1].Value);
    }

    [Fact]
    public void Apply_derivedLensWithNullReference_fallsBackToNominal()
    {
        var nominal = new[] { Obs(2020, 1, 10m) };

        var result = Lens.Apply(PriceLens.RealDollars, nominal, reference: null);

        Assert.Equal(10m, Assert.Single(result).Value);
    }

    // --- Real-dollars projection ---

    [Fact]
    public void Apply_RealDollars_restatesPastValuesInTodaysDollars()
    {
        var nominal = new[] { Obs(2020, 1, 100m), Obs(2021, 1, 110m) };
        var cpi = new[] { Obs(2020, 1, 200m), Obs(2021, 1, 220m) }; // base = latest = 220

        var result = Lens.Apply(PriceLens.RealDollars, nominal, cpi);

        // 2020: 100 * 220/200 = 110; 2021: 110 * 220/220 = 110
        Assert.Equal(110m, result[0].Value);
        Assert.Equal(110m, result[1].Value);
    }

    [Fact]
    public void Apply_RealDollars_dropsMonthsWithoutAReferenceValue()
    {
        var nominal = new[] { Obs(2020, 1, 100m), Obs(2022, 6, 130m) };
        var cpi = new[] { Obs(2020, 1, 200m), Obs(2021, 1, 220m) }; // no 2022-06

        var result = Lens.Apply(PriceLens.RealDollars, nominal, cpi);

        Assert.Equal(new DateTime(2020, 1, 1), Assert.Single(result).Date);
    }

    [Fact]
    public void Apply_matchesReferenceByMonthRegardlessOfDay()
    {
        var nominal = new[] { new Observation { Date = new DateOnly(2020, 1, 15), Value = 100m } };
        var cpi = new[] { new Observation { Date = new DateOnly(2020, 1, 1), Value = 100m } };

        var result = Lens.Apply(PriceLens.RealDollars, nominal, cpi);

        Assert.Single(result); // day-of-month differs but the month lines up
    }

    // --- Time-price projection ---

    [Fact]
    public void Apply_TimePrice_dividesPriceByWage()
    {
        var nominal = new[] { Obs(2020, 1, 100m) };
        var wage = new[] { Obs(2020, 1, 25m) };

        var result = Lens.Apply(PriceLens.TimePrice, nominal, wage);

        Assert.Equal(4m, Assert.Single(result).Value); // 100 / 25 = 4 hours
    }

    [Fact]
    public void Apply_ignoresReferenceMonthsWithZeroValue()
    {
        var nominal = new[] { Obs(2020, 1, 100m) };
        var wage = new[] { Obs(2020, 1, 0m) }; // zero would divide-by-zero, so it's excluded

        var result = Lens.Apply(PriceLens.TimePrice, nominal, wage);

        Assert.Empty(result);
    }

    // --- Point-in-time helpers ---

    [Theory]
    [InlineData(100, 25, 4)]
    [InlineData(50, 20, 2.5)]
    [InlineData(100, 0, 0)] // guards against divide-by-zero
    public void TimePriceAt_convertsPriceToHoursOfWork(decimal price, decimal wage, decimal expected)
    {
        Assert.Equal(expected, Lens.TimePriceAt(price, wage));
    }

    [Fact]
    public void BaseMonth_isTheLatestNonZeroReferenceMonth()
    {
        var cpi = new[] { Obs(2020, 1, 200m), Obs(2021, 1, 220m), Obs(2021, 2, null) };

        Assert.Equal(new DateTime(2021, 1, 1), Lens.BaseMonth(cpi));
    }

    // --- Percent change ---

    private static LensPoint Pt(int year, decimal value) => new(new DateTime(year, 1, 1), value);

    [Fact]
    public void PercentChangeOverYears_measuresFromTheStartOfTheWindow()
    {
        var pts = new[] { Pt(2015, 100m), Pt(2020, 150m), Pt(2025, 200m) };

        Assert.Equal(100m, Lens.PercentChangeOverYears(pts, 10));
        Assert.Equal(33.33m, Math.Round(Lens.PercentChangeOverYears(pts, 5)!.Value, 2));
    }

    [Fact]
    public void PercentChangeOverYears_picksTheNewestPointAtOrBeforeTheCutoff()
    {
        var pts = new[] { Pt(2010, 50m), Pt(2014, 100m), Pt(2015, 120m), Pt(2025, 240m) };

        Assert.Equal(100m, Lens.PercentChangeOverYears(pts, 10)); // against 2015, not 2014
    }

    [Fact]
    public void PercentChangeOverYears_isNullWhenTheChangeCannotBeComputed()
    {
        Assert.Null(Lens.PercentChangeOverYears(Array.Empty<LensPoint>(), 1));
        Assert.Null(Lens.PercentChangeOverYears(new[] { Pt(2020, 100m), Pt(2025, 200m) }, 25));
        Assert.Null(Lens.PercentChangeOverYears(new[] { Pt(2015, 0m), Pt(2025, 200m) }, 10));
    }
}
