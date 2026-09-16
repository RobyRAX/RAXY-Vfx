# Regenerates GUIDs for all .meta under Samples~/Basic Setup and updates references
# ONLY inside that folder (safe for package sample; avoids collision with Project Alice Claw VFX).
$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot '..\Samples~\Basic Setup' | Resolve-Path

$map = @{}
Get-ChildItem -Path $root -Recurse -Filter '*.meta' | ForEach-Object {
    $text = Get-Content $_.FullName -Raw
    if ($text -match 'guid:\s*([a-f0-9]{32})') {
        $old = $Matches[1]
        if (-not $map.ContainsKey($old)) {
            $map[$old] = [guid]::NewGuid().ToString('N')
        }
    }
}

Write-Host "Remapping $($map.Count) GUIDs under $root"

$files = Get-ChildItem -Path $root -Recurse -File
foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $changed = $false
    foreach ($entry in $map.GetEnumerator()) {
        if ($text.Contains($entry.Key)) {
            $text = $text.Replace($entry.Key, $entry.Value)
            $changed = $true
        }
    }
    if ($changed) {
        [System.IO.File]::WriteAllText($file.FullName, $text)
    }
}

Write-Host 'Done.'
