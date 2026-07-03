using Microsoft.EntityFrameworkCore;
using PricePulse.Components;
using PricePulse.Data;
using PricePulse.Fred;
using ApexCharts;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<FredClient>(client =>
{
    client.BaseAddress = new Uri("https://api.stlouisfed.org/");
});

builder.Services.AddDbContext<PriceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PriceDb")
        ?? "Data Source=pricepulse.db"));

builder.Services.AddScoped<PriceService>();

builder.Services.AddRadzenComponents();

builder.Services.AddApexCharts();

var app = builder.Build();

// Apply any pending migrations and create the database file on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PriceContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
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
