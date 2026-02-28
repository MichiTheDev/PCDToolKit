using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketToMentionConverter.Services;

namespace TicketToMentionConverter;

internal class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .UseWindowsService(options =>
            {
                options.ServiceName = "TicketToMentionConverter";
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