using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrueCodeTest.CurrencyFetcher.Cbr;
using TrueCodeTest.CurrencyFetcher.Options;
using TrueCodeTest.Finance.Infrastructure.Persistence;

namespace TrueCodeTest.CurrencyFetcher;

public sealed class CurrencyFetchWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<CurrencyFetcherOptions> options,
    ILogger<CurrencyFetchWorker> logger)
    : BackgroundService
{
    private readonly CurrencyFetcherOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_options.StartupDelaySeconds > 0)
        {
            try { await Task.Delay(TimeSpan.FromSeconds(_options.StartupDelaySeconds), stoppingToken); }
            catch (TaskCanceledException) { return; }
        }

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.IntervalSeconds));
        do
        {
            try
            {
                await ExecuteIterationAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Currency fetch iteration failed");
            }
        } while (await WaitAsync(timer, stoppingToken));
    }

    private static async Task<bool> WaitAsync(PeriodicTimer timer, CancellationToken ct)
    {
        try
        {
            return await timer.WaitForNextTickAsync(ct);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private async Task ExecuteIterationAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<CbrClient>();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        var fetched = await client.FetchAsync(ct);
        if (fetched.Count == 0)
            return;

        var existingIds = await db.Currencies
            .Select(c => c.Id)
            .ToListAsync(ct);
        var existingSet = new HashSet<int>(existingIds);

        var added = 0;
        var updated = 0;
        foreach (var incoming in fetched)
        {
            if (existingSet.Contains(incoming.Id))
            {
                var tracked = await db.Currencies.FirstAsync(c => c.Id == incoming.Id, ct);
                tracked.CharCode = incoming.CharCode;
                tracked.Name = incoming.Name;
                tracked.Rate = incoming.Rate;
                tracked.Nominal = incoming.Nominal;
                tracked.UpdatedAt = incoming.UpdatedAt;
                updated++;
            }
            else
            {
                await db.Currencies.AddAsync(incoming, ct);
                added++;
            }
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Currency refresh complete: {Added} added, {Updated} updated", added, updated);
    }
}
