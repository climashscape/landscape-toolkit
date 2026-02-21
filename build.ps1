# Landscape Toolkit Build Script

$ErrorActionPreference = "Stop"

# 1. Update Version Info
Write-Host "Updating Version Info..."
.\tools\update_version.ps1

$solutionDir = "src"
$distDir = "dist"
$version = Get-Content "VERSION" -Raw | ForEach-Object { $_.Trim() }

Write-Host "Building Landscape Toolkit v$version..." -ForegroundColor Green

# Clean
Write-Host "Cleaning..."
if (Test-Path $distDir) { Remove-Item $distDir -Recurse -Force }
dotnet clean "$solutionDir\LandscapeToolkit.csproj"

# Build
Write-Host "Building Release..."
dotnet build "$solutionDir\LandscapeToolkit.csproj" -c Release

# Package
$releaseDir = "$distDir\v$version"
New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null

$binPath = "$solutionDir\bin\Release\net48\LandscapeToolkit.gha"
if (Test-Path $binPath) {
    Copy-Item $binPath -Destination $releaseDir
    Write-Host "Packaged LandscapeToolkit.gha to $releaseDir" -ForegroundColor Cyan
} else {
    Write-Error "Build failed: .gha file not found at $binPath"
    exit 1
}

# Create Zip (optional, using Compress-Archive)
$zipPath = "$distDir\LandscapeToolkit_v$version.zip"
Compress-Archive -Path "$releaseDir\*" -DestinationPath $zipPath -Force
Write-Host "Created release zip at $zipPath" -ForegroundColor Cyan

Write-Host "Build Complete!" -ForegroundColor Green
