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
})
.AddStandardResilienceHandler();

// Price data is a small, read-mostly mirror of FRED, so it lives in memory rather than a database:
// DataSyncService runs FredDataLoader on startup and on a timer; each pass replaces the PriceStore
// snapshot. FRED is the source of truth, so there is nothing to persist.
builder.Services.AddSingleton<PriceStore>();
builder.Services.AddScoped<FredDataLoader>();
builder.Services.AddScoped<PriceService>();
builder.Services.AddHostedService<DataSyncService>();

builder.Services.AddHealthChecks()
    .AddCheck<PriceCacheHealthCheck>("price-cache");

builder.Services.AddRadzenComponents();

builder.Services.AddApexCharts();

var app = builder.Build();

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

app.MapHealthChecks("/health");

app.Run();
