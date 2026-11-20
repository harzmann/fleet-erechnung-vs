# EHFleet XRechnung 1.1.9 – 20.11.2026

## Änderungen

Der XML-Export bricht bei fehlenden optionalen Rechnungsnummern-Formatparametern
nicht mehr ab. Das betrifft Parameter 156 (Werkstatt), 1156 (Tanken) und 2156
(manuelle Rechnung). Ein fehlender Eintrag entspricht deaktivierter Formatierung;
die Rechnungsnummer bleibt dann unverändert. Aktive Formate bleiben erhalten.
Der Fix wird mit Klassenbibliothek 1.3.11.1 ausgeliefert.

Der Validator erhält nach fehlgeschlagener XML-Erzeugung keine leere Datei mehr.
Die ursprüngliche Fehlermeldung erscheint im Exportprotokoll.

## Logdateien und Schreibrechte

Bisher gab es keinen Ausweichpfad. Fehlende Schreibrechte konnten zum Verlust der
Dateilog-Einträge führen; Validator-Berichte konnten nicht angelegt werden.

Ab 1.1.9 wird beim ersten Zugriff tatsächlich eine temporäre Datei geschrieben
und wieder entfernt. Der erste beschreibbare Pfad wird für den Prozess verwendet:

1. `logs` neben `EHFleetXRechnung.Viewer.dll` (bisheriger Ablageort).
2. `%LOCALAPPDATA%\eh-systemhaus\EHFleetXRechnung\logs`.
3. `%TEMP%\eh-systemhaus\EHFleetXRechnung\logs`.

Anwendungslogs heißen `EHFleetXRechnungViewer.JJJJ-MM-TT.log`, SMTP-Logs
`EHFleetXRechnungSmtp.JJJJ-MM-TT.log`. Validator-Berichte liegen unter `html`.
Die erste Logmeldung nennt das gewählte Verzeichnis. Auch ohne `logger.config`
wird ein Standard-Dateilog eingerichtet. Sind alle drei Pfade gesperrt, wird ein
ausdrücklicher Fehler gemeldet; es wird kein weiterer Ablageort behauptet.

LocalAppData und Temp gehören zum jeweils ausführenden Benutzer. Bei geplanten
Tasks ist das das Task-Konto; bei COM-Aufrufen das Konto des VB6-Hosts.
Temp-Dateien können von der Systembereinigung entfernt werden.

## Vollständiges Setup: Neuinstallation oder Update

`EHFleetXRechnung.Setup-1.1.9.zip` vollständig entpacken und `setup.exe` starten.
MSI und Ordner `net8desktopruntime_x86` müssen daneben bleiben. Das Paket enthält
die .NET Desktop Runtime 8.0.33 x86; Java wird nicht mitinstalliert.

Vor dem Update Fleet/VB6-Host, XRechnung und Hybrid-Viewer beenden. Das Setup
benötigt Administratorrechte, aktualisiert bestehende MSI-Installationen über die
unveränderte Upgrade-ID und verhindert die Installation über neuere Versionen.
Neue Product-/Package-IDs unterscheiden dieses Release von 1.1.8. Auch DLLs mit
gleicher Dateiversion werden ersetzt. Benutzerkonfiguration wird beim ersten Start
der neuen App-Version übernommen. Keine Datenbankmigration.

Für eine erstmals auf diesem Rechner eingerichtete Benutzerkonfiguration die
Fleet-Verbindung wie gewohnt konfigurieren. Kundenspezifische globale
Konfigurationsdateien und Berichtsvorlagen vor einem Setup-Update sichern.

## Dateipatch: ausschließlich von unverändertem Release 1.1.8

`EHFleetXRechnung.Patch-1.1.8-to-1.1.9.zip` vollständig in einen separaten Ordner
entpacken. Alle Fleet-/XRechnung-Prozesse beenden. PowerShell für ein Ziel unter
Programme als Administrator öffnen. Zuerst prüfen, dann anwenden:

```powershell
.\Apply-CustomerPatch.ps1 -InstallDirectory 'C:\Program Files (x86)\EHFleet Fuhrpark IM System\XRechnung' -VerifyOnly
.\Apply-CustomerPatch.ps1 -InstallDirectory 'C:\Program Files (x86)\EHFleet Fuhrpark IM System\XRechnung'
```

Den tatsächlichen Installationspfad einsetzen. Die Vorprüfung kontrolliert
Ausgangsversion, SHA-256 aller Nutzdateien, bekannte Ausgangsdateien und offene
Dateisperren. Abweichende Programmdateien werden abgewiesen; dann das vollständige
Setup verwenden. Nicht für ältere oder individuell zusammengestellte Stände.

Der Patch enthält nur geänderte Dateien. Anwendungs-/Datenbank-Konfiguration wird
nicht überschrieben. `logger.config` wird für die neue Pfadauflösung ersetzt und
vorher gesichert; eigene Log-Level anschließend bei Bedarf übertragen.
Originaldateien liegen nach Anwendung unter `patch-backup` im Programmverzeichnis.
Bei Kopier- oder Prüfsummenfehlern stellt das Skript die bereits ersetzten Dateien
wieder her. Bei einem Fehler während der Rücknahme nennt es die Sicherung und die
betroffenen Dateien.

Dies ist ein Dateipatch, kein MSP. Die MSI-Produktregistrierung bleibt beim alten
Stand. Eine Reparatur des alten MSI kann den Dateipatch zurücksetzen. Für korrekt
registrierte Updates und weitere Installer-Wartung das vollständige Setup 1.1.9
verwenden. Vor einem manuellen Rückspielen der Sicherung alle Prozesse beenden.

## Prüfung und Grenzen

Die Release-Prüfung kontrolliert MSI-Produkt-/Dateiversionen, Upgrade-ID,
EXE-Verknüpfungen, vollständige extrahierte Dateien per SHA-256, die enthaltene
Klassenbibliothek und die Microsoft-Signatur der Offline-Laufzeit.

`tools/LoggingSmokeTest` testet mit echten Windows-Deny-ACLs beide Ausweichpfade,
den Fehler bei vollständig gesperrten Pfaden sowie tatsächliches Schreiben beider
Log-Appender und eines HTML-Berichts. `tools/Test-Logging.ps1` prüft außerdem die
ausgelieferte log4net-DLL. Die Rechnungsnummern-Prüfung deckt fehlende, deaktivierte,
aktive Zahlen- und Datumsformate der drei Rechnungsarten ab.

Der Dateipatch wird gegen eine administrative Extraktion des ursprünglichen MSI
1.1.8 geprüft. Eine administrative Extraktion installiert oder registriert kein
Produkt. Die Kunden-Datenbank, eine produktive Festschreibung, Rechnungsversand
und eine Neuinstallation auf einem leeren Kundensystem sind dadurch nicht getestet.
Der W25-Bericht für 1.1.8 ist historisch und keine Abnahme von 1.1.9.
