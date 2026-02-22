using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TicketToMentionConverter.Services;

public class TicketToMentionService(
    IOptionsMonitor<MentionSettings> mention,
    IOptionsMonitor<FolderSettings> folders,
    IOptionsMonitor<ProcessingSettings> processing)
    : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        FolderProcessor.EnsureDirectories(folders.CurrentValue);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan interval = TimeSpan.FromSeconds(Math.Max(1, processing.CurrentValue.ScanIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                FolderProcessor.ProcessOnce(folders.CurrentValue, mention.CurrentValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}