@echo off
setlocal

REM ====================================================
REM TicketToMentionConverter Service Uninstaller
REM ====================================================

set "SERVICE_NAME=TicketToMentionConverter"

set "BASE_DIR=C:\Services\TicketToMentionConverter"
set "CONFIG_DIR=C:\ProgramData\PCD\TicketToMentionConverter"

set "DROP_ROOT=C:\Drop"

echo.
echo =============================================
echo   TicketToMentionConverter Uninstall
echo =============================================
echo.

REM === ADMIN CHECK ===
net session >nul 2>&1
if %errorlevel% neq 0 (
  echo ERROR: Please run this script as Administrator.
  pause
  exit /b 1
)

REM === STOP SERVICE ===
echo [1/5] Stopping service...
sc query "%SERVICE_NAME%" >nul 2>&1
if %errorlevel%==0 (
    sc stop "%SERVICE_NAME%" >nul 2>&1
    timeout /t 3 /nobreak >nul
) else (
    echo Service not found.
)

REM === DELETE SERVICE ===
echo [2/5] Removing service registration...
sc delete "%SERVICE_NAME%" >nul 2>&1

REM === WAIT FOR SERVICE REMOVAL ===
timeout /t 2 /nobreak >nul

REM === DELETE SERVICE FILES ===
echo [3/5] Removing service directory...
if exist "%BASE_DIR%" (
    rmdir /s /q "%BASE_DIR%"
) else (
    echo Service directory not found.
)

REM === DELETE CONFIG ===
echo [4/5] Removing config directory...
if exist "%CONFIG_DIR%" (
    rmdir /s /q "%CONFIG_DIR%"
) else (
    echo Config directory not found.
)

REM === OPTIONAL DROP FOLDER DELETE ===
echo.
set /p DELETE_DROP=Delete Drop folders as well? (Y/N): 

if /i "%DELETE_DROP%"=="Y" (
    if exist "%DROP_ROOT%" (
        echo Removing Drop folder...
        rmdir /s /q "%DROP_ROOT%"
    )
)

echo.
echo Uninstall complete.
pause
exit /b 0