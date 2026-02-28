using CsvHelper.Configuration.Attributes;

namespace TicketToMentionConverter;

public record TicketCsvRow
{
    [Name("Ticketnummer")]
    public string TicketNumber { get; init; }
    
    [Name("Tickettitel")]
    public string TicketTitle { get; init; }
    
    [Name("Techniker:in")]
    public string TechnicianName { get; init; }
    
    [Name("Artikelname")]
    public string ArticleName { get; init; }
    
    [Name("Servicedatum")]
    public DateTime ServiceDate { get; init; }
    
    [Name("Startzeit")]
    public string StartTime { get; init; }
    
    [Name("Endzeit")]
    public string EndTime { get; init; }
    
    [Name("Anzahl (geleistete Stunden oder Artikelmenge)")]
    public string TotalTime { get; init; }
    
    [Name("Dokumentationstext")]
    public string Description { get; init; }
    
    [Name("Positionspreis (Einzelpreis * Menge)")]
    public decimal UnitPrice { get; init; }
    
    [Name("Kundennummer")]
    public string CustomerNumber { get; init; }
}