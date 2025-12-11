# XRechnung-Export – Testmatrix und Ablaufdiagramm

Diese Dokumentation beschreibt die Testfälle für den Export von XRechnungen aus **Fleet** sowie den zugrunde liegenden Ablauf.
Getestet werden folgende Dimensionen:

* **Festschreiben**: ja/nein
* **Exportformat**: nur XML / Hybrid-PDF (PDF mit eingebettetem XML)
* **Validierungsergebnis**: erfolgreich / fehlgeschlagen

> **Annahme für die Tests:**
>
> * Vor jedem Export wird eine Validierung durchgeführt.
> * **Mit Festschreiben = Ja**: Export + Festschreiben nur bei **erfolgreicher** Validierung.
> * **Mit Festschreiben = Nein**: Export darf auch bei **fehlgeschlagener** Validierung erfolgen, aber nur mit Warnhinweis, der Beleg bleibt **nicht festgeschrieben**.

---

## 1. Testmatrix XRechnung-Export

| Test-ID | Festschreiben | Exportformat | Validierung    | Erwartetes Systemverhalten                                                                                                                                                                                                    |
| ------: | ------------- | ------------ | -------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|     T01 | Nein          | Nur XML      | Erfolgreich    | XML-Datei wird erzeugt und gespeichert. Belegstatus bleibt **nicht festgeschrieben**. Export-Log wird ohne Fehler protokolliert.                                                                                              |
|     T02 | Nein          | Nur XML      | Fehlgeschlagen | Validierungsfehler wird angezeigt (Fehlerliste/Log). Nutzer erhält **Warnhinweis**, dass die XRechnung nicht valide ist. XML-Datei wird dennoch erzeugt (Test-/Entwurfszwecke). Belegstatus bleibt **nicht festgeschrieben**. |
|     T03 | Nein          | Hybrid-PDF   | Erfolgreich    | Hybrid-PDF (PDF mit eingebettetem XML) wird erzeugt und gespeichert. Belegstatus bleibt **nicht festgeschrieben**. Export-Log ohne Fehler.                                                                                    |
|     T04 | Nein          | Hybrid-PDF   | Fehlgeschlagen | Validierungsfehler + Warnhinweis. Hybrid-PDF wird dennoch erzeugt (inkl. eingebettetem XML). Belegstatus bleibt **nicht festgeschrieben**. Export-Log enthält Hinweis auf fehlgeschlagene Validierung.                        |
|     T05 | Ja            | Nur XML      | Erfolgreich    | XML-Datei wird erzeugt und gespeichert. Beleg wird **festgeschrieben** (nicht mehr änderbar). Exportstatus/Flag in der Datenbank wird gesetzt. Erfolgsprotokoll im Log.                                                       |
|     T06 | Ja            | Nur XML      | Fehlgeschlagen | Validierungsfehler wird angezeigt. **Kein Export**, **kein Festschreiben**. Beleg bleibt änderbar. Im Log wird der Export als fehlgeschlagen protokolliert.                                                                   |
|     T07 | Ja            | Hybrid-PDF   | Erfolgreich    | Hybrid-PDF wird erzeugt und gespeichert. Beleg wird **festgeschrieben**. Exportstatus/Flag in der Datenbank wird gesetzt. Erfolgsprotokoll im Log.                                                                            |
|     T08 | Ja            | Hybrid-PDF   | Fehlgeschlagen | Validierungsfehler wird angezeigt. **Kein Export**, **kein Festschreiben**. Beleg bleibt änderbar. Logeintrag mit Fehlerdetails.                                                                                              |

### 1.1. Allgemeine Prüfungen pro Testfall

Für jeden Testfall sollten zusätzlich folgende Punkte geprüft werden:

* **Dateiausgabe**

  * Existiert die erzeugte Datei am erwarteten Speicherort?
  * Dateiname/-schema korrekt (z. B. Rechnungsnummer, Datum, Mandant)?
  * Bei Hybrid-PDF: Ist das XML korrekt im PDF eingebettet?

* **Validierungsprotokoll**

  * Erfolgreiche Validierung: keine Fehler, ggf. nur Hinweise/Warnungen.
  * Fehlgeschlagene Validierung: Fehlercodes, Zeilen/Positionen klar erkennbar.

* **Datenbankstatus**

  * Festschreiben: Status-/Flag-Felder korrekt gesetzt.
  * Kein Festschreiben: Status bleibt unverändert.
  * Exportstatus (z. B. „exportiert“, „validierungsfehler“) korrekt gesetzt.

* **UI/Fehlermeldungen**

  * Meldungstexte verständlich und eindeutig (Erfolg, Warnung, Fehler).
  * bei Fehler/Warnung: Benutzerführung klar (z. B. „Details anzeigen“, „Erneut validieren“).

---

## 2. Ablaufdiagramm (Mermaid)

Nachfolgend der Export-Ablauf als Mermaid-Diagramm.
Kann z. B. in GitHub oder anderen Markdown-Viewern mit Mermaid-Unterstützung direkt gerendert werden.

```mermaid
flowchart TD
    A[Start XRechnung-Export] --> B[Export-Parameter wählen<br/>• Festschreiben (Ja/Nein)<br/>• Exportformat (XML/Hybrid-PDF)]
    B --> C[Validierung der XRechnung starten]
    
    C -->|erfolgreich| D{Festschreiben?}
    C -->|fehlgeschlagen| E{Festschreiben?}
    
    D -->|Ja| F[Export XML/Hybrid-PDF erzeugen<br/>Beleg festschreiben<br/>Status = 'exportiert/festgeschrieben']
    D -->|Nein| G[Export XML/Hybrid-PDF erzeugen<br/>Status = 'nicht festgeschrieben'<br/>Validierung OK]
    
    E -->|Ja| H[Export abbrechen<br/>Fehlermeldung anzeigen<br/>Status bleibt 'nicht festgeschrieben']
    E -->|Nein| I[Export mit Warnhinweis fortsetzen<br/>XML/Hybrid-PDF erzeugen<br/>Status bleibt 'nicht festgeschrieben'<br/>Log enthält Validierungsfehler]
    
    F --> J[Ende]
    G --> J
    H --> J
    I --> J
```

---

## 3. Testdurchführung

### 3.1. Voraussetzungen

* Mindestens eine testfähige Rechnung im System (mit allen Pflichtfeldern für XRechnung).
* XRechnungsprofil/Fassung (z. B. EN16931) im System korrekt konfiguriert.
* Validierungs-Engine angebunden (lokal oder über Service).
* Logging aktiviert (z. B. Datei oder DB-Log für Exporte und Validierungen).

### 3.2. Schrittfolge je Testfall

Für jeden der acht Testfälle (T01–T08):

1. **Testdaten vorbereiten**

   * Rechnung auswählen (ggf. eigens angelegte Testrechnung).
   * Sicherstellen, dass der Beleg **noch nicht** festgeschrieben ist.
   * Ggf. gezielt Fehler in die Rechnung einbauen, um Validierungsfehler zu provozieren (für die „fehlgeschlagen“-Fälle).
   
   **Testdaten Stand 25/12/10**

   * WA-Rechnung 4: Validierung fehlerhaft, nicht festgeschrieben.
   * WA-Rechnung 40: Validierung erfolgreich, bereits festgeschrieben.
   * WA-Rechnung 42: Validierung erfolgreich, nicht festgeschrieben.

2. **Parameter setzen**

   * Exportmaske öffnen.
   * Festschreiben = Ja/Nein gemäß Testfall.
   * Exportformat = Nur XML / Hybrid-PDF gemäß Testfall.

3. **Export auslösen**

   * Export-Button ausführen.
   * Verhalten der Anwendung beobachten (UI-Meldungen, Ladezeiten).

4. **Ergebnisse prüfen**

   * Meldungsfenster (Erfolg/Warnung/Fehler).
   * Exportierte Datei(en) im Dateisystem.
   * Inhalt des XML (stichprobenartig, z. B. Kopf- und Positionsdaten).
   * Bei Hybrid-PDF: Öffnen des PDFs, Prüfung auf eingebettetes XML.
   * Datenbankstatus (Festschreibungs-Flags, Exportstatus).

5. **Validierungs- und Exportlog kontrollieren**

   * Validierungsfehler bzw. Erfolg im Log nachvollziehen.
   * Prüfen, ob der Testfall korrekt protokolliert ist (Zeit, User, Beleg).

---

## 4. Erweiterungsmöglichkeiten

Die obige Testmatrix bildet die **Mindestabdeckung** für den XRechnung-Export.
Folgende Erweiterungen sind sinnvoll:

* Tests mit unterschiedlichen Profilen (z. B. **XRechnung**, **ZUGFeRD**, falls unterstützt).
* Tests mit unterschiedlichen Steuerszenarien (Inland, EU, Drittland).
* Tests mit sehr vielen Positionen und Grenzfällen (lange Texte, Rabatte, Skonto).
* Performance-Tests (Batch-Export vieler Rechnungen).

---
