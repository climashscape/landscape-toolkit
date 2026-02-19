# Landscape Toolkit Build Script
# v1.0.0 Release

$ErrorActionPreference = "Stop"

$solutionDir = "src"
$distDir = "dist"
$version = "1.0.0"

Write-Host "Building Landscape Toolkit v$version..." -ForegroundColor Green

# Clean
Write-Host "Cleaning..."
if (Test-Path $distDir) { Remove-Item $distDir -Recurse -Force }
dotnet clean "$solutionDir/LandscapeToolkit.csproj"

# Build
Write-Host "Building Release..."
dotnet build "$solutionDir/LandscapeToolkit.csproj" -c Release

# Package
$releaseDir = "$distDir/v$version"
New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null

$binPath = "$solutionDir/bin/Release/net48/LandscapeToolkit.gha"
if (Test-Path $binPath) {
    Copy-Item $binPath -Destination $releaseDir
    Write-Host "Packaged LandscapeToolkit.gha to $releaseDir" -ForegroundColor Cyan
} else {
    Write-Error "Build failed: .gha file not found at $binPath"
}

# Create Zip (optional, using Compress-Archive)
$zipPath = "$distDir/LandscapeToolkit_v$version.zip"
Compress-Archive -Path "$releaseDir/*" -DestinationPath $zipPath -Force
Write-Host "Created release zip at $zipPath" -ForegroundColor Cyan

Write-Host "Build Complete!" -ForegroundColor Green
