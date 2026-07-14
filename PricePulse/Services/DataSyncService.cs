namespace PricePulse.Services;

// Keeps the price cache fresh. The cache is warmed synchronously at startup (Program.cs), so this
// waits a full interval before its first pass, then picks up each month's new FRED data.
public class DataSyncService : BackgroundService
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(12);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(IServiceScopeFactory scopeFactory, ILogger<DataSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(RefreshInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var loader = scope.ServiceProvider.GetRequiredService<FredDataLoader>();
                var total = await loader.RefreshAsync(stoppingToken);
                _logger.LogInformation("Price cache refreshed: {Total} observations.", total);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Price cache refresh failed.");
            }
        }
    }
}
