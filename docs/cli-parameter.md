# CLI-Parameter und Testtool-Dokumentation

## Übersicht

Dieses Dokument beschreibt die Kommandozeilenparameter (CLI) der Anwendung **EHFleetXRechnung** sowie die Funktionsweise des Testtools **EHFleetXRechnung.CliTester**.

---

## 1. CLI-Parameter der Hauptanwendung

### 1.1 `--request <PfadZurRequestDatei>`

- Startet die Anwendung im "Request-File-Mode".
- Erwartet eine JSON-Datei mit allen nötigen Informationen (siehe unten).
- Beispiel:
  