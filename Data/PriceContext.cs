using Microsoft.EntityFrameworkCore;
using PricePulse.Models;

namespace PricePulse.Data;

public class PriceContext : DbContext
{
    public PriceContext(DbContextOptions<PriceContext> options) : base(options)
    {
    }

    public DbSet<Series> Series => Set<Series>();
    public DbSet<Observation> Observations => Set<Observation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Observation>()
            .HasIndex(o => new { o.SeriesId, o.Date })
            .IsUnique();

        modelBuilder.Entity<Series>().HasData(
            // Groceries — dollar prices
            new Series { Id = "APU0000708111", Name = "Eggs", Units = "per dozen", Icon = "egg", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000709112", Name = "Milk", Units = "per gallon", Icon = "lucide:milk", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000702111", Name = "Bread", Units = "per pound", Icon = "bakery_dining", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000703112", Name = "Ground Beef", Units = "per pound", Icon = "lunch_dining", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000706111", Name = "Chicken", Units = "per pound", Icon = "lucide:drumstick", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000717311", Name = "Coffee", Units = "per pound", Icon = "coffee", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000711211", Name = "Bananas", Units = "per pound", Icon = "lucide:banana", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000701312", Name = "Rice", Units = "per pound", Icon = "rice_bowl", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },
            new Series { Id = "APU0000712311", Name = "Tomatoes", Units = "per pound", Icon = "eco", Category = SeriesCategory.Groceries, Kind = SeriesKind.Price },

            // Energy — dollar prices
            new Series { Id = "APU000074714", Name = "Gasoline", Units = "per gallon", Icon = "local_gas_station", Category = SeriesCategory.Energy, Kind = SeriesKind.Price },
            new Series { Id = "APU000072610", Name = "Electricity", Units = "per kWh", Icon = "bolt", Category = SeriesCategory.Energy, Kind = SeriesKind.Price },

            // Housing
            new Series { Id = "MSPUS", Name = "Median Home Price", Units = "U.S. median · MSPUS", Icon = "home", Category = SeriesCategory.Housing, Kind = SeriesKind.Price },
            new Series { Id = "CUUR0000SEHA", Name = "Rent", Units = "price index", Icon = "apartment", Category = SeriesCategory.Housing, Kind = SeriesKind.PriceIndex },

            // Education
            new Series { Id = "CUUR0000SEEB", Name = "Tuition", Units = "price index", Icon = "school", Category = SeriesCategory.Education, Kind = SeriesKind.PriceIndex },

            // Healthcare
            new Series { Id = "CPIMEDNS", Name = "Medical Care", Units = "price index", Icon = "medical_services", Category = SeriesCategory.Healthcare, Kind = SeriesKind.PriceIndex },

            // Consumer goods
            new Series { Id = "CPIAPPNS", Name = "Clothing", Units = "price index", Icon = "checkroom", Category = SeriesCategory.ConsumerGoods, Kind = SeriesKind.PriceIndex },
            new Series { Id = "CUUR0000SERE01", Name = "Toys", Units = "price index", Icon = "toys", Category = SeriesCategory.ConsumerGoods, Kind = SeriesKind.PriceIndex },
            new Series { Id = "CUUR0000SEEE", Name = "Electronics", Units = "price index", Icon = "devices", Category = SeriesCategory.ConsumerGoods, Kind = SeriesKind.PriceIndex },

            // Wages — dollar wage (promoted from reference so it gets its own page; still powers work-time)
            new Series { Id = "CEU0500000008", Name = "Non-Executive Wage", Units = "per hour", Icon = "payments", Category = SeriesCategory.Wages, Kind = SeriesKind.Wage },

            // Productivity — real index
            new Series { Id = "OPHNFB", Name = "Productivity", Units = "output per hour", Icon = "speed", Category = SeriesCategory.Productivity, Kind = SeriesKind.Indicator },

            // Reference only — hidden from the catalog, powers the inflation lens
            new Series { Id = "CPIAUCNS", Name = "Consumer Price Index", Units = "price index", Icon = "trending_up", Category = SeriesCategory.Housing, Kind = SeriesKind.PriceIndex, IsReference = true });
    }
}
