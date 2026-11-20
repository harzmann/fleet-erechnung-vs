# Release-Setup

Das Setup 1.1.9 enthält App, Viewer und CLI-Tester aus dem aktuellen Release-Build,
den KoSIT-Validator 1.5.2 mit der Konfiguration aus `EHWinFormViews/validator`,
die vier Rechnungsvorlagen und den extern erstellten Hybrid-PDF-Viewer.
Die Versionsnummer ist gegenüber 1.1.8 erhöht; ein neuer ProductCode bei
unverändertem UpgradeCode ermöglicht das Update bestehender Installationen.

Version 1.1.6 aktualisiert log4net von 3.0.4 auf 3.5.0. Damit ist insbesondere
die gemeldete XML-Logging-Lücke CVE-2026-40021 / GHSA-4f7c-pmjv-c25w behoben.
Die Paketprüfung stellt sicher, dass auch das MSI die konfigurierte neue
log4net-DLL enthält. Referenzen: [Apache-Sicherheitshinweise](https://logging.apache.org/security.html)
und [Release Notes](https://logging.apache.org/log4net/release-notes.html).

Version 1.1.8 legt `System.Security.Cryptography.Pkcs` direkt auf 8.0.2 fest,
statt die transitive Anforderung 8.0.1 aus SqlClient zu übernehmen. Dies behebt
CVE-2026-84008 / GHSA-846f-f764-76mx und CVE-2026-70162 / GHSA-2fmv-rv2c-59x7
für net48 und net8.0-windows. Der ergänzende NuGet-Audit des Viewers einschließlich
transitiver Abhängigkeiten meldet keine bekannten verwundbaren Pakete.
Der vollständige lokale Audit-Bericht liegt in `EHFleetXRechnung.Setup/package-audit.json`.
Die benachbarte Klassenbibliothek verwendet in ihrem eigenständigen Paketgraphen
weiterhin die alte SqlClient-Abhängigkeit; im Viewer/App-Graphen wird sie durch
den direkten Sicherheitspatch übersteuert.

## Änderungen in 1.1.9

Beschreibbare Ausweichpfade für App-/SMTP-Logs und Validator-Berichte,
Exportkorrektur bei fehlenden Formatparametern (Klassenbibliothek 1.3.11.1),
Abbruch der Validierung nach XML-Fehlern und Übernahme der Benutzereinstellungen.
Details, Kundenpatch und Installationsanleitung: `docs/release-1.1.9.md`
(im Setup-ZIP als `release-1.1.9.md`).

## Voraussetzungen auf dem Zielrechner

- Windows x64: Die Fleet-Komponenten laufen als x86 für die VB6-/COM-Anbindung.
  Der separate Hybrid-PDF-Viewer ist ein x64-Programm.
- .NET 8 **Desktop Runtime x86**, auch unter Windows x64. `setup.exe` prüft diese
  Voraussetzung und installiert bei Bedarf die mitgelieferte Microsoft-Laufzeit
  8.0.33. Der Ordner `net8desktopruntime_x86` muss neben `setup.exe` bleiben;
  die Installation benötigt dann keinen Online-Download. Das MSI enthält
  zusätzlich eine Laufzeitprüfung.
- Java 8 oder höher für die Rechnungsvalidierung. Java wird über `JRE_HOME`,
  `JAVA_HOME` oder `PATH` gesucht. Die Anwendung nutzt den gefundenen vollständigen
  Pfad. Java wird nicht mitinstalliert; fehlendes Java wird beim Validieren
  gemeldet. Der frühere pauschale Installationsblocker auf einen einzelnen
  JavaSoft-1.8-Registrierungsschlüssel ist entfernt.
- Vorhandene Fleet-Konfiguration und eine zum Programmstand passende Datenbank.
  Dieses Setup führt keine Datenbankmigration durch.

.NET Framework 4.x und .NET Desktop Runtime x64 sind keine Voraussetzungen für
die ausgelieferten Fleet-x86-Komponenten. Die Desktop Runtime enthält auch die
Core Runtime, die der CLI-Tester benötigt.

## Erstellen

Benötigt werden Visual Studio 2022 mit **Visual Studio Installer Projects**, das
.NET-8-SDK, das Bootstrapperpaket `Microsoft.NetCore.DesktopRuntime.8.0.x86`,
die NuGet-Pakete einschließlich gültiger Telerik-Lizenz sowie das benachbarte
Repository `../ehfleet_classlibrary`. Diese Klassenbibliothek ist in der
Projektmappe eingebunden und wird in Release gebaut. Ein zusätzlicher, alter
NuGet-Verweis auf dieselbe Bibliothek wird nicht verwendet.

Aus dem Repository-Verzeichnis:

```powershell
./tools/Build-Setup.ps1 -HybridViewerPath 'C:\Pfad\FleetXRechnungHybridViewer.exe'
```

Ohne Parameter wird der bisherige lokale PyCharm-Ausgabepfad unterhalb von
`$env:USERPROFILE` verwendet. Der Viewer ist ein externes Build-Artefakt und wird
unverändert übernommen; sein Hash wird im Release-Manifest dokumentiert.

Falls alle privaten Pakete bereits im NuGet-Cache liegen, kann der Zugriff auf
private Feeds für diesen Build vermieden werden:

```powershell
./tools/Build-Setup.ps1 -RestoreSources 'https://api.nuget.org/v3/index.json'
```

Der Build leert nur die beiden generierten Setup-Publish-Verzeichnisse, erstellt
die Release-Abhängigkeiten neu und verwendet für beide Programme das Profil
`My Project/PublishProfiles/Setup.pubxml` (win-x86, framework-dependent).
App und CLI-Tester werden ausschließlich über **Publish Items** eingebunden.
Validator-Dateien, App-EXE und Abhängigkeiten werden nicht zusätzlich manuell
eingetragen. Die alte Validator-JAR 1.5.0 ist von der Veröffentlichung ausgeschlossen.

Ergebnis unter `EHFleetXRechnung.Setup/Release`:

- `setup.exe`: Einstieg für die Installation einschließlich Laufzeitprüfung/-download.
- `EHFleetXRechnung.Setup.msi`: Anwendungspaket.
- `release-manifest.json`: SHA-256-Prüfsummen und vollständige Dateiliste.
- `EHFleetXRechnung.Setup-1.1.9.zip`: Auslieferungspaket mit den drei Dateien und
  dieser Anleitung. ZIP vor der Installation vollständig entpacken.

Das vollständige ZIP entpacken und einschließlich `net8desktopruntime_x86`
weitergeben. `setup.exe` und MSI sind derzeit unsigniert; das Laufzeitpaket
trägt die geprüfte Microsoft-Signatur.
Setup-Definition, Profile und Build-/Prüfskripte sind versioniert;
Build-Ausgaben, Protokolle und Staging bleiben ignoriert.

`tools/Build-Bootstrapper.ps1` erzeugt nach dem MSI-Build einen separaten
Offline-Bootstrapper über `tools/Bootstrapper.proj`. Es verwendet eine private
Kopie der Microsoft-Bootstrappervorlage und verändert die installierten SDK-Dateien
nicht. Die Laufzeitversion und ihr SHA-512-Hash sind in `tools/desktop-runtime.json`
festgelegt (Microsoft-Release-Metadaten). Zusätzlich wird die Microsoft-Authenticode-
Signatur geprüft. Nur der erste Download auf dem Build-Rechner benötigt Internet.
Auf w25 war der bisherige WinINet-Download des Bootstrappers mit HRESULT
0x800C0008 fehlgeschlagen; das Offline-Paket beseitigt diese Downloadabhängigkeit.

## Prüfen

`Finalize-Setup.ps1` setzt vor der Prüfung `REINSTALLMODE=emus` im MSI.
Dadurch werden beim Upgrade auch DLLs gleicher Dateiversion ersetzt, deren
Inhalt sich geändert hat (etwa RID-spezifische Assemblies und der COM-Host).
Die übliche MSI-Regel behält solche alten Dateien sonst trotz erfolgreichem
Upgrade. Neuere Dateiversionen werden weiterhin nicht überschrieben.
Siehe [Microsoft: REINSTALLMODE](https://learn.microsoft.com/windows/win32/msi/reinstallmode).
Das Skript korrigiert außerdem die von Installer Projects angekündigten
Verknüpfungen auf explizite EXE-Ziele. Andernfalls versucht Windows beim
Export-Publish-Output, die verwaltete DLL statt des Apphosts zu starten.

`Build-Setup.ps1` führt anschließend `tools/Verify-Setup.ps1` aus. Es prüft die
Produkt-/Dateiversionen, eindeutige MSI-Zielpfade, den aktuellen Validator und die
.NET-Startbedingung. Eine administrative Extraktion mit `msiexec /a` registriert
kein Produkt und installiert die Anwendung nicht. Die extrahierten Dateien
werden vollständig per SHA-256 mit den Publish-Ausgaben und den zusätzlichen
Setup-Dateien verglichen. Die Klassenbibliothek muss dem lokalen Release-Build
entsprechen. Die bytegleiche zusätzliche Kopie von `WebView2Loader.dll` im
Publish-Hauptverzeichnis wird von Installer Projects dedupliziert; ausgeliefert
wird `runtimes/win-x86/native/WebView2Loader.dll`. Diese eine Ausnahme wird nur
bei identischen Prüfsummen akzeptiert und im Manifest dokumentiert. Der
CLI-Tester wird außerdem ohne Parameter gestartet (Hilfetext, kein Datenbankzugriff).
`tools/Test-Logging.ps1` prüft mit der tatsächlich ausgelieferten log4net-DLL die
XML-Layouts auf Erhalt von Logeinträgen mit XML-unzulässigen Steuerzeichen sowie
die bestehende RollingFileAppender-Konfiguration in einem isolierten Verzeichnis.

Auf W25 wurden das Upgrade mit Altdateien, die Offline-Laufzeitinstallation,
alle 268 installierten Dateiprüfsummen, CLI und Pkcs-Funktionen geprüft.
Die Anwendung startet und lädt alle drei Rechnungsarten. Die Berichtsvorschau
ist dort durch einen veralteten Datenbankpfad auf eine fehlende Vorlage unter
`D:\ehfleet\Stimulsoft` blockiert. Details und Testgrenzen stehen im
W25-Testbericht `docs/w25-release-1.1.8.md` (im ZIP direkt als
`w25-release-1.1.8.md`). Eine Neuinstallation auf einem
leeren System und die vollständige VB6-Integration wurden nicht geprüft.

Technische Referenz: [Microsoft: Installer Projects und .NET](https://learn.microsoft.com/en-us/visualstudio/deployment/installer-projects-net-core).
