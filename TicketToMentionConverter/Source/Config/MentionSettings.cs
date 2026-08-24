namespace TicketToMentionConverter;

public record MentionSettings
{
    public string Language { get; init; } = "ger";
    public string Currency { get; init; } = "EUR";
    public PartySettings Supplier { get; init; } = new();
    public string DeductionArticleId { get; init; } = "";
}

public record PartySettings
{
    public string Id { get; init; } = "PCD";
    public string IdType { get; init; } = "buyer_specific";
}