namespace TicketToMentionConverter;

public record MentionSettings
{
    public string Language { get; init; }
    public string Currency { get; init; }
    public PartySettings Supplier { get; init; }
}

public record PartySettings
{
    public string Id { get; init; }
    public string IdType { get; init; }
}