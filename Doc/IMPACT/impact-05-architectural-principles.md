# Principles - Progetto IVS_DNA

## Sezioni Principali
### 1. Architectural Principles
> Nota: nel codebase non è presente un manifesto architetturale esplicito; i principi sotto riportati sono **inferiti** dal design implementato e utili per guidare manutenzione e modernizzazione.

| Principio | Evidenza osservata | Implicazione |
|---|---|---|
| Single entry point verso il dominio | PN812 centralizza `IServizioLiquidazione` | Riduce dispersione ma aumenta accoppiamento |
| Specializzazione per fondo | PN813/PN815/PN818 separati | Buon allineamento con varianti di business |
| Retrocompatibilità contrattuale | WCF DataContract/ServiceContract molto stabili | Modifiche costose ma controllate |
| Config-driven behavior | `appSettings`, `ControlliDinamici`, file `CHG*` | Flessibilità runtime, rischio opacità |
| Persistenza transazionale | `TransactionScope` diffuso | Coerenza locale dei salvataggi |
| User-context first | Ruolo, sede e matricola sono parametri sempre presenti | Audit e sicurezza centrali |

### 2. Design Principles (SOLID, DRY, KISS)
| Tema | Stato attuale | Commento |
|---|---|---|
| SRP | Parzialmente rispettato | Molte classi `Gestione*` e service hanno molte responsabilità |
| DRY | Debole | Logica simile ripetuta tra fondi e presenter |
| KISS | Misto | Alcuni flussi sono lineari, ma le varianti di dominio e i toggle aumentano complessità |
| Explicit outcomes | Forte | `AreaEsito` standardizza OK/KO + messaggio |

### 3. Development Principles
- Limitare le modifiche al modulo strettamente necessario.
- Preservare i DataContract esistenti e gli endpoint WCF finché non esiste un piano di sostituzione.
- Aggiornare orchestratore e modulo fondo in modo coerente quando si tocca una capability trasversale.
- Testare almeno ricerca, prelievo, verify, definitivo e stato pratica dopo modifiche a flussi core.

### 4. Coding Standards
Osservazioni pragmatiche dal repository:
- Convenzioni naming miste italiano/inglese, spesso aderenti al lessico business.
- Pattern ricorrente `Presenter` / `IView` lato UI.
- Pattern ricorrente `Gestione*` lato BL e `DAGestione*` lato data access.
- Ampio uso di classi DTO `Area*` per boundary contract.

### 5. Technology Selection Principles
Principi storicamente impliciti:
- Preferire tecnologie standard enterprise Microsoft disponibili all’epoca di costruzione del sistema.
- Riutilizzare framework istituzionali (`INPS.DNA`) invece di costruire infrastruttura custom.
- Tenere la UI separata dai motori fondo tramite orchestrazione WCF.
- Integrare sistemi esistenti anche a costo di eterogeneità tecnica (SQL Server + DB2 + SOAP + host).

### 6. Buy vs Build Philosophy
| Area | Filosofia osservata |
|---|---|
| Sicurezza, hosting, context | **Reuse** di piattaforma `INPS.DNA` |
| Regole pensionistiche | **Build** custom nei moduli fondo |
| Integrazione sistemi enterprise | **Reuse** di servizi esistenti e proxy generati |
| UI operatore | **Build** specifica sul dominio |

### 7. Principi raccomandati per la modernizzazione
1. **Strangler by capability** invece di riscrittura totale.
2. **Contract-first compatibility** finché esistono consumatori legacy.
3. **Security by default**: secret esternalizzati, debug disabilitato, masking log.
4. **Observability first**: correlation-id, metriche tecniche e business.
5. **State minimization**: ridurre Session/ViewState e introdurre API più stateless.

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
