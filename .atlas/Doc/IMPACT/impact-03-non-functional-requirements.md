# Non-Functional Overview - Progetto IVS_DNA

## 1. MANIFEST Service Level Requirements
### 1.1 Performance
| Aspetto | Evidenza attuale | Stato |
|---|---|---|
| Elaborazioni sincrone | Chiamate WCF e accesso DB principalmente sincroni | Critico |
| Payload | `maxReceivedMessageSize` elevato e JSON fino a 50MB | Rischio di performance e memoria |
| UI state | 3.344 usi di `ViewState` | Impatto su rendering e payload HTML |
| Session state | `StateServer` + 1.805 accessi `Session[` | Impatto su latenza e scalabilità |

**Target SMART disponibile nel repo:** TBD.

### 1.2 Usability
| Aspetto | Evidenza attuale | Stato |
|---|---|---|
| Coerenza processo | Flusso guidato per quadri e semafori | Positivo |
| Complessità | Molte schermate e sottosezioni | Alto carico cognitivo |
| Browser strategy | Header `X-UA-Compatible = IE=9` | Esperienza modern browser non prioritaria |

### 1.3 Reliability
| Aspetto | Evidenza attuale | Stato |
|---|---|---|
| Transazioni | Uso diffuso di `TransactionScope` | Buona atomicità locale |
| Error handling | `AreaEsito` + catch differenziati | Moderato |
| Dipendenze esterne | Numerosi sistemi interni/host | Fragilità sistemica |

### 1.4 Availability
- Non esistono nel repository SLO/SLA formali.
- Esistono ambienti separati (`TEST`, `COLL`, `ESERCIZIO`) ma non prove di HA/cluster.
- Disponibilità effettiva: **TBD**.

## 2. OPERATIONAL Service Level Requirements
| Categoria | Requirement dedotto | Evidenza |
|---|---|---|
| Throughput | Il sistema deve sostenere carichi da operatori di sede su base intranet | Pattern sincrono e stateful |
| Serviceability | Deve essere possibile diagnosticare errori per modulo e integrazione | Onboarding + log SOAP/generici |
| Testability | Flussi critici devono essere verificabili almeno con smoke test | Documento onboarding |
| Manageability | Configurazioni per ambiente devono essere modificabili senza cambiare codice | `CHG*`, `appSettings`, `ControlliDinamici` |
| Security | Ruolo/sede devono governare tutte le operazioni sensibili | Check applicativi e framework DNA |

## 3. DEVELOPMENT Service Level Requirements
| Categoria | Osservazione |
|---|---|
| Realizability | Il sistema è buildabile solo in ecosistema Windows/Visual Studio legacy; su Linux `dotnet test` fallisce per import di `Microsoft.WebApplication.targets`. |
| Planability | La modularizzazione per PN809/PN812/PN813/PN815/PN818 aiuta la pianificazione per capability, ma il contratto condiviso amplia il blast radius. |
| Maintainability in sviluppo | Naming misto IT/EN, generated code e proxy voluminosi rendono più oneroso l’onboarding tecnico. |

## 4. EVOLUTIONARY Service Level Requirements
| Categoria | Stato corrente | Implicazione |
|---|---|---|
| Scalability | Architettura monolitica distribuita e sincrona | Scalabilità principalmente verticale |
| Extensibility | Nuove regole spesso aggiunte via `ControlliDinamici` / flag | Estendibilità pragmatica ma opaca |
| Maintainability | Debito tecnico elevato su UI, config e contratti WCF | Rallenta evoluzione |
| Flexibility | Forte dipendenza da ecosistema INPS e host | Bassa portabilità |
| Portability | .NET 3.5 + WebForms + WCF | Molto bassa |
| Reusability | Alcune logiche condivise in BLCommon/DataCommon | Riutilizzo presente ma non completo |

## 5. NFR Priority Matrix
| NFR | Priorità | Motivazione |
|---|---|---|
| Sicurezza configurazione | Alta | Secret in chiaro e crittografia disabilitata |
| Affidabilità integrazioni | Alta | Dipendenze molteplici e sincrone |
| Manutenibilità | Alta | Tecnologia legacy + contratti estesi |
| Testabilità | Media-Alta | Test presenti ma parziali |
| Prestazioni | Media | Impatto forte di Session/ViewState, ma assenza di metriche runtime |
| Disponibilità | Media | Dati infrastrutturali insufficienti |
| Portabilità | Alta | Ostacola qualsiasi modernizzazione |

## 6. Out of Scope
- KPI di produzione reali (throughput, error rate, latency) non presenti nel codebase.
- Requisiti contrattuali/sistemistici di availability e DR: **TBD / N/A**.
- Requisiti formali di accessibilità o performance browser: **TBD**.

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
