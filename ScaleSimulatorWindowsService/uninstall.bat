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

echo Deleting  Wayware Scale Simulator service
sc delete "Wayware Scale Simulator"   

echo Done!
pause
