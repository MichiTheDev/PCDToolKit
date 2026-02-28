namespace TicketToMentionConverter.Services;

public static class OrderMapper
{
    public static List<OrderItemData> Map(List<TicketCsvRow> rows)
    {
        return rows.Select(ticketRow => new OrderItemData
        {
            TicketNumber = ticketRow.TicketNumber,
            TicketTitle =  ticketRow.TicketTitle,
            TechnicianName = ticketRow.TechnicianName,
            ArticleName = ticketRow.ArticleName,
            ServiceDate = ticketRow.ServiceDate,
            StartTime = ticketRow.StartTime,
            EndTime = ticketRow.EndTime,
            TotalTime = ticketRow.TotalTime,
            Documentation = ticketRow.Description,
            Price = ticketRow.UnitPrice,
            MentionArticleId = "114787",
            CustomerNumber = ticketRow.CustomerNumber,
        }).ToList();
    }
}