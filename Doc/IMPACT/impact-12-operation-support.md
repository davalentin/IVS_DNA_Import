# Operation and Support - Progetto IVS_DNA

## Sezioni Principali
### 1. Monitoring & Alerting
| Aspetto | Evidenza | Stato |
|---|---|---|
| Endpoint tecnico | `IMonitoringService` in config PN812 | Base |
| Log applicativo | `INPS.DNA.Logging.Logger` | Presente |
| Metriche business | Non visibili nel repo | TBD |
| Alerting | Non visibile nel repo | TBD |

### 2. Logging & Log Access
- Log di eccezione tramite `Logger.LogException` / `Logger.WriteError`.
- Log SOAP tramite `GestioneLogSoap.SalvaLogSoap` per chiamate verso servizi fondo.
- Log generici applicativi tramite `GestioneLogGenerico.SalvaLogGenerico`.
- Log sicurezza frontend con ID evento configurati (`ACQUISIZIONE`, `CALCOLO`, `ELIMINAZIONE`, ecc.).

### 3. Configuration Management
- `appSettings` e `connectionStrings` costituiscono la principale superficie operativa.
- File `CHGTEST`, `CHGCOLL`, `CHGESERCIZIO` usati per separare ambienti.
- Alcune regole runtime sono governate da `ControlliDinamici` in database.

### 4. Diagnostics & Troubleshooting
Sequenza raccomandata dal materiale di onboarding:
1. Confermare contesto utente (ruolo, sede, abilitazioni).
2. Verificare quadri obbligatori e semafori.
3. Tracciare endpoint invocato e payload.
4. Verificare mapping PN812 → servizio fondo.
5. Isolare anomalia in host integration o persistence.
6. Correlare log applicativo e log SOAP.

### 5. Backup & Restore Procedures
- Non presenti nel repository.
- **Stato:** TBD.

### 6. Maintenance Tasks
| Task | Evidenza / motivazione |
|---|---|
| Allineamento config ambiente | Varianti `CHG*` |
| Controllo endpoint esterni | Molte integrazioni enterprise |
| Monitoraggio log tecnici | Necessario per troubleshooting |
| Verifica controlli dinamici | Influenzano calcolo e comportamenti runtime |
| Smoke test flussi core | Raccomandati in onboarding |

### 7. Support Escalation
| Livello operativo dedotto | Focus |
|---|---|
| L1 applicativo | Verifica ruolo/sede, riproduzione problema |
| L2 sviluppo UI/orchestrator | PN809 / PN812 |
| L3 dominio fondo / integrazioni | PN813 / PN815 / PN818 / host |
| Piattaforme enterprise | DNA, DBA, host, sistemi esterni |

### 8. Suggested Smoke Test Matrix
| Flusso | Priorità |
|---|---|
| Ricerca per NDomus | Alta |
| Ricerca per CF | Alta |
| Prelievo domanda | Alta |
| Verify | Alta |
| Definitivo | Alta |
| Stampa finale | Alta |
| Cambio stato / sblocco | Media |
| Aggiornamento WebDom/FELPE/SAI | Media |

### 9. Operational Risks
1. Osservabilità non sufficiente per diagnosi rapida a scala enterprise.
2. Configurazioni manuali con potenziale errore umano.
3. Gap di documentazione su backup, DR e allarmi.
4. Complessità di triage dovuta a dipendenze esterne numerose.

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
