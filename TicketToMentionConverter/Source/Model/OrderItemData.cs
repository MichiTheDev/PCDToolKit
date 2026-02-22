namespace TicketToMentionConverter;

public record OrderItemData
{
    public string TicketNumber { get; init; }
    public string TechnicianName { get; init; }
    public string ArticleName { get; init; }
    public DateTime ServiceDate { get; init; }
    
    public string StartTime { get; init; }
    public string EndTime { get; init; }
    public string TotalTime { get; init; }
    
    public decimal Quantity { get; init; } = 1;
    public string Documentation { get; init; }
    public decimal Price { get; init; }
    public string MentionArticleId { get; init; }
    
    public string CustomerNumber { get; init; }
}