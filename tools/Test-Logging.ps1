[CmdletBinding()]
param([string]$AssemblyPath)
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if (-not $AssemblyPath) { $AssemblyPath = Join-Path $repo 'EHWinFormViews.App\bin\Release\setup\log4net.dll' }
$assembly = [Reflection.Assembly]::LoadFrom([IO.Path]::GetFullPath($AssemblyPath))
if ($assembly.GetName().Version -lt [version]'3.5.0.0') { throw 'Veraltete log4net-Assembly.' }

# Regression for CVE-2026-40021: attacker-controlled identity/MDC must not
# suppress an XML log record through an XML 1.0 forbidden control character.
$control = [string][char]1
$data = [log4net.Core.LoggingEventData]::new()
$data.LoggerName = 'SecurityRegression'
$data.Level = [log4net.Core.Level]::Info
$data.Message = 'Log record must survive invalid XML characters'
$data.TimeStampUtc = [DateTime]::UtcNow
$data.Identity = "user${control}name"
$data.Properties = [log4net.Util.PropertiesDictionary]::new()
$data.Properties["key${control}name"] = "value${control}text"
$event = [log4net.Core.LoggingEvent]::new($data)
foreach ($layout in @([log4net.Layout.XmlLayout]::new(), [log4net.Layout.XmlLayoutSchemaLog4j]::new())) {
    $layout.ActivateOptions()
    $writer = [IO.StringWriter]::new()
    try {
        $layout.Format($writer, $event)
        $rendered = $writer.ToString()
        $document = [xml]("<test xmlns:log4net='urn:log4net' xmlns:log4j='urn:log4j'>$rendered</test>")
        $message = $document.SelectSingleNode("//*[local-name()='message']")
        if ($null -eq $message -or $message.InnerText -ne $data.Message) { throw 'Logeintrag verloren.' }
        if ($rendered.Contains($control)) { throw 'XML enthält weiterhin unzulässige Steuerzeichen.' }
    } finally { $writer.Dispose() }
}

# Exercise the existing rolling-file configuration in an isolated directory.
$output = Join-Path $repo ('EHFleetXRechnung.Setup\staging\logging-test-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $output -Force | Out-Null
$config = [xml](Get-Content (Join-Path $repo 'EHWinFormViews\logger.config') -Raw)
foreach ($file in $config.SelectNodes('//appender/file')) { $file.SetAttribute('value', (Join-Path $output ([IO.Path]::GetFileName($file.GetAttribute('value'))))) }
$repository = [log4net.LogManager]::CreateRepository('ReleaseTest-' + [guid]::NewGuid().ToString('N'))
try {
    $errors = [log4net.Config.XmlConfigurator]::Configure($repository, $config.SelectSingleNode('//log4net'))
    if ($errors.Count -gt 0) { throw ('Fehler in logger.config: ' + ($errors -join '; ')) }
    $logger = [log4net.LogManager]::GetLogger($repository.Name, 'ReleaseSmoke')
    $encoding = @($repository.GetAppenders() | Where-Object { $_.Name -eq 'RollingFileAppender' })[0].Encoding
    $marker = 'Release logging smoke test: ÄÖÜ <invoice>'
    $logger.Info($marker)
} finally { [log4net.LogManager]::ShutdownRepository($repository.Name) }
$text = (Get-ChildItem -LiteralPath $output -Filter *.log | ForEach-Object { [IO.File]::ReadAllText($_.FullName, $encoding) }) -join "`n"
if ($text -notmatch [regex]::Escape($marker)) { throw 'RollingFileAppender hat den Logeintrag nicht geschrieben.' }
Write-Host "log4net $($assembly.GetName().Version): beide XML-Regressionstests und RollingFileAppender erfolgreich."
