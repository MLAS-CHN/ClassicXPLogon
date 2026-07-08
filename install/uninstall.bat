@echo off
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Please run as Administrator.
    pause
    exit /b 1
)

set SERVICE_NAME=ClassicLogonShell

echo === Classic Logon Shell Uninstaller ===
echo.

:: Stop service
echo Stopping service...
sc stop "%SERVICE_NAME%" >nul 2>&1
timeout /t 3 >nul

:: Delete service
echo Removing service...
sc delete "%SERVICE_NAME%" >nul 2>&1

:: Kill processes
taskkill /f /im LogonShell.exe >nul 2>&1

echo.
echo Service removed. Deleting files...

:: Self-delete: start a detached cmd that waits then deletes the directory
set INSTALL_DIR=%~dp0
start "" /b cmd /c "timeout /t 3 >nul & rmdir /s /q "%INSTALL_DIR%""

echo.
echo === Uninstall complete ===
echo Files will be deleted in a few seconds.
timeout /t 2
exit
