# Price Pulse

[![CI](https://github.com/luke-randolph/price-pulse/actions/workflows/ci.yml/badge.svg)](https://github.com/luke-randolph/price-pulse/actions/workflows/ci.yml)

A .NET 9 Blazor web app that tracks U.S. consumer prices — groceries, energy, housing, healthcare, and more — using official data from [FRED](https://fred.stlouisfed.org/) (the St. Louis Fed's economic data service). Beyond the raw numbers, it reframes prices through three **lenses** so the data actually means something: the nominal price, today's dollars, and hours of work.

> _Add a screenshot or two here — the dashboard and a series detail page make a strong first impression._
>
> `![Dashboard](docs/dashboard.png)`

## What it does

- **Dashboard** — every tracked item as a card grouped by category, each showing the latest price, year-over-year change, and a 10-year sparkline.
- **Series detail pages** — full price history with selectable ranges (past year / 10 / 25 / 50 years) and a "then → now" summary.
- **Three lenses** for viewing any price:
  - **Nominal** — the price as reported.
  - **Inflation-adjusted** — past prices restated in today's dollars using CPI, so amounts compare like-for-like.
  - **Work-time** — how long you'd work to afford it at average non-executive wages. Enter your own pay to personalize it.

## Tech stack

- **[.NET 9](https://dotnet.microsoft.com/) / Blazor Server** (Interactive Server rendering)
- **[Radzen.Blazor](https://blazor.radzen.com/)** — UI components and icons
- **[Blazor-ApexCharts](https://apexcharts.github.io/Blazor-ApexCharts/)** — charts and sparklines
- **[FRED API](https://fred.stlouisfed.org/docs/api/fred/)** — price data source

## Architecture

The price data is a small, read-mostly mirror of FRED (~20 series, a few MB), and FRED is the source of truth — so there's no database. The data lives **in memory**:

- **`FredClient`** — a typed `HttpClient` that fetches series observations from FRED.
- **`FredDataLoader`** — fetches every catalog series (bounded concurrency) and publishes a fresh snapshot to the store. A series that fails to fetch keeps its previous data, so a transient FRED hiccup never blanks the cache.
- **`PriceStore`** — a singleton holding the current snapshot. New snapshots are swapped in atomically, so reads are lock-free and never see a torn map.
- **`DataSyncService`** — a background service that refreshes the store every 12 hours.
- **`PriceService`** — the read-side API the pages use; every read is served synchronously from memory.

The cache is **warmed at startup before the app accepts traffic**, so the first request is already fast. If the FRED key is missing or FRED is unavailable, the app starts anyway with an empty cache and the background service retries — startup is never blocked by an upstream outage.

`PricePulse/Pricing/Lens.cs` holds the pure functions behind the lenses (CPI real-dollar adjustment, work-time conversion, and display formatting) — which is also where the test suite is focused.

## Running locally

Requires the [.NET 9 SDK](https://dotnet.microsoft.com/download) and a free [FRED API key](https://fredaccount.stlouisfed.org/apikeys).

```bash
# Provide your FRED API key (stored outside source control via user-secrets)
dotnet user-secrets set "Fred:ApiKey" "<your-key>" --project PricePulse

# Run
dotnet run --project PricePulse
```

Then open the URL shown in the console. On startup the app fetches the full history for each series into memory (a few seconds), after which every page serves instantly.

## Tests

```bash
dotnet test
```

`PricePulse.Tests` covers the pricing logic in `PricePulse/Pricing/Lens.cs` — the CPI real-dollar adjustment, work-time conversion, month-matching of reference series, and display formatting. The same suite runs on every push and pull request via GitHub Actions (see the badge above).

## Configuration

| Setting | How to set it | Notes |
| --- | --- | --- |
| `Fred:ApiKey` | `dotnet user-secrets set "Fred:ApiKey" "<key>" --project PricePulse` (dev) or an environment variable / `Fred__ApiKey` (prod) | Required. The app will start without it but the cache stays empty until it's provided. |

## Data source

All price data comes from [FRED](https://fred.stlouisfed.org/), maintained by the Federal Reserve Bank of St. Louis, drawing primarily on the U.S. Bureau of Labor Statistics. Each series links back to its FRED page.
