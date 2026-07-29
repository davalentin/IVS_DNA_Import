# 🔍 ANTIPATTERN ASSESSMENT REPORT

## 📊 RIASSUNTO ESECUTIVO
L’analisi di IVS_DNA evidenzia un insieme coerente di antipattern tipici dei sistemi enterprise legacy: **God Contract / God Service**, **stateful UI coupling**, **configuration leakage**, **shotgun surgery cross-fund** e **generated-code gravity**. Il sistema resta business-critical e funzionalmente ricco, ma l’inerzia tecnica rende costoso qualsiasi change trasversale.

## 🚨 TABELLA PRIORITÀ (Top Findings)
| Priorità | Anti-pattern | Area | Evidenza |
|---|---|---|---|
| 1 | God Contract / God Service | PN812 | 252 OperationContract in un solo contratto |
| 2 | Stateful Web UI / Session as database | PN809 | 1.805 accessi `Session[` |
| 3 | Configuration Secrets Leakage | Config | Secret in chiaro e crittografia disabilitata |
| 4 | Shotgun Surgery | PN812 + fondi | Variazioni cross-capability richiedono modifiche multiple |
| 5 | Generated Code Gravity | Tutti | Service references e designer molto voluminosi |

## 🔍 ANALISI DETTAGLIATA (Top 3 Problemi Critici)
### 1. God Contract / God Service in `PN812/WSInpsPensioniLiquidazione`
**Sintomi**
- Un unico contratto applicativo concentra 252 operation.
- La service class centrale coordina ricerca, stato pratica, titolare, dante causa, pagamento, calcolo, stampa, admin, batch update.

**Impatto**
- Blast radius elevatissimo per qualsiasi modifica contrattuale.
- Alto rischio regressivo.
- Difficoltà di ownership per team e capability.

**Raccomandazione**
- Segmentare gradualmente il contratto per capability (`Pratiche`, `Anagrafica`, `Calcolo`, `Amministrazione`, ecc.).

### 2. Stateful Web UI / Session Coupling in `PN809`
**Sintomi**
- Uso esteso di chiavi sessione string-based.
- `ViewState` massivo.
- Navigazione e flusso fortemente dipendenti dal contesto server-side.

**Impatto**
- Testabilità bassa.
- Scalabilità orizzontale difficile.
- Debugging complesso su journey multi-step.

**Raccomandazione**
- Ridurre progressivamente stato server-side, introdurre view model espliciti e journey stateless dove possibile.

### 3. Configuration Leakage / Secret Sprawl nei file `Web.config` e `CHG*`
**Sintomi**
- Password DB, API client secret e credenziali applicative presenti in chiaro.
- `connectionStringCryptography=false`.
- Config multi-ambiente duplicate in repository.

**Impatto**
- Rischio security/compliance elevato.
- Maggiore probabilità di errore umano in rilascio.
- Difficoltà di rotazione credenziali.

**Raccomandazione**
- Secret vault, config transform/sostituzione in pipeline, rotazione credenziali, rimozione hardcoded secrets dal VCS.

## 🧩 Altri Anti-pattern Rilevati
| Anti-pattern | Descrizione |
|---|---|
| Shotgun Surgery | Modifiche di regole comuni spesso toccano PN809, PN812 e almeno un modulo fondo |
| Big Ball of Mud parziale | Pur esistendo moduli, la visione end-to-end resta fortemente intrecciata |
| Vendor/Internal Platform Lock-in | Forte dipendenza da `INPS.DNA`, host e stack WCF |
| Primitive Obsession | Parametri tecnici primitivi ripetuti (matricola, sede, flags, stringhe di stato) |
| Boilerplate proxy handling | Pattern ripetitivo `new client / try-catch / close` |

## 🛠️ RACCOMANDAZIONI GENERALI & NEXT STEPS
1. Hardening sicurezza come pre-condizione.
2. Capability map e ownership chiara per ridurre shotgun surgery.
3. Contratti più piccoli e API più mirate.
4. Progressiva eliminazione di Session/ViewState dai journey a maggior valore.
5. Introduzione di metriche e regression suite automatizzata.

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
