@echo off
setlocal

REM ====================================================
REM TicketToMentionConverter Update
REM Diese Datei neben die NEUE .exe legen und ausfuehren.
REM ====================================================

set "SERVICE_NAME=TicketToMentionConverter"
set "SOURCE_EXE=TicketToMentionConverter.exe"

REM Installationsordner des Dienstes. Anpassen, falls woanders installiert.
set "TARGET_DIR=C:\Services\TicketToMentionConverter"

echo.
echo =============================================
echo   TicketToMentionConverter Update
echo =============================================
echo.

REM === ADMIN CHECK ===
net session >nul 2>&1
if errorlevel 1 (
  echo ERROR: Bitte als Administrator ausfuehren.
  pause
  exit /b 1
)

REM === CHECKS ===
if not exist "%~dp0%SOURCE_EXE%" (
  echo ERROR: Neue EXE nicht gefunden: %~dp0%SOURCE_EXE%
  pause
  exit /b 1
)
if not exist "%TARGET_DIR%\%SOURCE_EXE%" (
  echo ERROR: Installation nicht gefunden: %TARGET_DIR%\%SOURCE_EXE%
  echo TARGET_DIR in dieser Datei anpassen oder install.bat verwenden.
  pause
  exit /b 1
)
REM Quelle und Ziel identisch: copy wuerde mit "cannot be copied onto itself" abbrechen.
if /i "%~dp0"=="%TARGET_DIR%\" (
  echo ERROR: Diese Datei liegt im Installationsordner selbst.
  echo Update-Ordner und Installationsordner muessen verschieden sein.
  pause
  exit /b 1
)
sc query "%SERVICE_NAME%" >nul 2>&1
if errorlevel 1 (
  echo ERROR: Dienst ist nicht installiert. Zuerst install.bat ausfuehren.
  pause
  exit /b 1
)

REM === STOP AND WAIT ===
REM sc stop kehrt sofort zurueck, der Dienst laeuft noch. Solange die .exe
REM laeuft, ist sie gesperrt und copy schlaegt fehl. Also wirklich warten.
echo [1/3] Dienst stoppen...
sc stop "%SERVICE_NAME%" >nul 2>&1

for /l %%i in (1,1,20) do (
  sc query "%SERVICE_NAME%" | findstr /c:"STOPPED" /c:"BEENDET" >nul && goto :stopped
  timeout /t 1 /nobreak >nul
)
echo ERROR: Dienst wurde nach 20 Sekunden nicht gestoppt. Update abgebrochen.
pause
exit /b 1

:stopped

REM === COPY ===
echo [2/3] EXE ersetzen...
copy /Y "%~dp0%SOURCE_EXE%" "%TARGET_DIR%\%SOURCE_EXE%" >nul
if errorlevel 1 (
  echo ERROR: Kopieren fehlgeschlagen. Dienst wird mit der ALTEN EXE gestartet.
  sc start "%SERVICE_NAME%" >nul
  pause
  exit /b 1
)

REM === START ===
echo [3/3] Dienst starten...
sc start "%SERVICE_NAME%" >nul
if errorlevel 1 (
  echo ERROR: Dienst konnte nicht gestartet werden.
  echo Details in der Ereignisanzeige ^> Windows-Protokolle ^> Anwendung.
  pause
  exit /b 1
)

timeout /t 2 /nobreak >nul
sc query "%SERVICE_NAME%" | findstr /c:"STATE" /c:"ZUSTAND"

echo.
echo Update abgeschlossen.
pause
exit /b 0
