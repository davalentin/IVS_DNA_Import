# Development Environment - Progetto IVS_DNA

## Sezioni Principali
### 1. Prerequisites & Dependencies
| Prerequisito | Stato |
|---|---|
| Windows + IIS | Necessario per esecuzione realistica locale |
| Visual Studio 2010 / toolchain compatibile web application | Necessario |
| .NET Framework 3.5 | Necessario |
| Accesso a assembly `INPS.DNA` | Necessario |
| Accesso DB SQL Server interni | Necessario |
| Provider DB2 / Host Integration | Necessario per alcune capability |
| ReportViewer 9 | Necessario per reporting |
| IdM / simulazione identità | `IdmSimulator` disponibile nel repo |

### 2. IDE Setup & Configuration
- Le soluzioni sono in formato Visual Studio 2010.
- I progetti sono web application project, non SDK-style.
- `UseIIS=True` e URL locali sono configurati nei `.csproj`.
- PN809 richiede anche ASP.NET State Server su `localhost:42424`.

### 3. Repository Setup & Branching
- Repository Git con branch corrente `main`.
- Nessuna branching policy nel repo.
- Storicamente il sistema sembra provenire da Subversion/Serena Source Control.

### 4. Build Instructions
1. Aprire la soluzione del modulo di interesse (`PN809`, `PN812`, `PN813`, `PN815`, `PN818`) in Visual Studio su Windows.
2. Verificare disponibilità delle librerie esterne e degli assembly `INPS.DNA`.
3. Selezionare configurazione `Debug` o `Release`.
4. Configurare il `Web.config` corretto o usare ambiente locale dedicato.

**Osservazione importante:** in questo ambiente Linux `dotnet test PN812/WSInpsPensioniLiquidazione.sln` fallisce per mancanza di `Microsoft.WebApplication.targets`, confermando la dipendenza dalla toolchain Visual Studio legacy.

### 5. Test Execution
- Test unitari e service test sono presenti solo in PN812.
- Esecuzione prevista via MSTest / Visual Studio legacy.
- Non sono presenti test UI o E2E nel repository.

### 6. Database Setup
- Sono richieste connection string verso SQL Server interni INPS e, per alcuni flussi, DB2/host.
- Il repository non include script di bootstrap DB completi né dump locali.
- **Setup locale realisticamente isolato:** TBD.

### 7. Local Development Workflow
1. Studiare documentazione tecnica e flusso funzionale.
2. Riprodurre il caso d’uso in PN809.
3. Tracciare la chiamata verso PN812 e poi verso il servizio fondo.
4. Modificare il modulo più vicino al problema.
5. Eseguire smoke test minimi sui flussi critici.

### 8. Troubleshooting Guide
| Sintomo | Primo punto di analisi |
|---|---|
| Problema UI / navigazione | PN809 page code-behind + presenter |
| Errore business comune | PN812 service + BL |
| Errore fondo specifico | PN813 / PN815 / PN818 |
| Errore integrazione anagrafica | ARCA / PN812 BL |
| Errore su oneri / host | DB2 / mapping / Gestione* dedicata |

### 9. Known Local Setup Gaps
- Nessuna infrastruttura containerizzata.
- Nessun seed data locale.
- Nessun workflow scriptato di start all-in-one.
- Diversi endpoint puntano a sistemi interni non emulati nel repository.

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
