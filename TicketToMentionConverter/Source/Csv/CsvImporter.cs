using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace TicketToMentionConverter.Csv;

public static class CsvImporter
{
    public static List<TicketCsvRow> Load(string filePath)
    {
        CsvConfiguration csvConfiguration = new CsvConfiguration(new CultureInfo("de-DE"))
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            BadDataFound = null
        };
        
        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csvReader = new CsvReader(reader, csvConfiguration);
        return csvReader.GetRecords<TicketCsvRow>().ToList();
    }    
}