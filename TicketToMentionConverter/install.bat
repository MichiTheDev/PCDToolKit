@echo off
setlocal enabledelayedexpansion

REM ====================================================
REM TicketToMentionConverter Service Installer
REM ====================================================

REM === CONFIGURABLE VARIABLES ===
set "SERVICE_NAME=TicketToMentionConverter"
set "DISPLAY_NAME=Ticket To Mention Converter"
set "BASE_DIR=%~dp0"
set "BASE_DIR=%BASE_DIR:~0,-1%"
set "EXE_NAME=TicketToMentionConverter.exe"

set "CONFIG_FILE=%BASE_DIR%\appsettings.json"

REM Arbeitsordner liegen neben der .exe. Wer sie woanders haben will,
REM traegt in der appsettings.json absolute Pfade ein - die haben Vorrang.
set "DROP_IN=%BASE_DIR%\Input"
set "DROP_OUT=%BASE_DIR%\Output"
set "DROP_BACKUP=%BASE_DIR%\Backup"

echo.
echo =============================================
echo   TicketToMentionConverter Service Setup
echo =============================================
echo.
echo Installationsordner: %BASE_DIR%
echo.

REM === ADMIN CHECK ===
net session >nul 2>&1
if errorlevel 1 (
  echo ERROR: Bitte dieses Skript als Administrator ausfuehren.
  pause
  exit /b 1
)

REM === CHECK EXE EXISTS ===
if not exist "%BASE_DIR%\%EXE_NAME%" (
  echo ERROR: EXE nicht gefunden:
  echo   %BASE_DIR%\%EXE_NAME%
  echo Bitte zuerst die veroeffentlichte EXE hierher kopieren.
  pause
  exit /b 1
)

REM === CREATE DIRECTORIES ===
echo [1/6] Ordner anlegen...
mkdir "%DROP_IN%" 2>nul
mkdir "%DROP_OUT%" 2>nul
mkdir "%DROP_BACKUP%" 2>nul

REM === CREATE DEFAULT CONFIG IF NOT EXISTING ===
echo [2/6] Konfiguration pruefen...
if not exist "%CONFIG_FILE%" (
  echo Standard appsettings.json wird erstellt...
  >"%CONFIG_FILE%" (
    echo {
    echo   "Mention": {
    echo     "Language": "ger",
    echo     "Currency": "EUR",
    echo     "Supplier": { "Id": "PCD", "IdType": "buyer_specific" },
    echo     "DeductionArticleId": ""
    echo   },
    echo   "Folders": {
    echo     "Input": "Input",
    echo     "Output": "Output",
    echo     "Backup": "Backup"
    echo   },
    echo   "Processing": {
    echo     "ScanIntervalSeconds": 5
    echo   }
    echo }
  )
) else (
  echo Konfiguration existiert bereits. Wird nicht ueberschrieben.
)

REM === SET PERMISSIONS FOR WINDOWS SERVICE ===
REM Der Dienst laeuft als LocalSystem und braucht Zugriff auf .exe, Config und Ordner.
echo [3/6] NTFS-Rechte fuer SYSTEM setzen...
icacls "%BASE_DIR%" /grant "SYSTEM:(OI)(CI)F" /T >nul

REM === CREATE OR UPDATE SERVICE ===
echo [4/6] Windows-Dienst installieren/aktualisieren...
sc query "%SERVICE_NAME%" >nul 2>&1
if %errorlevel%==0 (
  echo Dienst existiert bereits. Wird aktualisiert...
  sc stop "%SERVICE_NAME%" >nul 2>&1
  timeout /t 3 /nobreak >nul
  sc config "%SERVICE_NAME%" binPath= "\"%BASE_DIR%\%EXE_NAME%\"" start= auto DisplayName= "%DISPLAY_NAME%" >nul
) else (
  echo Neuer Dienst wird erstellt...
  sc create "%SERVICE_NAME%" binPath= "\"%BASE_DIR%\%EXE_NAME%\"" start= auto DisplayName= "%DISPLAY_NAME%" >nul
)
if errorlevel 1 (
  echo ERROR: Dienst konnte nicht registriert werden.
  pause
  exit /b 1
)

REM === ALLOW USER TO EDIT CONFIG ===
echo.
echo [5/6] Konfiguration jetzt pruefen und ggf. anpassen:
echo   %CONFIG_FILE%
echo.
start "" /wait notepad "%CONFIG_FILE%"

set /p ANSWER=Y eingeben und ENTER, wenn die Konfiguration passt (alles andere bricht ab):

if /i not "%ANSWER%"=="Y" (
  echo Setup beendet. Dienst wurde NICHT gestartet.
  echo Spaeter starten mit: start-service.bat
  pause
  exit /b 0
)

REM === START SERVICE ===
echo [6/6] Dienst starten...
sc start "%SERVICE_NAME%" >nul
if errorlevel 1 (
  echo ERROR: Dienst konnte nicht gestartet werden.
  echo Details in der Ereignisanzeige ^> Windows-Protokolle ^> Anwendung.
  pause
  exit /b 1
)

timeout /t 2 /nobreak >nul
sc query "%SERVICE_NAME%" | find "STATE"

echo.
echo Setup abgeschlossen.
echo Eingangsordner: %DROP_IN%
echo Logs: Ereignisanzeige ^> Windows-Protokolle ^> Anwendung
pause
exit /b 0
