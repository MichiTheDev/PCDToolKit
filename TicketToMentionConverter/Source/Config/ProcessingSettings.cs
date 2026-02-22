namespace TicketToMentionConverter;

public record ProcessingSettings
{
    public int ScanIntervalSeconds { get; init; } = 5;
}