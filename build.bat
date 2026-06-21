@echo off
setlocal enabledelayedexpansion

:: Find MSBuild via vswhere
set TMPFILE=%TEMP%\cls_msbuild.txt
"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" > "%TMPFILE%" 2>nul
set MSBUILD=
set /p MSBUILD=<"%TMPFILE%"
del "%TMPFILE%" >nul 2>&1
if not defined MSBUILD (
    echo [ERROR] MSBuild not found. Install Visual Studio Build Tools.
    pause
    exit /b 1
)

:: Find .NET Framework 4.8 reference assemblies
set REFPATH=
for /d %%d in ("%USERPROFILE%\.nuget\packages\microsoft.netframework.referenceassemblies.net48\*") do set REFPATH=%%d\build\.NETFramework\v4.8
if not defined REFPATH set REFPATH=C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8
if not exist "%REFPATH%\mscorlib.dll" (
    echo [ERROR] .NET Framework 4.8 reference assemblies not found.
    pause
    exit /b 1
)

echo === Classic Logon Shell Build ===
echo MSBuild: %MSBUILD%
echo.

:: Build
"%MSBUILD%" "%~dp0LogonShell.sln" /t:Rebuild /p:Configuration=Release "/p:Platform=Any CPU" "/p:FrameworkPathOverride=%REFPATH%" /verbosity:minimal
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Build failed.
    pause
    exit /b 1
)

:: Create output directory
set OUT=%~dp0build
if exist "%OUT%" rmdir /s /q "%OUT%"
mkdir "%OUT%"
mkdir "%OUT%\resources"

:: Copy files
copy /y "%~dp0LogonShell\bin\Release\LogonShell.exe" "%OUT%\" >nul
copy /y "%~dp0LogonShell\bin\Release\FlaUI.Core.dll" "%OUT%\" >nul
copy /y "%~dp0LogonShell\bin\Release\FlaUI.UIA3.dll" "%OUT%\" >nul
copy /y "%~dp0LogonShell\bin\Release\Interop.UIAutomationClient.dll" "%OUT%\" >nul
copy /y "%~dp0Logon Screen Launcher\bin\Release\LauncherService.exe" "%OUT%\" >nul
copy /y "%~dp0resources\*.png" "%OUT%\resources\" >nul
copy /y "%~dp0scripts\install.bat" "%OUT%\" >nul
copy /y "%~dp0scripts\uninstall.bat" "%OUT%\" >nul

echo.
echo === Build complete ===
echo Output: %OUT%
echo.
echo Files:
dir "%OUT%" /b
echo.
echo resources:
dir "%OUT%\resources" /b
pause
