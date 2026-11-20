[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$verify = Join-Path $repo 'EHFleetXRechnung.Setup\staging\verify'
if (-not (Test-Path -LiteralPath (Join-Path $verify 'EHFleetXRechnung.Viewer.dll'))) { throw 'Zuerst Verify-Setup.ps1 ausführen.' }
& dotnet build (Join-Path $PSScriptRoot 'ValidatorLoggingSmokeTest\ValidatorLoggingSmokeTest.vbproj') -v:q
if ($LASTEXITCODE -ne 0) { throw 'Validator-Integrationstest konnte nicht gebaut werden.' }
$helper = Join-Path $verify 'ValidatorLoggingSmokeTest.dll'
if (Test-Path -LiteralPath $helper) { throw 'Unerwartete bestehende Testdatei.' }
try {
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidatorLoggingSmokeTest\bin\Debug\net8.0-windows\ValidatorLoggingSmokeTest.dll') -Destination $helper
    & dotnet exec --depsfile (Join-Path $verify 'EHFleetXRechnung.App.deps.json') --runtimeconfig (Join-Path $verify 'EHFleetXRechnung.App.runtimeconfig.json') $helper
    if ($LASTEXITCODE -ne 0) { throw 'Validator-Integrationstest fehlgeschlagen.' }
} finally {
    if (Test-Path -LiteralPath $helper) { Remove-Item -LiteralPath $helper }
}
