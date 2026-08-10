using PricePulse.Pricing;

namespace PricePulse.Components;

public static class ChartTheme
{
    public const string Primary = "#4f46e5"; // --indigo
    public const string Blue = "#3949ab";    // --blue
    public const string Purple = "#7c3aed";  // --purple

    public static string ForLens(PriceLens lens) => lens switch
    {
        PriceLens.RealDollars => Blue,
        PriceLens.TimePrice => Purple,
        _ => Primary
    };
}
