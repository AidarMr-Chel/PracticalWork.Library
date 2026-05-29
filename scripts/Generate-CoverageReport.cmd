@echo off
REM Runs coverage script without changing system ExecutionPolicy.
setlocal
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Generate-CoverageReport.ps1" %*
exit /b %ERRORLEVEL%
