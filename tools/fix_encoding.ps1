$root = "g:\CODE\landscape-toolkit"
$docs = Join-Path $root "docs"

# Function to ensure UTF-8 No BOM
function Convert-ToUtf8NoBom {
    param([string]$Path)
    
    try {
        # Read as UTF-8 (assuming it's valid UTF-8 or ASCII)
        $content = [System.IO.File]::ReadAllText($Path, [System.Text.Encoding]::UTF8)
        
        # Write as UTF-8 No BOM
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($Path, $content, $utf8NoBom)
        
        Write-Host "Processed: $Path"
    }
    catch {
        Write-Host "Error processing $Path : $_"
    }
}

# Get all .md and .html files
$files = Get-ChildItem -Path $docs -Recurse -Include *.md,*.html
$rootFiles = Get-ChildItem -Path $root -Include *.md,*.html

foreach ($file in $files) {
    Convert-ToUtf8NoBom -Path $file.FullName
}
foreach ($file in $rootFiles) {
    Convert-ToUtf8NoBom -Path $file.FullName
}

Write-Host "Encoding fix complete."

# Search for old version strings (1.2.2 or older) to align version info
Write-Host "`nSearching for old version strings (1.2.2)..."
$allFiles = $files + $rootFiles
foreach ($file in $allFiles) {
    if ($file.Name -eq "CHANGELOG.md") { continue }
    
    $content = [System.IO.File]::ReadAllText($file.FullName)
    if ($content -match "1\.2\.2") {
        Write-Host "Found '1.2.2' in $($file.Name):"
        $lines = $content -split "`r`n"
        for ($i=0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match "1\.2\.2") {
                Write-Host "  Line $($i+1): $($lines[$i].Trim())"
            }
        }
    }
}
