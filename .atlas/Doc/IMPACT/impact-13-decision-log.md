# Decision Log - Progetto IVS_DNA

## Sezioni Principali
> Le decisioni seguenti sono **inferite** dall’implementazione e dalla documentazione, non da ADR formali presenti nel repository.

### 1. Technology Decisions
| Decisione | Rationale inferito | Trade-off |
|---|---|---|
| Adozione .NET 3.5 + WCF + WebForms | Standard enterprise Microsoft dell’epoca, integrazione con DNA | Oggi forte obsolescenza |
| Uso framework `INPS.DNA` | Uniformare sicurezza, context, logging, hosting | Lock-in verso piattaforma interna |
| Uso LINQ-to-SQL / DBML | Rapida modellazione su SQL Server | Accoppiamento schema-codice e tecnologia superata |
| ReportViewer 9 | Supporto reporting intranet | Stack datato |

### 2. Architectural Decisions (ADRs)
| ADR inferita | Evidenza |
|---|---|
| PN812 come facade/orchestratore unico | Contratto `IServizioLiquidazione` e BL centrale |
| Separazione per fondo in servizi dedicati | PN813/PN815/PN818 |
| UI server-side con MVP | PN809 presenter/IView |
| Regole runtime configurabili | `ControlliDinamici`, `appSettings` |
| Uso forte di transazioni locali | `TransactionScopeFactory` nei DAO |

### 3. Pattern Decisions
- **MVP** per la UI per separare (parzialmente) view e logica di presentazione.
- **Generated proxy** per standardizzare consumo di servizi interni.
- **Shared outcome contract** (`AreaEsito`) per normalizzare l’esito delle operazioni.
- **Config per ambiente** come meccanismo principale di differenziazione tra test/collaudo/esercizio.

### 4. Trade-offs Analysis
| Scelta | Beneficio | Costo oggi |
|---|---|---|
| Orchestratore unico | Punto d’accesso chiaro | Contratto enorme e alto coupling |
| Specializzazione per fondo | Adesione al dominio | Duplicazione logica |
| Session/ViewState | Semplicità WebForms | Scalabilità e testabilità ridotte |
| Config manuale CHG* | Facilità operativa iniziale | Rischio errori e no automation |

### 5. Alternative Solutions Considered
Il repository non documenta alternative scartate. Le opzioni oggi più plausibili sarebbero:
- segmentazione API per capability;
- frontend moderno stateless;
- data access con ORM/mapper aggiornato;
- modernizzazione incrementale via strangler.

### 6. Decision Context & Rationale
Le decisioni storiche sembrano coerenti con quattro esigenze:
1. coprire rapidamente un dominio previdenziale complesso;
2. riusare l’ecosistema applicativo e infrastrutturale INPS già disponibile;
3. separare le varianti di fondo senza moltiplicare la UI;
4. mantenere il controllo tramite configurazioni e ruoli di sede.

### 7. Decision Debt
- Mancanza di ADR testuali nel repository.
- Gap fra razionale storico e sostenibilità attuale.
- Opportunità alta di introdurre ADR moderni durante la trasformazione.

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
