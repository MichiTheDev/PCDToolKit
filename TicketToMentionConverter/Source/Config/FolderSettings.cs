namespace TicketToMentionConverter;

public record FolderSettings
{
    public string Input { get; init; } = "";
    public string Output { get; init; } = "";
    public string Backup { get; init; } = "";
}