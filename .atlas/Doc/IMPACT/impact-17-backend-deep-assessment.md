# Backend Deep Assessment - IVS_DNA

## Executive Summary
### Overall Health Score: 4.8/10
Il backend è robusto nel presidiare un dominio business complesso ma soffre fortemente di obsolescenza tecnica, contratti sovradimensionati, integrazioni sincrone e scarsa portabilità.

### Metrics Snapshot
| Metrica | Valore |
|---|---:|
| Moduli backend principali | 4 |
| OperationContract PN812 | 252 |
| File C# PN812 | 616 |
| Generated client files PN812 | 75 |
| File con TransactionScope | 252 |
| Unit test PN812 | 42 |

## 1. Architecture Analysis
### 1.1 Modular Monolith + SOA Analysis (adattamento del prompt BE)
- Non si tratta di microservizi moderni; è più accurato parlare di **monolite distribuito per servizi WCF e fondi**.
- PN812 funge da facade/orchestratore unico.
- PN813/PN815/PN818 implementano specializzazioni di fondo mantenendo pattern simili.

### 1.2 Package / Layer Structure
- Service host (`.svc`, contracts)
- BL / BLCommon
- Data / DataCommon
- UnitTest / ServiceTest (solo PN812)

## 2. Domain Model & Business Logic
### 2.1 Domain-Driven Design
- Esiste una chiara decomposizione per concetti di dominio (titolare, familiari, dante causa, quadro, liquidazione, fondo, prelievo).
- Tuttavia la modellazione è principalmente transaction-script / manager class, non DDD ricco con aggregate ben incapsulati.

### 2.2 Business Logic Analysis
- `GestioneCalcoloDomanda` in PN812 governa gran parte delle decisioni di routing e validazione.
- I servizi fondo incapsulano le variazioni di dominio ma con ampie porzioni ripetitive.
- I controlli dinamici consentono di attivare/disattivare regole business senza rilascio codice.

## 3. Persistence Layer
### 3.1 LINQ-to-SQL Analysis
- `PensioniDataContext`, `CIBaseDataContext` e DBML generated code.
- Uso con query LINQ e stored procedure (`InsertPensione`, ecc.).
- Tecnologia semplice ma datata.

### 3.2 Repository / DAO Layer
- DAO `DAGestione*` abbastanza chiari e coerenti.
- Persistenza fortemente accoppiata al database e alle SP.

### 3.3 Database Schema
- Tabelone centrale `Pensione` + molti `Quadro*`, decodifiche, entità specialistiche per fondo.

## 4. API Layer
### 4.1 WCF API Design
- Contratto centrale vastissimo, orientato a use case applicativi.
- RPC-style, non resource-oriented.
- Buona esplicitazione dei parametri tecnici (ruolo, sede, matricola, flag verify, area quadri, ecc.).

### 4.2 API Documentation
- Il repository contiene il contratto C#, ma non documentazione machine-readable esterna (OpenAPI, WSDL documentato a parte).

### 4.3 API Security
- Sicurezza applicativa demandata a DNA + check in codice.
- Security transport/message disomogenea e talvolta configurata con `mode="None"`.

## 5. Service Decomposition
### 5.1 Current Decomposition
- PN812: common capability facade.
- PN813: fondo FS.
- PN815: fondo AGO.
- PN818: fondo CI.

### 5.2 Service Resilience
- Nessuna evidenza di circuit breaker, retry policy esplicite lato backend WCF, health check moderni o bulkhead.

## 6. Integration Layer
### 6.1 External Systems Integration
Il backend dialoga con numerosi sistemi: ARCA, WebDom, SAI, SCRIWO, ANF, Redditi, Uffici Pagatori, DB2/Oneri, host fondo, ecc.

### 6.2 Messaging & Events
- Nessuna evidenza di event-driven architecture.
- Integrazioni prevalentemente sincrone RPC/host.

## 7. Caching Strategy
### 7.1 Application Caching
- Nessuna strategia di cache applicativa esplicita nel repo.

### 7.2 Database Caching
- N/A dal repository.

## 8. Security Backend
### 8.1 Configuration Security
- Principale criticità: secret in chiaro in config.

### 8.2 Data Security
- Dominio sensibile; misure di protezione dati non pienamente verificabili dal solo repo.

### 8.3 Secrets Management
- Assente nel repository.

## 9. Performance & Scalability
### 9.1 Application Performance
- Synchronous I/O e forte dipendenza dalle latenze delle integrazioni.

### 9.2 Scalability Patterns
- Scalabilità presumibilmente verticale.
- Nessuna evidenza di elasticità o parallelismo moderno.

### 9.3 Batch Processing
- Alcune funzioni di aggiornamento sono batch-like ma invocate dal perimetro applicativo tradizionale.

## 10. Error Handling & Logging
### 10.1 Exception Handling
- Catch tipizzati per `FaultException` application/infrastructure/security.
- Standardizzazione migliorabile.

### 10.2 Logging Strategy
- Logger DNA + log SOAP + log generici.

### 10.3 Monitoring & Alerting
- Osservabilità moderna assente o non visibile.

## 11. Testing Strategy
### 11.1 Unit Testing
- 42 file MSTest in PN812.

### 11.2 Integration Testing
- ServiceTest legacy ma nessuna suite end-to-end visibile.

### 11.3 Performance Testing
- N/A nel repository.

## 12. Build & Deployment
### 12.1 Build Configuration
- Visual Studio 2010, .NET 3.5, web application legacy.

### 12.2 Containerization
- Assente.

### 12.3 CI/CD Pipeline
- Assente.

## 13. Configuration Management
### 13.1 Externalized Configuration
- Forte uso di `appSettings` e `CHG*`.

### 13.2 Environment Management
- TEST / COLL / ESERCIZIO via file distinti.

## 14. Dependency Management
### 14.1 Dependency Analysis
- Numerosi generated proxies e dipendenze enterprise interne.

### 14.2 Upgrade Path
- Alto effort per upgrade completo; preferibile strangler capability-by-capability.

## 15. Code Quality & Maintainability
### 15.1 Code Metrics
- Dimensioni elevate ma separazione modulare leggibile.

### 15.2 Code Smells & Anti-patterns
- God contract, classi estese, duplicazioni, config sprawl.

### 15.3 Code Style & Conventions
- Convenzioni pragmatiche ma non uniformi al 100%.

## 16. Observability & Operations
### 16.1 Logging & Tracing
- Presente logging, assente tracing distribuito moderno.

### 16.2 Metrics & Monitoring
- KPI tecnici non visibili nel repo.

### 16.3 Health Checks & Readiness
- Solo endpoint tecnico legacy, nessun readiness/liveness moderno.

## 17. Database Operations
### 17.1 Schema Migration
- Nessuna pipeline migration code-first; schema implicito via DBML/SP.

### 17.2 Database Monitoring
- TBD.

## 18. Technical Debt & Issues
### 18.1 Critical Issues
1. Secret in chiaro.
2. Contratto centralizzato troppo ampio.
3. Stack legacy EOL.
4. Dipendenze sincrone numerose.

### 18.2 Technical Debt Inventory
- Generated proxy sprawl.
- Duplicazione fra fondi.
- Controlli dinamici opachi.
- Gaps test e automation.

### 18.3 Dependency Technical Debt
- WCF/host/DB2/INPS.DNA sono vincoli strutturali ad alto costo.

## 19. Recommendations
### 19.1 Quick Wins (< 2 settimane effort)
- Secret hardening.
- Inventario consumer del contratto WCF.
- Checklist smoke test e metriche minime.

### 19.2 Short Term (1-3 mesi)
- Segmentazione capability principali in API dedicate.
- Uniformazione logging/error model.
- Test integration su use case ad alto valore.

### 19.3 Long Term (6-12 mesi)
- Estrazione progressiva di capability backend moderne.
- Sostituzione graduale dei proxy WCF.
- Nuovo data access e orchestrazione per servizi prioritari.

### 19.4 Non-Functional Improvements
- Correlation ID, metriche, alerting, policy secret, contract governance.

## 20. Capacity Planning
### 20.1 Resource Requirements
- Non stimabili con precisione dal repo per workload runtime.

### 20.2 Cost Optimization
- Possibile solo dopo riduzione dipendenze legacy e consolidamento capability.

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
