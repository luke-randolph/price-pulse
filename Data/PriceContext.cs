using Microsoft.EntityFrameworkCore;

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
            new Series { Id = "APU0000708111", Name = "Eggs", Units = "per dozen", Icon = "egg" },
            new Series { Id = "APU0000709112", Name = "Milk", Units = "per gallon", Icon = "svg:milk" },
            new Series { Id = "APU000074714", Name = "Gasoline", Units = "per gallon", Icon = "local_gas_station" },
            new Series { Id = "APU000072610", Name = "Electricity", Units = "per kWh", Icon = "bolt" },
            new Series { Id = "APU0000702111", Name = "Bread", Units = "per pound", Icon = "bakery_dining" },
            new Series { Id = "APU0000703112", Name = "Ground Beef", Units = "per pound", Icon = "lunch_dining" },
            new Series { Id = "CPIAUCNS", Name = "Consumer Price Index", Units = "index (1982-84=100)", Icon = "trending_up", IsReference = true },
            new Series { Id = "CEU0500000008", Name = "Average Hourly Earnings", Units = "per hour", Icon = "payments", IsReference = true });
    }
}
