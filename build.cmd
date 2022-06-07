@echo off
setlocal
color 07
cls

ECHO ----------------------------
ECHO Restore Tools
ECHO ----------------------------
dotnet tool restore --no-cache
if errorlevel 1 (
  GOTO :end
)

ECHO ----------------------------
ECHO Run Cake
ECHO ----------------------------
dotnet cake %*

:end
exit /b %errorlevel%
