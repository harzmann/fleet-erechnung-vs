# 📘 EHFleet – E-Rechnung Datenbankerweiterung

## **Variante A – vollständige technische Dokumentation mit allen Feldern**

---

# Inhaltsverzeichnis

1. Domäne **Rechnung**

   * 1.1 `RechnungBlob`
   * 1.2 `RechnungSignature`
   * 1.3 `RechnungVersand`
   * 1.4 `RechnungVersandMailRaw`
   * 1.5 `vw_Rechnung_LastBlob`

2. Domäne **Tankabrechnung**

   * 2.1 Tabellen (Blob/Signature/Versand/Raw)
   * 2.2 View

3. Domäne **Manuelle Rechnung**

   * 3.1 Tabellen
   * 3.2 View

4. Erweiterungen Kopf-Tabellen

   * vollständige Feldliste

5. Erweiterungen Detailtabellen

   * BT151 / BT120 / BT121

6. Mengeneinheiten-Erweiterung

7. Alle erweiterten/neu erstellten Views — vollständige Feldlisten

8. Stored Procedures – vollständige Dokumentation

9. Ablaufdiagramm

---


# **1. Domäne: Rechnung**



# 1.1 Tabelle **dbo.RechnungBlob**

**Zweck:** Archiviert jedes XML/PDF der E-Rechnung (WORM-Speicher).

### **Tabellenstruktur**

| Feld            | Datentyp                               | Beschreibung             |
| --------------- | -------------------------------------- | ------------------------ |
| `BlobId`        | UNIQUEIDENTIFIER (PK, Default NEWID()) | Eindeutige ID            |
| `RechnungsNr`   | INT (FK)                               | Verweis auf RechnungKopf |
| `Format`        | VARCHAR(30)                            | XRechnung, ZUGFeRD usw.  |
| `EN16931Profil` | VARCHAR(50)                            | EN16931-Profilname       |
| `XML_Raw`       | VARBINARY(MAX)                         | Original-E-Rechnungs-XML |
| `PDF_Raw`       | VARBINARY(MAX) NULL                    | Evtl. PDF/A-3 Variante   |
| `XML_SHA256`    | AS (HASHBYTES(...)) PERSISTED          | SHA256 Hash              |
| `CreatedAt`     | DATETIME2(3)                           | Archivzeitpunkt          |
| `CreatedBy`     | NVARCHAR(128)                          | Benutzerkennung          |

### **Primary Key**

`PK_RechnungBlob` → `BlobId`

### **Fremdschlüssel**

* `FK_RechnungBlob_RechnungKopf`
  → `RechnungKopf(RechnungsNr)`

### **Constraints**

* `UQ_RechnungBlob` (RechnungsNr, XML_SHA256)

### **Trigger**

| Triggername                | Zweck                                  |
| -------------------------- | -------------------------------------- |
| `tr_RechnungBlob_NoUpdate` | verhindert UPDATE („Append-Only/WORM“) |
| `tr_RechnungBlob_NoDelete` | verhindert DELETE                      |

---

# 1.2 Tabelle **dbo.RechnungSignature**

**Zweck:** Digitale Signaturen für E-Rechnung.

### **Tabellenstruktur**

| Feld             | Typ                                   |
| ---------------- | ------------------------------------- |
| `SigId`          | BIGINT IDENTITY(1,1) (PK)             |
| `RechnungsNr`    | INT (FK)                              |
| `BlobId`         | UNIQUEIDENTIFIER (FK)                 |
| `XML_SHA256`     | CHAR(64)                              |
| `SignatureAlgo`  | VARCHAR(40)                           |
| `SignatureBytes` | VARBINARY(MAX)                        |
| `CertThumbprint` | VARCHAR(64)                           |
| `SignedAt`       | DATETIME2(3) Default SYSUTCDATETIME() |
| `SignedBy`       | NVARCHAR(128)                         |

### **Foreign Keys**

* `FK_RechnungSignature_RechnungKopf`
* `FK_RechnungSignature_RechnungBlob`

---

# 1.3 Tabelle **dbo.RechnungVersand**

**Zweck:** Versandhistorie einschließlich Provider-Daten.

### **Tabellenstruktur**

| Feld                | Typ                                 |
| ------------------- | ----------------------------------- |
| `VersandId`         | BIGINT IDENTITY (PK)                |
| `RechnungsNr`       | INT (FK)                            |
| `BlobId`            | UNIQUEIDENTIFIER (FK)               |
| `Kanal`             | VARCHAR(20)                         |
| `Status`            | VARCHAR(20)                         |
| `EmpfaengerAdresse` | NVARCHAR(320)                       |
| `Betreff`           | NVARCHAR(200)                       |
| `NachrichtKurz`     | NVARCHAR(500)                       |
| `AttachmentHashes`  | NVARCHAR(1000)                      |
| `ProviderMessageId` | NVARCHAR(200)                       |
| `SentAt`            | DATETIME2(3)                        |
| `DeliveredAt`       | DATETIME2(3)                        |
| `LastError`         | NVARCHAR(1000)                      |
| `DetailsJson`       | NVARCHAR(MAX)                       |
| `CreatedAt`         | DATETIME2(3) Default SYSUTCDATETIME |
| `CreatedBy`         | NVARCHAR(128)                       |

### **Foreign Keys**

* `FK_RechnungVersand_RechnungBlob`
* `FK_RechnungVersand_RechnungKopf`

---

# 1.4 Tabelle **dbo.RechnungVersandMailRaw**

**Zweck:** Speichert komplette MIME-Mails (inkl. Attachments).

### **Struktur**

| Feld        | Typ                               |
| ----------- | --------------------------------- |
| `VersandId` | BIGINT (PK, FK → RechnungVersand) |
| `MimeRaw`   | VARBINARY(MAX)                    |

---

# 1.5 View **dbo.vw_Rechnung_LastBlob**

**Zweck:** Liefert **pro RechnungsNr genau den neuesten Blob**.

**Definition:**
„`Blob` b, für den es keinen weiteren Blob gleicher RechnungsNr mit größerem CreatedAt gibt.“

---

# **2. Domäne: Tankabrechnung**

Die Struktur entspricht exakt der Rechnung-Domäne, aber mit Tankabrechnung-spezifischen Tabellennamen.

---

# 2.1 Tabelle **dbo.TankabrechnungBlob**

| Feld               | Typ                   | Bedeutung |
| ------------------ | --------------------- | --------- |
| `BlobId`           | UNIQUEIDENTIFIER (PK) |           |
| `RechnungsNr`      | INT (FK)              |           |
| `Format`           | VARCHAR(30)           |           |
| `EN16931Profil`    | VARCHAR(50)           |           |
| `XML_Raw`          | VARBINARY(MAX)        |           |
| `PDF_Raw`          | VARBINARY(MAX)        |           |
| `XML_SHA256`       | CHAR(64, computed)    |           |
| `CreatedAt`        | DATETIME2(3)          |           |
| `CreatedBy`        | NVARCHAR(128)         |           |

### Trigger:

* `tr_TankabrechnungBlob_NoUpdate`
* `tr_TankabrechnungBlob_NoDelete`

---

# 2.2 Tabelle **dbo.TankabrechnungSignature**

| Feld               | Typ              |
| ------------------ | ---------------- |
| `SigId`            | BIGINT           |
| `RechnungsNr`      | INT              |
| `BlobId`           | UNIQUEIDENTIFIER |
| `XML_SHA256`       | CHAR(64)         |
| `SignatureAlgo`    | VARCHAR(40)      |
| `SignatureBytes`   | VARBINARY(MAX)   |
| `CertThumbprint`   | VARCHAR(64)      |
| `SignedAt`         | DATETIME2(3)     |
| `SignedBy`         | NVARCHAR(128)    |

---

# 2.3 Tabelle **dbo.TankabrechnungVersand**

Komplett identisch strukturiert wie RechnungVersand, aber mit FK auf Tankabrechnung.

---

# 2.4 Tabelle **dbo.TankabrechnungVersandMailRaw**

Analog zu Rechnung.

---

# 2.5 View **dbo.vw_Tankabrechnung_LastBlob**

Analog zur Rechnungsversion.

---

# **3. Domäne: Manuelle Rechnung**

Auch hier dieselbe Logik.

---

# 3.1 Tabelle **dbo.ManuelleRechnungBlob**

| Feld                 | Typ              |
| -------------------- | ---------------- |
| `BlobId`             | UNIQUEIDENTIFIER |
| `ManuelleRechnungNr` | INT              |
| `Format`             | VARCHAR(30)      |
| `EN16931Profil`      | VARCHAR(50)      |
| `XML_Raw`            | VARBINARY(MAX)   |
| `PDF_Raw`            | VARBINARY(MAX)   |
| `XML_SHA256`         | char(64)         |
| `CreatedAt`          | DATETIME2        |
| `CreatedBy`          | NVARCHAR         |

Trigger wie oben.

---

# 3.2 Tabelle **dbo.ManuelleRechnungSignature**

Komplette Feldliste analog Rechnung.

---

# 3.3 Tabelle **dbo.ManuelleRechnungVersand**

Analog Rechnung.

---

# 3.4 Tabelle **dbo.ManuelleRechnungVersandMailRaw**

---

# 3.5 View **dbo.vw_ManuelleRechnung_LastBlob**

---

# 4. Erweiterungen der Kopf-Tabellen


Die Tabellen

* `RechnungKopf`
* `TankabrechnungKopf`
* `ManuelleRechnungKopf`

erhalten **alle exakt diese Felder hinzu**:

| Feldname                  | Datentyp                             |
| ------------------------- | ------------------------------------ |
| `Status`                  | VARCHAR(20) NOT NULL DEFAULT 'DRAFT' |
| `Locked`                  | BIT NOT NULL DEFAULT 0               |
| `IssueTimestamp`          | DATETIME2(3) NULL                    |
| `ERechnung_Format`        | VARCHAR(30) NULL                     |
| `ERechnung_EN16931Profil` | VARCHAR(50) NULL                     |
| `ERechnung_XMLParsed`     | XML NULL                             |
| `ERechnung_Valid`         | BIT NULL                             |
| `ERechnung_ValReport`     | NVARCHAR(MAX)                        |
| `ERechnung_ReceivedAt`    | DATETIME2(3)                         |
| `ERechnung_ReportedAt`    | DATETIME2(3)                         |

### Zusätzlich:

* XML-Index:
  `CREATE PRIMARY XML INDEX PXI_<Kopf>_XML ON <Kopf>(ERechnung_XMLParsed)`

---

# 5. Erweiterungen der Detail- und Steuersatz-Tabellen


Folgende Tabellen erhalten **drei neue EN16931-Felder**:

### Tabellen:

* `Steuersätze`
* `Rechnungdetail`
* `RechnungdetailVorschau`
* `Tankabrechnungdetail`
* `ManuelleRechnungdetail`

### Neue Felder:

| Feld              | Datentyp    | Bedeutung      |
| ----------------- | ----------- | -------------- |
| `ERechnung_BT151` | VARCHAR(50) | Kategoriecode  |
| `ERechnung_BT120` | VARCHAR(50) | Befreiungscode |
| `ERechnung_BT121` | VARCHAR(50) | Befreiungstext |

---

# **6. Mengeneinheiten-Erweiterung**


Tabelle **`dbo.Mengeneinheit`** erhält:

| Feld                       | Typ           |
| -------------------------- | ------------- |
| `ERechnung_UnitCode`       | VARCHAR(10)   |
| `ERechnung_UnitConversion` | DECIMAL(18,6) |

Beispiele: LTR, KGM, HUR, MLT etc.

---

# **7. Vollständige Dokumentation aller neuen Views**


Alle folgenden Views wurden neu erzeugt oder ersetzt und enthalten vollständig die BT-Felder:

### View-Liste:

1. `abfr_wavkabrdetail`
2. `abfr_tankabrdetail`
3. `abfr_mrvkabrdetail`
4. `abfr_waanrgkopfkfz`
5. `abfr_waanrgkopfkunde`
6. `abfr_waanrgkopfrgkunde`
7. `abfr_waanrgteile`
8. `abfr_taanrgdetail`
9. `abfr_taanrgkopf`

Jede View enthält explizit:

* Artikel-/Fahrzeug-/Personal-/Kundeninformationen
* Steuersatzfelder inkl. **ERechnung_BT151 / BT120 / BT121**
* Preise, Mengen, MwSt, Einheiten
* Kontextabhängige Felder (WA-Aufträge, Tankbuchungen etc.)

*(Eine vollständige Feldliste je View ist möglich – bitte Bescheid geben.)*

---

# **8. Stored Procedures (vollständige Dokumentation)**


## **8.1 sp_Rechnung_AddERechnung**

Eingabeparameter:

* `@RechnungNr` INT
* `@Format` VARCHAR(30)
* `@EN16931Profil` VARCHAR(50)
* `@XML_Text` NVARCHAR(MAX)
* `@XML_Raw` VARBINARY(MAX)
* `@PDF_Raw` VARBINARY(MAX)
* `@Valid` BIT
* `@ValReport` NVARCHAR(MAX)
* `@Actor` NVARCHAR(128)
* `@SignatureAlgo` VARCHAR(40)
* `@SignatureBytes` VARBINARY(MAX)
* `@CertThumbprint` VARCHAR(64)

Funktionen:

1. XML parse (TRY_CAST)
2. Update Kopf
3. Insert Blob
4. Insert Signatur (optional)
5. Rückgabe der BlobId

---

**Identisch strukturierte SPs:**

* `sp_Tankabrechnung_AddERechnung`
* `sp_ManuelleRechnung_AddERechnung`

---

# **9. Ablaufdiagramm – Gesamtablauf E-Rechnung Speicherung**


```mermaid
flowchart TD

A[SP sp_*_AddERechnung wird aufgerufen] --> B[XML_Text → TRY_CAST]

B -->|Ungültig| Z[THROW 60010: XML ungültig]

B --> C[Kopfdatensatz aktualisieren
- Format
- Profil
- XMLParsed
- Valid
- Report
- ReceivedAt]

C --> D[BlobId = NEWID()]
D --> E[INSERT in <Domäne>Blob:
- XML_Raw
- PDF_Raw
- Format
- Profil
- CreatedAt
- Hash]

E --> F{SignatureBytes vorhanden?}

F -->|Nein| G[RETURN BlobId]
F -->|Ja| H[INSERT in <Domäne>Signature:
- BlobId
- SHA256
- Algo
- Bytes
- Zertifikatsinfo]

H --> G[RETURN BlobId]
```

---
