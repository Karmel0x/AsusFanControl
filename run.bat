@echo off
setlocal

set "PSEXEC=%~dp0PsExec.exe"
if not exist "%PSEXEC%" set "PSEXEC=%ProgramFiles%\ASUS\ASUS Fan Control\PsExec.exe"

if not exist "%PSEXEC%" (
    echo PsExec.exe was not found next to run.bat or in "%ProgramFiles%\ASUS\ASUS Fan Control".
    exit /b 1
)

net session >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Requesting administrative privileges...
    powershell -NoProfile -Command "Start-Process -FilePath 'cmd.exe' -ArgumentList '/c \"\"%~f0\"\" %*' -Verb RunAs"
    exit /b
)

"%PSEXEC%" -accepteula -i -s -d "%~dp0AsusFanControlGUI.exe" %*