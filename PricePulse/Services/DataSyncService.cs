namespace PricePulse.Services;

// Loads the price cache on startup, then refreshes it on a timer. The first pass starts immediately
// but nothing waits on it, so the app serves (with a loading state) before FRED has answered.
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

            try
            {
                await Task.Delay(RefreshInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
