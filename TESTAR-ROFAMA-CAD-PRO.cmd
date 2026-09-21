@echo off
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\prepare-test-build.ps1" -Target Both
if errorlevel 1 pause & exit /b 1
echo.
echo Builds criados. Para 2026.1.2+ execute:
echo powershell -ExecutionPolicy Bypass -File .\install-test.ps1 -Target Net10
echo Para 2026 ate 2026.1.1 use Net8.
pause
