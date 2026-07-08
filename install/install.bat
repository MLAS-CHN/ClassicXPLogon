@echo off
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Please run as Administrator.
    pause
    exit /b 1
)

set SERVICE_NAME=ClassicLogonShell
set DEFAULT_PATH=C:\ClassicLogonShell
set /p INSTALL_PATH="Install path [%DEFAULT_PATH%]: "
if "%INSTALL_PATH%"=="" set INSTALL_PATH=%DEFAULT_PATH%

echo.
echo === Classic Logon Shell Installer ===
echo Install to: %INSTALL_PATH%
echo.

:: Stop and remove existing service
sc query "%SERVICE_NAME%" >nul 2>&1
if %errorlevel% equ 0 (
    echo Stopping existing service...
    sc stop "%SERVICE_NAME%" >nul 2>&1
    timeout /t 3 >nul
    echo Removing existing service...
    sc delete "%SERVICE_NAME%" >nul 2>&1
    timeout /t 2 >nul
)

:: Kill existing processes
taskkill /f /im LogonShell.exe >nul 2>&1

:: Create install directory
if not exist "%INSTALL_PATH%" mkdir "%INSTALL_PATH%"
if not exist "%INSTALL_PATH%\assets" mkdir "%INSTALL_PATH%\assets"

:: Copy files
echo Copying files...
copy /y "%~dp0LogonShell.exe" "%INSTALL_PATH%\" >nul
copy /y "%~dp0LauncherService.exe" "%INSTALL_PATH%\" >nul
copy /y "%~dp0FlaUI.Core.dll" "%INSTALL_PATH%\" >nul
copy /y "%~dp0FlaUI.UIA3.dll" "%INSTALL_PATH%\" >nul
copy /y "%~dp0Interop.UIAutomationClient.dll" "%INSTALL_PATH%\" >nul
copy /y "%~dp0assets\*.png" "%INSTALL_PATH%\assets\" >nul
copy /y "%~dp0uninstall.bat" "%INSTALL_PATH%\" >nul

:: Create service
echo Creating service...
sc create "%SERVICE_NAME%" binPath= "%INSTALL_PATH%\LauncherService.exe" start= auto obj= LocalSystem
if %errorlevel% neq 0 (
    echo [ERROR] Failed to create service.
    pause
    exit /b 1
)

:: Start service
echo Starting service...
sc start "%SERVICE_NAME%" >nul 2>&1

echo.
echo === Installation complete ===
echo Location: %INSTALL_PATH%
echo Service: %SERVICE_NAME% (Running)
echo.
echo To uninstall, run: %INSTALL_PATH%\uninstall.bat
echo.
pause
