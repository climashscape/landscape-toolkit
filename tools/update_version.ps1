# Update Version Script
# Reads version from VERSION file and updates all project files
# Must be run from the project root directory

$ErrorActionPreference = "Stop"
$root = "."
$versionFile = Join-Path $root "VERSION"

if (-not (Test-Path $versionFile)) {
    Write-Error "VERSION file not found at $versionFile. Please run this script from the project root."
    exit 1
}

$version = Get-Content $versionFile -Raw
$version = $version.Trim()
Write-Host "Updating project to version $version..." -ForegroundColor Cyan

# Helper function to write with UTF-8 without BOM (compatible with most tools)
function Set-ContentUtf8 {
    param(
        [string]$Path,
        [string]$Content
    )
    [System.IO.File]::WriteAllText($Path, $Content, [System.Text.Encoding]::UTF8)
}

# Helper function to read with UTF-8 (trying to avoid system default ANSI)
function Get-ContentUtf8 {
    param(
        [string]$Path
    )
    # Get-Content without encoding uses system default (often ANSI) in PS5.1
    # System.IO.File.ReadAllText defaults to UTF-8
    return [System.IO.File]::ReadAllText($Path, [System.Text.Encoding]::UTF8)
}

# 1. Update LandscapeToolkitInfo.cs
$assemblyInfoPath = Join-Path $root "src\LandscapeToolkitInfo.cs"
if (Test-Path $assemblyInfoPath) {
    $assemblyContent = Get-ContentUtf8 -Path $assemblyInfoPath
    $assemblyContent = $assemblyContent -replace 'public override string Version => ".*";', "public override string Version => ""$version"";"
    Set-ContentUtf8 -Path $assemblyInfoPath -Content $assemblyContent
    Write-Host "Updated src\LandscapeToolkitInfo.cs"
} else {
    Write-Warning "File not found: $assemblyInfoPath"
}

# 2. Update LandscapeToolkit.csproj
$csprojPath = Join-Path $root "src\LandscapeToolkit.csproj"
if (Test-Path $csprojPath) {
    $csprojContent = Get-ContentUtf8 -Path $csprojPath
    $csprojContent = $csprojContent -replace '<Version>.*</Version>', "<Version>$version</Version>"
    Set-ContentUtf8 -Path $csprojPath -Content $csprojContent
    Write-Host "Updated src\LandscapeToolkit.csproj"
} else {
    Write-Warning "File not found: $csprojPath"
}

# 3. Update tools/Tools.csproj
$toolsCsprojPath = Join-Path $root "tools\Tools.csproj"
if (Test-Path $toolsCsprojPath) {
    $toolsCsprojContent = Get-ContentUtf8 -Path $toolsCsprojPath
    $toolsCsprojContent = $toolsCsprojContent -replace '<Version>.*</Version>', "<Version>$version</Version>"
    Set-ContentUtf8 -Path $toolsCsprojPath -Content $toolsCsprojContent
    Write-Host "Updated tools/Tools.csproj"
} else {
    Write-Warning "File not found: $toolsCsprojPath"
}

# 4. Update docs/README.md
$docsReadmePath = Join-Path $root "docs\README.md"
if (Test-Path $docsReadmePath) {
    $readmeContent = Get-ContentUtf8 -Path $docsReadmePath
    # Update English section
    $readmeContent = $readmeContent -replace '\*\*Current Version: .*\*\*', "**Current Version: $version**"
    $readmeContent = $readmeContent -replace '> \*\*New in v.*\*\*: ', "> **New in v$version**: "
    # Update Chinese section
    $readmeContent = $readmeContent -replace '\*\*当前版本: .*\*\*', "**当前版本: $version**"
    $readmeContent = $readmeContent -replace '> \*\*v.* 更新\*\*: ', "> **v$version 更新**: "
    
    Set-ContentUtf8 -Path $docsReadmePath -Content $readmeContent
    Write-Host "Updated docs/README.md"
} else {
    Write-Warning "File not found: $docsReadmePath"
}

# 5. Update root README.md
$rootReadmePath = Join-Path $root "README.md"
if (Test-Path $rootReadmePath) {
    $readmeContent = Get-ContentUtf8 -Path $rootReadmePath
    
    # Update Badge
    $readmeContent = $readmeContent -replace 'Version-.*-blue\.svg', "Version-$version-blue.svg"
    
    # Update Text (assuming similar structure to docs/README.md or at least the "New in v..." part)
    $readmeContent = $readmeContent -replace '> \*\*New in v.*\*\*: ', "> **New in v$version**: "
    
    Set-ContentUtf8 -Path $rootReadmePath -Content $readmeContent
    Write-Host "Updated README.md"
} else {
    Write-Warning "File not found: $rootReadmePath"
}

# 6. Update docs/docs.html
$docsHtmlPath = Join-Path $root "docs\docs.html"
if (Test-Path $docsHtmlPath) {
    $content = Get-ContentUtf8 -Path $docsHtmlPath
    
    # Update Title
    $content = $content -replace '<title>Landscape Toolkit v.* - Documentation</title>', "<title>Landscape Toolkit v$version - Documentation</title>"
    
    # Update Docsify Name
    $content = $content -replace "name: 'Landscape Toolkit <span class=""version-tag"">v.*</span>',", "name: 'Landscape Toolkit <span class=""version-tag"">v$version</span>',"
    
    Set-ContentUtf8 -Path $docsHtmlPath -Content $content
    Write-Host "Updated docs/docs.html"
} else {
    Write-Warning "File not found: $docsHtmlPath"
}

# 7. Update docs/index.html
$indexHtmlPath = Join-Path $root "docs\index.html"
if (Test-Path $indexHtmlPath) {
    $content = Get-ContentUtf8 -Path $indexHtmlPath
    
    # Update Title (Handle both generic and versioned patterns)
    if ($content -match '<title>Landscape Toolkit - Documentation</title>') {
        $content = $content -replace '<title>Landscape Toolkit - Documentation</title>', "<title>Landscape Toolkit v$version - Documentation</title>"
    } else {
        $content = $content -replace '<title>Landscape Toolkit v.* - Documentation</title>', "<title>Landscape Toolkit v$version - Documentation</title>"
    }
    
    # Update H1 Version Tag
    $content = $content -replace '<span class="version-tag">v.*</span>', "<span class=""version-tag"">v$version</span>"
    
    # Update Download Link Href
    $content = $content -replace 'LandscapeToolkit_v.*\.zip', "LandscapeToolkit_v$version.zip"
    
    # Update Download Button Text (English)
    $content = $content -replace '<span class="lang-en">Download v.*</span>', "<span class=""lang-en"">Download v$version</span>"
    
    # Update Download Button Text (Chinese)
    $content = $content -replace '<span class="lang-zh">下载 v.*</span>', "<span class=""lang-zh"">下载 v$version</span>"
    
    Set-ContentUtf8 -Path $indexHtmlPath -Content $content
    Write-Host "Updated docs/index.html"
} else {
    Write-Warning "File not found: $indexHtmlPath"
}

Write-Host "Version update complete! New version: $version" -ForegroundColor Green

# 8. Sync CHANGELOG.md
$rootChangelog = Join-Path $root "CHANGELOG.md"
$docsChangelog = Join-Path $root "docs\CHANGELOG.md"
if (Test-Path $rootChangelog) {
    # Copy-Item might copy encoding as is. Let's force read/write to ensure UTF-8.
    $changelogContent = Get-ContentUtf8 -Path $rootChangelog
    Set-ContentUtf8 -Path $docsChangelog -Content $changelogContent
    Write-Host "Synced CHANGELOG.md to docs/CHANGELOG.md (UTF-8)"
} else {
    Write-Warning "File not found: $rootChangelog"
}

Write-Host "Version update complete! New version: $version" -ForegroundColor Green
