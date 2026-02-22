namespace TicketToMentionConverter;

public record TicketHeaderData
{
    public string OrderId { get; init; }
    public DateTime OrderDate { get; init; }
    public string BuyerId { get; init; }
}