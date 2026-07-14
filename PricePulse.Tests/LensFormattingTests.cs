using System.Globalization;
using PricePulse.Models;
using PricePulse.Pricing;

namespace PricePulse.Tests;

// Formatting uses the current culture (currency symbol, decimal/grouping separators), so pin en-US
// to keep assertions deterministic across machines and CI runners. The original culture is restored
// after each test so this pinning can't leak into other test classes on the same thread.
public class LensFormattingTests : IDisposable
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;

    public LensFormattingTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
    }

    [Theory]
    [InlineData(4.99, "$4.99")]     // under $1,000 keeps cents
    [InlineData(1000, "$1,000")]    // $1,000 and up drop cents
    [InlineData(1200.5, "$1,201")]
    public void FormatDollars_showsCentsOnlyBelowAThousand(decimal value, string expected)
    {
        Assert.Equal(expected, Lens.FormatDollars(value));
    }

    [Theory]
    [InlineData(0.5, "30 min")]     // under an hour → minutes
    [InlineData(1, "1.0 hr")]       // 60 min boundary → hours
    [InlineData(2.5, "2.5 hr")]
    [InlineData(40, "1.0 wk")]      // exactly one work-week boundary → weeks
    [InlineData(80, "2.0 wk")]
    [InlineData(2080, "1.0 yr")]    // exactly one work-year boundary → years
    [InlineData(4160, "2.0 yr")]
    public void FormatWorkTime_picksTheNaturalUnit(decimal hours, string expected)
    {
        Assert.Equal(expected, Lens.FormatWorkTime(hours));
    }

    [Fact]
    public void FormatValue_dollarKind_formatsAsCurrency()
    {
        Assert.Equal("$2.50", Lens.FormatValue(2.50m, PriceLens.Nominal, SeriesKind.Price));
    }

    [Fact]
    public void FormatValue_indexKind_formatsAsPlainNumber()
    {
        Assert.Equal("150.5", Lens.FormatValue(150.5m, PriceLens.Nominal, SeriesKind.PriceIndex));
    }

    [Fact]
    public void FormatValue_timePriceLens_formatsAsWorkTime()
    {
        Assert.Equal("4.0 hr", Lens.FormatValue(4m, PriceLens.TimePrice, SeriesKind.Price));
    }
}
