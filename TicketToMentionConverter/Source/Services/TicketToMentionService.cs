using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TicketToMentionConverter.Services;

public class TicketToMentionService(
    IOptionsMonitor<MentionSettings> mention,
    IOptionsMonitor<FolderSettings> folders,
    IOptionsMonitor<ProcessingSettings> processing)
    : BackgroundService
{
    private readonly SemaphoreSlim executionLock = new(1, 1);
    private CancellationTokenSource delayCts = new();

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        FolderProcessor.EnsureDirectories(folders.CurrentValue);

        folders.OnChange(FolderProcessor.EnsureDirectories);
        
        processing.OnChange(_ =>
        {
            delayCts.Cancel();
        });

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await executionLock.WaitAsync(0, stoppingToken))
            {
                try
                {
                    FolderProcessor.ProcessOnce(folders.CurrentValue, mention.CurrentValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    executionLock.Release();
                }
            }

            int seconds = Math.Max(1, processing.CurrentValue.ScanIntervalSeconds);

            delayCts.Dispose();
            delayCts = new CancellationTokenSource();

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, delayCts.Token);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}