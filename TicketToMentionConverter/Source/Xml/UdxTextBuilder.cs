namespace TicketToMentionConverter;

public static class UdxTextBuilder
{
    public static string Build(OrderItemData item)
    {
        return $"""
                Ticket Nr.: {item.TicketNumber}
                {item.TechnicianName}
                {item.ArticleName}
                -----------------------------------
                Datum: {item.ServiceDate:dd.MM.yyyy} {item.StartTime} - {item.EndTime}
                Geleistete Stunden: {item.TotalTime}
                -----------------------------------
                Dokumentation:
                {item.Documentation}
                """;
    }
}