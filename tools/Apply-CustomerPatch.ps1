[CmdletBinding()]
param([Parameter(Mandatory)][string]$InstallDirectory, [switch]$VerifyOnly)
$ErrorActionPreference = 'Stop'
$target = (Resolve-Path -LiteralPath $InstallDirectory).Path.TrimEnd('\')
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'patch-manifest.json') -Raw | ConvertFrom-Json
$app = Join-Path $target 'EHFleetXRechnung.App.dll'
if (-not (Test-Path -LiteralPath $app)) { throw 'Im Ziel wurde keine Fleet-XRechnung-Installation gefunden.' }
if ([Diagnostics.FileVersionInfo]::GetVersionInfo($app).FileVersion -ne ($manifest.FromVersion + '.0')) {
    throw "Dieser Patch erfordert App-Version $($manifest.FromVersion). Für andere Versionen bitte das vollständige Setup verwenden."
}
function Resolve-Child([string]$root, [string]$relative) {
    $root = [IO.Path]::GetFullPath($root)
    if ([IO.Path]::IsPathRooted($relative)) { throw "Absoluter Paketpfad: $relative" }
    $path = [IO.Path]::GetFullPath((Join-Path $root $relative))
    if (-not $path.StartsWith($root.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) { throw "Ungültiger Paketpfad: $relative" }
    return $path
}
# Validate the entire package and baseline before changing any installed file.
$payloadRoot = Join-Path $PSScriptRoot 'payload'
foreach ($entry in $manifest.Files) {
    $source = Resolve-Child $payloadRoot $entry.Path
    $destination = Resolve-Child $target $entry.Path
    if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $entry.SHA256) { throw "Paketdatei beschädigt: $($entry.Path)" }
    if ($entry.PreviousSHA256) {
        if (-not (Test-Path -LiteralPath $destination -PathType Leaf)) { throw "Ausgangsdatei fehlt: $($entry.Path)" }
        # Customers may customize log levels. Back up their logger.config before replacing it.
        if ($entry.Path -ne 'logger.config' -and (Get-FileHash -LiteralPath $destination).Hash -ne $entry.PreviousSHA256) {
            throw "Ausgangsdatei weicht von Release $($manifest.FromVersion) ab: $($entry.Path). Vollständiges Setup verwenden."
        }
    } elseif (Test-Path -LiteralPath $destination) { throw "Unerwartete bestehende Datei: $($entry.Path)" }
    if (Test-Path -LiteralPath $destination) {
        try {
            $lock = [IO.File]::Open($destination, 'Open', 'ReadWrite', 'None')
            $lock.Dispose()
        } catch { throw "Datei gesperrt oder nicht beschreibbar: $($entry.Path). Fleet, VB6-Host und XRechnung beenden; PowerShell ggf. als Administrator starten." }
    }
}
if ($VerifyOnly) { Write-Host 'Paket und Ausgangsinstallation erfolgreich geprüft. Keine Dateien geändert.'; return }
$backup = Join-Path $target ('patch-backup\' + $manifest.FromVersion + '-to-' + $manifest.ToVersion + '-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $backup -Force | Out-Null
$changed = [Collections.Generic.List[object]]::new()
try {
    foreach ($entry in $manifest.Files) {
        $destination = Resolve-Child $target $entry.Path
        $saved = Resolve-Child $backup $entry.Path
        $existed = Test-Path -LiteralPath $destination
        if ($existed) {
            New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($saved)) -Force | Out-Null
            Copy-Item -LiteralPath $destination -Destination $saved
        }
        $changed.Add(@{ Destination=$destination; Saved=$saved; Existed=$existed })
        New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($destination)) -Force | Out-Null
        Copy-Item -LiteralPath (Resolve-Child $payloadRoot $entry.Path) -Destination $destination -Force
        if ((Get-FileHash -LiteralPath $destination).Hash -ne $entry.SHA256) { throw "Prüfsumme nach Kopieren falsch: $($entry.Path)" }
    }
} catch {
    $failure = $_
    $rollbackErrors = @()
    for ($i = $changed.Count - 1; $i -ge 0; $i--) {
        $item = $changed[$i]
        try {
            if ($item.Existed) { Copy-Item -LiteralPath $item.Saved -Destination $item.Destination -Force }
            elseif (Test-Path -LiteralPath $item.Destination) { Remove-Item -LiteralPath $item.Destination -Force }
        } catch { $rollbackErrors += $_.Exception.Message }
    }
    if ($rollbackErrors.Count) { throw "Patch fehlgeschlagen: $failure. Rücksicherung unvollständig: $($rollbackErrors -join '; '). Sicherung: $backup" }
    throw "Patch fehlgeschlagen und zurückgerollt: $failure. Sicherung: $backup"
}
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'patch-manifest.json') -Destination $backup
Write-Host "Patch $($manifest.ToVersion) erfolgreich. Sicherung: $backup"
Write-Host 'Die MSI-Produktregistrierung bleibt auf der bisherigen Version. Für Installerverwaltung, Reparatur und weitere Updates das vollständige Setup verwenden.'
