namespace TicketToMentionConverter;

public record FolderSettings
{
    private readonly string input = "Input";
    private readonly string output = "Output";
    private readonly string backup = "Backup";

    public string Input  { get => Resolve(input);  init => input  = value; }
    public string Output { get => Resolve(output); init => output = value; }
    public string Backup { get => Resolve(backup); init => backup = value; }

    // Relative Pfade liegen neben der .exe. Noetig, weil der Windows-Service
    // mit System32 als Arbeitsverzeichnis startet. Absolute Pfade bleiben unangetastet.
    private static string Resolve(string path) =>
        Path.Combine(AppContext.BaseDirectory, string.IsNullOrWhiteSpace(path) ? "." : path);
}
