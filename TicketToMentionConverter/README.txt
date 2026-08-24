==================

Installation Guide:

1. Verschiebe die Dateien in den Ordner, wo der Service gespeichert werden soll
2. Fuehre die install.bat als Administrator aus und folgende den Anweisungen
3. Fertig

==================
------------------
==================

Update Guide:

Variante A (update.bat):
1. Neue .exe und update.bat in einen SEPARATEN Ordner legen
   (nicht in den Installationsordner)
2. TARGET_DIR in update.bat auf den Installationsordner setzen
3. update.bat als Administrator ausfuehren

Variante B (manuell):
1. Fuehre stop-service.bat als Administrator aus
2. Ersetze die .exe Datei aus dem aktuellen Update
3. Fuehre start-service.bat als Administrator aus

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