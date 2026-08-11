namespace PricePulse.Models;

public enum SeriesCategory
{
    Groceries,
    Energy,
    Housing,
    Education,
    Healthcare,
    ConsumerGoods,
    Wages,
    Productivity
}

public enum SeriesKind
{
    Price,
    PriceIndex,  // CPI component index (1982-84=100): nominal, real — no work-time (not a dollar)
    Wage,
    Indicator    // real index like productivity: nominal only — inflation-adjusting it is meaningless
}

public class Series
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Units { get; set; }
    public required string Icon { get; set; }
    public SeriesCategory Category { get; set; }
    public SeriesKind Kind { get; set; }
}

public static class SeriesCategories
{
    public static string Label(this SeriesCategory category) => category switch
    {
        SeriesCategory.ConsumerGoods => "Consumer Goods",
        _ => category.ToString()
    };
}
