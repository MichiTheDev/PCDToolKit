@echo off
set "SERVICE_NAME=TicketToMentionConverter"

net session >nul 2>&1
if errorlevel 1 (
  echo ERROR: Bitte als Administrator ausfuehren.
  pause
  exit /b 1
)

sc start "%SERVICE_NAME%"
timeout /t 2 /nobreak >nul
sc query "%SERVICE_NAME%" | find "STATE"
pause
