# Frontend Deep Assessment - IVS_DNA

## Executive Summary
### Overall Health Score: 4.3/10
Score euristico basato su tecnologia, testabilità, sicurezza, performance percepita e manutenibilità. Il frontend svolge efficacemente funzioni operative complesse ma poggia su stack e pattern ampiamente superati.

### Metrics Snapshot
| Metrica | Valore |
|---|---:|
| Pagine ASPX | 112 |
| User control ASCX | 190 |
| Presenter | 71 |
| IView | 83 |
| Session usages | 1.805 |
| ViewState usages | 3.344 |
| UI test automation | Assente |

## 1. Architecture Analysis
### 1.1 WebForms Monolith Analysis (adattamento del prompt FE)
Il “frontend” non è una SPA né un micro-frontend: è un **server-rendered WebForms application** con pattern MVP. La gran parte della logica di stato è lato server.

### 1.2 Routing Architecture
- Routing basato su pagine `.aspx` e redirezioni classiche.
- Nessuna client-side router.
- Gerarchia importante di pagine sotto `ElaborazionePosizione/` e `AltreFunzioni/`.

## 2. State Management
### 2.1 State Architecture
- Stato applicativo distribuito in `Session`, con chiavi stringa non tipizzate.
- Stato UI distribuito in `ViewState` e nei controlli server.
- Session server esterno via `StateServer` su `localhost:42424`.

### 2.2 Performance State
- Alto volume di `ViewState` implica payload HTML più pesanti.
- L’uso esteso di sessione complica scalabilità orizzontale e debugging multi-step.

## 3. Component Architecture
### 3.1 Component Design
- Granularità buona a livello di user control, ma con forte coupling a pagine e sessione.
- Presenters riusabili in parte, ma spesso orientati al singolo caso d’uso/pagina.

### 3.2 Template & Styling
- Theme legacy `BlueINPS1` e varianti `iFrame`, `SistemaUnico`.
- Header `IE=9` indica focus su browser enterprise legacy.
- Nessuna pipeline moderna assets (bundling, transpiling, npm, webpack, ecc.).

## 4. Services & HTTP
### 4.1 Service Layer
- Il frontend usa generated WCF proxies verso PN812 e, in alcuni casi, verso servizi fondo.
- Pattern ricorrente: istanziazione client, chiamata sincrona, catch, `Utility.CloseClient`.

### 4.2 Async Patterns
- Nessuna evidenza di async/await.
- Tutto il flusso UX dipende dal roundtrip server.

## 5. Security Frontend
### 5.1 Authentication & Authorization
- Autenticazione delegata a moduli DNA/IDM.
- Ruolo in sessione (`Session["Ruolo"]`) e controlli di visibilità/abilitazione lato UI.

### 5.2 Data Protection
- Nessuna evidenza di protezione CSP, anti-forgery o XSS hardening moderno nel repo.
- Session state e config sensitive richiedono attenzione.

## 6. Performance Optimization
### 6.1 Loading Performance
- Stack WebForms e ViewState costituiscono i principali colli di bottiglia percepibili.
- Nessuna evidenza di lazy loading, asset optimization o CDN.

### 6.2 Runtime Performance
- Nessun instrumentation front-end moderno.
- Le prestazioni dipendono soprattutto da roundtrip server e WCF backend.

## 7. Testing Strategy
### 7.1 Unit Testing
- Non emergono unit test dedicati al frontend PN809.

### 7.2 E2E Testing
- Nessuna evidenza di test end-to-end o browser automation.

## 8. Build & Deployment
### 8.1 Build Configuration
- Progetto Visual Studio 2010 Web Application, target .NET 3.5.
- Dipende da IIS e da librerie enterprise Windows.

### 8.2 Deployment Strategy
- Deploy tradizionale su IIS con config ambiente manuali.

## 9. Accessibility (a11y)
### 9.1 WCAG 2.1 Compliance
- Nessuna evidenza di audit o conformità formale.
- Probabile gap significativo rispetto a standard attuali.

### 9.2 A11y Testing
- N/A nel repository.

## 10. Developer Experience
### 10.1 Code Quality
- Struttura chiara ma verbosa; forte boilerplate presenter/proxy.
- Session/ViewState riducono prevedibilità del comportamento.

### 10.2 Documentation
- Buona documentazione funzionale e tecnica di contorno.
- Scarsa documentazione specifica FE moderna.

### 10.3 Development Workflow
- Fortemente dipendente da toolchain Windows/Visual Studio legacy.

## 11. Third-Party Dependencies
### 11.1 Dependency Analysis
- `INPS.DNA.*`
- `Microsoft.ReportViewer.*`
- `Polly 4.3`
- `System.Web.Extensions 3.5`

### 11.2 Upgrade Path
- Upgrade incrementale consigliato: prima isolamento capability e servizi, poi rinnovo UI.

## 12. Metrics & KPI
### 12.1 Performance Metrics
- KPI runtime nel repo: TBD.

### 12.2 Code Metrics
- 112 pagine, 190 user control, 1.805 session access, 3.344 viewstate access.

## 13. Issues & Technical Debt
### 13.1 Critical Issues
1. State management fortemente server-side e non tipizzato.
2. Assenza di automation test FE.
3. Tecnologia WebForms e compatibilità browser legacy.
4. Rischi sicurezza/configurazione.

### 13.2 Technical Debt
- Presenter ripetitivi.
- Generated service references pesanti.
- Tema/UI stratificati nel tempo.

## 14. Recommendations
### 14.1 Quick Wins (< 1 settimana effort)
- Mappare e tipizzare le chiavi Session più critiche.
- Disabilitare `debug=true` nei package non dev.
- Introdurre checklist smoke test FE.

### 14.2 Short Term (1-3 mesi)
- Estrarre API backend per i journey più usati.
- Ridurre progressivamente ViewState in schermate ad alto traffico.
- Inserire test browser automatizzati sui flussi principali.

### 14.3 Long Term (6-12 mesi)
- Nuovo frontend capability-based (es. Razor/SPA) in strangler mode.
- Disaccoppiare progressivamente UI e session state.
- Rimuovere dipendenze dirette verso servizi fondo dal layer presenter.

## 15. Action Plan
### Phase 1: Critical Fixes (2-4 settimane)
- Hardening config.
- Smoke test.
- Observability base.

### Phase 2: Performance (1-2 mesi)
- Riduzione stato lato server.
- Journey prioritari più leggeri.

### Phase 3: Technical Debt (3-6 mesi)
- Estrazione capability FE nuove e decommission progressivo delle pagine legacy.

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
