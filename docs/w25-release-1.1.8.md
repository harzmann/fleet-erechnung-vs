# Release 1.1.8 – Prüfung auf W25

Prüfdatum: 09.11.2026. Ziel: W25, Windows Server 2025 Standard, x64,
interaktive Administrator-Sitzung. Fernzugriff über die vorhandene
WinRM-Konfiguration des Projekts DLTNGDownloadTool; keine Zugangsdaten im Repository.

## Sicherheitsstand und Paketprüfung

- App, Viewer und CLI: 1.1.8.0, Release, win-x86, frameworkabhängig.
- log4net: 3.5.0; XML-Regressionstests und RollingFileAppender-Test bestanden.
- System.Security.Cryptography.Pkcs: NuGet 8.0.2, ausgelieferte Windows-DLL
  8.0.3226.50405. Direkte Referenz für net48 und net8.0-windows.
- NuGet-Audit des Viewers einschließlich transitiver Pakete: keine bekannten
  verwundbaren Pakete. Der eigenständige Paketgraph der benachbarten
  Klassenbibliothek wird dadurch nicht geändert.
- MSI administrativ extrahiert; alle 268 Dateien stimmen per SHA-256 mit
  den Publish-Ausgaben und den zusätzlichen Setup-Eingaben überein.
- Offline-Bootstrapper mit Microsoft .NET Desktop Runtime 8.0.33 x86;
  SHA-512 gegen Release-Metadaten und Authenticode-Signatur geprüft.

## Installation und Upgrade

Ausgangslage: Produktregistrierung 1.1.4, App-/Viewer-Dateien 1.1.3.0,
log4net 3.0.4, alte Pkcs-DLL, Desktop Runtime 8.0.25 x86.
Die ursprüngliche Bootstrapper-Downloadroutine scheiterte auf W25 mit
0x800C0008. Die mitgelieferte Offline-Laufzeit ließ sich erfolgreich auf
8.0.33 aktualisieren.

Beim ersten Upgrade auf den Zwischenstand 1.1.7 blieben zwölf alte DLLs mit
gleicher Dateiversion erhalten. Deshalb wurde das endgültige Release 1.1.8
mit `REINSTALLMODE=emus` erstellt. Damit werden auch gleiche Dateiversionen
ersetzt, während höhere Versionen geschützt bleiben. Der Wiederholungstest
verwendet die Installation 1.1.7 mit diesen noch vorhandenen Altdateien.

Die Vorinstallation bleibt als Sicherung unter
`C:\ProgramData\eh-systemhaus\EHFleetXRechnung-Test` erhalten.

Ergebnis des endgültigen Upgrades: MSI erfolgreich, genau ein registriertes
Produkt 1.1.8. Alle 268 installierten Dateien stimmen mit dem Release-Manifest
überein. CLI-Hilfetext mit Exitcode 0. Funktionstest mit der tatsächlich
installierten Pkcs-DLL: SignedCms-Signatur gültig, manipulierte Signatur
abgewiesen, EnvelopedCms-Verschlüsselung/Entschlüsselung erfolgreich.

Die Programmverknüpfungen werden im finalen MSI auf die EXE-Apphosts gesetzt.
Das nachträglich finalisierte MSI wurde mit neuem PackageCode auf W25 erneut
installiert und erfolgreich repariert (Exitcode 0). Die Startmenüziele zeigen
auf die installierten EXE-Dateien. Auf dem Desktop lagen noch manuell kopierte
angekündigte Verknüpfungen auf das bereits entfernte Produkt 1.1.4; diese wurden
gesichert und durch die aktuellen Startmenüverknüpfungen ersetzt.
Der anschließende Start über die Desktopverknüpfung zeigt App und Viewer
1.1.8.0 und lädt die Werkstattrechnungsübersicht erfolgreich. Auch mit
wiederhergestellter Basiskonfiguration greift die migrierte Benutzereinstellung.

## Anwendungstest

Die installierte Anwendung startet und liest die Fleet-Verbindung aus der
Registry. Werkstattrechnungen, Tankabrechnungen und manuelle Rechnungen laden
und lassen sich umschalten. Die Vorschau einer manuellen Rechnung scheitert
an der vorhandenen Datenbankkonfiguration: `D:\ehfleet\Stimulsoft\MRechnung.mrt`
fehlt auf W25. Die Vorlage ist korrekt unter `reports\MRechnung.mrt` im
Installationsverzeichnis enthalten. Der Berichtspfad in der Datenbank muss
auf einen auf W25 erreichbaren Pfad eingestellt werden; die Vorschau ist
deshalb nicht als bestanden gewertet.

Die vorübergehend geänderte App-Konfiguration und das für den Test auf INFO
gesetzte Logging wurden bytegenau wiederhergestellt. Die anschließende
Dateiprüfung und die CLI-/Pkcs-Tests bestanden erneut. Die bereits in der
Benutzerkonfiguration 1.1.3.0 vorhandene Einstellung `UseRegDbCnStr=True`
wurde für 1.1.8.0 übernommen; keine Verbindungszeichenfolge wurde kopiert.

## Umfang

Ein Upgrade auf dem vorhandenen W25 ersetzt keinen Test einer Neuinstallation
auf einem leeren Rechner. Rechnungsversand, produktive Rechnungsfestschreibung,
Datenbankmigration und die vollständige VB6-Integration sind nicht Bestandteil
dieses Abnahmetests.
