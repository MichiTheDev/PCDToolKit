namespace TicketToMentionConverter;

public static class UdxTextBuilder
{
    public static string Build(OrderItemData item)
    {
        string totalTimeDisplay = "Geleistete Stunden:";
        if (item.TotalTime.Contains("Km") || item.TotalTime.Contains("km"))
        {
            totalTimeDisplay = "Gefahrene Kilometer:";
        }
        
        return $"""
                {item.TechnicianName}
                {item.ArticleName}
                -----------------------------------
                Datum: {item.ServiceDate:dd.MM.yyyy} {item.StartTime} - {item.EndTime}
                {totalTimeDisplay} {item.TotalTime}
                -----------------------------------
                Dokumentation:
                {item.Documentation}
                """;
    }
}