
net session >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Requesting administrative privileges...
    powershell -Command "Start-Process -FilePath 'cmd.exe' -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

"%~dp0PsExec" -i -s -d "%~dp0AsusFanControlGUI.exe"
