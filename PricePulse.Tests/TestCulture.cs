using System.Globalization;
using System.Runtime.CompilerServices;

namespace PricePulse.Tests;

// Program.cs pins en-US for the app, but it doesn't run under the test host — pin the same culture
// once here so currency/number assertions stay deterministic on any machine.
internal static class TestCulture
{
    [ModuleInitializer]
    public static void Init()
    {
        var enUs = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = enUs;
        CultureInfo.DefaultThreadCurrentUICulture = enUs;
    }
}
