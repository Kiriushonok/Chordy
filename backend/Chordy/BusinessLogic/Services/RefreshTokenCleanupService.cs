using Chordy.DataAccess.Repositories.Interfaces;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RefreshTokenCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                await repo.DeleteOldTokensAsync(stoppingToken);
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // раз в сутки
        }
    }
}