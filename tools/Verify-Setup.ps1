[CmdletBinding()]
param([switch]$SkipExtraction)
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$setup = Join-Path $repo 'EHFleetXRechnung.Setup'
$release = Join-Path $setup 'Release'
$msi = Join-Path $release 'EHFleetXRechnung.Setup.msi'
$version = ([xml](Get-Content (Join-Path $repo 'EHWinFormViews.App\EHFleetXRechnung.App.vbproj') -Raw)).Project.PropertyGroup.Version | Where-Object { $_ }
$installer = New-Object -ComObject WindowsInstaller.Installer
$db = $installer.OpenDatabase($msi, 0)
function Read-MsiRows([string]$query, [int]$columns) {
    $view = $db.OpenView($query)
    try {
        $null = $view.Execute()
        while ($record = $view.Fetch()) {
            $values = for ($i=1; $i -le $columns; $i++) { $record.StringData($i) }
            ,$values
        }
    } finally { $null = $view.Close() }
}
$properties = @{}
foreach ($row in (Read-MsiRows 'SELECT `Property`, `Value` FROM `Property`' 2)) { $properties[$row[0]] = $row[1] }
if ($properties.ProductVersion -ne $version) { throw "MSI-Version $($properties.ProductVersion) statt $version" }
if ($properties.UpgradeCode -ne '{3021B662-8FC1-4CA9-BF36-09CF6EBB31D8}') { throw 'MSI-UpgradeCode wurde geändert: bestehende Installationen würden nicht aktualisiert.' }
if ($properties.REINSTALLMODE -ne 'emus') { throw 'MSI muss auch DLLs mit gleicher Dateiversion aktualisieren (REINSTALLMODE=emus).' }
$shortcuts = @(Read-MsiRows 'SELECT `Name`, `Target` FROM `Shortcut`' 2)
if ($shortcuts.Count -ne 2) { throw 'Unerwartete Anzahl Programmverknüpfungen.' }
foreach ($shortcut in $shortcuts) {
    $expectedTarget = if ($shortcut[0].EndsWith('Export')) { '[TARGETDIR]EHFleetXRechnung.App.exe' } else { '[TARGETDIR]FleetXRechnungHybridViewer.exe' }
    if ($shortcut[1] -ne $expectedTarget) { throw "Falsches Verknüpfungsziel: $($shortcut[0])" }
}
$directories = @{}
foreach ($row in (Read-MsiRows 'SELECT `Directory`, `Directory_Parent`, `DefaultDir` FROM `Directory`' 3)) { $directories[$row[0]] = @($row[1], $row[2]) }
function Resolve-MsiDirectory([string]$id) {
    if ($id -eq 'TARGETDIR') { return '' }
    if (-not $directories.ContainsKey($id)) { throw "Unbekanntes MSI-Verzeichnis: $id" }
    $row = $directories[$id]
    $name = ($row[1].Split(':')[0].Split('|') | Select-Object -Last 1)
    $parent = Resolve-MsiDirectory $row[0]
    if ($name -eq '.') { return $parent }
    if ($parent) { return "$parent\$name" }
    return $name
}
$components = @{}
foreach ($row in (Read-MsiRows 'SELECT `Component`, `Directory_` FROM `Component`' 2)) { $components[$row[0]] = $row[1] }
$files = @{}
foreach ($row in (Read-MsiRows 'SELECT `FileName`, `Component_`, `Version`, `FileSize` FROM `File`' 4)) {
    $name = $row[0].Split('|') | Select-Object -Last 1
    $folder = Resolve-MsiDirectory $components[$row[1]]
    $target = if ($folder) { "$folder\$name" } else { $name }
    if ($files.ContainsKey($target)) { throw "Doppelter MSI-Zielpfad: $target" }
    $files[$target] = @{ Version=$row[2]; Size=[long]$row[3] }
}
foreach ($name in @('EHFleetXRechnung.App.exe','EHFleetXRechnung.App.dll','EHFleetXRechnung.Viewer.dll','EHFleetXRechnung.CliTester.exe','EHFleetXRechnung.CliTester.dll')) {
    if (-not $files.ContainsKey($name) -or $files[$name].Version -ne "$version.0") { throw "Fehlende oder falsche Version: $name" }
}
$loggingVersion = ([xml](Get-Content (Join-Path $repo 'EHWinFormViews\EHFleetXRechnung.Viewer.vbproj') -Raw)).Project.ItemGroup.PackageReference |
    Where-Object { $_.Include -eq 'log4net' } | Select-Object -ExpandProperty Version
if (-not $files.ContainsKey('log4net.dll') -or $files['log4net.dll'].Version -ne "$loggingVersion.0") {
    throw "MSI enthält nicht die erwartete log4net-Version $loggingVersion."
}
$pkcsVersion = ([xml](Get-Content (Join-Path $repo 'EHWinFormViews\EHFleetXRechnung.Viewer.vbproj') -Raw)).Project.ItemGroup.PackageReference |
    Where-Object { $_.Include -eq 'System.Security.Cryptography.Pkcs' } | Select-Object -ExpandProperty Version
$assets = Get-Content (Join-Path $repo 'EHWinFormViews\obj\project.assets.json') -Raw | ConvertFrom-Json -AsHashtable
foreach ($target in $assets.targets.Values) {
    if (-not $target.ContainsKey("System.Security.Cryptography.Pkcs/$pkcsVersion")) { throw 'Pkcs-Sicherheitspatch ist nicht für alle Ziel-Frameworks aufgelöst.' }
}
$packageRoot = @($assets.packageFolders.Keys)[0]
$pkcsAssembly = Join-Path $packageRoot "system.security.cryptography.pkcs\$pkcsVersion\runtimes\win\lib\net8.0\System.Security.Cryptography.Pkcs.dll"
$pkcsFileVersion = [Diagnostics.FileVersionInfo]::GetVersionInfo($pkcsAssembly).FileVersion
if (-not $files.ContainsKey('System.Security.Cryptography.Pkcs.dll') -or $files['System.Security.Cryptography.Pkcs.dll'].Version -ne $pkcsFileVersion) {
    throw "MSI enthält nicht den erwarteten Pkcs-Sicherheitspatch $pkcsVersion ($pkcsFileVersion)."
}
if (-not $files.ContainsKey('validator\validator-1.5.2-standalone.jar')) { throw 'Aktueller Validator fehlt.' }
if ($files.ContainsKey('validator\validationtool-1.5.0-standalone.jar')) { throw 'Veralteter Validator wird noch ausgeliefert.' }
$launchConditions = Read-MsiRows 'SELECT `Condition`, `Description` FROM `LaunchCondition`' 2
if ($properties.VSDNetCoreVersion -notlike '8.0.*' -or $properties.VSDNetCoreArch -ne 'x86' -or $properties.VSDNetCoreRuntime -ne 'Microsoft.WindowsDesktop.App') {
    throw "Falsche MSI-Laufzeitprüfung: $($properties.VSDNetCoreVersion) / $($properties.VSDNetCoreArch) / $($properties.VSDNetCoreRuntime)"
}
$runtimeCheck = @(Read-MsiRows 'SELECT `Action`, `Type` FROM `CustomAction`' 2 | Where-Object { $_[0] -eq 'DIRCA_CheckNETCore' })
if ($runtimeCheck.Count -ne 1) { throw '.NET-Laufzeitprüfung fehlt im MSI.' }
$setupDefinition = Get-Content (Join-Path $setup 'EHFleetXRechnung.Setup.vdproj') -Raw
if ($setupDefinition -match '\.NETFramework,Version|DesktopRuntime\.8\.0\.x64') { throw 'Falsche Bootstrapper-Voraussetzungen.' }
if (-not (Test-Path -LiteralPath (Join-Path $release 'setup.exe'))) { throw 'Bootstrapper setup.exe fehlt.' }
$bootstrapper = [Text.Encoding]::Unicode.GetString([IO.File]::ReadAllBytes((Join-Path $release 'setup.exe')))
if ($bootstrapper -notmatch 'Microsoft.NetCore.DesktopRuntime.8.0.x86' -or $bootstrapper -match 'Microsoft.NetCore.DesktopRuntime.8.0.x64|\.NETFramework,Version=v4') {
    throw 'Bootstrapper enthält falsche Laufzeitpakete.'
}
$runtimePackage = Get-Content (Join-Path $PSScriptRoot 'desktop-runtime.json') -Raw | ConvertFrom-Json
$runtimeInstaller = Join-Path $release ($runtimePackage.folder + '\' + $runtimePackage.file)
if (-not $bootstrapper.Contains($runtimePackage.file) -or -not (Test-Path -LiteralPath $runtimeInstaller)) { throw 'Offline-Laufzeitpaket fehlt oder passt nicht zum Bootstrapper.' }
if ((Get-FileHash -LiteralPath $runtimeInstaller -Algorithm SHA512).Hash -ine $runtimePackage.sha512) { throw 'Offline-Laufzeitpaket hat eine falsche SHA-512-Prüfsumme.' }
$runtimeSignature = Get-AuthenticodeSignature -LiteralPath $runtimeInstaller
if ($runtimeSignature.Status -ne 'Valid' -or $runtimeSignature.SignerCertificate.Subject -notmatch 'O=Microsoft Corporation') { throw 'Offline-Laufzeitpaket hat keine gültige Microsoft-Signatur.' }

if (-not $SkipExtraction) {
    # Administrative extraction only: no product registration or application installation.
    $extracted = [IO.Path]::GetFullPath((Join-Path $setup 'staging\verify'))
    if (-not $extracted.StartsWith($setup + '\staging\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Unsicheres Extraktionsverzeichnis.' }
    if (Test-Path -LiteralPath $extracted) { Remove-Item -LiteralPath $extracted -Recurse -Force }
    New-Item -ItemType Directory -Path $extracted -Force | Out-Null
    $extractLog = Join-Path $setup 'extract-release.log'
    $process = Start-Process msiexec.exe -ArgumentList @('/a', "`"$msi`"", '/qn', "TARGETDIR=`"$extracted`"", '/L*v', "`"$extractLog`"") -WindowStyle Hidden -Wait -PassThru
    if ($process.ExitCode -ne 0) { throw "MSI-Extraktion fehlgeschlagen ($($process.ExitCode)). Siehe $extractLog" }
    $expected = @{}
    foreach ($relative in @('EHWinFormViews.App\bin\Release\setup','EHFleetXRechnung.CliTester\bin\Release\setup')) {
        $publish = Join-Path $repo $relative
        if (-not (Test-Path -LiteralPath $publish)) { throw "Publish-Ausgabe fehlt: $publish" }
        foreach ($file in (Get-ChildItem -LiteralPath $publish -File -Recurse)) {
            $target = $file.FullName.Substring($publish.Length+1)
            if ($expected.ContainsKey($target)) { throw "Doppelte Publish-Datei: $target" }
            $expected[$target] = $file.FullName
        }
    }
    foreach ($file in (Get-ChildItem (Join-Path $repo 'reports') -Filter *.mrt -File)) { $expected['reports\'+$file.Name] = $file.FullName }
    $expected['ehfleet.ico'] = Join-Path $repo 'EHWinFormViews.App\ehfleet.ico'
    $expected['FleetXRechnungHybridViewer.exe'] = Join-Path $setup 'staging\FleetXRechnungHybridViewer.exe'
    # WebView2's multi-target build publishes the same native loader twice.
    # Installer Projects keeps its runtime-specific destination. Accept this
    # single deduplication only when the omitted copy is byte-identical.
    $deduplicated = @()
    $loader = 'runtimes\win-x86\native\WebView2Loader.dll'
    if ($expected.ContainsKey('WebView2Loader.dll') -and -not $files.ContainsKey('WebView2Loader.dll') -and $files.ContainsKey($loader)) {
        if ((Get-FileHash -LiteralPath $expected['WebView2Loader.dll']).Hash -ne (Get-FileHash -LiteralPath $expected[$loader]).Hash) {
            throw 'Die beiden WebView2-Loader stimmen nicht überein.'
        }
        $expected.Remove('WebView2Loader.dll')
        $deduplicated = @(@{ Omitted='WebView2Loader.dll'; InstalledAs=$loader })
    }
    if ($files.Count -ne $expected.Count) { throw "MSI enthält $($files.Count), Publish enthält $($expected.Count) Dateien." }
    $manifest = foreach ($target in ($expected.Keys | Sort-Object)) {
        if (-not $files.ContainsKey($target)) { throw "Datei fehlt im MSI: $target" }
        $installedFile = Join-Path $extracted $target
        if (-not (Test-Path -LiteralPath $installedFile)) { throw "Extrahierte Datei fehlt: $target" }
        $hash = (Get-FileHash -LiteralPath $expected[$target] -Algorithm SHA256).Hash
        if ($hash -ne (Get-FileHash -LiteralPath $installedFile -Algorithm SHA256).Hash) { throw "MSI-Inhalt weicht vom Build ab: $target" }
        [pscustomobject]@{ Path=$target; SHA256=$hash; Version=$files[$target].Version; Size=$files[$target].Size }
    }
    # Check against the local Release build, not a cached Debug reference.
    $library = Join-Path $repo '..\ehfleet_classlibrary\bin\Release\net8.0-windows\ehfleet_classlibrary.dll'
    if ((Get-FileHash -LiteralPath $library).Hash -ne (Get-FileHash -LiteralPath (Join-Path $extracted 'ehfleet_classlibrary.dll')).Hash) { throw 'Klassenbibliothek stammt nicht aus dem aktuellen Release-Build.' }
    foreach ($name in @('EHFleetXRechnung.App.exe','EHFleetXRechnung.CliTester.exe','EHFleetXRechnung.Viewer.comhost.dll')) {
        $reader = [IO.BinaryReader]::new([IO.File]::OpenRead((Join-Path $extracted $name)))
        try {
            $reader.BaseStream.Position = 0x3c
            $header = $reader.ReadInt32()
            $reader.BaseStream.Position = $header + 4
            if ($reader.ReadUInt16() -ne 0x14c) { throw "Datei hat nicht die erwartete x86-Architektur: $name" }
        } finally { $reader.Dispose() }
    }
    $cliOutput = & (Join-Path $extracted 'EHFleetXRechnung.CliTester.exe') 2>&1
    if ($LASTEXITCODE -ne 0 -or ($cliOutput -join "`n") -notmatch 'EHFleetXRechnung CLI Tester') { throw 'CLI-Smoke-Test fehlgeschlagen.' }
    & (Join-Path $PSScriptRoot 'Test-Logging.ps1') -AssemblyPath (Join-Path $extracted 'log4net.dll')
    $report = [ordered]@{
        Version=$version; VerifiedUtc=[DateTime]::UtcNow.ToString('o'); FileCount=$files.Count
        SourceCommit=(& git -C $repo rev-parse HEAD)
        ClassLibraryCommit=(& git -C (Join-Path $repo '..\ehfleet_classlibrary') rev-parse HEAD)
        ProductCode=$properties.ProductCode; UpgradeCode=$properties.UpgradeCode
        MsiSHA256=(Get-FileHash -LiteralPath $msi).Hash
        SetupSHA256=(Get-FileHash -LiteralPath (Join-Path $release 'setup.exe')).Hash
        BootstrapperRuntime=@{ Version=$runtimePackage.version; File=($runtimePackage.folder + '/' + $runtimePackage.file); SHA512=$runtimePackage.sha512; Offline=$true }
        LaunchConditions=@($launchConditions | ForEach-Object { $_ -join ' | ' })
        Runtime=@{ Framework=$properties.VSDNetCoreRuntime; Version=$properties.VSDNetCoreVersion; Architecture=$properties.VSDNetCoreArch }
        DeduplicatedFiles=$deduplicated
        Files=@($manifest)
    }
    $report | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $release 'release-manifest.json') -Encoding UTF8
}
Write-Host "Release $version geprüft: $($files.Count) eindeutige MSI-Dateien."
