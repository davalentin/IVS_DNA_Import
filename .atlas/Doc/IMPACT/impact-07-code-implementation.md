# Code - Progetto IVS_DNA

## Sezioni Principali
### 1. Code Organization & Structure
| Layer | Pattern / naming | Esempi |
|---|---|---|
| UI | `*.aspx.cs`, `UserControls`, `MasterPage`, `CodeUtils` | PN809 |
| Presenter | `Presenter*`, `IView/*`, `Contract/*` | PN809/LiquidazionePensioniFS.Presenter |
| Service | `.svc`, `Contracts/DataContracts`, `Contracts/ServiceContracts` | PN812/813/815/818 |
| BL | `Gestione*` | Orchestrazione, regole dominio, integrazioni |
| Data | `DAGestione*`, DBML, designer | SQL Server e decodifiche |
| Test | MSTest classico | PN812 UnitTest / ServiceTest |

### 2. Key Implementation Patterns
- **Presenter-driven UI**: la pagina implementa una `IView`, il presenter compone la richiesta e chiama il proxy WCF.
- **Outcome standard**: quasi tutte le operazioni rientrano in `AreaEsito` con `OK/KO + Messaggio`.
- **Generated proxy usage**: il codice istanzia `new ServizioLiquidazioneClient()` o equivalenti e chiude il client in `finally` tramite `Utility.CloseClient(...)`.
- **Domain-specific manager classes**: `GestioneCalcoloDomanda`, `GestioneLiquidazioneFs`, `GestioneContrib`, `GestionePrelievo`, ecc.

### 3. Framework Usage (MVC, ORM, DI)
| Tema | Stato |
|---|---|
| MVC / SPA | Assente |
| MVP | Presente in PN809 |
| ORM | LINQ-to-SQL (`PensioniDataContext`, `CIBaseDataContext`) |
| DI / IoC | Assente nel repository |
| Async/Await | Assente (compatibile con .NET 3.5) |

### 4. Security Implementation
- Autenticazione e principal context gestiti da `INPS.DNA`.
- Controlli di ruolo e sede distribuiti tra UI e backend.
- Logging di eventi sensibili con ID configurabili (es. acquisizione/calcolo/eliminazione in PN809).
- Debolezze rilevate: secret in chiaro, `connectionStringCryptography=false`, `security mode="None"` in più binding WCF.

### 5. Exception Handling & Logging
| Pattern | Evidenza |
|---|---|
| `try/catch/finally` su client WCF | Presenter PN809 e BL orchestrator |
| `Logger.LogException` | PN812/813/815/818 |
| `GestioneLogSoap.SalvaLogSoap(...)` | Orchestrazione chiamate fondo |
| `GestioneLogGenerico.SalvaLogGenerico(...)` | Tracciamento errori tecnici e contesto |

### 6. Transaction Management
- `TransactionScopeFactory.Create(TransactionScopeOption.Suppress/Required)` è usato diffusamente nei DAO e in parte della BL.
- Le operazioni di persistenza principali (es. `SalvaPensione`) combinano LINQ-to-SQL e stored procedure.
- Non è emersa dal repository una strategia esplicita di coordinamento transazionale distribuito fra più servizi.

### 7. Configuration Management
- Forte uso di `appSettings` per feature toggle, endpoint, bypass, chiavi e opzioni dominio.
- Configurazioni per ambiente duplicate per modulo.
- Dipendenza da file DNA esterni al repo.
- Assenza di gestione centralizzata dei secret moderna.

### 8. Testing Strategy
| Livello | Evidenza | Limiti |
|---|---|---|
| Unit test | 42 file MSTest in PN812 | Focus soprattutto backend/orchestrator |
| Service test | 6 file in PN812 | Test legacy dipendenti da contesto WCF |
| UI test | Nessuna evidenza | Nessuna regressione automatica frontend |
| Integration / E2E | Nessuna evidenza nel repo | Gap elevato |

### 9. Code Quality Assessment
- **Punti forti**: naming aderente al business, layering riconoscibile, DTO espliciti, regole di dominio tracciabili.
- **Punti deboli**: generated code molto pesante, classi lunghe, forte stato server-side, pattern ripetitivo, testabilità bassa, assenza DI.

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
