using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using TicketToMentionConverter.Csv;

namespace TicketToMentionConverter.Services;

public static class FolderProcessor
{
    public static void EnsureDirectories(FolderSettings folders)
    {
        Directory.CreateDirectory(folders.Input);
        Directory.CreateDirectory(folders.Output);
        Directory.CreateDirectory(folders.Backup);
    }
    
    public static void ProcessOnce(FolderSettings folderSettings, MentionSettings mentionSettings)
    {
        List<string> csvFiles = Directory.EnumerateFiles(folderSettings.Input, "*.csv", SearchOption.TopDirectoryOnly)
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (string csvPath in csvFiles)
        {
            ProcessSingleCsv(csvPath, folderSettings, mentionSettings);
        }
    }
    
    private static void ProcessSingleCsv(string csvPath, FolderSettings folderSettings, MentionSettings mentionSettings)
    {
        if (!IsFileReady(csvPath)) return;

        try
        {
            // 1) Read CSV
            List<TicketCsvRow> rows = CsvImporter.Load(csvPath);
            if (rows.Count == 0) 
                throw new Exception("CSV is empty.");

            // 2) ORDER: OrderId = first number of file (adding + 0000 because of customers wish)
            string fileName = Path.GetFileNameWithoutExtension(csvPath);
            string orderId = $"LN-Nr. {fileName.Split(" - ")[0]}";

            // Checking for multiple customers (it's not valid to have more than 1 customer in 1 .csv)
            List<string> customers = rows.Select(row => row.CustomerNumber).Distinct().ToList();
            if (customers.Count != 1) 
                throw new Exception($"CSV has multiple customers: {string.Join(", ", customers)}");

            TicketHeaderData header = new TicketHeaderData
            {
                OrderId = orderId,
                OrderDate = rows.Min(r => r.ServiceDate),
                BuyerId = customers[0]
            };

            // 3) Build XML Document
            XElement headerElement = OrderHeaderBuilder.Build(header, mentionSettings);
            XDocument document  = OrderRootBuilder.Create(headerElement);

            XElement itemList = document.Descendants(Namespaces.OpenTrans + "ORDER_ITEM_LIST").First();

            List<OrderItemData> items = OrderMapper.Map(rows);
            int lineId = 1;
            OrderItemData previousItem = null;
            foreach (OrderItemData item in items)
            {
                bool shouldSkipUdx = false;
                if (previousItem is not null)
                {
                    shouldSkipUdx = previousItem.Quantity > 0 && item.Quantity < 0;

                    // PCD want a different article id for this case
                    if (shouldSkipUdx)
                    {
                        item.MentionArticleId = mentionSettings.DeductionArticleId;
                    }
                }
                
                itemList.Add(OrderItemBuilder.Create(lineId++, item, shouldSkipUdx));
                previousItem = item;
            }

            // 4) Generate file names (unique)
            string baseName = $"{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}_{ShortHash(csvPath)}";
            string outXmlPath = Path.Combine(folderSettings.Output, baseName + ".xml");
            string backupXmlPath = Path.Combine(folderSettings.Backup, baseName + ".xml");

            // 5) Saving + Backup
            document.Save(outXmlPath);
            File.Copy(outXmlPath, backupXmlPath, overwrite: true);

            // 6) Delete CSV
            File.Delete(csvPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {Path.GetFileName(csvPath)}: {ex.Message}");
        }
    }

    private static bool IsFileReady(string path)
    {
        try
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return stream.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static string ShortHash(string path)
    {
        FileInfo fi = new FileInfo(path);
        string input = $"{fi.Name}|{fi.Length}|{fi.LastWriteTimeUtc:O}";
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes)[..8];
    }
}