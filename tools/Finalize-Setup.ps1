[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$msi = Join-Path $PSScriptRoot '..\EHFleetXRechnung.Setup\Release\EHFleetXRechnung.Setup.msi'
$installer = New-Object -ComObject WindowsInstaller.Installer
$db = $installer.OpenDatabase([IO.Path]::GetFullPath($msi), 1)
# RID-specific assemblies and the generated COM host can change bytes without
# changing their file version. Replace equal versions during normal upgrades too.
# https://learn.microsoft.com/windows/win32/msi/reinstallmode
function Invoke-MsiSql([string]$sql) {
    $view = $db.OpenView($sql)
    try { $null = $view.Execute() } finally {
        $null = $view.Close()
        [Runtime.InteropServices.Marshal]::FinalReleaseComObject($view) | Out-Null
    }
}
Invoke-MsiSql "DELETE FROM ``Property`` WHERE ``Property`` = 'REINSTALLMODE'"
Invoke-MsiSql "INSERT INTO ``Property`` (``Property``, ``Value``) VALUES ('REINSTALLMODE', 'emus')"
# Installer Projects advertises Publish Items through the managed DLL keypath.
# Windows cannot launch that DLL as an application. Use explicit apphost targets.
$view = $db.OpenView('SELECT `Shortcut`, `Name` FROM `Shortcut`')
$null = $view.Execute()
$shortcuts = @()
while ($record = $view.Fetch()) { $shortcuts += @{ Id=$record.StringData(1); Name=$record.StringData(2) } }
$null = $view.Close()
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($view) | Out-Null
foreach ($shortcut in $shortcuts) {
    $target = if ($shortcut.Name.EndsWith('Export')) { '[TARGETDIR]EHFleetXRechnung.App.exe' }
              elseif ($shortcut.Name.EndsWith('Import')) { '[TARGETDIR]FleetXRechnungHybridViewer.exe' }
              else { throw "Unexpected shortcut: $($shortcut.Name)" }
    Invoke-MsiSql "UPDATE ``Shortcut`` SET ``Target`` = '$target' WHERE ``Shortcut`` = '$($shortcut.Id)'"
}
$null = $db.Commit()
$definition = Get-Content (Join-Path $PSScriptRoot '..\EHFleetXRechnung.Setup\EHFleetXRechnung.Setup.vdproj') -Raw
$packageCode = [regex]::Match($definition, '"PackageCode" = "8:(\{[^}]+\})"').Groups[1].Value
$summary = $db.SummaryInformation(1)
$summary.Property(9) = $packageCode
$summary.Persist()
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($summary) | Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($db) | Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($installer) | Out-Null
