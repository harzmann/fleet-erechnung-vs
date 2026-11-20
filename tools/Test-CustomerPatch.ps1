[CmdletBinding()]
param([Parameter(Mandatory)][string]$PatchArchive, [Parameter(Mandatory)][string]$BaselineDirectory)
$ErrorActionPreference = 'Stop'
$testRoot = Join-Path $env:TEMP ('EHFleetPatchTest-' + [guid]::NewGuid().ToString('N'))
$package = Join-Path $testRoot 'package'
$target = Join-Path $testRoot 'installed'
Expand-Archive -LiteralPath $PatchArchive -DestinationPath $package
New-Item -ItemType Directory -Path $target -Force | Out-Null
Copy-Item -Path (Join-Path $BaselineDirectory '*') -Destination $target -Recurse
$manifest = Get-Content -LiteralPath (Join-Path $package 'patch-manifest.json') -Raw | ConvertFrom-Json
$script = Join-Path $package 'Apply-CustomerPatch.ps1'
$app = Join-Path $target 'EHFleetXRechnung.App.dll'
$originalApp = (Get-FileHash -LiteralPath $app).Hash
$config = Join-Path $target 'EHFleetXRechnung.App.dll.config'
Add-Content -LiteralPath $config -Value '<!-- customer settings marker -->'
$configHash = (Get-FileHash -LiteralPath $config).Hash
Add-Content -LiteralPath (Join-Path $target 'logger.config') -Value '<!-- customer log settings marker -->'
$loggerHash = (Get-FileHash -LiteralPath (Join-Path $target 'logger.config')).Hash
function Expect-Rejection([scriptblock]$action, [string]$message) {
    $rejected = $false
    try { & $action } catch { $rejected = $true }
    if (-not $rejected) { throw "Kein Abbruch bei $message" }
    if ((Get-FileHash -LiteralPath $app).Hash -ne $originalApp) { throw "App wurde bei $message verändert." }
    Write-Host "PASS $message"
}
& $script -InstallDirectory $target -VerifyOnly
$first = $manifest.Files[0]
$source = Join-Path $package ('payload\' + $first.Path)
$sourceBytes = [IO.File]::ReadAllBytes($source)
try {
    [IO.File]::AppendAllText($source, 'tampered')
    Expect-Rejection { & $script -InstallDirectory $target } 'beschädigtes Paket abgewiesen'
} finally { [IO.File]::WriteAllBytes($source, $sourceBytes) }
$lock = [IO.File]::Open($app, 'Open', 'Read', 'None')
try {
    $rejected = $false
    try { & $script -InstallDirectory $target } catch { $rejected = $true }
    if (-not $rejected) { throw 'Gesperrte Datei wurde nicht erkannt.' }
    Write-Host 'PASS gesperrte Datei abgewiesen'
} finally { $lock.Dispose() }

# Inject a copy failure after a successful replacement and verify rollback.
$global:ehFleetPatchFailureTarget = [IO.Path]::GetFullPath((Join-Path $target $manifest.Files[1].Path))
$global:ehFleetPatchFailureTriggered = $false
function global:Copy-Item {
    param([string]$LiteralPath, [string]$Destination, [switch]$Force)
    if ($Destination -eq $global:ehFleetPatchFailureTarget -and -not $global:ehFleetPatchFailureTriggered) {
        $global:ehFleetPatchFailureTriggered = $true
        throw 'Injected patch copy failure'
    }
    Microsoft.PowerShell.Management\Copy-Item -LiteralPath $LiteralPath -Destination $Destination -Force:$Force
}
try {
    Expect-Rejection { & $script -InstallDirectory $target } 'Rücknahme nach Kopierfehler'
} finally {
    Remove-Item Function:\Copy-Item
    Remove-Variable ehFleetPatchFailureTarget,ehFleetPatchFailureTriggered -Scope Global
}
foreach ($entry in $manifest.Files) {
    $expected = if ($entry.Path -eq 'logger.config') { $loggerHash } else { $entry.PreviousSHA256 }
    $destination = Join-Path $target $entry.Path
    if ($expected -and (Get-FileHash -LiteralPath $destination).Hash -ne $expected) { throw "Rücknahme unvollständig: $($entry.Path)" }
    if (-not $expected -and (Test-Path -LiteralPath $destination)) { throw "Neue Datei nach Rücknahme vorhanden: $($entry.Path)" }
}
& $script -InstallDirectory $target
foreach ($entry in $manifest.Files) {
    if ((Get-FileHash -LiteralPath (Join-Path $target $entry.Path)).Hash -ne $entry.SHA256) { throw "Falscher Patch-Inhalt: $($entry.Path)" }
}
if ((Get-FileHash -LiteralPath $config).Hash -ne $configHash) { throw 'Kundenkonfiguration überschrieben.' }
$backups = @(Get-ChildItem -LiteralPath (Join-Path $target 'patch-backup') -Directory)
if (-not @($backups | Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'logger.config') } |
    Where-Object { (Get-FileHash -LiteralPath (Join-Path $_.FullName 'logger.config')).Hash -eq $loggerHash }).Count) { throw 'Kundenspezifisches logger.config nicht gesichert.' }
Write-Host "PASS Patch angewendet, $($manifest.Files.Count) SHA-256 korrekt, Kundenkonfiguration erhalten und logger.config gesichert."
Write-Host "Testdateien: $testRoot"
