==================

Installation Guide:

1. Verschiebe die Dateien in den Ordner, wo der Service gespeichert werden soll
2. Fuehre die install.bat als Administrator aus und folgende den Anweisungen
3. Fertig

==================
------------------
==================

Update Guide:

1. Fuehre stop-service.bat aus
2. Ersetze die .exe Datei aus dem Aktuellen Update
3. Fuehre start-Service.bat aus

==================

Extra Info:

Input: Der Ordner, in dem die Dateien reinkommen, die ausgewertet werden sollen
Output: Hier werden die verarbeiteten Datein als .xml kopiert, ready fuer mention import
Backup: Hier werden alle output datein nochmals extra kopiert

Die drei Ordner werden neben der .exe angelegt, wenn keine appsettings.json
danebenliegt. Absolute Pfade in der appsettings.json haben Vorrang.

==================

Build (eine einzelne .exe):

dotnet publish TicketToMentionConverter.csproj -c Release -r win-x64 -o publish

Ergebnis: publish\TicketToMentionConverter.exe (self-contained, kein .NET
auf dem Zielrechner noetig) plus appsettings.json als Vorlage.

Zum Testen: .exe in einen Ordner legen und doppelklicken. Zum Ausliefern:
.exe + appsettings.json + die .bat Dateien zippen.

==================