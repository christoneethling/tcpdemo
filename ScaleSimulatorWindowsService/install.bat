@setlocal enableextensions enabledelayedexpansion
@echo off

echo Make sure you run as administrator
echo Make sure you run as administrator
echo Make sure you run as administrator
echo Make sure you run as administrator
echo.
echo.

%~d0
cd %~dp0

echo Installing Wayware Scale Simulator service...
sc create "Wayware Scale Simulator"     binpath= "%~dp0ScaleSimulatorWindowsService.exe" start=delayed-auto
sc description  "Wayware Scale Simulator"  "Simulates Scales"

echo Done!
pause
