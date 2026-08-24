using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TicketToMentionConverter.Services;

public class TicketToMentionService(
    IOptionsMonitor<MentionSettings> mention,
    IOptionsMonitor<FolderSettings> folders,
    IOptionsMonitor<ProcessingSettings> processing,
    ILogger<TicketToMentionService> logger)
    : BackgroundService
{
    private readonly SemaphoreSlim executionLock = new(1, 1);
    private CancellationTokenSource delayCts = new();

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        FolderSettings current = folders.CurrentValue;
        logger.LogInformation("Ordner: Input={Input}, Output={Output}, Backup={Backup}",
            current.Input, current.Output, current.Backup);

        FolderProcessor.EnsureDirectories(current);

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
                    FolderProcessor.ProcessOnce(folders.CurrentValue, mention.CurrentValue, logger);
                }
                catch (Exception ex)
                {
                    // Als Dienst gibt es keine Konsole: Fehler muessen ins Event Log.
                    logger.LogError(ex, "Verarbeitung fehlgeschlagen");
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