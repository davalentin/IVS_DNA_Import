# Software Specification: Modernization Estimation Engine

## 1. Process Overview
Questa specifica descrive un motore di stima applicabile a IVS_DNA per produrre scenari di modernizzazione coerenti, ripetibili e giustificabili. L’obiettivo è trasformare evidenze di reverse engineering (stack, SLOC, dipendenze, complessità, vincoli) in stime comparabili tra opzioni di modernizzazione.

## 2. Module 1: The Scanner (Automated Input)
### 2.1 Logica di Ingestione
Input automatizzabili per IVS_DNA:
- SLOC raw e hand-written.
- Conteggio pagine WebForms, user controls, presenter, operation contract.
- Rilevazione stack (`.NET 3.5`, `WCF`, `LINQ-to-SQL`, `INPS.DNA`).
- Rilevazione config risk (secret, debug, environment variants).
- Mappatura dipendenze esterne (ARCA, WebDom, SCRIWO, DB2, ecc.).
- Mappatura capability per fondo (FS/AGO/CI).

### 2.2 Tabella di Backfiring (Database di Riferimento)
| Tecnologia sorgente | LOC/FP base | Note |
|---|---:|---|
| C# enterprise legacy | 53 | Default scenario base |
| C# complesso / integration-heavy | 47 | Scenario ottimistico per FP più alti |
| C# con molto boilerplate/generated code | 60 | Scenario conservativo |

## 3. Module 2: The Wizard (Discovery & Scoring)
### SEZIONE A: Strategia & Architettura (Determine Target Complexity)
Questionario per IVS_DNA:
- Sostituzione completa o strangler incrementale?
- UI target: Razor Pages / SPA / altro?
- Contratti da preservare temporaneamente?
- Grado di riuso logica fondo?
- Necessità di parallel run con sistema legacy?

### SEZIONE B: Team Capabilities (PDR Calibration)
- Disponibilità skill WebForms/WCF legacy.
- Disponibilità skill modern .NET / frontend.
- Presenza esperti dominio pensioni.
- Capacità DevOps / platform engineering.

### SEZIONE C: COCOMO II Drivers (Advanced Scoring)
Driver chiave per IVS_DNA:
- Complexità dominio: alta.
- Riuso platform interna: medio-alto.
- Vincoli integrazione: molto alti.
- Portabilità: bassa.
- Security remediation: necessaria.

## 4. Module 3: The Engine (Algorithms)
### Step 3.1: Calcolo Dimensione Target
- Partire da SLOC hand-written.
- Applicare fattore di riduzione target per stack moderno (es. 0,45-0,60) se si prevede consolidamento e semplificazione.
- Applicare fattore di incremento per parallel run o compatibilità transitoria (+10%/+25%).

### Step 3.2: Stima Lineare (PDR Method)
Formula proposta:
- `Effort_PM = FP / PDR`
- PDR calibrabile per stream (frontend, backend, integrazioni, test automation).

### Step 3.3: Stima Esponenziale (COCOMO II)
Formula di base:
- `PM = A * Size^E * EAF`
- Parametri variabili per scenario:
  - Replatforming conservativo.
  - Re-architecting capability-by-capability.
  - Rebuild esteso con parallel run.

## 5. Module 4: The Strategist (Output & Scenarios)
### Layout Dashboard Risultati
Output attesi:
- dimensione sorgente e target;
- range FP;
- PM per scenario;
- durata calendario;
- costo per scenario;
- team mix raccomandato;
- top risk driver.

#### Scenario A: Re-platforming (Conservativo)
- Obiettivo: sostituire runtime e sicurezza, mantenendo gran parte della logica.
- Benefit: rischio business più basso.
- Limite: persistenza di debito architetturale.

#### Scenario B: Re-architecting (Aggressivo)
- Obiettivo: capability APIs + nuovo frontend + strangler dei moduli fondo.
- Benefit: riduzione debito strutturale.
- Limite: investimento maggiore e più dipendenze organizzative.

### Funzionalità Roadmap (Strangler Fig Generator)
Il motore deve suggerire una roadmap a slice:
1. hardening sicurezza/config;
2. ricerca/stato pratiche;
3. titolare/anagrafica;
4. calcolo verify;
5. calcolo definitivo;
6. stampa e post-calcolo;
7. amministrazione.

## 6. Technical Implementation Notes
### Architecture Stack Suggestion
- Backend di calcolo stime: servizio applicativo moderno .NET.
- Storage delle baseline: SQLite/PostgreSQL.
- UI reportistica: dashboard web + export DocMind.
- Input scanner: parser repo, config scanner, metrics collector.

### Integrazione Dependency Graph (Advanced)
Per IVS_DNA è particolarmente utile collegare il motore a:
- grafo dipendenze tra PN809/PN812/PN813/PN815/PN818;
- inventario endpoint WCF e consumer;
- matrice capability × fondo × integrazione esterna.

## 7. Applicazione a IVS_DNA
- Size baseline consigliata: 630.434 LOC hand-written.
- Complexity profile: enterprise embedded.
- Critical drivers: integrazioni esterne, compatibilità contrattuale, security remediation, test gap.
- Recommended estimation mode: **scenario-based**, non stima singola.

## Reference Documents
- `/root/IVS_DNA/README.md`
- `/root/IVS_DNA/Doc/Legacy_IVS_AnalisiTecnica.md`
- `/root/IVS_DNA/Doc/IVS_Onboarding_Tecnico.md`
- `/root/IVS_DNA/Doc/IVS_Requisiti_Tecnici_Approfonditi.md`
- `/root/IVS_DNA/Doc/Manuale_utente.md`
- `PN809/LiquidazionePensioniFS/Web.config`
- `PN812/WSInpsPensioniLiquidazione/Web.config`
- `PN812/WSInpsPensioniLiquidazione/Contracts/ServiceContracts/IServizioLiquidazione.cs`

## Change Log
- 2026-06-24 — Documento generato da analisi del repository, dei file di configurazione e della documentazione tecnica esistente.
