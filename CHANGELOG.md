# Changelog

## 1.1.9 – 2026-11-20

- Logging prüft die Schreibbarkeit des Programmverzeichnisses. Bei fehlenden Rechten werden Anwendungs- und SMTP-Logs nach `%LOCALAPPDATA%\eh-systemhaus\EHFleetXRechnung\logs` geschrieben; letzter Ausweichpfad ist `%TEMP%\eh-systemhaus\EHFleetXRechnung\logs`.
- Validator-Berichte verwenden denselben beschreibbaren Pfad unter `html`. Der gewählte Pfad wird beim Start protokolliert. Auch ohne `logger.config` wird ein Standard-Dateilog eingerichtet.
- Einheitliche Logging-Konfiguration für Desktop-App, CLI-/Request-Modus und COM-Aufruf. Keine Abhängigkeit vom aktuellen Arbeitsverzeichnis.
- Fehlende Rechnungsnummern-Formatparameter 156/1156/2156 verhindern den XML-Export nicht mehr (Klassenbibliothek 1.3.11.1). Bestehende aktive Formate bleiben erhalten.
- Die Einzelvalidierung startet nur nach erfolgreicher XML-Erzeugung; Exportfehler bleiben in der Ergebnisliste sichtbar.
- Benutzereinstellungen werden beim ersten Start einer neuen App-Version aus dem vorherigen Versionsverzeichnis übernommen.
- Neues MSI mit neuer Product-/Package-ID und unveränderter Upgrade-ID für Neuinstallation und Upgrade. Zusätzlich ein geprüfter Dateipatch von 1.1.8 auf 1.1.9 mit Vorprüfung, Sicherung und Rücknahme bei Kopierfehlern.
- Reproduzierbare Release-Skripte, Prüfsummenmanifest sowie Tests mit tatsächlich verweigerten Windows-Schreibrechten.

## 1.1.8 – bisheriger lokaler Release-Stand

- log4net 3.5.0 und direkte Pkcs-Abhängigkeit 8.0.2.
- Java-Erkennung über JRE_HOME, JAVA_HOME und PATH.
- MSI-/Offline-Bootstrapper für .NET Desktop Runtime x86, EXE-Verknüpfungen und Ersatz gleicher Dateiversionen beim Upgrade.
- Korrekte Rechnungszuordnung bei sortiertem Grid und UI-sicherer Report-Vorschau.
- Historischer W25-Prüfbericht: `docs/w25-release-1.1.8.md`.
