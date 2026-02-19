@echo off
echo Checking environment...

:: Check for dotnet
where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] 'dotnet' command not found.
    echo Please install .NET SDK from https://dotnet.microsoft.com/download
) else (
    echo [OK] dotnet found.
)

:: Check for MSBuild
if exist "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    echo [OK] MSBuild found at Visual Studio 18 BuildTools.
    
    :: Try to build to check for SDK
    "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe" "src\LandscapeToolkit.csproj" -t:Restore >nul 2>nul
    if %errorlevel% neq 0 (
        echo [ERROR] MSBuild found but .NET SDK is missing.
        echo Please open Visual Studio Installer and install the ".NET Desktop Development" workload.
    ) else (
        echo [OK] SDK seems to be present.
    )
) else (
    echo [WARNING] MSBuild not found in expected location.
)

pause
