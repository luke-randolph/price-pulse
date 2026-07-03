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
            new Series { Id = "APU0000708111", Name = "Eggs", Units = "per dozen" },
            new Series { Id = "APU0000709112", Name = "Milk", Units = "per gallon" },
            new Series { Id = "APU000074714", Name = "Gasoline", Units = "per gallon" },
            new Series { Id = "APU000072610", Name = "Electricity", Units = "per kWh" },
            new Series { Id = "APU0000702111", Name = "Bread", Units = "per pound" },
            new Series { Id = "APU0000703112", Name = "Ground Beef", Units = "per pound" });
    }
}
