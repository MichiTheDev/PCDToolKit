using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketToMentionConverter.Services;

namespace TicketToMentionConverter;

internal class Program
{
    public static void Main(string[] args)
    {
        const string configPath = @"C:\ProgramData\PCD\TicketToMentionConverter\appsettings.json";

        Host.CreateDefaultBuilder(args)
            .UseWindowsService(options =>
            {
                options.ServiceName = "TicketToMentionConverter";
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();

                config.AddJsonFile(configPath, optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<MentionSettings>(context.Configuration.GetSection("Mention"));
                services.Configure<FolderSettings>(context.Configuration.GetSection("Folders"));
                services.Configure<ProcessingSettings>(context.Configuration.GetSection("Processing"));

                services.AddHostedService<TicketToMentionService>();
            })
            .Build()
            .Run();
    }
}