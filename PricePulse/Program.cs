using System.Globalization;
using PricePulse.Components;
using PricePulse.Services;
using PricePulse.Fred;
using ApexCharts;
using Radzen;

// Every figure in this app is U.S. dollars / U.S. number formatting, so pin en-US once here.
// Otherwise formatting inherits the host's culture, which is the invariant culture inside a Linux
var enUs = CultureInfo.GetCultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = enUs;
CultureInfo.DefaultThreadCurrentUICulture = enUs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<FredClient>(client =>
{
    client.BaseAddress = new Uri("https://api.stlouisfed.org/");
});

// Price data is a small, read-mostly mirror of FRED, so it lives in memory rather than a database:
// FredDataLoader warms PriceStore at startup and DataSyncService refreshes it on a timer. FRED is
// the source of truth, so there is nothing to persist.
builder.Services.AddSingleton<PriceStore>();
builder.Services.AddScoped<FredDataLoader>();
builder.Services.AddScoped<PriceService>();
builder.Services.AddHostedService<DataSyncService>();

builder.Services.AddRadzenComponents();

builder.Services.AddApexCharts();

var app = builder.Build();

// Warm the cache before serving so the first request is instant. Best-effort: a missing API key or
// a FRED outage must not stop the app from starting — the background refresher will retry.
using (var scope = app.Services.CreateScope())
{
    var loader = scope.ServiceProvider.GetRequiredService<FredDataLoader>();
    try
    {
        var total = await loader.RefreshAsync();
        app.Logger.LogInformation("Price cache warmed: {Total} observations.", total);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Initial price load failed; starting with an empty cache.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
