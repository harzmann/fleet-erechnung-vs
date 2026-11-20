[CmdletBinding()]
param(
    [string]$HybridViewerPath = "$env:USERPROFILE\PycharmProjects\EHFleetXRechnung.HybridPdfViewer\dist\FleetXRechnungHybridViewer.exe",
    [string]$RestoreSources
)

$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$setup = Join-Path $repo 'EHFleetXRechnung.Setup'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$vs = & $vswhere -products '*' -requires Microsoft.Component.MSBuild -property installationPath |
    Where-Object { Test-Path -LiteralPath (Join-Path $_ 'Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe') } |
    Select-Object -First 1
if (-not $vs) { throw 'Visual Studio mit MSBuild und Installer Projects wird benötigt.' }
$devenv = Join-Path $vs 'Common7\IDE\devenv.com'
$disableOutOfProc = Join-Path $vs 'Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe'
if (-not (Test-Path -LiteralPath $disableOutOfProc)) { throw 'Visual Studio Installer Projects fehlt.' }
if (-not (Test-Path -LiteralPath $HybridViewerPath -PathType Leaf)) {
    throw "Hybrid-PDF-Viewer fehlt: $HybridViewerPath. Bitte -HybridViewerPath angeben."
}
if (-not (Test-Path -LiteralPath (Join-Path $repo '..\ehfleet_classlibrary\ehfleet_classlibrary.vbproj'))) {
    throw 'Der Quellcode von ehfleet_classlibrary muss im benachbarten Repository liegen.'
}

$oldRestoreSources = $env:RestoreSources
Push-Location $repo
try {
    if ($RestoreSources) { $env:RestoreSources = $RestoreSources }
    # Delete only the two generated publish folders, never arbitrary caller paths.
    foreach ($relative in @('EHWinFormViews.App\bin\Release\setup', 'EHFleetXRechnung.CliTester\bin\Release\setup')) {
        $generated = [IO.Path]::GetFullPath((Join-Path $repo $relative))
        if (-not $generated.StartsWith($repo + '\', [StringComparison]::OrdinalIgnoreCase)) {
            throw "Publish-Verzeichnis außerhalb des Repositorys: $generated"
        }
        if (Test-Path -LiteralPath $generated) { Remove-Item -LiteralPath $generated -Recurse -Force }
    }
    New-Item -ItemType Directory -Path (Join-Path $setup 'staging') -Force | Out-Null
    Copy-Item -LiteralPath $HybridViewerPath -Destination (Join-Path $setup 'staging\FleetXRechnungHybridViewer.exe') -Force
    Push-Location $vs
    try {
        & $disableOutOfProc
        if ($LASTEXITCODE -ne 0) { throw 'Installer-Projects-Kommandozeilenbuild konnte nicht eingerichtet werden.' }
    } finally { Pop-Location }

    $log = Join-Path $setup 'build-release.log'
    # devenv /Out appends; keep this report limited to the current build.
    Set-Content -LiteralPath $log -Value '' -Encoding UTF8
    # Rebuild includes the local class library and prevents packaging stale Debug binaries.
    & $devenv (Join-Path $repo 'EHFleet_VB6_GUI.sln') /Rebuild 'Release|x86' /Project EHFleetXRechnung.Setup /Out $log
    if ($LASTEXITCODE -ne 0) { throw "Setup-Build fehlgeschlagen. Siehe $log" }
    & (Join-Path $PSScriptRoot 'Finalize-Setup.ps1')
    & (Join-Path $PSScriptRoot 'Build-Bootstrapper.ps1')
    & (Join-Path $PSScriptRoot 'Verify-Setup.ps1')
    $version = ([xml](Get-Content (Join-Path $repo 'EHWinFormViews.App\EHFleetXRechnung.App.vbproj') -Raw)).Project.PropertyGroup.Version | Where-Object { $_ }
    $release = Join-Path $setup 'Release'
    $archive = Join-Path $release "EHFleetXRechnung.Setup-$version.zip"
    Compress-Archive -LiteralPath @(
        (Join-Path $release 'setup.exe'),
        (Join-Path $release 'EHFleetXRechnung.Setup.msi'),
        (Join-Path $release 'release-manifest.json'),
        (Join-Path $release 'net8desktopruntime_x86'),
        (Join-Path $setup 'README.md'),
        (Join-Path $repo ('docs\release-' + $version + '.md'))
    ) -DestinationPath $archive -CompressionLevel NoCompression -Force
    Write-Host "Auslieferungspaket: $archive"
} finally {
    $env:RestoreSources = $oldRestoreSources
    Pop-Location
}
