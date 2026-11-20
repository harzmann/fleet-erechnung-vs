# Release 1.1.9 – Prüfergebnis vom 20.11.2026

## Auslieferung

- App, Viewer und CLI: 1.1.9.0, Release, win-x86, frameworkabhängig.
- Klassenbibliothek: 1.3.11.1.
- Binärquellstand: `083161a431fff861b33b8a4a20c66a6d74a26e8c`.
- Klassenbibliotheksquellstand: `fb3cede1c6e232b5d0eb863b2a62966b95afacf6`.
- Patchskript-Quellstand: `3bff737097763bbb46a6a8b7341717034799a9a0`,
  auch im enthaltenen `patch-manifest.json` dokumentiert.
- Gebaut aus separaten Git-Worktrees. Die uncommittierten Änderungen am
  Etikettendruck der Arbeitskopie sind nicht enthalten.
- Setup-ZIP: 371.355.570 Bytes. Patch-ZIP: 960.007 Bytes, 101 geänderte Dateien.

## Bestandene Prüfungen

- 17 Prüfungen für Rechnungsnummern: fehlende, deaktivierte, aktive Zahlen- und
  Datumsformate für WA/TA/MR, korrekte Parameternummern sowie Sonderfälle.
- 13 Logging-Prüfungen mit echten Windows-Deny-ACLs: Programmverzeichnis,
  LocalAppData- und Temp-Ausweichpfad, ausdrücklich gemeldeter Gesamtfehler,
  beide Appender, HTML-Verzeichnis und entfernte Schreibproben.
- Tatsächliches Schreiben eines Standard-Dateilogs ohne `logger.config`.
- Builds für .NET 8 und .NET Framework 4.8 sowie vollständiger Setup-Rebuild:
  fünf Projekte erfolgreich, keines fehlgeschlagen. Bestehende Compiler- und
  Plattformwarnungen wurden nicht als Bestandteil dieses Fixes bereinigt.
- Administrativ extrahiertes MSI: alle 268 Dateien per SHA-256 gegen die
  Publish-Ausgaben und die zusätzlichen Setup-Dateien geprüft. Enthaltene
  Klassenbibliothek entspricht exakt dem isolierten Release-Build.
- Produkt-/Dateiversionen, x86-Apphosts und COM-Host, EXE-Verknüpfungen,
  `REINSTALLMODE=emus`, unveränderte Upgrade-ID und Upgrade-Erkennung unterhalb
  1.1.9 geprüft. ProductCode und PackageCode wurden für das Release erneuert.
- Offline-Bootstrapper mit .NET Desktop Runtime 8.0.33 x86: SHA-512 und gültige
  Microsoft-Authenticode-Signatur bestätigt.
- Ausgelieferte log4net-DLL: beide XML-Regressionstests und RollingFileAppender
  erfolgreich. CLI-Hilfe aus dem extrahierten Paket erfolgreich gestartet.
- Ausgelieferte Viewer-DLL und echter KoSIT-Validator 1.5.2: Bei verweigertem
  Programm-Logverzeichnis entstand unter LocalAppData ein HTML-Bericht für die
  absichtlich unvollständige XML-Testrechnung. Die fachliche Ablehnung dieser
  Testrechnung ist erwartet. Keine Datenbankverbindung und keine Löschung
  vorhandener Kundenberichte.
- Original-MSI 1.1.8 separat extrahiert und alle 268 Ausgangsprüfsummen bestätigt.
- Kundenpatch: Vorprüfung ohne Änderungen, beschädigtes Paket abgewiesen,
  Dateisperre erkannt, Kopierfehler nach erster Ersetzung simuliert und alle
  Originaldateien korrekt wiederhergestellt. Anschließende erfolgreiche
  Anwendung: alle 101 Zielprüfsummen korrekt, App-Konfiguration bytegenau
  erhalten und angepasstes Kunden-`logger.config` gesichert.
- Auslieferungspakete ins normale Release-Verzeichnis kopiert und ihre
  Prüfsummen gegen die isolierten Build-Ausgaben erneut verglichen.

## SHA-256

| Datei | SHA-256 |
| --- | --- |
| EHFleetXRechnung.Setup-1.1.9.zip | `7A118CFF54B73209884194053AE64252EC8364C21D5B393D6A3FD2B8A7AFC9E9` |
| EHFleetXRechnung.Patch-1.1.8-to-1.1.9.zip | `E5EDE98F572E3E4E42793CB7DE05D7CBCA4E578A68AC5F2BD4849FB0A8DA1252` |
| EHFleetXRechnung.Setup.msi | `B907DE074B49012C990870BB5A4F97DFAF641B922EF2BBDF2D53337F421EE2B0` |
| setup.exe | `D5D7E4A6AD2FDBAE24AFA5438A51A1A79B7CB5C3305281FF9098BADFD8EAC429` |

## Grenzen

Die administrative MSI-Extraktion ist keine ausgeführte Neuinstallation oder
ein Upgrade eines registrierten Produkts. Neuinstallation auf einem leeren
System, Upgrade mit einem echten Kundenprofil, Migration der Benutzereinstellungen,
produktive Datenbank, Festschreibung und Rechnungsversand wurden nicht ausgeführt.
Der Patch ist ein Dateipatch von 1.1.8 auf 1.1.9; die MSI-Registrierung bleibt
dabei unverändert. Für Installer-Wartung das vollständige Setup verwenden.
Die Kundendatenbank wird von keinem Paket migriert.

Das Setup-ZIP einschließlich Laufzeitordner vollständig entpacken. Details zu
Installation, Patchanwendung und Logpfaden stehen in `release-1.1.9.md`.
