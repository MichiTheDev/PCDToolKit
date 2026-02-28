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

set "CONFIG_DIR=%BASE_DIR%"
set "CONFIG_FILE=%CONFIG_DIR%\appsettings.json"

set "DROP_IN=C:\Drop\In"
set "DROP_OUT=C:\Drop\Out"
set "DROP_BACKUP=C:\Drop\Backup"

echo.
echo =============================================
echo   TicketToMentionConverter Service Setup
echo =============================================
echo.

REM === ADMIN CHECK ===
net session >nul 2>&1
if errorlevel 1 (
  echo ERROR: Please run this script as Administrator.
  pause
  exit /b 1
)

REM === CREATE DIRECTORIES ===
echo [1/6] Creating directories...
mkdir "%BASE_DIR%" 2>nul
mkdir "%CONFIG_DIR%" 2>nul
mkdir "%DROP_IN%" 2>nul
mkdir "%DROP_OUT%" 2>nul
mkdir "%DROP_BACKUP%" 2>nul

REM === CHECK EXE EXISTS ===
if not exist "%BASE_DIR%\%EXE_NAME%" (
  echo ERROR: EXE not found at:
  echo   %BASE_DIR%\%EXE_NAME%
  echo Please copy the published EXE first.
  pause
  exit /b 1
)

REM === CREATE DEFAULT CONFIG IF NOT EXISTING ===
echo [2/6] Checking configuration...
if not exist "%CONFIG_FILE%" (
  echo Creating default appsettings.json...
  >"%CONFIG_FILE%" (
    echo {
    echo   "Mention": {
    echo     "Language": "ger",
    echo     "Currency": "EUR",
    echo     "Supplier": { "Id": "PCD", "IdType": "buyer_specific" }
    echo   },
    echo   "Folders": {
    echo     "Input": "%DROP_IN:\=\\%",
    echo     "Output": "%DROP_OUT:\=\\%",
    echo     "Backup": "%DROP_BACKUP:\=\\%"
    echo   },
    echo   "Processing": {
    echo     "ScanIntervalSeconds": 5
    echo   }
    echo }
  )
) else (
  echo Configuration already exists. Skipping creation.
)

REM === SET PERMISSIONS FOR WINDOWS SERVICE ===
echo [3/6] Setting NTFS permissions for SYSTEM account...
icacls "%DROP_IN%" /grant "SYSTEM:(OI)(CI)F" >nul
icacls "%DROP_OUT%" /grant "SYSTEM:(OI)(CI)F" >nul
icacls "%DROP_BACKUP%" /grant "SYSTEM:(OI)(CI)F" >nul
icacls "%CONFIG_DIR%" /grant "SYSTEM:(OI)(CI)F" >nul

REM === CREATE OR UPDATE SERVICE ===
echo [4/6] Installing/updating Windows service...
sc query "%SERVICE_NAME%" >nul 2>&1
if %errorlevel%==0 (
  echo Service already exists. Updating...
  sc stop "%SERVICE_NAME%" >nul 2>&1
  timeout /t 2 /nobreak >nul
  sc config "%SERVICE_NAME%" binPath= "\"%BASE_DIR%\%EXE_NAME%\"" start= auto DisplayName= "%DISPLAY_NAME%" >nul
) else (
  echo Creating new service...
  sc create "%SERVICE_NAME%" binPath= "\"%BASE_DIR%\%EXE_NAME%\"" start= auto DisplayName= "%DISPLAY_NAME%" >nul
)

REM === ALLOW USER TO EDIT CONFIG ===
echo.
echo [5/6] Please review and edit the configuration file now:
echo   %CONFIG_FILE%
echo.
start "" notepad "%CONFIG_FILE%"

set /p ANSWER=Type Y and press ENTER when configuration is ready (or anything else to cancel): 

if /i not "%ANSWER%"=="Y" (
  echo Setup finished. Service NOT started.
  echo You can start manually using:
  echo   sc start "%SERVICE_NAME%"
  pause
  exit /b 0
)

REM === START SERVICE ===
echo [6/6] Starting service...
sc start "%SERVICE_NAME%"

echo.
echo Setup complete.
echo Logs can be found in Windows Event Viewer ^> Application.
pause
exit /b 0