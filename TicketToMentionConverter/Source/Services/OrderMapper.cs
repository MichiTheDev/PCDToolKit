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
            // Minus-Positionen (Ausgleich aus Vertrag) drehen: Stück -1, Preis positiv
            Quantity = ticketRow.UnitPrice < 0 ? -1 : 1,
            Price = Math.Abs(ticketRow.UnitPrice),
            MentionArticleId = "114787",
            CustomerNumber = ticketRow.CustomerNumber,
        }).ToList();
    }
}