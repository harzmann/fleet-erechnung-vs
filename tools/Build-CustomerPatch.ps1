[CmdletBinding()]
param([Parameter(Mandatory)][string]$BaselineManifest)
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$setup = Join-Path $repo 'EHFleetXRechnung.Setup'
$baseline = Get-Content -LiteralPath $BaselineManifest -Raw | ConvertFrom-Json
$current = Get-Content -LiteralPath (Join-Path $setup 'Release\release-manifest.json') -Raw | ConvertFrom-Json
$previous = @{}
foreach ($entry in $baseline.Files) { $previous[$entry.Path] = $entry }
$package = Join-Path $setup ('staging\patch-' + $current.Version + '-' + [guid]::NewGuid().ToString('N'))
$payload = Join-Path $package 'payload'
New-Item -ItemType Directory -Path $payload -Force | Out-Null
$entries = foreach ($entry in $current.Files) {
    # Preserve application/database settings. logger.config must gain the new path pattern.
    if ($entry.Path -like '*.config' -and $entry.Path -ne 'logger.config') { continue }
    $old = $previous[$entry.Path]
    if ($old -and $old.SHA256 -eq $entry.SHA256) { continue }
    $source = Join-Path $setup ('staging\verify\' + $entry.Path)
    if ((Get-FileHash -LiteralPath $source).Hash -ne $entry.SHA256) { throw "Extraktion passt nicht zum Manifest: $($entry.Path)" }
    $destination = Join-Path $payload $entry.Path
    New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($destination)) -Force | Out-Null
    Copy-Item -LiteralPath $source -Destination $destination
    [pscustomobject]@{ Path=$entry.Path; SHA256=$entry.SHA256; PreviousSHA256=if ($old) { $old.SHA256 } else { $null } }
}
if (@($entries).Count -eq 0) { throw 'Keine geänderten Dateien für den Patch.' }
@{ FromVersion=$baseline.Version; ToVersion=$current.Version; Files=@($entries) } |
    ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $package 'patch-manifest.json') -Encoding UTF8
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Apply-CustomerPatch.ps1') -Destination $package
Copy-Item -LiteralPath (Join-Path $repo ('docs\release-' + $current.Version + '.md')) -Destination (Join-Path $package 'README.md')
$archive = Join-Path $setup ('Release\EHFleetXRechnung.Patch-' + $baseline.Version + '-to-' + $current.Version + '.zip')
Compress-Archive -Path (Join-Path $package '*') -DestinationPath $archive -CompressionLevel Optimal -Force
Write-Host "Kundenpatch: $archive ($(@($entries).Count) Dateien)"
