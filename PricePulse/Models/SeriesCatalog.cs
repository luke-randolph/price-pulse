using PricePulse.Pricing;

namespace PricePulse.Models;

public static class SeriesCatalog
{
    public static readonly IReadOnlyList<Series> All = new List<Series>
    {
        // Groceries — dollar prices
        new() { Id = "APU0000708111", Name = "Eggs", Units = "per dozen", Icon = "egg", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000709112", Name = "Milk", Units = "per gallon", Icon = "lucide:milk", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000702111", Name = "Bread", Units = "per pound", Icon = "bakery_dining", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000703112", Name = "Ground Beef", Units = "per pound", Icon = "lunch_dining", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000706111", Name = "Chicken", Units = "per pound", Icon = "lucide:drumstick", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000717311", Name = "Coffee", Units = "per pound", Icon = "coffee", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000711211", Name = "Bananas", Units = "per pound", Icon = "lucide:banana", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000701312", Name = "Rice", Units = "per pound", Icon = "rice_bowl", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
        new() { Id = "APU0000712311", Name = "Tomatoes", Units = "per pound", Icon = "eco", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },

        // Energy — dollar prices
        new() { Id = "APU000074714", Name = "Gasoline", Units = "per gallon", Icon = "local_gas_station", Category = SeriesCategory.Energy, Kind = SeriesKind.Price },
        new() { Id = "APU000072610", Name = "Electricity", Units = "per kWh", Icon = "bolt", Category = SeriesCategory.Energy, Kind = SeriesKind.Price },

        // Housing
        new() { Id = "MSPUS", Name = "Median Home Price", Units = "U.S. median · MSPUS", Icon = "home", Category = SeriesCategory.Housing, Kind = SeriesKind.Price },
        new() { Id = "CUUR0000SEHA", Name = "Rent", Units = "price index", Icon = "apartment", Category = SeriesCategory.Housing, Kind = SeriesKind.PriceIndex },

        // Education
        new() { Id = "CUUR0000SEEB", Name = "Tuition", Units = "price index", Icon = "school", Category = SeriesCategory.Education, Kind = SeriesKind.PriceIndex },

        // Healthcare
        new() { Id = "CPIMEDNS", Name = "Medical Care", Units = "price index", Icon = "medical_services", Category = SeriesCategory.Healthcare, Kind = SeriesKind.PriceIndex },

        // Consumer goods
        new() { Id = "CPIAPPNS", Name = "Clothing", Units = "price index", Icon = "checkroom", Category = SeriesCategory.ConsumerGoods, Kind = SeriesKind.PriceIndex },
        new() { Id = "CUUR0000SERE01", Name = "Toys", Units = "price index", Icon = "toys", Category = SeriesCategory.ConsumerGoods, Kind = SeriesKind.PriceIndex },
        new() { Id = "CUUR0000SEEE", Name = "Electronics", Units = "price index", Icon = "devices", Category = SeriesCategory.ConsumerGoods, Kind = SeriesKind.PriceIndex },

        // Wages — dollar wage (promoted from reference so it gets its own page; still powers work-time)
        new() { Id = "CEU0500000008", Name = "Non-Executive Wage", Units = "per hour", Icon = "payments", Category = SeriesCategory.Wages, Kind = SeriesKind.Wage },

        // Productivity — real index
        new() { Id = "OPHNFB", Name = "Productivity", Units = "output per hour", Icon = "speed", Category = SeriesCategory.Productivity, Kind = SeriesKind.Indicator },

    };

    // Powers the inflation lens but has no page of its own, so it carries no display metadata — the
    // loader only needs the ID. The wage series is not here: it backs the work-time lens and is browsable.
    private static readonly IReadOnlyList<string> ReferenceIds = new[] { Lens.CpiSeriesId };

    public static readonly IReadOnlyList<string> FetchIds =
        All.Select(s => s.Id).Concat(ReferenceIds).ToList();
}
