@echo off
setlocal

REM ====================================================
REM TicketToMentionConverter Service Uninstaller
REM Entfernt nur die Dienstregistrierung. Dateien, Konfiguration
REM und die Input/Output/Backup Ordner bleiben absichtlich liegen.
REM ====================================================

set "SERVICE_NAME=TicketToMentionConverter"

echo.
echo =============================================
echo   TicketToMentionConverter Uninstall
echo =============================================
echo.

REM === ADMIN CHECK ===
net session >nul 2>&1
if errorlevel 1 (
  echo ERROR: Bitte dieses Skript als Administrator ausfuehren.
  pause
  exit /b 1
)

sc query "%SERVICE_NAME%" >nul 2>&1
if errorlevel 1 (
  echo Dienst ist nicht installiert. Nichts zu tun.
  pause
  exit /b 0
)

echo [1/2] Dienst stoppen...
sc stop "%SERVICE_NAME%" >nul 2>&1
timeout /t 3 /nobreak >nul

echo [2/2] Dienstregistrierung entfernen...
sc delete "%SERVICE_NAME%" >nul
if errorlevel 1 (
  echo ERROR: Dienst konnte nicht entfernt werden.
  pause
  exit /b 1
)

echo.
echo Uninstall abgeschlossen. Der Ordner %~dp0 wurde nicht angetastet.
pause
exit /b 0
