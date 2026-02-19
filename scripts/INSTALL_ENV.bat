@echo off
echo Launching Visual Studio Installer to add .NET Desktop Development workload...
echo Please click "Modify" or "Install" when the installer window appears.

if exist "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vs_installer.exe" (
    start "" "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vs_installer.exe" modify --installPath "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools" --add Microsoft.VisualStudio.Workload.NetDesktop --includeRecommended
) else (
    echo [ERROR] Visual Studio Installer not found at standard location.
    echo Please open "Visual Studio Installer" from your Start Menu manually.
)
pause
