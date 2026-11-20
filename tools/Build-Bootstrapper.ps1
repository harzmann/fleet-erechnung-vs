[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$setup = Join-Path $repo 'EHFleetXRechnung.Setup'
$runtime = Get-Content (Join-Path $PSScriptRoot 'desktop-runtime.json') -Raw | ConvertFrom-Json
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$vs = & $vswhere -products '*' -requires Microsoft.Component.MSBuild -property installationPath |
    Where-Object { Test-Path -LiteralPath (Join-Path $_ 'MSBuild\Microsoft\VisualStudio\BootstrapperPackages\net8desktopruntime_x86\product.xml') } | Select-Object -First 1
if (-not $vs) { throw '.NET-8-x86-Bootstrappervorlage fehlt.' }
$sdk = Join-Path ${env:ProgramFiles(x86)} 'Microsoft SDKs\ClickOnce Bootstrapper'
$private = Join-Path $setup ('staging\bootstrapper-' + $runtime.version)
New-Item -ItemType Directory -Path $private -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $sdk 'Engine') -Destination $private -Recurse -Force
Copy-Item -LiteralPath (Join-Path $sdk 'Schemas') -Destination $private -Recurse -Force
$packages = Join-Path $private 'Packages'
New-Item -ItemType Directory -Path $packages -Force | Out-Null
$template = Join-Path $vs 'MSBuild\Microsoft\VisualStudio\BootstrapperPackages\net8desktopruntime_x86'
$package = Join-Path $packages $runtime.folder
if (-not (Test-Path -LiteralPath $package)) { Copy-Item -LiteralPath $template -Destination $package -Recurse }
$product = Join-Path $package 'product.xml'
$xml = [xml](Get-Content -LiteralPath (Join-Path $template 'product.xml') -Raw)
$ns = New-Object Xml.XmlNamespaceManager($xml.NameTable)
$ns.AddNamespace('b','http://schemas.microsoft.com/developer/2004/01/bootstrapper')
$file = $xml.SelectSingleNode('//b:PackageFile[@HomeSite]', $ns)
$oldFile = $file.GetAttribute('Name')
$oldVersion = [regex]::Match($oldFile, '\d+\.\d+\.\d+').Value
$text = $xml.OuterXml.Replace($oldVersion, $runtime.version)
$text = $text.Replace(' PublicKey="0"', '')
[IO.File]::WriteAllText($product, $text, [Text.Encoding]::UTF8)
foreach ($culture in (Get-ChildItem -LiteralPath $template -Directory)) {
    $source = Join-Path $culture.FullName 'package.xml'
    if (Test-Path -LiteralPath $source) {
        $destination = Join-Path $package ($culture.Name + '\package.xml')
        [IO.File]::WriteAllText($destination, (Get-Content -LiteralPath $source -Raw).Replace($oldVersion, $runtime.version), [Text.Encoding]::UTF8)
    }
}
$installer = Join-Path $package $runtime.file
if (-not (Test-Path -LiteralPath $installer)) { Invoke-WebRequest -Uri $runtime.url -OutFile $installer -TimeoutSec 180 }
if ((Get-FileHash -LiteralPath $installer -Algorithm SHA512).Hash -ine $runtime.sha512) { throw 'Microsoft-Laufzeit: SHA-512 stimmt nicht mit dem Release-Metadatum überein.' }
$signature = Get-AuthenticodeSignature -LiteralPath $installer
if ($signature.Status -ne 'Valid' -or $signature.SignerCertificate.Subject -notmatch 'O=Microsoft Corporation') { throw 'Microsoft-Laufzeit: ungültige Herstellersignatur.' }
& (Join-Path $vs 'MSBuild\Current\Bin\MSBuild.exe') (Join-Path $PSScriptRoot 'Bootstrapper.proj') /nologo /v:minimal "/p:BootstrapperDirectory=$private" "/p:ReleaseDirectory=$(Join-Path $setup 'Release')"
if ($LASTEXITCODE -ne 0) { throw 'Offline-Bootstrapper konnte nicht erstellt werden.' }
Write-Host "Offline-Bootstrapper: Desktop Runtime $($runtime.version) x86, SHA-512 und Microsoft-Signatur geprüft."
